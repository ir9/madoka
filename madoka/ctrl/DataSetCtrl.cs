using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	struct TableDirectoryRowCompati
	{
		public int id;
		public Directory dir;
	};

	class DataSetCtrl
	{
		private readonly DataSet _dataSet;

		public DataSetCtrl(DataSet dataSet)
		{
			_dataSet = dataSet;
		}

		public int Init()
		{
			int rootNodeId = IDIssuer.DirectoryID;
			DataSet.DirectoryTableRow root = _dataSet.DirectoryTable.AddDirectoryTableRow(
				rootNodeId, new Directory(null, new System.IO.FileInfo[] { })
			);
			return rootNodeId;
		}

		public void RegsiterDirectory(IEnumerable<TableDirectoryRowCompati> rowList)
		{
			lock (_dataSet)
			{
				foreach (TableDirectoryRowCompati row in rowList)
				{
					_dataSet.DirectoryTable.AddDirectoryTableRow(row.id, row.dir);
				}
			}
		}

	}
}
