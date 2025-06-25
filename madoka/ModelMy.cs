using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace madoka
{
	class Dir
	{
		public Dir(int id, DirectoryInfo info, FileInfo[] fontList)
		{
			ID = id;
			DirectoryInfo = info;
			FontFileList = fontList;
		}

		public int ID { get; private set; }
		public DirectoryInfo DirectoryInfo { get; private set; }
		public FileInfo[] FontFileList { get; private set; }
	}

	struct TreeModelPair : IComparable<TreeModelPair>
	{
		public int parent;
		public int child;

		public int CompareTo(TreeModelPair ro)
		{   // 本当は 64bit で処理すべきだけど…まぁ 65536 とか登録しないやろう…ダルい
			int lv = this.parent << 16 | this.child;
			int rv = ro.parent << 16 | ro.child;

			return lv - rv;
		}
	};

	/*
	class ModelTag
	{
		public string name;
		public List<string> fontPathList;
	}
	*/

	class ModelMy
	{
		public Queue<Task> taskList = new Queue<Task>();
		public CancellationTokenSource cancelToken = new CancellationTokenSource();

		public bool addFontDirectoryTaskTerminateFlag = true;
		public ConcurrentQueue<string> addFontDirectoryPathList = new ConcurrentQueue<string>();

		public int rootDirID = 0;
		public readonly List<TreeModelPair> treeRelationModel = new List<TreeModelPair>();
		// public List<string> fontFolderList;
		// public List<ModelTag> tagList;
	}
}
