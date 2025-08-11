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
		private delegate int ResultAnalyzeFunc(int[] retList);

		private readonly IFontInstallingAPI _api;
		private readonly Task _task;
		private readonly InstallDialogActionType _actionType;
		private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
		private readonly ctrl.FontInstallationCtrl _fontInstallCtrl;

		private readonly DataSet1 _dataSet;
		private readonly int[] _opFontIdList;

		public InstallingDialog(
			IFontInstallingAPI api,
			InstallDialogActionType type,
			int[] operationTargetFontIdList,
			DataSet1 dataSet)
		{
			InitializeComponent();

			_api = api;
			_dataSet = dataSet;
			_actionType = type;
			_opFontIdList = operationTargetFontIdList;

			_fontInstallCtrl = new ctrl.FontInstallationCtrl(dataSet, _cancelToken.Token);

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

		private void timerUpdate_Tick(object sender, EventArgs e)
		{
			UpdateProgressMessage();
		}

		/// <summary>
		/// inboked by worker thread
		/// </summary>
		private void Main()
		{
			int[] retInstallList;
			if (_actionType == InstallDialogActionType.INSTALL)
			{
				retInstallList = _fontInstallCtrl.InstallFonts(_opFontIdList);
			}
			else if (_actionType == InstallDialogActionType.UNINSTALL)
			{
				retInstallList = _fontInstallCtrl.UninstallFonts(_opFontIdList);
			}
		}
	}
}
