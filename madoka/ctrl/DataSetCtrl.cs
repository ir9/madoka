using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class DataSetCtrl
	{
		private readonly DataSet1 _dataSet;

		public DataSetCtrl(DataSet1 dataSet)
		{
			_dataSet = dataSet;
		}

		public int Init()
		{
			int rootNodeId = IDIssuer.DirectoryID;
			DataSet1.DirectoryTableRow root = _dataSet.DirectoryTable.AddDirectoryTableRow(
				rootNodeId, new Dir(rootNodeId, null, new int[] { })
			);
			return rootNodeId;
		}

		public void RegisterDirectoryList(IEnumerable<Dir> rowList)
		{
			lock (_dataSet.DirectoryTable)
			{
				foreach (Dir row in rowList)
				{
					_dataSet.DirectoryTable.AddDirectoryTableRow(row.ID, row);
				}
			}
		}

		public void RegisterFontFileList(IEnumerable<FontFile> rowList)
		{
			lock (_dataSet.FontFileTable)
			{
				foreach (FontFile row in rowList)
				{
					_dataSet.FontFileTable.AddFontFileTableRow(row.ID, row.FilePath, row.OwnerDirID);
				}
			}
		}

		public Dir GetDirectory(int nodeId)
		{
			DataSet1.DirectoryTableRow row = (DataSet1.DirectoryTableRow)_dataSet.DirectoryTable.Rows.Find(nodeId);
			return (Dir)row.directory;
		}
	}
}
