using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace madoka
{
	class Directory
	{
		public Directory(DirectoryInfo info, FileInfo[] fontList)
		{
			DirectoryInfo = info;
			FontFileList = fontList;
		}

		public DirectoryInfo DirectoryInfo { get; private set; }
		public FileInfo[] FontFileList { get; private set; }
	}
}
