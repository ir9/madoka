using System;
using System.Collections.Generic;
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
		TreeNode FindRootTreeNode(string name)
		{
			return treeView1.Nodes.Cast<TreeNode>().First((n) => n.Name == name);
		}
	}
}
