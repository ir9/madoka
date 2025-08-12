using System;
using System.Runtime.InteropServices;

namespace madoka
{
	static class WinAPI
	{
		[DllImport("Shell32.dll")]
		public static extern int ExtractIconEx(string sFile, int iIndex, IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

		[DllImport("user32.dll")]
		public static extern bool DestroyIcon(IntPtr handle);

		/// <returns>1以上で成功</returns>
		[DllImport("gdi32.dll")]
		public static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

		/// <returns>1以上で成功</returns>
		[DllImport("gdi32.dll")]
		public static extern int RemoveFontResourceEx(string name, uint fl, IntPtr pdv);

		/// <returns>関数が成功すると、0 以外の値が返ります</returns>
		[DllImport("user32.dll")]
		public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);
		public const uint SMTO_ABORTIFHUNG = 0x0002;

		[DllImport("user32.dll")]
		public static extern int EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
		public delegate int EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
		public const uint WM_FONTCHANGE = 0x001D;
	}
}
