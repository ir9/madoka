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
		IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);
		int EnumWindow(WinAPI.EnumWindowsProc lpEnumFunc, IntPtr lParam);
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

		public IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult)
		{
			return WinAPI.SendMessageTimeout(hWnd, msg, wParam, lParam, fuFlags, uTimeout, lpdwResult);
		}

		public int EnumWindow(WinAPI.EnumWindowsProc lpEnumFunc, IntPtr lParam)
		{
			return WinAPI.EnumWindows(lpEnumFunc, lParam);
		}
	}
}
