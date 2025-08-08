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
	}
}
