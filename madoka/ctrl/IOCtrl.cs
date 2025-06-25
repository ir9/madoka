using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	using NodeID = Int32;

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
		public ScanDirTaskResult(List<TableDirectoryRowCompati> recordList, List<TreeModelPair> relationList)
		{
			RecordList = recordList;
			RelationList = relationList;
		}

		public List<TableDirectoryRowCompati> RecordList { get; private set; }
		public List<TreeModelPair> RelationList { get; private set; }
	};

	class ScanDirTask
	{
		private readonly List<TableDirectoryRowCompati> _recordList = new List<TableDirectoryRowCompati>();
		private readonly List<TreeModelPair> _relationList = new List<TreeModelPair>();

		private readonly ModelMy _model;
		private readonly DataSet1 _dataSet;
		private readonly Dictionary<string, NodeID> _path2dirIDTemp;
		private readonly TreeModelCtrl _treeModelCtr;

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

		static public Task<ScanDirTaskResult> EnumDirs(string[] pathList, ModelMy model, DataSet1 dataSet)
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
						ScanDirTask enumDir = new ScanDirTask(model, dataSet);
						ScanDirTaskResult result = enumDir.RegisterDirs();
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
						_relationList.Add(new TreeModelPair() { parent = model.rootDirID, child = childNodeId });
					}
				}

				model.addFontDirectoryTaskTerminateFlag = model.addFontDirectoryPathList.IsEmpty;
				if (model.addFontDirectoryTaskTerminateFlag)
					break;
			}

			// === commit ===
			DataSetCtrl dataSetCtrl = new DataSetCtrl(_dataSet);
			dataSetCtrl.RegsiterDirectory(_recordList);

			TreeModelCtrl treeModelCtrl = new TreeModelCtrl(_model);
			treeModelCtrl.AddDirRelation(_relationList);

			return new ScanDirTaskResult(_recordList, _relationList);
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
				Dir newDir = new Dir(parentDirId, currDir, fontList);
				_recordList.Add(new TableDirectoryRowCompati() { id = parentDirId, dir = newDir });
				_path2dirIDTemp.Add(currDir.FullName, parentDirId);
			}

			var newPairList = from c in childNodeIdList
							  let pair = new TreeModelPair() { parent = parentDirId, child = c }
							  where !_treeModelCtr.Contain(pair)
							  select pair;
			_relationList.AddRange(newPairList);
			return parentDirId;
		}
	}
}
