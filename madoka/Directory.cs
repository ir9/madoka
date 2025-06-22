using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace madoka
{
	class TreeModel
	{
		class ModelPair: IComparable<ModelPair>
		{
			public int parent;
			public int child;

			public int CompareTo(ModelPair ro)
			{
				int lv = this.parent << 16 | this.child;
				int rv = ro.parent << 16 | ro.child;

				return lv - rv;
			}
		};

		public IReadOnlyList<Directory> DirectoryList { get { return _directoryList; } }
		private readonly List<Directory> _directoryList = new List<Directory>();
		private readonly List<ModelPair> _treeModel = new List<ModelPair>();

		public TreeModel()
		{
			_directoryList.Add(new Directory()); // dummy node
		}

		public int AddDirectory(int parent, Directory dir)
		{
			int currIndex = _directoryList.Count;
			_directoryList.Add(dir);
			_treeModel.Add(new ModelPair { parent = parent, child = currIndex });
			_treeModel.Sort();

			return currIndex;
		}

		public int[] GetChildIndexes(int parent)
		{
			List<ModelPair> treeModel = _treeModel;

			int start = treeModel.BinarySearch(new ModelPair { parent = parent, child = 0 });
			if (start < 0) start = ~start;

			int left = treeModel.Count - start;
			int end = treeModel.BinarySearch(start, left, new ModelPair { parent = parent + 1, child = 0 }, null);
			if (end < 0) end = ~end;

			int length = end - start;
			int[] ret = new int[length];
			for (int i = 0; i < length; ++i)
			{
				ret[i] = treeModel[i + start].child;
			}

			return ret;
		}
	}

	class Directory
	{
		public Directory()
		{

		}

		public DirectoryInfo DirectoryInfo { get; private set; }
		public FileInfo[] FontFileList { get; private set; }
	}
}
