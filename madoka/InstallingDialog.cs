using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace madoka
{
	enum InstallDialogActionType
	{
		INSTALL,
		UNINSTALL,
		NOTIFY_ONLY,
	};

	enum InstallingDialogState
	{
		NOTIFY_FONT_CHANGE,
	};

	partial class InstallingDialog : Form
	{
		private const int NO_OP = -1;
		private delegate int FontActionFunc(int fontId);
		private delegate int ResultAnalyzeFunc(int[] retList);

		private readonly IInstallingDialogAPI _api;
		private readonly Task _task;
		private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

		private readonly DataSet1 _dataSet;
		private readonly int[] _opFontIdList;

		public InstallingDialog(
			IInstallingDialogAPI api,
			InstallDialogActionType type,
			int[] operationTargetFontIdList,
			DataSet1 dataSet)
		{
			InitializeComponent();

			_api = api;
			_dataSet = dataSet;
			_opFontIdList = operationTargetFontIdList;

			_task = new Task(Main, TaskCreationOptions.LongRunning);
			_task.Start();
		}

		private void InstallingDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			_task.Wait(10000);
		}

		private void InstallingDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			_cancelToken.Dispose();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			_cancelToken.Cancel();
			buttonCancel.Enabled = false;
		}

		/// <summary>
		/// inboked by worker thread
		/// </summary>
		private void Main()
		{
			ApplyActionTolFontList(InstallFont);
		}

		private void ApplyActionTolFontList(FontActionFunc func)
		{
			using (_dataSet.GetWriteLocker())
			{
				int[] retList = _opFontIdList.AsParallel()
					.WithCancellation(_cancelToken.Token)
					.Select((fontId) => func(fontId))
					.ToArray();
			}
		}

		private int InstallFont(int fontId)
		{
			DataSet1.FontFileTableDataTable fontTable = _dataSet.FontFileTable;
			try
			{
				DataSet1.FontFileTableRow row = fontTable.FindByid(fontId);
				if (row.state != 0)
					return NO_OP;

				string fontPath = row.filepath;
				int ret;
				using (MemoryMappedFile m = CreateFileMapping(fontPath))
				{
					ret = _api.AddFontResourceEx(fontPath, 0, IntPtr.Zero);
				}

				row.state = ret > 0 ? K.FONTSTATE_INSTALLED : K.FONTSTATE_ERROR;
				return ret;
			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
		}

		private int UninstallFont(int fontId)
		{
			DataSet1.FontFileTableDataTable fontTable = _dataSet.FontFileTable;
			try
			{
				DataSet1.FontFileTableRow row = fontTable.FindByid(fontId);
				if (row.state == 0) // FONTSTATE_ERROR でもまぁ試してみる
					return NO_OP;

				string fontPath = row.filepath;
				int ret = _api.RemoveFontResourceEx(fontPath, 0, IntPtr.Zero);

				if (row.state == K.FONTSTATE_INSTALLED)
				{
					if (ret > 0)
					{
						row.state = 0;
					}
					else
					{
						// no-op
					}
				}
				else
				{   // error or other
					if (ret == 0)
					{
						ret = NO_OP;
					}
					row.state = 0; // clear
				}
				return ret;
			}
			catch (IndexOutOfRangeException)
			{
				return NO_OP;
			}
		}

		static private MemoryMappedFile CreateFileMapping(string file)
		{
			FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
			return MemoryMappedFile.CreateFromFile(fstream, null, 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);
		}

	}
}
