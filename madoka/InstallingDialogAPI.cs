using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka
{
	interface IInstallingDialogAPI
	{
		int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

		int RemoveFontResourceEx(string name, uint fl, IntPtr pdv);
	};

	class InstallingDialogAPI : IInstallingDialogAPI
	{
		public int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv)
		{
			return WinAPI.AddFontResourceEx(lpszFilename, fl, pdv);
		}

		public int RemoveFontResourceEx(string name, uint fl, IntPtr pdv)
		{
			return WinAPI.RemoveFontResourceEx(name, fl, pdv);
		}
	}
}
