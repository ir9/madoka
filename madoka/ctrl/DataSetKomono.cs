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
		public DataSet1.TagTableRow AddNewTag()
		{
			Tag newTag = new Tag(IDIssuer.TagID);
			using (_dataSet.GetWriteLocker())
			{
				string name = $"New tag name {newTag.ID}";
				var ret = _dataSet.TagTable.AddTagTableRow(newTag.ID, name, newTag);
				_dataSet.TagTable.Version.Inc();
				return ret;
			}
		}

		public void UpdateTagLabel(int tagId, string newLabel)
		{
			using (_dataSet.GetWriteLocker())
			{
				DataSet1.TagTableRow row = _dataSet.TagTable.FindByid(tagId);
				row.name = newLabel;
			}
		}
	}
}
