using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace madoka
{
	class FontFile
	{
		public FontFile(int id, string filePath)
		{
			ID = id;
			FilePath = filePath;
		}

		public int ID { get; }
		public string FilePath { get; }
	}

	class Dir
	{
		public Dir(int id, DirectoryInfo info, int[] fontFileId)
		{
			ID = id;
			DirectoryInfo = info;
			FontFileID = fontFileId;
		}

		public int ID { get; private set; }
		public DirectoryInfo DirectoryInfo { get; }
		public int[] FontFileID { get; }
	}

	class Tag
	{
		public Tag(int id)
		{
			ID = id;
		}

		public int ID { get; }
		public HashSet<int> FontIdList { get; } = new HashSet<int>();
	}

	class DirRoot { };
	class TagRoot { };

	[Flags]
	enum AppState
	{
		NONE = 0,
		CONFIG_LOADED = 0x01,
	};

	class ModelMain
	{
		public Queue<Task> taskList = new Queue<Task>();
		public CancellationTokenSource cancelToken = new CancellationTokenSource();
		public AppState appState = AppState.NONE;

		public bool addFontDirectoryTaskTerminateFlag = true;
		public ConcurrentQueue<string> addFontDirectoryPathList = new ConcurrentQueue<string>();

		public int rootDirID = 0;
		public readonly SortedSet<ctrl.RelationPair> treeRelationModel = new SortedSet<ctrl.RelationPair>();
		public readonly CommitVersion treeRelationModelVersion = new CommitVersion();
		public readonly SortedSet<ctrl.RelationPair> dir2fontRelationModel = new SortedSet<ctrl.RelationPair>();
		public readonly CommitVersion dir2fontRelationModelVersion = new CommitVersion();
		public readonly SortedSet<ctrl.RelationPair> font2TagModel = new SortedSet<ctrl.RelationPair>();
		public readonly CommitVersion font2TagModelVersion = new CommitVersion();
	}
}
