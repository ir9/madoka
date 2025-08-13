using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka
{
	static class K
	{
		static public readonly string ConfigFileName = "appconfig.xml";
		static public readonly string ConfigTmpFileName = ConfigFileName + ".tmp";
		static public readonly string ConfigBackFileName = ConfigFileName + ".bak";
		static public readonly string AppName = "madoka";
		static private string _AppConfigPath;
		static public string AppConfigPath
		{
			get {
				if (_AppConfigPath == null)
				{
					_AppConfigPath = U.CreateAppConfigPath();
				}
				return _AppConfigPath;
			}
		}


		static public readonly int[] IconIndexList = {
			7, // drive
			3, // folder
			134, // multi file
			70, // solo file
		};
		public const int IMAGE_HEIGHT = 16;
		public const int IMAGE_WIDTH  = 16;

		public const int IMAGELIST_INDEX_DRIVE = 0;
		public const int IMAGELIST_INDEX_FOLDER = 1;
		public const int IMAGELIST_INDEX_TAG_MULTI = 2;
		public const int IMAGELIST_INDEX_TAG_SOLO = 3;

		public const int FONTSTATE_INSTALLED = 0x01;
		public const int FONTSTATE_ERROR = 0x80;

		public const string TREENODE_NAME_DIRECTORY_ROOT = "DirectoryRoot";
		public const string TREENODE_NAME_TAG_ROOT = "TagRoot";
	}

	enum GridViewDataType: int
	{
		DIRECTORY,
		FONT,
	};
}
