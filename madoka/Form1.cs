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
		private Model _model = new Model();
		private Queue<Task> _taskList = new Queue<Task>();
		private CancellationTokenSource _cancelToken = new CancellationTokenSource();

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
			_cancelToken.Cancel();
			Task.WaitAll(_taskList.ToArray(), 10000);

			_cancelToken.Dispose();
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
		}

		private void AddTask(Task task)
		{
			CleanTaskQueue();
			_taskList.Enqueue(task);
		}

		private void CleanTaskQueue()
		{
			if (_taskList.Count > 0)
			{   // remove completed tasks
				Task prev = _taskList.Peek();
				while (prev.IsCompleted)
				{
					_taskList.Dequeue();
					prev = _taskList.Peek();
				}
			}
		}
	}
}
