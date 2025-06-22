using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace madoka
{
	partial class Form1
	{
		private void InitializeImageList()
		{
			using (Graphics g = this.CreateGraphics())
			{
				IntPtr[] hIconList = C.IconIndexList.AsParallel().Select(U.LoadIcon).ToArray();

				var iconList = hIconList.AsParallel().Select(Icon.FromHandle);
				foreach (Icon icon_ in iconList)
				{
					using (Icon icon = icon_)
					{
						imageList1.Images.Add(icon);
					}
				}

				hIconList.AsParallel().Select(WinAPI.DestroyIcon).ToArray();
			}
		}

		private void AddFontDirectory(string path)
		{
			TreeNode dirNode = treeView1.Nodes["Directory"];
			Task task = Task.Run(() =>
			{
				DirectoryInfo rootDir = new DirectoryInfo(path);
				TreeNode newNode = GetDirChildNode(rootDir);

				this.Invoke(new Action(() =>
				{
					InsertNewNode(dirNode, newNode);
					dirNode.ExpandAll();
				}));
			}, _cancelToken.Token);
			AddTask(task);
		}

		private TreeNode GetDirChildNode(DirectoryInfo currDir)
		{
			_cancelToken.Token.ThrowIfCancellationRequested();

			TreeNode[] childNodeList = currDir.GetDirectories()
				.Select(GetDirChildNode)
				.Where((ret) => ret != null)
				.ToArray();
			bool hasFontFile = currDir.GetFiles().Any(U.IsFontFile);

			if (childNodeList.Length == 0 && !hasFontFile)
				return null;

			return new TreeNode(currDir.Name, C.FOLDER_LEAF, C.FOLDER_LEAF, childNodeList);
		}

		private void InsertNewNode(TreeNode parent, TreeNode newItem)
		{
			List<string> nameList = parent.Nodes.Cast<TreeNode>().Select((node) => node.Name).ToList();
			int index = U.ComputeInsertPosition(nameList, newItem.Name);
			parent.Nodes.Insert(index, newItem);
		}
	}
}
