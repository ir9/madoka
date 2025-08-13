using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class DataSetKomono
	{
		private readonly DataSet1 _dataSet;

		public DataSetKomono(DataSet1 dataSet)
		{
			_dataSet = dataSet;
		}

		public string[] ComputeNewRootFontDir(string[] dirPathList)
		{
			return dirPathList.Except(_dataSet.RootFontDirTable.Select((r) => r.path)).ToArray();
		}

		/* ----------------------------- *
		 * font
		 * ----------------------------- */
		public void AppendNewRootFontDir(IEnumerable<string> dirPathList)
		{
			foreach (string dir in dirPathList)
			{
				try
				{
					_dataSet.RootFontDirTable.AddRootFontDirTableRow(dir);
				}
				catch (System.Data.ConstraintException)
				{
					// pass
				}
			}
		}

		public void RegisterFontList(IEnumerable<string> fontPathList)
		{
			using (_dataSet.GetWriteLocker())
			{
				foreach (string path in fontPathList)
				{
					// System.Data.ConstraintException?
					_dataSet.FontFileTable.AddFontFileTableRow(IDIssuer.FontFileID, path, 0);
				}
				_dataSet.FontFileTable.Version.Inc();
			}
		}

		public bool HasInstalledFont()
		{
			using (_dataSet.GetReadLocker())
			{
				return _dataSet.FontFileTable.AsParallel().Any((r) => r.state != 0);
			}
		}

		public int[] GetInstalledFontIDs()
		{
			using (_dataSet.GetReadLocker())
			{
				var it = from r in _dataSet.FontFileTable.AsParallel()
						 where r.state != 0
						 select r.id;
				return it.ToArray();
			}
		}

		/* ------------------------- *
		 * tag
		 * ------------------------- */
		public DataSet1.TagTableRow AddNewTag(string label = null)
		{
			Tag newTag = new Tag(IDIssuer.TagID);
			using (_dataSet.GetWriteLocker())
			{
				if(label == null)
					label = $"New tag name {newTag.ID}";
				var ret = _dataSet.TagTable.AddTagTableRow(newTag.ID, label, newTag);
				_dataSet.TagTable.Version.Inc();
				return ret;
			}
		}

		public void UpdateTagLabel(int tagID, string newLabel)
		{
			using (_dataSet.GetWriteLocker())
			{
				DataSet1.TagTableRow row = _dataSet.TagTable.FindByid(tagID);
				row.name = newLabel;
			}
		}

		public void RemoveTag(int tagID)
		{
			using (_dataSet.GetWriteLocker())
			{
				DataSet1.TagTableDataTable tagTable = _dataSet.TagTable;

				DataSet1.TagTableRow row = tagTable.FindByid(tagID);
				tagTable.RemoveTagTableRow(row);
				tagTable.Version.Inc();
			}
		}
	}
}
