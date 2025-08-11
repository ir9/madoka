using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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

		public int PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			Thread.Sleep(4000);
			return 1;
		}
	}
}
