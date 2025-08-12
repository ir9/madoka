using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Data;
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

			// フォームデザイナで設定しても TreeView あたりの設定が何故か消える…
			TreeNode dirRoot = FindRootTreeNode(K.TREENODE_NAME_DIRECTORY_ROOT);
			dirRoot.ContextMenuStrip = contextMenuFolder;
			dirRoot.ImageIndex = K.IMAGELIST_INDEX_DRIVE;
			dirRoot.SelectedImageIndex = K.IMAGELIST_INDEX_DRIVE;

			TreeNode tagRoot = FindRootTreeNode(K.TREENODE_NAME_TAG_ROOT);
			tagRoot.ImageIndex = K.IMAGELIST_INDEX_TAG_MULTI;
			tagRoot.SelectedImageIndex = K.IMAGELIST_INDEX_TAG_SOLO;
		}

		private void ApplyConfig(Task<ctrl.AppConfig> loaderTask)
		{
			ctrl.AppConfig config = loaderTask.Result;
			if (config != null)
			{
				if (!U.IsNullOrEmpty(config.RootFontDirList))
				{
					ReceivedFilePathList(config.RootFontDirList);
				}
			}

			_model.appState |= AppState.CONFIG_LOADED;
		}

		private void OnDeleteTreeNode(TreeNode selectedTreeNode)
		{
			if (selectedTreeNode.Tag is Dir)
			{
				DeleteRootFontDirectory(selectedTreeNode);
			}
		}

		private void OpenFontInstallDialog(InstallDialogActionType actionType, int[] fontIdList)
		{
			VerityFontIds(fontIdList);
			InstallingDialog dlg = new InstallingDialog(
				new FontInstallingAPI(),
				actionType,
				fontIdList,
				dataSet1
			);
			dlg.ShowDialog(this);
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
			ctrl.DataSetKomono ctrlKomono = new ctrl.DataSetKomono(dataSet1);
			string[] newDirList = ctrlKomono.ComputeNewRootFontDir(dirList);

			LaunchScanFontDirectoryTask(newDirList);
		}


		private void LaunchScanFontDirectoryTask(string[] pathList)
		{
			Task<ctrl.ScanDirTaskResult> task = ctrl.ScanDirTask.Scan(pathList, _model, dataSet1);
			task?.ContinueWith((prevTask) => RebuildTreeDirectory());
		}

		private void RebuildTreeDirectory()
		{
			ctrl.TreeBuilderDirectory builder = new ctrl.TreeBuilderDirectory(_model, dataSet1, contextMenuFolder);
			TreeNode[] subRootList = builder.Rebuild();

			TreeNode nodeDir = FindRootTreeNode(K.TREENODE_NAME_DIRECTORY_ROOT);
			this.Invoke((Action)(() =>
			{
				nodeDir.Nodes.Clear();
				nodeDir.Nodes.AddRange(subRootList);
				nodeDir.ExpandAll();
			}));
		}

		private void DeleteRootFontDirectory(TreeNode node)
		{
			Dir dir = node.Tag as Dir;
			ctrl.TreeModelCtrl tree = new ctrl.TreeModelCtrl(_model);
			tree.RemoveNode(_model.rootDirID, dir.ID);
		}

		// ----
		/// <returns>null が帰りえます</returns>
		private int[] CollectFontIds(TreeNode node)
		{
			int dirId;
			if (node.Tag is Dir dir)
			{
				dirId = dir.ID;
			}
			else if (node.Name == K.TREENODE_NAME_DIRECTORY_ROOT)
			{
				dirId = _model.rootDirID;
			}
			else
			{
				return null;
			}

			ctrl.TreeModelCtrl treeCtrl = new ctrl.TreeModelCtrl(_model);
			int[] dirIdList = treeCtrl.GetChildIndexesRecuresive(dirId);

			ctrl.Dir2FontCtrl fontCtrl = new ctrl.Dir2FontCtrl(_model);
			int[] fontIdList = dirIdList.SelectMany((id) => fontCtrl.GetChildIndexes(id)).ToArray();

			return fontIdList;
		}


		/* ------------------------------------------ *
		 * DataGridView
		 * ------------------------------------------ */
		private void GetDataGridViewRow(TreeNode selectTreeNode)
		{
			switch (selectTreeNode.Tag)
			{
			case Dir dir:
				{
					SwitchToDirDataGridView(dir);
				}
				break;
			default:
				break;
			}

		}

		private void SwitchToDirDataGridView(Dir selectedDir)
		{
			gridViewDataTableBindingSource.DataSource = new object[] { };
			dataGridView1.Columns.Clear();
			dataGridView1.Columns.Add(new DataGridViewTextBoxColumn()
			{
				HeaderText = "FileName",
				DataPropertyName = "name",
			});

			DirGridViewDataRecordBuilder b = new DirGridViewDataRecordBuilder(_model, dataSet1);
			DataSet1.DirGridViewDataTableDataTable dataTable = b.Build(selectedDir);
			gridViewDataTableBindingSource.DataSource = dataTable;
		}

		/* ------------------------------------------ *
		 * Util
		 * ------------------------------------------ */
		private TreeNode FindRootTreeNode(string name)
		{
			return treeView1.Nodes.Cast<TreeNode>().First((n) => n.Name == name);
		}

		/* ------------------------------------------ *
		 * Debug
		 * ------------------------------------------ */
		[Conditional("DEBUG")]
		private void VerityFontIds(int[] fontIds)
		{
			using (dataSet1.GetReadLocker())
			{
				HashSet<int> existsFontIdList = new HashSet<int>(dataSet1.FontFileTable.Select((row) => row.id));
				var it = from id in fontIds.AsParallel()
						 let contain = existsFontIdList.Contains(id)
						 where !contain
						 select id;
				int[] notContainIDList = it.ToArray();
				if (notContainIDList.Length > 0)
				{
					throw new IndexOutOfRangeException(
						"FontID が不正です。DBに存在しません: " + string.Join(",", notContainIDList)
					);
				}
			}
		}
	}
}
