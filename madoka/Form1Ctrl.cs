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
			dirRoot.Tag = new DirRoot();

			TreeNode tagRoot = FindRootTreeNode(K.TREENODE_NAME_TAG_ROOT);
			tagRoot.ContextMenuStrip = contextMenuFolder;
			tagRoot.ImageIndex = K.IMAGELIST_INDEX_TAG_MULTI;
			tagRoot.SelectedImageIndex = K.IMAGELIST_INDEX_TAG_MULTI;
			tagRoot.Tag = new TagRoot();
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

		private void AdjustTreeMenuEnableState(TreeNode node)
		{
			menuFolderInstall.Enabled = false;
			menuReleaseTemporaryInstallation.Enabled = false;
			menuFolderDeleteNode.Enabled = false;
			menuAddNewTag.Enabled = false;

			if (node.Tag is Dir || node.Tag is Tag || node.Tag is DirRoot)
			{
				menuFolderInstall.Enabled = true;
				menuReleaseTemporaryInstallation.Enabled = true;
			}

			if (node.Tag is Dir || node.Tag is Tag)
			{
				menuFolderDeleteNode.Enabled = node.Level == 1;
			}

			if (node.Tag is TagRoot)
			{
				menuAddNewTag.Enabled = true;
			}
		}

		private void AdjustDataGridViewEnabmeState()
		{
			menuGridViewDelete.Enabled = false;
			if (gridViewDataTableBindingSource.DataSource is DataSet1.TagGridViewTableDataTable)
			{
				menuGridViewDelete.Enabled = true;
			}
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
		 * tag
		 * ------------------------------------------ */
		private void AddNewTag()
		{
			ctrl.DataSetKomono dataSetCtrl = new ctrl.DataSetKomono(dataSet1);
			DataSet1.TagTableRow newRow = dataSetCtrl.AddNewTag();

			TreeNode newNode = new TreeNode();
			newNode.ImageIndex = K.IMAGELIST_INDEX_TAG_SOLO;
			newNode.SelectedImageIndex = K.IMAGELIST_INDEX_TAG_SOLO;
			newNode.Text = newRow.name;
			newNode.Tag = newRow.tagObj;

			TreeNode tagRoot = FindRootTreeNode(K.TREENODE_NAME_TAG_ROOT);
			tagRoot.Nodes.Add(newNode);
			tagRoot.ExpandAll();

			UpdateMenuForTagList();
		}

		/// <summary>
		/// Tag の表示中しかこない
		/// </summary>
		private void RemoveFontFromTag(TreeNode selectedTagNode)
		{
			var gridViewRows = dataGridView1.SelectedRows;

			Tag tag = selectedTagNode.Tag as Tag;
			if (tag == null)
				return;
			DataSet1.TagGridViewTableDataTable table = gridViewDataTableBindingSource.DataSource as DataSet1.TagGridViewTableDataTable;
			if (table == null)
				return;

			int[] rowIndexList = gridViewRows.Cast<DataGridViewRow>().Select((row) => row.Index).ToArray();
			int[] selectedFontIdList = rowIndexList.Select((index) => table[index].fontId).ToArray();

			// === commit ===
			ctrl.Font2TagCtrl f2tCtrl = new ctrl.Font2TagCtrl(_model);
			foreach (int fontId in selectedFontIdList)
			{
				f2tCtrl.RemoveNode(fontId);
			}
			tag.FontIdList.ExceptWith(selectedFontIdList);
		}

		private void UpdateTagLabel(TreeNode node, string newLabel)
		{
			if (newLabel == null)
				return;
			Tag tag = node.Tag as Tag;
			if (tag == null)
				return;

			ctrl.DataSetKomono dataSetCtrl = new ctrl.DataSetKomono(dataSet1);
			dataSetCtrl.UpdateTagLabel(tag.ID, newLabel);

			UpdateMenuForTagList();
		}

		private void UpdateMenuForTagList()
		{
			ToolStripMenuItem Convert(DataSet1.TagTableRow row)
			{
				return new ToolStripMenuItem(row.name, null, menuAddToTag_Submenu_Click)
				{
					Tag = row.tagObj,
				};
			}

			ToolStripMenuItem[] menuItemList;
			using (dataSet1.GetReadLocker())
			{
				menuItemList = dataSet1.TagTable.Select(Convert).ToArray();
			}

			// 何もない
			if (menuItemList.Length <= 0)
			{
				menuItemList = new ToolStripMenuItem[] {
					new ToolStripMenuItem()
					{
						Text = "(None)",
						Enabled = false,
					}
				};
			}

			menuGridViewAddFontToTag.DropDownItems.Clear();
			menuGridViewAddFontToTag.DropDownItems.AddRange(menuItemList);
		}

		private void AddFontToTagGroup(object senderMenu)
		{
			ToolStripMenuItem menuItem = senderMenu as ToolStripMenuItem;
			if (menuItem == null)
				return;
			Tag tag = menuItem.Tag as Tag;
			if (tag == null)
				return;

			var rows = dataGridView1.SelectedRows;
			int[] indexList = rows.Cast<DataGridViewRow>().Select((row) => row.Index).ToArray();

			int[] fontIdList;
			switch (gridViewDataTableBindingSource.DataSource)
			{
			case DataSet1.DirGridViewTableDataTable dirTable:
				fontIdList = GetFontIDsFromDirGridViewTable(dirTable, indexList);
				break;
			case DataSet1.TagGridViewTableDataTable tagTable:
				fontIdList = GetFontIDsFromTagGridViewTable(tagTable, indexList);
				break;
			default: // unkown...
				return;
			}

			if (fontIdList.Length <= 0)
				return;

			// commit
			ctrl.Font2TagCtrl f2tCtrl = new ctrl.Font2TagCtrl(_model);
			f2tCtrl.AddRelation(fontIdList.Select((id) => new ctrl.RelationPair(id, tag.ID)));
			tag.FontIdList.UnionWith(fontIdList);
		}

		private int[] GetFontIDsFromDirGridViewTable(
			DataSet1.DirGridViewTableDataTable table, int[] selectedIndexList)
		{
			var it = from index in selectedIndexList
					 let record = table[index]
					 where record.objectType == (int)GridViewDataType.FONT
					 select record.objectID;
			return it.ToArray();
		}

		private int[] GetFontIDsFromTagGridViewTable(
			DataSet1.TagGridViewTableDataTable table, int[] selectedIndexList)
		{
			var it = from index in selectedIndexList
					 let record = table[index]
					 select record.fontId;
			return it.ToArray();
		}

		/* ------------------------------------------ *
		 * DataGridView
		 * ------------------------------------------ */
		private void SwitchDataGridView(TreeNode selectTreeNode)
		{
			DataGridViewTextBoxColumn[] headerList;
			DataTable dataTable;
			switch (selectTreeNode.Tag)
			{
			case Dir dir:
				(headerList, dataTable) = GetDirDataGridView(dir);
				break;
			case Tag tag:
				(headerList, dataTable) = GetTagDataGridView(tag);
				break;
			default:
				dataTable = null;
				headerList = null;
				break;
			}

			ClearDataGridView();
			if (dataTable == null)
				return;

			dataGridView1.Columns.AddRange(headerList);
			gridViewDataTableBindingSource.DataSource = dataTable;
		}

		private (DataGridViewTextBoxColumn[], DataTable) GetDirDataGridView(Dir selectedDir)
		{
			int width = dataGridView1.Size.Width - 4;
			int fileNameWidth = width * 60 / 100;
			int tagWidth = width - fileNameWidth;

			DataGridViewTextBoxColumn[] headerList = {
				new DataGridViewTextBoxColumn()
				{
					HeaderText = "FileName",
					DataPropertyName = "name",
					ContextMenuStrip = contextMenuStripGridView,
					Width = fileNameWidth,
				},
				new DataGridViewTextBoxColumn()
				{
					HeaderText = "Tag",
					ContextMenuStrip = contextMenuStripGridView,
					Width = tagWidth,
				}
			};

			ctrl.DirGridViewRecordBuilder b = new ctrl.DirGridViewRecordBuilder(_model, dataSet1);
			DataSet1.DirGridViewTableDataTable dataTable = b.Build(selectedDir);

			return (headerList, dataTable);
		}

		private (DataGridViewTextBoxColumn[], DataTable) GetTagDataGridView(Tag selectedTag)
		{
			int width = dataGridView1.Size.Width - 4;
			int nameWidth = width * 30 / 100;
			int pathWith = width - nameWidth;

			DataGridViewTextBoxColumn[] headerList = {
				new DataGridViewTextBoxColumn()
				{
					HeaderText = "Name",
					DataPropertyName = "name",
					ContextMenuStrip = contextMenuStripGridView,
					Width = nameWidth,
				},
				new DataGridViewTextBoxColumn()
				{
					HeaderText = "Path",
					DataPropertyName = "path",
					ContextMenuStrip = contextMenuStripGridView,
					Width = pathWith,
				}
			};

			ctrl.TagGridViewRecordBuilder b = new ctrl.TagGridViewRecordBuilder(dataSet1);
			DataSet1.TagGridViewTableDataTable dataTable = b.Build(selectedTag);

			return (headerList, dataTable);
		}


		private void ClearDataGridView()
		{
			gridViewDataTableBindingSource.DataSource = new object[] { };
			dataGridView1.Columns.Clear();
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
