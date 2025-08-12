using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class FontChangeBroadcasterCtrl
	{
		private readonly InstallingDialogModel _model;

		public FontChangeBroadcasterCtrl(InstallingDialogModel model)
		{
			_model = model;
			HWndList = GetTopLevelWindowHandles(model);
		}

		public Task<IntPtr[]> HWndList { get; }

		public async Task<bool> BroadcastMessage()
		{
			bool success = await SendMessage(HWndList.Result);
			return success;
		}

		/// <returns>問題無ければ true</returns>
		private Task<bool> SendMessage(IntPtr[] hWndList)
		{
			/// <returns>成功したら true</returns>
			bool SendMessage(IntPtr hWnd)
			{
				IntPtr Zero = IntPtr.Zero;
				IntPtr ret = _model.api.SendMessageTimeout(
					hWnd, WinAPI.WM_FONTCHANGE, Zero, Zero, WinAPI.SMTO_ABORTIFHUNG, 10000, Zero
				);

				Interlocked.Increment(ref _model.progressCompletedCount);
				return ret != Zero;
			}

			bool Main()
			{
				var it = from hWnd in hWndList.AsParallel().WithCancellation(_model.cancelToken.Token)
						 let success = SendMessage(hWnd)
						 where !success
						 select hWnd;
				IntPtr[] failedhWndList = it.ToArray(); // 将来的に失敗した hWnd の情報を料理したい
				return failedhWndList.Length == 0;
			}

			// === main ===
			Task<bool> task = new Task<bool>(Main, _model.cancelToken.Token);
			task.Start();
			return task;
		}

		private Task<IntPtr[]> GetTopLevelWindowHandles(InstallingDialogModel model)
		{
			List<IntPtr> hWndList = new List<IntPtr>();
			int EnumProc(IntPtr hWnd, IntPtr lParam)
			{
				if (model.cancelToken.IsCancellationRequested)
					return 0;

				hWndList.Add(hWnd);
				return 1;
			}

			IntPtr[] func()
			{
				model.api.EnumWindow(EnumProc, IntPtr.Zero);
				return hWndList.ToArray();
			}

			Task<IntPtr[]> task = new Task<IntPtr[]>(func, model.cancelToken.Token);
			task.Start();
			return task;
		}
	}
}
