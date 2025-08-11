using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka
{
	interface IFontInstallingAPI
	{
		int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

		int RemoveFontResourceEx(string name, uint fl, IntPtr pdv);

		int PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
	};

	class FontInstallingAPI : IFontInstallingAPI
	{
		public int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv)
		{
			return WinAPI.AddFontResourceEx(lpszFilename, fl, pdv);
		}

		public int RemoveFontResourceEx(string name, uint fl, IntPtr pdv)
		{
			return WinAPI.RemoveFontResourceEx(name, fl, pdv);
		}

		public int PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
		{
			return WinAPI.PostMessage(hWnd, msg, wParam, lParam);
		}
	}
}
