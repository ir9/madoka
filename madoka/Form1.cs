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
		private ModelMy _model = new ModelMy();
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
			GetDataGridViewRow(e.Node);
			/*
			gridViewDataTableBindingSource.name
			dataGridView1.DataSource = gridViewDataTableBindingSource;
			dataGridView1.name
			*/
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
			menuFolderDeleteNode.Enabled = false;
			if (node.Tag is Dir)
			{
				menuFolderDeleteNode.Enabled = node.Level == 1;
			}
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
	}
}
