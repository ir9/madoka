using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace madoka
{
	partial class InstallingDialog
	{
		private async void Main()
		{
			bool hasErrorOnInstallPhase = false;
			bool hasErrorOnNotifyPhase = false;

			if (_model.actionType != InstallDialogActionType.NOTIFY_ONLY)
			{
				InitializeInstallPhase();
				hasErrorOnInstallPhase = await _InstallOrUninstallFont();
			}

			bool requireNotifyChangeMessage = GetFontChangeNotifyActionType();
			if (requireNotifyChangeMessage)
			{
				await InitializeNotifyPhase();
				hasErrorOnNotifyPhase = await _eventBroadcasterCtrl.BroadcastMessage();
			}

			await FinishPhase();
			this.Close();
		}

		private void InitializeInstallPhase()
		{
			string text = _model.actionType == InstallDialogActionType.UNINSTALL
				? Properties.Resources.FontInstallationDialog_MessageUninstall
				: Properties.Resources.FontInstallationDialog_MessageInstall;
			text = AddSpecialSuffixIfJP(text);
			labelMessage.Text = text;

			_model.progressCompletedCount = 0;
			_model.progressMaxValue = _model.opFontIdList.Length;
			progressBar1.Value = _model.progressCompletedCount;
			progressBar1.Maximum = _model.progressMaxValue;
		}

		private async Task InitializeNotifyPhase()
		{
			string text = Properties.Resources.FontInstallationDialog_MessageNotify;
			text = AddSpecialSuffixIfJP(text);
			labelMessage.Text = text;

			IntPtr[] hWndList = await _eventBroadcasterCtrl.HWndList;

			_model.progressCompletedCount = 0;
			_model.progressMaxValue = hWndList.Length;
			progressBar1.Value = _model.progressCompletedCount;
			progressBar1.Maximum = _model.progressMaxValue;
			EnableGroupBox(false);
		}

		private async Task FinishPhase()
		{
			buttonCancel.Enabled = false;
			timerUpdate.Enabled = false;
			UpdateProgressMessage();

			await Task.Delay(1000);
		}

		private async Task<bool> _InstallOrUninstallFont()
		{
			int[] retInstallList;

			if (_model.actionType == InstallDialogActionType.INSTALL)
			{
				retInstallList = await _fontInstallCtrl.InstallFontsAsync(_model.opFontIdList);
			}
			else if (_model.actionType == InstallDialogActionType.UNINSTALL)
			{
				retInstallList = await _fontInstallCtrl.UninstallFontsAsync(_model.opFontIdList);
			}
			else
			{
				retInstallList = new int[] { };
			}

			bool hasErrorOnInstallPhase = retInstallList.Any((ret) => ret == 0);
			return hasErrorOnInstallPhase;
		}


		/* ======================= *
		 * util
		 * ======================= */
		private void UpdateProgressMessage()
		{
			int progress = _model.progressCompletedCount;

			progressBar1.Value = progress;
			labelProgress.Text = $"{progress}/{_model.progressMaxValue}";
		}

		private void EnableGroupBox(bool enable)
		{
			groupBoxNoNotify.Enabled = enable;
			radioButtonNoAction.Enabled = enable;
			radioButtonRequireNotify.Enabled = enable;
		}

		private bool GetFontChangeNotifyActionType()
		{
			return radioButtonRequireNotify.Checked;
		}

		static private string GetSpecialSuffix()
		{
			// 一時インストールしている${suffix}
			string[] suffix = {
				"にょ", "にょ", "にゅ", "ゲマ", "んだよもん", "のかしら"
			};
			Random rand = new Random();
			return suffix[rand.Next(suffix.Length)];
		}

		private string AddSpecialSuffixIfJP(string msg)
		{
			if (!U.CultureIsJaJp())
				return msg;

			return msg + _model.specialSuffix;
		}
	}
}
