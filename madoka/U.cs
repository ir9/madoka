using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
	}
}
