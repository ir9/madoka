namespace madoka
{
	partial class Form1
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.gridViewDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.contextMenuFolder = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.menuFolderInstall = new System.Windows.Forms.ToolStripMenuItem();
			this.menuReleaseTemporaryInstallation = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuFolderDeleteNode = new System.Windows.Forms.ToolStripMenuItem();
			this.menuNotifyFontInstallationChangeMessage = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.dataSet1 = new madoka.DataSet1();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewDataTableBindingSource)).BeginInit();
			this.contextMenuFolder.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.treeView1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
			// 
			// treeView1
			// 
			this.treeView1.AllowDrop = true;
			resources.ApplyResources(this.treeView1, "treeView1");
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Name = "treeView1";
			this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("treeView1.Nodes1")))});
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
			this.treeView1.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			resources.ApplyResources(this.imageList1, "imageList1");
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// dataGridView1
			// 
			this.dataGridView1.AllowUserToAddRows = false;
			this.dataGridView1.AllowUserToDeleteRows = false;
			this.dataGridView1.AutoGenerateColumns = false;
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.DataSource = this.gridViewDataTableBindingSource;
			resources.ApplyResources(this.dataGridView1, "dataGridView1");
			this.dataGridView1.MultiSelect = false;
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.ReadOnly = true;
			this.dataGridView1.RowHeadersVisible = false;
			this.dataGridView1.RowTemplate.Height = 21;
			this.dataGridView1.ShowCellErrors = false;
			this.dataGridView1.ShowEditingIcon = false;
			this.dataGridView1.ShowRowErrors = false;
			// 
			// contextMenuFolder
			// 
			this.contextMenuFolder.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFolderInstall,
            this.menuReleaseTemporaryInstallation,
            this.toolStripSeparator1,
            this.menuFolderDeleteNode,
            this.menuNotifyFontInstallationChangeMessage});
			this.contextMenuFolder.Name = "contextMenuFolder";
			resources.ApplyResources(this.contextMenuFolder, "contextMenuFolder");
			this.contextMenuFolder.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuFolder_Opening);
			// 
			// menuFolderInstall
			// 
			resources.ApplyResources(this.menuFolderInstall, "menuFolderInstall");
			this.menuFolderInstall.Name = "menuFolderInstall";
			this.menuFolderInstall.Click += new System.EventHandler(this.menuFolderInstall_Click);
			// 
			// menuReleaseTemporaryInstallation
			// 
			this.menuReleaseTemporaryInstallation.Name = "menuReleaseTemporaryInstallation";
			resources.ApplyResources(this.menuReleaseTemporaryInstallation, "menuReleaseTemporaryInstallation");
			this.menuReleaseTemporaryInstallation.Click += new System.EventHandler(this.menuReleaseTemporaryInstallation_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			// 
			// menuFolderDeleteNode
			// 
			this.menuFolderDeleteNode.Name = "menuFolderDeleteNode";
			resources.ApplyResources(this.menuFolderDeleteNode, "menuFolderDeleteNode");
			this.menuFolderDeleteNode.Click += new System.EventHandler(this.menuFolderDeleteNode_Click);
			// 
			// menuNotifyFontInstallationChangeMessage
			// 
			this.menuNotifyFontInstallationChangeMessage.Name = "menuNotifyFontInstallationChangeMessage";
			resources.ApplyResources(this.menuNotifyFontInstallationChangeMessage, "menuNotifyFontInstallationChangeMessage");
			this.menuNotifyFontInstallationChangeMessage.Click += new System.EventHandler(this.menuNotifyFontInstallationChangeMessage_Click);
			// 
			// statusStrip1
			// 
			resources.ApplyResources(this.statusStrip1, "statusStrip1");
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1});
			this.statusStrip1.Name = "statusStrip1";
			// 
			// toolStripProgressBar1
			// 
			this.toolStripProgressBar1.Name = "toolStripProgressBar1";
			resources.ApplyResources(this.toolStripProgressBar1, "toolStripProgressBar1");
			this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			// 
			// tableLayoutPanel1
			// 
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.statusStrip1, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			// 
			// dataSet1
			// 
			this.dataSet1.DataSetName = "DataSet";
			this.dataSet1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// Form1
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "Form1";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewDataTableBindingSource)).EndInit();
			this.contextMenuFolder.ResumeLayout(false);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ContextMenuStrip contextMenuFolder;
		private System.Windows.Forms.ToolStripMenuItem menuFolderInstall;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuFolderDeleteNode;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private DataSet1 dataSet1;
		private System.Windows.Forms.BindingSource gridViewDataTableBindingSource;
		private System.Windows.Forms.ToolStripMenuItem menuReleaseTemporaryInstallation;
		private System.Windows.Forms.ToolStripMenuItem menuNotifyFontInstallationChangeMessage;
	}
}

