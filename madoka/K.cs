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

		public const int FONTSTATE_INSTALLED = 0x01;
		public const int FONTSTATE_ERROR = 0x80;

		public const string TREENODE_NAME_DIRECTORY_ROOT = "DirectoryRoot";
	}

	enum GridViewDataType: int
	{
		DIRECTORY,
		FONT,
	};
}
