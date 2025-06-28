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
	delegate void MainThradInvoker(Action func);

	public partial class Form1 : Form
	{
		private ModelMy _model = new ModelMy();

		public Form1()
		{
			InitializeComponent();
			InitializeImageList();
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e)
		{
			ctrl.TaskCtrl task = new ctrl.TaskCtrl(_model);
			task.DisposeTask();
		}

		private void Form1_SizeChanged(object sender, EventArgs e)
		{
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.Hide();
			}
		}

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
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop);
			string[] dirList = fileList.Where((f) => Directory.Exists(f)).ToArray();

			if (dirList.Length == 0)
			{
				var dirList2 = from f in fileList
							   let dir = Directory.GetParent(f)
							   select dir.FullName;
				dirList = dirList2.Distinct().ToArray();
			}

			if (dirList.Length == 0)
				return; // no-op

			LaunchScanFontDirectoryTask(dirList);
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			/*
			switch (e.Node.Tag)
			{
			case Dir d;
				break;
			case null;
				break;
			}
			*/
		}
	}
}
