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
		private readonly InstallDialogActionType _actionType;
		private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
		private readonly string _specialSuffix = GetSpecialSuffix();
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
			Initialize();

			_api = api;
			_dataSet = dataSet;
			_actionType = type;
			_opFontIdList = operationTargetFontIdList;

			_fontInstallCtrl = new ctrl.FontInstallationCtrl(api, dataSet, _cancelToken.Token);
			Main();
		}

		private void Initialize()
		{	// form で text を空にすると、編集しづらくなるんじゃなぁ…
			labelMessage.Text = "";
			labelProgress.Text = "";
			this.Text = Properties.Resources.FontInstallationDialog_Title;

			if (_actionType == InstallDialogActionType.UNINSTALL)
			{
				groupBoxNoNotify.Text = Properties.Resources.FontInstallationDialog_GroupBoxUninstallLabel;
				radioButtonNoAction.Text = Properties.Resources.FontInstallationDialog_GroupBoxUninstallNoActionLabel;
				radioButtonRequireNotify.Text = Properties.Resources.FontInstallationDialog_GroupBoxUninstallRequireActionLabel;
			}
			else
			{   // install OR notify
				groupBoxNoNotify.Text = Properties.Resources.FontInstallationDialog_GroupBoxInstallLabel;
				radioButtonNoAction.Text = Properties.Resources.FontInstallationDialog_GroupBoxInstallNoActionLabel;
				radioButtonRequireNotify.Text = Properties.Resources.FontInstallationDialog_GroupBoxInstallRequireActionLabel;
			}
		}

		private void InstallingDialog_FormClosing(object sender, FormClosingEventArgs e)
		{

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
	}
}
