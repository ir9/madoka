using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka
{
	static class K
	{
		static public readonly int[] IconIndexList = {
			7, // drive
			3, // folder
			134, // multi file
			70, // solo file
		};

		public const int FOLDER_ROOT = 0;
		public const int FOLDER_LEAF = 1;
		public const int TAG_ROOT = 2;
		public const int TAG_LEAF = 3;

		public const string TREENODE_NAME_DIRECTORY_ROOT = "DirectoryRoot";
	}

	enum GridViewDataType: int
	{
		DIRECTORY,
		FONT,
	};
}
