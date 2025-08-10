using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka_test
{
	class InstallingDialogAPI : madoka.IInstallingDialogAPI
	{
		public int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv)
		{
			return 0;
		}

		public int RemoveFontResourceEx(string name, uint fl, IntPtr pdv)
		{
			return 0;
		}
	}
}
