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

		public void AddDirRelation(int parentDir, int[] childDir)
		{
			AddDirRelation(childDir.Select(
				(c) => new TreeModelPair() { parent = parentDir, child = c })
			);
		}

		public void AddDirRelation(IEnumerable<TreeModelPair> pairList)
		{
			_model.treeRelationModel.AddRange(pairList);
			_model.treeRelationModel.Sort();
		}

		public bool Contain(TreeModelPair rv)
		{
			return _model.treeRelationModel.BinarySearch(rv) >= 0;
		}

		public int[] GetChildIndexes(int parent)
		{
			List<TreeModelPair> treeModel = _model.treeRelationModel;

			int start = treeModel.BinarySearch(new TreeModelPair { parent = parent, child = 0 });
			if (start < 0) start = ~start;

			int left = treeModel.Count - start;
			int end = treeModel.BinarySearch(start, left, new TreeModelPair { parent = parent + 1, child = 0 }, null);
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
}
