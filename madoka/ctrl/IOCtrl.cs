using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	using NodeID = Int32;

	class IOCtrl
	{
		private readonly ModelMy _model;

		public IOCtrl(ModelMy model, DataSet dataSet)
		{
			_model = model;
		}

	}

	class EnumDir
	{
		private readonly List<TableDirectoryRowCompati> _recordList = new List<TableDirectoryRowCompati>();
		private readonly List<TreeModelPair> _relationList = new List<TreeModelPair>();

		private readonly ModelMy _model;
		private readonly DataSet _dataSet;
		private readonly Dictionary<string, NodeID> _path2dirID;

		private EnumDir(ModelMy model, DataSet dataSet)
		{
			_model   = model;
			_dataSet = dataSet;

			_path2dirID = dataSet.DirectoryTable.ToDictionary(
				(r) => ((Directory)r.directory).DirectoryInfo.FullName,
				(r) => r.id
			);
		}

		static public Task EnumeDirs(string path, ModelMy model, DataSet dataSet)
		{
			Task task = Task.Run(() =>
			{
				EnumDir enumDir = new EnumDir(model, dataSet);
				enumDir.RegisterDir(path);
			}, model.cancelToken.Token);

			TaskCtrl ctrl = new TaskCtrl(model);
			ctrl.AddTask(task);
			return task;
		}

		private void RegisterDir(string path)
		{
			DirectoryInfo root = new DirectoryInfo(path);
			NodeID nodeID = RegisterDir(root);

			DataSetCtrl dataSetCtrl = new DataSetCtrl(_dataSet);
			dataSetCtrl.RegsiterDirectory(_recordList);

			TreeModelCtrl treeModelCtrl = new TreeModelCtrl(_model);
			treeModelCtrl.AddDirRelation(_relationList);
		}

		private NodeID RegisterDir(DirectoryInfo currDir)
		{
			_model.cancelToken.Token.ThrowIfCancellationRequested();

			NodeID[] childNodeIdList = currDir.GetDirectories()
				.Select(RegisterDir)
				.Where((ret) => ret < 0)
				.ToArray();
			FileInfo[] fontList = currDir.GetFiles().Where(U.IsFontFile).ToArray();

			if (childNodeIdList.Length == 0 && fontList.Length == 0)
				return -1;

			NodeID parentDirId;
			if (!_path2dirID.TryGetValue(currDir.FullName, out parentDirId))
			{	// not exist... -> pre-regsiter an new dir object
				Directory newDir = new Directory(currDir, fontList);
				parentDirId = IDIssuer.DirectoryID;
				_recordList.Add(new TableDirectoryRowCompati() { id = parentDirId, dir = newDir });
				_path2dirID.Add(currDir.FullName, parentDirId);
			}

			_relationList.AddRange(childNodeIdList.Select(
				(c) => new TreeModelPair() { parent = parentDirId, child = c })
			);
			return parentDirId;
		}
	}
}
