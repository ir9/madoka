using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
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

		public int ID { get; private set; }
		public string FilePath { get; private set; }
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
		public DirectoryInfo DirectoryInfo { get; private set; }
		public int[] FontFileID { get; private set; }
	}

	[StructLayout(LayoutKind.Explicit)]
	struct RelationPair : IComparable<RelationPair>, IEquatable<RelationPair>
	{
		[FieldOffset(0)]
		private int _child;
		[FieldOffset(4)]
		private int _parent;

		[FieldOffset(0)]
		private long _value;

		public RelationPair(int parent, int child)
		{
			_value = 0;
			this._parent = parent;
			this._child = child;
		}

		public int Parent => _parent;
		public int Child => _child;

		public int CompareTo(RelationPair ro)
		{
			ulong diff = (ulong)(this._value - ro._value);
			uint sign = (uint)((diff & 0x80000000_00000000u) >> 32);
			uint value = (uint)(((diff & 0x7fffffff_ffffffffu) + 0x7fffffff_ffffffffu) >> 63);

			return (int)(sign | value);
		}

		public bool Equals(RelationPair rv)
		{
			return this._value == rv._value;
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
		public readonly SortedSet<RelationPair> treeRelationModel = new SortedSet<RelationPair>();
		public readonly CommitVersion treeRelationModelVersion = new CommitVersion();
		public readonly SortedSet<RelationPair> dir2fontRelationModel = new SortedSet<RelationPair>();
		public readonly CommitVersion dir2fontRelationModelVersion = new CommitVersion();
		// public List<string> fontFolderList;
		// public List<ModelTag> tagList;
	}
}
