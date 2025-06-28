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
			ctrl.DataSetCtrl dataSet = new ctrl.DataSetCtrl(this.dataSet1);
			_model.rootDirID = dataSet.Init();
		}

		/* ------------------------------------------ *
		 * TreeView
		 * ------------------------------------------ */
		private void InitializeImageList()
		{
			using (Graphics g = this.CreateGraphics())
			{
				IntPtr[] hIconList = K.IconIndexList.AsParallel().Select(U.LoadIcon).ToArray();

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

		private void LaunchScanFontDirectoryTask(string[] pathList)
		{
			Task<ctrl.ScanDirTaskResult> task = ctrl.ScanDirTask.Scan(pathList, _model, dataSet1);
			task?.ContinueWith((prevTask) => RebuildTreeDirectory());
		}

		private void RebuildTreeDirectory()
		{
			TreeNode nodeDir = FindRootTreeNode(K.TREENODE_NAME_DIRECTORY_ROOT);
			ctrl.TreeBuilderDirectory builder = new ctrl.TreeBuilderDirectory(nodeDir, _model, dataSet1);
			builder.Rebuild();
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
			return null;
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
