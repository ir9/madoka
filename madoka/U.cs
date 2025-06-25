using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace madoka
{
	class U
	{
		public static IntPtr LoadIcon(int index)
		{
			IntPtr small = IntPtr.Zero;
			int ret = WinAPI.ExtractIconEx("shell32.dll", index, IntPtr.Zero, out small, 1);

			return ret == 0 ? IntPtr.Zero : small;
		}

		public static readonly Regex RE_FONTFILE_EXT_CHECKER = new Regex(@"\.(fon|fnt|ttf|ttc|fot|otf|mmm|pfb|pfm)$", RegexOptions.IgnoreCase);
		public static bool IsFontFile(string path)
		{
			return RE_FONTFILE_EXT_CHECKER.IsMatch(path);
		}

		/*
		public static int ComputeInsertPosition(List<string> arr, string item)
		{
			int index = arr.BinarySearch(item);
			return index >= 0 ? index : ~index;
		}
		*/
	}
}
