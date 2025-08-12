using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class TagGridViewRecordBuilder
	{
		class TmpRecord
		{
			public string name;
			public string path;
			public int fontID;
		};

		private readonly DataSet1 _dataSet;

		public TagGridViewRecordBuilder(DataSet1 dataSet)
		{
			_dataSet = dataSet;
		}

		public DataSet1.TagGridViewTableDataTable Build(Tag tag)
		{
			TmpRecord[] recordList;
			using (_dataSet.GetReadLocker())
			{
				var it = from fontID in tag.FontIdList.AsParallel()
						 let conv = Convert(fontID)
						 where conv != null
						 orderby conv.name
						 select conv;
				recordList = it.ToArray();
			}

			DataSet1.TagGridViewTableDataTable table = new DataSet1.TagGridViewTableDataTable();
			foreach (TmpRecord record in recordList)
			{
				table.AddTagGridViewTableRow(
					IDIssuer.GridViewDataID,
					record.name,
					record.path,
					record.fontID
				);
			}
			return table;
		}

		private TmpRecord Convert(int fontID)
		{
			try
			{
				DataSet1.FontFileTableRow row = _dataSet.FontFileTable.FindByid(fontID);
				FileInfo file = new FileInfo(row.filepath);
				return new TmpRecord()
				{
					name = file.Name,
					path = file.DirectoryName,
					fontID = fontID,
				};
			}
			catch (IndexOutOfRangeException)
			{
				return null;
			}
		}
	}
}
