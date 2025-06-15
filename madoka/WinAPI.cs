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
	}
}
