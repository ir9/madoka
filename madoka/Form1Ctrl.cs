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
		private void Initialize()
		{
			_model.rootDirID = dataSet1.Initialize();
		}

		/* ------------------------------------------ *
		 * TreeView
		 * ------------------------------------------ */
		private void InitializeImageList()
		{
			using (Graphics g = this.CreateGraphics())
			{
				IntPtr[] hIconList = K.IconIndexList.AsParallel().Select(U.LoadIcon).ToArray();

				var iconList = hIconList.AsParallel().Select(Icon.FromHandle).ToList();
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

		private void ReceivedFilePathList(string[] pathList)
		{
			string[] dirList = pathList.Where((f) => Directory.Exists(f)).ToArray();

			if (dirList.Length == 0)
			{
				var dirList2 = from f in pathList
							   let dir = Directory.GetParent(f)
							   select dir.FullName;
				dirList = dirList2.Distinct().ToArray();
			}

			if (dirList.Length == 0)
				return; // no-op

			LaunchScanFontDirectoryTask(dirList);
		}


		private void LaunchScanFontDirectoryTask(string[] pathList)
		{
			Task<ctrl.ScanDirTaskResult> task = ctrl.ScanDirTask.Scan(pathList, _model, dataSet1);
			task?.ContinueWith((prevTask) => RebuildTreeDirectory());
		}

		private void RebuildTreeDirectory()
		{
			ctrl.TreeBuilderDirectory builder = new ctrl.TreeBuilderDirectory(_model, dataSet1);
			TreeNode[] subRootList = builder.Rebuild();

			TreeNode nodeDir = FindRootTreeNode(K.TREENODE_NAME_DIRECTORY_ROOT);
			this.Invoke((Action)(() =>
			{
				nodeDir.Nodes.Clear();
				nodeDir.Nodes.AddRange(subRootList);
				nodeDir.ExpandAll();
			}));
		}

		/* ------------------------------------------ *
		 * DataGridView
		 * ------------------------------------------ */
		private DataGridViewRow[] GetDataGridViewRow(TreeNode selectTreeNode)
		{
			switch (selectTreeNode.Tag)
			{
			case Dir e: return EnumerateDirectories(e);
			}

			// null
			return new DataGridViewRow[] { };
		}

		private DataGridViewRow[] EnumerateDirectories(Dir d)
		{
			ctrl.TreeModelCtrl treeCtrl = new ctrl.TreeModelCtrl(_model);
			int[] dirIDList = treeCtrl.GetChildIndexesRecuresive(d.ID);

			return ctrl.DirGridViewCtrl.GetRows(_model, dataSet1, dirIDList);
		}

		/* ------------------------------------------ *
		 * Util
		 * ------------------------------------------ */
		TreeNode FindRootTreeNode(string name)
		{
			return treeView1.Nodes.Cast<TreeNode>().First((n) => n.Name == name);
		}
	}
}
