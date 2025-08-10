using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	abstract class AbtractRelationCtrl
	{
		private readonly SortedSet<RelationPair> _relationPair;
		private readonly CommitVersion _version;

		public AbtractRelationCtrl(SortedSet<RelationPair> relationPair, CommitVersion version)
		{
			_relationPair = relationPair;
			_version = version;
		}

		public SortedSet<RelationPair> AddRelation(int parentDir, int[] childDir)
		{
			return AddRelation(childDir.Select(
				(c) => new RelationPair(parentDir, c)
			));
		}

		public SortedSet<RelationPair> AddRelation(IEnumerable<RelationPair> pairList)
		{
			SortedSet<RelationPair> newItemList = new SortedSet<RelationPair>(pairList);
			newItemList.ExceptWith(_relationPair);
			_relationPair.UnionWith(newItemList);

			return newItemList;
		}

		public bool RemoveNode(int parentId, int childId)
		{
			RelationPair pair = new RelationPair(parentId, childId);
			return _relationPair.Remove(pair);
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
