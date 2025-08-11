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
			InitializeInstallPhase();
			bool hasErrorOnInstallPhase = await _InstallOrUninstallFont();

			bool requireNotifyChangeMessage = true; // GetFontChangeNotifyActionType();
			if (requireNotifyChangeMessage)
			{
				InitializeNotifyPhase();
				bool hasErrorOnNotifyPhase = await _BroadCastFontChange();
			}
		}

		private void InitializeInstallPhase()
		{
			string text = _actionType == InstallDialogActionType.UNINSTALL
				? Properties.Resources.FontInstallationDialog_MessageUninstall
				: Properties.Resources.FontInstallationDialog_MessageInstall;

			text = AddSpecialSuffixIfJP(text);
			labelMessage.Text = text;
		}

		private void InitializeNotifyPhase()
		{
			string text = Properties.Resources.FontInstallationDialog_MessageNotify;
			text = AddSpecialSuffixIfJP(text);
			labelMessage.Text = text;

			EnableGroupBox(false);
			progressBar1.Style = ProgressBarStyle.Marquee;
			timerUpdate.Enabled = false;
			labelProgress.Text = "";
		}

		private async Task<bool> _InstallOrUninstallFont()
		{
			int[] retInstallList;
			progressBar1.Maximum = _opFontIdList.Length;

			if (_actionType == InstallDialogActionType.INSTALL)
			{
				retInstallList = await _fontInstallCtrl.InstallFontsAsync(_opFontIdList);
			}
			else if (_actionType == InstallDialogActionType.UNINSTALL)
			{
				retInstallList = await _fontInstallCtrl.UninstallFontsAsync(_opFontIdList);
			}
			else
			{
				retInstallList = new int[] { };
			}

			bool hasErrorOnInstallPhase = retInstallList.Any((ret) => ret == 0);
			return hasErrorOnInstallPhase;
		}

		private Task<bool> _BroadCastFontChange()
		{
			bool Func()
			{
				return _api.PostMessage(WinAPI.HWND_BROADCAST, WinAPI.WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero) != 0;
			}

			Task<bool> task = new Task<bool>(
				Func,
				_cancelToken.Token,
				TaskCreationOptions.LongRunning
			);
			task.Start();

			return task;
		}

		/* ======================= *
		 * util
		 * ======================= */
		private void UpdateProgressMessage()
		{
			int progress = _fontInstallCtrl.CompletedCount;

			progressBar1.Value = progress;
			labelProgress.Text = $"{progress}/{_opFontIdList.Length}";
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
			// 一時インストールしている
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

			return msg + _specialSuffix;
		}
	}
}
