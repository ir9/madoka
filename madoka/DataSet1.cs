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

		partial class TagTableDataTable
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
				FontFileTable.AddFontFileTableRow(row.ID, row.FilePath, 0);
			}
		}

		internal string GetFontFilePath(int fontID)
		{
			FontFileTableRow row = FontFileTable.FindByid(fontID);
			return row.filepath;
		}

		internal Dictionary<string, int> CreatePath2FontFileID()
		{
			using (GetReadLocker())
			{
				return FontFileTable.ToDictionary(
					(r) => r.filepath,
					(r) => r.id
				);
			}
		}

		/* ========================================= *
		 * GridViewDataTable
		 * ========================================= */
		internal void RebuildGridViewDataTable(ModelMain model)
		{
			DirGridViewTableDataTable table = new DirGridViewTableDataTable();

			try
			{
				table.BeginInit();
				// table.Clear();
				foreach (var row in table)
				{
					table.ImportRow(row);
				}
			}
			finally
			{
				table.EndInit();
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
			{   // まず無いハズだけどまぁ気持ち的に一応…
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
}
