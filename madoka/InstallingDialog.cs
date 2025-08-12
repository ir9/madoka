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

	partial class InstallingDialog : Form
	{
		private delegate int ResultAnalyzeFunc(int[] retList);

		private readonly InstallingDialogModel _model;
		private readonly ctrl.FontInstallationCtrl _fontInstallCtrl;
		private readonly ctrl.FontChangeBroadcasterCtrl _eventBroadcasterCtrl;

		public InstallingDialog(
			IFontInstallingAPI api,
			InstallDialogActionType type,
			int[] operationTargetFontIdList,
			DataSet1 dataSet)
		{
			_model = new InstallingDialogModel()
			{
				api = api,
				dataSet = dataSet,
				actionType = type,
				opFontIdList = operationTargetFontIdList,
				specialSuffix = GetSpecialSuffix(),
			};

			InitializeComponent();
			Initialize();

			_fontInstallCtrl = new ctrl.FontInstallationCtrl(_model);
			_eventBroadcasterCtrl = new ctrl.FontChangeBroadcasterCtrl(_model);

			Main();
		}

		private void Initialize()
		{	// form で text を空にすると、編集しづらくなるんじゃなぁ…
			labelMessage.Text = "";
			labelProgress.Text = "";
			this.Text = Properties.Resources.FontInstallationDialog_Title;

			if (_model.actionType == InstallDialogActionType.UNINSTALL)
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

			if (_model.actionType == InstallDialogActionType.NOTIFY_ONLY)
			{
				radioButtonRequireNotify.Checked = true;
			}
		}

		private void InstallingDialog_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void InstallingDialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			_model.cancelToken.Dispose();
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			_model.cancelToken.Cancel();
			buttonCancel.Enabled = false;
		}

		private void timerUpdate_Tick(object sender, EventArgs e)
		{
			UpdateProgressMessage();
		}
	}
}
