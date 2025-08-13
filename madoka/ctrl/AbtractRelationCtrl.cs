using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
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

	abstract class AbtractRelationCtrl
	{
		private readonly SortedSet<RelationPair> _relationPair;
		private readonly CommitVersion _version;

		public AbtractRelationCtrl(SortedSet<RelationPair> relationPair, CommitVersion version)
		{
			_relationPair = relationPair;
			_version = version;
		}

		public SortedSet<RelationPair> AddRelation(int parent, int child)
		{
			return AddRelation(new RelationPair[] { new RelationPair(parent, child) });
		}

		public SortedSet<RelationPair> AddRelation(int parent, int[] childList)
		{
			return AddRelation(childList.Select(
				(c) => new RelationPair(parent, c)
			));
		}

		public SortedSet<RelationPair> AddRelation(IEnumerable<RelationPair> pairList)
		{
			SortedSet<RelationPair> newItemList = new SortedSet<RelationPair>(pairList);
			newItemList.ExceptWith(_relationPair);
			_relationPair.UnionWith(newItemList);

			return newItemList;
		}

		public int RemoveNode(int parentId)
		{
			SortedSet<RelationPair> range = GetChildIndexesRaw(parentId);
			RelationPair[] removeItems = range.ToArray();
			_relationPair.ExceptWith(removeItems);
			return removeItems.Length;
		}

		public bool RemoveNode(int parentId, int childId)
		{
			RelationPair pair = new RelationPair(parentId, childId);
			return _relationPair.Remove(pair);
		}

		public void RemoveNode(IEnumerable<RelationPair> pairList)
		{
			_relationPair.ExceptWith(pairList);
		}

		public bool Contain(RelationPair rv)
		{
			return _relationPair.Contains(rv);
		}

		private SortedSet<RelationPair> GetChildIndexesRaw(int parentId)
		{
			SortedSet<RelationPair> childItemList = _relationPair.GetViewBetween(
				new RelationPair(parentId, 0),
				new RelationPair(parentId, int.MaxValue)
			);

			return childItemList;
		}

		public int[] GetChildIndexes(int parentId)
		{
			SortedSet<RelationPair> childItemList = GetChildIndexesRaw(parentId);
			return childItemList.Select((c) => c.Child).ToArray();
		}

		/// <remarks>
		/// Child と言いつつ parent も結果に入ってる…
		/// </remarks>
		public int[] GetChildIndexesRecuresive(int parent)
		{
			List<int> ret = new List<int>();

			void follow(int p)
			{
				ret.Add(p);

				int[] childIndexes = GetChildIndexes(p);
				foreach (int child in childIndexes)
				{
					follow(child);
				}
			}

			follow(parent);
			return ret.ToArray();
		}

		public int IncVersion()
		{
			return _version.Inc();
		}
	}
}
