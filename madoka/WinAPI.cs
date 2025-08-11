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

		[DllImport("user32.dll")]
		public static extern int PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern int SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, uint fuFlags, uint uTimeout, IntPtr lpdwResult);

		public static readonly IntPtr HWND_BROADCAST = (IntPtr)0xffff;
		public const uint WM_FONTCHANGE = 0x001D;
	}
}
