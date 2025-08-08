using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	using NodeID = Int32;
	using FontFileID = Int32;

	/*
	class IOCtrl
	{
		private readonly ModelMy _model;

		public IOCtrl(ModelMy model, DataSet1 dataSet)
		{
			_model = model;
		}
	}
	*/

	class ScanDirTaskResult
	{
		public ScanDirTaskResult(IEnumerable<Dir> dirList, IEnumerable<FontFile> fontFileList, IEnumerable<RelationPair> relationList)
		{
			MewDirList = dirList.ToArray();
			NewFontFileList = fontFileList.ToArray();
			NewRelationList = relationList.ToArray();
		}

		public Dir[] MewDirList { get; private set; }
		public FontFile[] NewFontFileList { get; private set; }
		public RelationPair[] NewRelationList { get; private set; }
	};

	class ScanDirTask
	{
		// いろいろ処理して登録OKになった系
		private readonly List<Dir> _tmpRecordList = new List<Dir>();
		private readonly List<RelationPair> _tmpTreeRelationList = new List<RelationPair>();
		private readonly List<FontFile> _tmpFontFileList = new List<FontFile>();
		private readonly List<string> _tmpRootFontDirPathList = new List<string>();

		private readonly ModelMy _model;
		private readonly DataSet1 _dataSet;
		private readonly Dictionary<string, NodeID> _path2dirIDTemp;
		private readonly Dictionary<string, FontFileID> _path2fontIDTemp;
		private readonly TreeModelCtrl _treeModelCtr;
		private readonly int _directoryTableVersion;
		private readonly int _fontFileTableVersion;
		private readonly int _treeRelationModelVersion;
		private readonly int _dir2fontRelationModelVersion;

		private ScanDirTask(ModelMy model, DataSet1 dataSet)
		{
			_model   = model;
			_dataSet = dataSet;
			_treeModelCtr = new TreeModelCtrl(model);

			using (DataSetReadLocker r = dataSet.GetReadLocker())
			{
				_directoryTableVersion = dataSet.DirectoryTable.Version;
				_fontFileTableVersion = dataSet.FontFileTable.Version;
				_treeRelationModelVersion = model.treeRelationModelVersion;
				_dir2fontRelationModelVersion = model.dir2fontRelationModelVersion;

				_path2dirIDTemp = dataSet.CreatePath2DirID();
				_path2fontIDTemp = dataSet.CreatePath2FontFileID();
			}
		}

		static public Task<ScanDirTaskResult> Scan(string[] pathList, ModelMy model, DataSet1 dataSet)
		{
			foreach (string path in pathList)
			{
				model.addFontDirectoryPathList.Enqueue(path);
			}

			lock (model.addFontDirectoryPathList)
			{
				if (!model.addFontDirectoryTaskTerminateFlag)
					return null;

				model.addFontDirectoryTaskTerminateFlag = false;
				Task<ScanDirTaskResult> task = new Task<ScanDirTaskResult>(
					() => U.HandleTableVersionMismatchException(() =>
						{
							ScanDirTask scanDir = new ScanDirTask(model, dataSet);
							ScanDirTaskResult result = scanDir.RegisterDirs();
							return result;
						}
					),
					model.cancelToken.Token
				);

				TaskCtrl ctrl = new TaskCtrl(model);
				ctrl.StartTask(task);
				return task;
			}
		}

		/// <summary>
		/// commit
		/// </summary>
		private ScanDirTaskResult RegisterDirs()
		{
			ModelMy model = _model;

			while (!model.cancelToken.IsCancellationRequested)
			{
				if (model.addFontDirectoryPathList.TryDequeue(out string path))
				{
					DirectoryInfo dir = new DirectoryInfo(path);
					NodeID childNodeId = RegisterDir(dir);
					if (childNodeId > 0)
					{
						_tmpTreeRelationList.Add(new RelationPair(model.rootDirID, childNodeId));
						_tmpRootFontDirPathList.Add(path);
					}
				}

				model.addFontDirectoryTaskTerminateFlag = model.addFontDirectoryPathList.IsEmpty;
				if (model.addFontDirectoryTaskTerminateFlag)
					break;
			}

			// === commit ===
			using (DataSetWriteLocker w = _dataSet.GetWriteLocker())
			{
				// throw TableVersionMismatchException
				_dataSet.DirectoryTable.Version.Test(_directoryTableVersion);
				_dataSet.FontFileTable.Version.Test(_directoryTableVersion);
				_model.treeRelationModelVersion.Test(_treeRelationModelVersion);
				_model.dir2fontRelationModelVersion.Test(_dir2fontRelationModelVersion);

				var cancelToken = _model.cancelToken.Token;
				Task[] taskList = {
					Task.Run((Action)AddDir2FontRelationListTask, cancelToken),
					Task.Run((Action)AddTreeModelRelationListTask, cancelToken),
					Task.Run((Action)RegisterFontFileListTask, cancelToken),
					Task.Run((Action)RegisterDirectryListTask, cancelToken),
					Task.Run((Action)AddRootFontDirList, cancelToken), // conflict check 抜けてる… まぁ… すまん…
				};
				Task.WaitAll(taskList, cancelToken);

				return new ScanDirTaskResult(_tmpRecordList, _tmpFontFileList, _tmpTreeRelationList);
			}
		}

		// === commit 系 ===

		private void RegisterDirectryListTask()
		{
			_dataSet.RegisterDirectoryList(_tmpRecordList);
			_dataSet.DirectoryTable.Version.Inc();
		}

		private void RegisterFontFileListTask()
		{
			_dataSet.RegisterFontFileList(_tmpFontFileList);
			_dataSet.FontFileTable.Version.Inc();
		}

		private void AddTreeModelRelationListTask()
		{
			TreeModelCtrl treeModelCtrl = new TreeModelCtrl(_model);
			IEnumerable<RelationPair> newItemList = treeModelCtrl.AddRelation(_tmpTreeRelationList);
			treeModelCtrl.IncVersion();
		}

		private void AddDir2FontRelationListTask()
		{
			var dir2FontRelationList = _tmpRecordList.SelectMany(
				(dir) => dir.FontFileID.Select((fontID) => new RelationPair(dir.ID, fontID))
			);

			Dir2FontCtrl dir2FontModelCtrl = new Dir2FontCtrl(_model);
			IEnumerable<RelationPair> newItemList = dir2FontModelCtrl.AddRelation(dir2FontRelationList);
			dir2FontModelCtrl.IncVersion();
		}

		private void AddRootFontDirList()
		{
			DataSetKomono ctrl = new DataSetKomono(_dataSet);
			ctrl.AppendNewRootFontDir(_tmpRootFontDirPathList);
		}

		// === main ===

		private NodeID RegisterDir(DirectoryInfo currDir)
		{
			_model.cancelToken.Token.ThrowIfCancellationRequested();

			NodeID[] childNodeIdList = currDir.GetDirectories()
				.Select(RegisterDir)
				.Where((ret) => ret > 0)
				.ToArray();
			FileInfo[] fontList = currDir.GetFiles().Where((f) => U.IsFontFile(f.Name)).ToArray();

			if (childNodeIdList.Length == 0 && fontList.Length == 0)
				return -1;

			NodeID parentDirId;
			if (!_path2dirIDTemp.TryGetValue(currDir.FullName, out parentDirId))
			{   // not exist... -> pre-regsiter an new dir object
				parentDirId = IDIssuer.DirectoryID;

				FontFileID[] fontFileIDList = fontList.Select(RegisterFontFile).ToArray();
				Dir newDir = new Dir(parentDirId, currDir, fontFileIDList);

				_tmpRecordList.Add(newDir);
				_path2dirIDTemp.Add(currDir.FullName, parentDirId);
			}

			var newPairList = from c in childNodeIdList
							  let pair = new RelationPair(parentDirId, c)
							  select pair;
			_tmpTreeRelationList.AddRange(newPairList);
			return parentDirId;
		}

		private FontFileID RegisterFontFile(FileInfo fontFile)
		{
			string fullPath = fontFile.FullName;
			if (_path2fontIDTemp.TryGetValue(fullPath, out FontFileID id))
				return id;

			FontFileID newID = IDIssuer.FontFileID;
			FontFile newFont = new FontFile(newID, fullPath);
			_tmpFontFileList.Add(newFont);
			_path2dirIDTemp.Add(fullPath, newID);

			return newID;
		}
	}
}
