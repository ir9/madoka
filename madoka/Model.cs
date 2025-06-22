using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka
{
	class ModelTag
	{
		public string name;
		public List<string> fontPathList;
	}

	class Model
	{
		public List<string> fontFolderList;
		public List<ModelTag> tagList;
	}
}
