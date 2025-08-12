using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace madoka
{
	public partial class Form1 : Form
	{
		private ModelMain _model = new ModelMain();
		private FormWindowState _prevState;

		private readonly ctrl.AppConfigSaverCtrl _configSaverCtrl;

		public Form1()
		{
			InitializeComponent();
			Initialize();
			InitializeImageList();

			_configSaverCtrl = new ctrl.AppConfigSaverCtrl(dataSet1, _model);

			ctrl.AppConfigLoaderCtrl configLoader = new ctrl.AppConfigLoaderCtrl(_model);
			configLoader.Load().ContinueWith(
				ApplyConfig,
				_model.cancelToken.Token,
				TaskContinuationOptions.None,
				TaskScheduler.FromCurrentSynchronizationContext()
			);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			_prevState = this.WindowState;
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			ctrl.DataSetKomono dataSetCtrl = new ctrl.DataSetKomono(dataSet1);
			if (dataSetCtrl.HasInstalledFont())
			{
				int[] fontIdList = dataSetCtrl.GetInstalledFontIDs();
				OpenFontInstallDialog(InstallDialogActionType.UNINSTALL, fontIdList);
			}
		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			_configSaverCtrl.WriteAsync(true);

			ctrl.TaskCtrl task = new ctrl.TaskCtrl(_model);
			task.DisposeTask();
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.Hide();
			}
			else
			{
				_prevState = this.WindowState;
			}
		}

		private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.Show();
			this.WindowState = _prevState;
		}

		private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				int mouseClickIndex = e.RowIndex;
				if (!dataGridView1.Rows[mouseClickIndex].Selected)
				{
					dataGridView1.ClearSelection();
					dataGridView1.Rows[mouseClickIndex].Selected = true;
				}
			}
		}

		/* ==================================== *
		 * treeView
		 * ==================================== */
		private void treeView1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void treeView1_DragDrop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;
			string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop);
			ReceivedFilePathList(pathList);
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			SwitchDataGridView(e.Node);
			/*
			gridViewDataTableBindingSource.name
			dataGridView1.DataSource = gridViewDataTableBindingSource;
			dataGridView1.name
			*/
		}

		private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			e.CancelEdit = !(e.Node.Tag is Tag);
		}

		private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			UpdateTagLabel(e.Node, e.Label);
		}

		/* ==================================== *
		 * menu
		 * ==================================== */
		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{	// menuに関する実装では無いけど非常に関連するので…
			if (e.Button == MouseButtons.Right)
			{
				treeView1.SelectedNode = e.Node;
			}
		}

		private void contextMenuFolder_Opening(object sender, CancelEventArgs e)
		{
			TreeNode node = treeView1.SelectedNode;
			AdjustTreeMenuEnableState(node);
		}

		private void menuFolderInstall_Click(object sender, EventArgs e)
		{
			TreeNode node       = treeView1.SelectedNode;
			int[]    fontIdList = CollectFontIds(node);
			OpenFontInstallDialog(InstallDialogActionType.INSTALL, fontIdList);
		}

		private void menuReleaseTemporaryInstallation_Click(object sender, EventArgs e)
		{
			TreeNode node       = treeView1.SelectedNode;
			int[]    fontIdList = CollectFontIds(node);
			OpenFontInstallDialog(InstallDialogActionType.UNINSTALL, fontIdList);
		}

		private void menuNotifyFontInstallationChangeMessage_Click(object sender, EventArgs e)
		{
			OpenFontInstallDialog(InstallDialogActionType.NOTIFY_ONLY, new int[] { });
		}

		private void menuFolderDeleteNode_Click(object sender, EventArgs e)
		{
			TreeNode selectedNode = treeView1.SelectedNode;
			OnDeleteTreeNode(selectedNode);
		}

		private void menuAddNewTag_Click(object sender, EventArgs e)
		{
			AddNewTag();
		}

		// === grid view ===

		private void contextMenuStripGridView_Opening(object sender, CancelEventArgs e)
		{
			AdjustDataGridViewEnabmeState();
		}

		private void menuGridViewDelete_Click(object sender, EventArgs e)
		{
			TreeNode selectedTagNode = treeView1.SelectedNode;
			RemoveFontFromTag(selectedTagNode);
			SwitchDataGridView(selectedTagNode);
		}

		private void menuAddToTag_Submenu_Click(object sender, EventArgs e)
		{
			AddFontToTagGroup(sender);
		}


	}
}
