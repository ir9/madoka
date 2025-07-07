using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace madoka
{
	class CommitVersion
	{
		private int _version = 0;

		public int Inc()
		{
			return ++_version;
		}

		public static implicit operator int(CommitVersion c)
		{
			return c._version;
		}

		public int Get => _version;

		public void Test(int expect)
		{
			int current = this;
			if (current != expect)
				throw new TableVersionMismatchException(current, expect);
		}
	};

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

		public static R HandleTableVersionMismatchException<R>(Func<R> func)
		{
			return HandleTableVersionMismatchException(func, 4);
		}

		public static R HandleTableVersionMismatchException<R>(Func<R> func, int maxRetryCount)
		{
			for (int i = 0; ; ++i)
			{
				try
				{
					return func();
				}
				catch (TableVersionMismatchException)
				{
					if (i >= maxRetryCount)
						throw;
				}
			}
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
