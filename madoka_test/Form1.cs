using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace madoka_test
{
	public partial class Form1 : Form
	{
		private string[] fontList = {
			"100t.TTF",
			"uni.ttf",
			"unip.ttf",
		};

		public Form1()
		{
			InitializeComponent();
		}

		private void buttonInstallingDialog_Click(object sender, EventArgs e)
		{
			string[] fontPathList = GetFontPathList();
			madoka.DataSet1 dataSet = new madoka.DataSet1();

			int id = 1;
			foreach (string fontPath in fontPathList)
			{
				dataSet.FontFileTable.AddFontFileTableRow(id, fontPath, 0);
				id++;
			}

			madoka.InstallingDialog dlg = new madoka.InstallingDialog(
				new InstallingDialogAPI(),
				madoka.InstallDialogActionType.INSTALL,
				new int[] { 1, 2, 3 },
				dataSet
			);
			dlg.ShowDialog(this);
		}

		private string[] GetFontPathList()
		{
			string fontDir = "./font";

			var it = from name in fontList
					 let relPath = Path.Combine(fontDir, name)
					 select Path.GetFullPath(relPath);

			return it.ToArray();
		}
	}
}
