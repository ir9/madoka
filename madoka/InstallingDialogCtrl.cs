using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;

namespace madoka
{
	partial class InstallingDialog
	{
		private async void Main()
		{
			bool hasErrorOnInstallPhase = await _InstallOrUninstallFont();
		}

		/// <summary>
		/// 
		/// </summary>
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

		private bool _BroadCastFontChange()
		{
			return _api.PostMessage(WinAPI.HWND_BROADCAST, WinAPI.WM_FONTCHANGE, IntPtr.Zero, IntPtr.Zero) != 0;
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
	}
}
