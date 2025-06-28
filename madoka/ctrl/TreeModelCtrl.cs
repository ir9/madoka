using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class TreeModelCtrl
	{
		private readonly ModelMy _model;

		public TreeModelCtrl(ModelMy model)
		{
			_model = model;
		}

		public SortedSet<RelationPair> AddDirRelation(int parentDir, int[] childDir)
		{
			return AddDirRelation(childDir.Select(
				(c) => new RelationPair(parentDir, c)
			));
		}

		public SortedSet<RelationPair> AddDirRelation(IEnumerable<RelationPair> pairList)
		{
			SortedSet<RelationPair> newItemList = new SortedSet<RelationPair>(pairList);
			newItemList.ExceptWith(_model.treeRelationModel);
			_model.treeRelationModel.UnionWith(newItemList);

			return newItemList;
		}

		public bool Contain(RelationPair rv)
		{
			return _model.treeRelationModel.Contains(rv);
		}

		public int[] GetChildIndexes(int parent)
		{
			SortedSet<RelationPair> treeModel = _model.treeRelationModel;
			SortedSet<RelationPair> childItemList = treeModel.GetViewBetween(
				new RelationPair(parent + 0, 0),
				new RelationPair(parent + 1, 0)
			);

			return childItemList.Select((c) => c.Child).ToArray();
		}
	}
}
