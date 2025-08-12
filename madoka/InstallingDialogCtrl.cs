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

			bool requireNotifyChangeMessage =
				_model.actionType == InstallDialogActionType.NOTIFY_ONLY ||
				GetFontChangeNotifyActionType();
			if (requireNotifyChangeMessage)
			{
				InitializeNotifyPhase();
				hasErrorOnNotifyPhase = await _BroadCastFontChange();
			}

			this.Close();
		}

		private void InitializeInstallPhase()
		{
			string text = _model.actionType == InstallDialogActionType.UNINSTALL
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
			progressBar1.Maximum = _model.opFontIdList.Length;

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

		private async Task<bool> _BroadCastFontChange()
		{
			IntPtr[] hWndList = await GetTopLevelWindowHandles();

			// return: 失敗した hWnd
			IntPtr func(IntPtr hWnd)
			{
				IntPtr Zero = IntPtr.Zero;
				IntPtr ret = _model.api.SendMessageTimeout(
					hWnd, WinAPI.WM_FONTCHANGE, Zero, Zero, WinAPI.SMTO_ABORTIFHUNG, 10000, Zero
				);

				return ret == Zero ? hWnd : Zero;
			}

			Task<bool> task = new Task<bool>(() =>
			{
				var it = from hWnd in hWndList.AsParallel().WithCancellation(_model.cancelToken.Token)
						 let ret = func(hWnd)
						 where ret != IntPtr.Zero
						 select ret;
				IntPtr[] failedhWndList = it.ToArray(); // 将来的に失敗した hWnd の情報を料理したい
				return failedhWndList.Length == 0;
			}, _model.cancelToken.Token);
			task.Start();

			return await task;
		}

		private Task<IntPtr[]> GetTopLevelWindowHandles()
		{
			List<IntPtr> hWndList = new List<IntPtr>();
			int EnumProc(IntPtr hWnd, IntPtr lParam)
			{
				if (_model.cancelToken.IsCancellationRequested)
					return 0;

				hWndList.Add(hWnd);
				return 1;
			}

			IntPtr[] func()
			{
				_model.api.EnumWindow(EnumProc, IntPtr.Zero);
				return hWndList.ToArray();
			}

			Task<IntPtr[]> task = new Task<IntPtr[]>(func, _model.cancelToken.Token);
			task.Start();
			return task;
		}

		/* ======================= *
		 * util
		 * ======================= */
		private void UpdateProgressMessage()
		{
			int progress = _model.completedCount;

			progressBar1.Value = progress;
			labelProgress.Text = $"{progress}/{_model.opFontIdList.Length}";
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

			return msg + _model.specialSuffix;
		}
	}
}
