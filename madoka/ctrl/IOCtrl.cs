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
		private readonly List<Dir> _tmpRecordList = new List<Dir>();
		private readonly List<RelationPair> _tmpTreeRelationList = new List<RelationPair>();
		private readonly List<FontFile> _tmpFontFileList = new List<FontFile>();

		private readonly ModelMy _model;
		private readonly DataSet1 _dataSet;
		private readonly Dictionary<string, NodeID> _path2dirIDTemp;
		private readonly TreeModelCtrl _treeModelCtr;
		private readonly Dictionary<string, FontFileID> _path2fontIDTemp;

		private ScanDirTask(ModelMy model, DataSet1 dataSet)
		{
			_model   = model;
			_dataSet = dataSet;
			_treeModelCtr = new TreeModelCtrl(model);

			_path2dirIDTemp = dataSet.DirectoryTable.ToDictionary(
				(r) => ((Dir)r.directory).DirectoryInfo.FullName,
				(r) => r.id
			);
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
					() =>
					{
						ScanDirTask scanDir = new ScanDirTask(model, dataSet);
						ScanDirTaskResult result = scanDir.RegisterDirs();
						return result;
					},
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
					}
				}

				model.addFontDirectoryTaskTerminateFlag = model.addFontDirectoryPathList.IsEmpty;
				if (model.addFontDirectoryTaskTerminateFlag)
					break;
			}

			// === commit ===
			DataSetCtrl dataSetCtrl = new DataSetCtrl(_dataSet);
			dataSetCtrl.RegisterDirectoryList(_tmpRecordList);
			dataSetCtrl.RegisterFontFileList(_tmpFontFileList);

			RelationPair[] dir2FontRelationList = _tmpRecordList.SelectMany(
				(dir) => dir.FontFileID.Select((fontID) => new RelationPair(dir.ID, fontID))
			).ToArray();

			TreeModelCtrl treeModelCtrl = new TreeModelCtrl(_model);
			IEnumerable<RelationPair> newItemList = treeModelCtrl.AddDirRelation(_tmpTreeRelationList);

			return new ScanDirTaskResult(_tmpRecordList, _tmpFontFileList, _tmpTreeRelationList);
		}

		private NodeID RegisterDir(DirectoryInfo currDir)
		{
			_model.cancelToken.Token.ThrowIfCancellationRequested();

			NodeID[] childNodeIdList = currDir.GetDirectories()
				.Select(RegisterDir)
				.Where((ret) => ret < 0)
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
