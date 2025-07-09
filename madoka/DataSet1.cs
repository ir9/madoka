using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace madoka
{
	partial class DataSet1
	{
		partial class DirectoryTableDataTable
		{
			internal CommitVersion Version { get; } = new CommitVersion();
		}

		partial class FontFileTableDataTable
		{
			internal CommitVersion Version { get; } = new CommitVersion();
		}

		/* ========================================= *
		 *
		 * ========================================= */
		public volatile int readLockCount = 0;

		internal int Initialize()
		{
			int rootNodeId = IDIssuer.DirectoryID;
			DataSet1.DirectoryTableRow root = DirectoryTable.AddDirectoryTableRow(
				rootNodeId, new Dir(rootNodeId, new System.IO.DirectoryInfo("."), new int[] { })
			);
			return rootNodeId;
		}

		internal DataSetReadLocker GetReadLocker()
		{
			return new DataSetReadLocker(this);
		}

		internal DataSetWriteLocker GetWriteLocker()
		{
			return new DataSetWriteLocker(this);
		}

		/* ========================================= *
		 * DirectryTable
		 * ========================================= */
		internal void RegisterDirectoryList(IEnumerable<Dir> rowList)
		{
			foreach (Dir row in rowList)
			{
				DirectoryTable.AddDirectoryTableRow(row.ID, row);
			}
		}

		internal Dictionary<string, int> CreatePath2DirID()
		{
			return DirectoryTable.ToDictionary(
				(r) => ((Dir)r.directory).DirectoryInfo.FullName,
				(r) => r.id
			);
		}

		internal Dir GetDirectory(int nodeID)
		{
			DirectoryTableRow row = DirectoryTable.FindByid(nodeID);
			return (Dir)row.directory;
		}

		/* ========================================= *
		 * FontFileTable
		 * ========================================= */
		internal void RegisterFontFileList(IEnumerable<FontFile> rowList)
		{
			foreach (FontFile row in rowList)
			{
				FontFileTable.AddFontFileTableRow(row.ID, row.FilePath);
			}
		}

		internal string GetFontFilePath(int fontID)
		{
			FontFileTableRow row = FontFileTable.FindByid(fontID);
			return row.filepath;
		}

		internal Dictionary<string, int> CreatePath2FontFileID()
		{
			return FontFileTable.ToDictionary(
				(r) => r.filepath,
				(r) => r.id
			);
		}

		/* ========================================= *
		 * GridViewDataTable
		 * ========================================= */
		internal void RebuildGridViewDataTable(ModelMy model)
		{
			GridViewDataTableDataTable table = new GridViewDataTableDataTable();

			try
			{
				GridViewDataTable.BeginInit();
				GridViewDataTable.Clear();
				foreach (var row in table)
				{
					GridViewDataTable.ImportRow(row);
				}
			}
			finally
			{
				GridViewDataTable.EndInit();
			}
		}
	}

	class DataSetReadLocker : IDisposable
	{
		private readonly DataSet1 _dataSet;

		public DataSetReadLocker(DataSet1 dataSet)
		{
			_dataSet = dataSet;

			Monitor.Enter(dataSet);
			try
			{
				Interlocked.Increment(ref _dataSet.readLockCount);
			}
			finally
			{
				Monitor.Exit(dataSet);
			}
		}

		public void Dispose()
		{
			int ret = Interlocked.Decrement(ref _dataSet.readLockCount);
			if (ret < 0)
			{	// まず無いハズだけどまぁ気持ち的に一応…
				_dataSet.readLockCount = 0;
			}
		}
	}

	class DataSetWriteLocker : IDisposable
	{
		private readonly DataSet1 _dataSet;
		public DataSetWriteLocker(DataSet1 dataSet)
		{
			_dataSet = dataSet;
			Monitor.Enter(dataSet);
			while (_dataSet.readLockCount > 0)
				; // うーん…
		}

		public void Dispose()
		{
			Monitor.Exit(_dataSet);
		}
	}

	class GridViewDataRecordBuilder
	{
		private readonly ModelMy _model;
		private readonly DataSet1 _dataSet;
		private readonly ctrl.TreeModelCtrl _treeModelCtrl;
		private readonly ctrl.Dir2FontCtrl _fontCtrl;
		private readonly DataSet1.GridViewDataTableDataTable _table = new DataSet1.GridViewDataTableDataTable();

		public GridViewDataRecordBuilder(ModelMy model, DataSet1 dataSet)
		{
			_model = model;
			_dataSet = dataSet;
			_treeModelCtrl = new ctrl.TreeModelCtrl(model);
			_fontCtrl = new ctrl.Dir2FontCtrl(model);
		}

		public DataSet1.GridViewDataTableDataTable Build()
		{
			Dir dirObj = _dataSet.GetDirectory(_model.rootDirID);
			string rootPath;

			System.IO.DirectoryInfo parent = dirObj.DirectoryInfo.Parent;
			if (parent != null)
			{
				rootPath = parent.FullName;
				rootPath = rootPath.TrimEnd(System.IO.Path.DirectorySeparatorChar) + System.IO.Path.DirectorySeparatorChar;
			}
			else
			{   // parent == null
				rootPath = "::::::"; // use a dummy path
			}

			Insert(_model.rootDirID, rootPath);

			return _table;
		}

		public void Insert(int dirNodeID, string rootPath)
		{
			int[] childDirNodeIDList = _treeModelCtrl.GetChildIndexes(dirNodeID);
			int[] fontFileIDList = _fontCtrl.GetChildIndexes(dirNodeID);

			// === insert dir ===
			Dir dirObj = _dataSet.GetDirectory(dirNodeID);
			string fullPath = dirObj.DirectoryInfo.FullName;
			string dirName = fullPath;
			if (fullPath.StartsWith(rootPath))
			{
				dirName = fullPath.Substring(rootPath.Length);
			}


			_table.AddGridViewDataTableRow(
				IDIssuer.GridViewDataID,
				dirName,
				false,
				(int)GridViewDataType.DIRECTORY,
				dirNodeID
			);

			// === insert fonts ===
			foreach (int fontFileID in fontFileIDList)
			{
				fullPath = _dataSet.GetFontFilePath(fontFileID);
				string fileName = System.IO.Path.GetFileName(fullPath);

				_table.AddGridViewDataTableRow(
					IDIssuer.GridViewDataID,
					fileName,
					false,
					(int)GridViewDataType.FONT,
					fontFileID
				);
			}
		}
	}
}
