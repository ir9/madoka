using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using madoka;

namespace madoka_test
{
	class InstallingDialogAPI : madoka.IFontInstallingAPI
	{
		public int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv)
		{
			Thread.Sleep(3000);
			return 1;
		}

		public int RemoveFontResourceEx(string name, uint fl, IntPtr pdv)
		{
			Thread.Sleep(1000);
			return 1;
		}

		public IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult)
		{
			return (IntPtr)1;
		}

		public int EnumWindow(WinAPI.EnumWindowsProc lpEnumFunc, IntPtr lParam)
		{
			return 1;
		}
	}
}
