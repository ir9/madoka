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

		public bool Contain(RelationPair rv)
		{
			return _relationPair.Contains(rv);
		}

		public int[] GetChildIndexes(int parent)
		{
			SortedSet<RelationPair> childItemList = _relationPair.GetViewBetween(
				new RelationPair(parent + 0, 0),
				new RelationPair(parent + 1, 0)
			);

			return childItemList.Select((c) => c.Child).ToArray();
		}

		public int IncVersion()
		{
			return _version.Inc();
		}
	}
}
