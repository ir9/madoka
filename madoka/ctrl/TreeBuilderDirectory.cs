using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class TreeBuilderDirectory
	{
		private readonly DataSet1 _dataSet;
		private readonly ModelMy _model;

		private readonly TreeModelCtrl _treeModelCtrl;

		public TreeBuilderDirectory(ModelMy model, DataSet1 dataSet)
		{
			_dataSet = dataSet;
			_model = model;

			_treeModelCtrl = new TreeModelCtrl(model);
		}

		/// <summary>
		/// Directory ノード直下の TreeItem を再構築して返します
		/// </summary>
		/// <returns></returns>
		public TreeNode[] Rebuild()
		{
			int[] childNodeIdList = _treeModelCtrl.GetChildIndexes(_model.rootDirID);
			return SortNodeList(childNodeIdList.Select((childId) => BuildNode(childId, true)));
		}

		private TreeNode BuildNode(int nodeId, bool isSubRoot)
		{
			Dir dirObj = _dataSet.GetDirectory(nodeId);

			TreeNode node = new TreeNode();
			node.Text = isSubRoot ? dirObj.DirectoryInfo.FullName : dirObj.DirectoryInfo.Name;
			node.Tag  = dirObj;

			int[] childNodeIdList = _treeModelCtrl.GetChildIndexes(dirObj.ID);
			node.Nodes.AddRange(
				SortNodeList(childNodeIdList.Select((childId) => BuildNode(childId, false)))
			);
			return node;
		}

		static private TreeNode[] SortNodeList(IEnumerable<TreeNode> nodeList)
		{
			return nodeList.OrderBy((node) => node.Name).ToArray();
		}
	}
}
