using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace madoka
{
	static class IDIssuer
	{
		static private int _directoryId = 0;
		static public int DirectoryID => Interlocked.Increment(ref _directoryId);

		static private int _fontFileId = 0;
		static public int FontFileID = Interlocked.Increment(ref _fontFileId);

		static private int _gridViewDataId = 0;
		static public int GridViewDataID = Interlocked.Increment(ref _gridViewDataId);
	}
}
