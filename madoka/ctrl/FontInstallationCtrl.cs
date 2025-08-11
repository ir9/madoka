using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class FontInstallationCtrl
	{
		private struct TargetRecord
		{
			public int fontId;
			public string fontPath;
			public int state; // [i/o]
		};


		public const int NO_OP = -1;
		private delegate int FontActionFunc(TargetRecord target);

		private readonly DataSet1 _dataSet;
		private readonly CancellationToken _cancelToken;
		private readonly IFontInstallingAPI _api;
		private readonly object _lockObject = new object();

		private int _completedCount = 0;

		public FontInstallationCtrl(IFontInstallingAPI api, DataSet1 dataSet, CancellationToken cancelToken)
		{
			_api = api;
			_dataSet = dataSet;
			_cancelToken = cancelToken;
		}

		public int CompletedCount => _completedCount;

		public Task<int[]> InstallFontsAsync(int[] fontIdList)
		{
			return ApplyActionTolFontList(fontIdList, _InstallFont);
		}

		public Task<int[]> UninstallFontsAsync(int[] fontIdList)
		{
			return ApplyActionTolFontList(fontIdList, _UninstallFont);
		}

		private int _InstallFont(TargetRecord target)
		{
			try
			{
				if (target.state != 0)
					return NO_OP;

				string fontPath = target.fontPath;
				int ret;
				using (MemoryMappedFile m = CreateFileMapping(fontPath))
				{
					ret = _api.AddFontResourceEx(fontPath, 0, IntPtr.Zero);
				}

				target.state = ret > 0 ? K.FONTSTATE_INSTALLED : K.FONTSTATE_ERROR;
				return ret;
			}
			catch (IndexOutOfRangeException)
			{
				return 0;
			}
			finally
			{
				Interlocked.Increment(ref _completedCount);
			}
		}

		private int _UninstallFont(TargetRecord target)
		{
			try
			{
				if (target.state == 0) // FONTSTATE_ERROR でもまぁ試してみる
					return NO_OP;

				string fontPath = target.fontPath;
				int ret = _api.RemoveFontResourceEx(fontPath, 0, IntPtr.Zero);

				if (target.state == K.FONTSTATE_INSTALLED)
				{
					if (ret > 0)
					{
						target.state = 0;
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
					target.state = 0; // clear
				}
				return ret;
			}
			catch (IndexOutOfRangeException)
			{
				return NO_OP;
			}
			finally
			{
				Interlocked.Increment(ref _completedCount);
			}
		}

		private Task<int[]> ApplyActionTolFontList(int[] fontIdList, FontActionFunc actionFunc)
		{
			int[] Func()
			{
				using (_dataSet.GetWriteLocker())
				{
					DataSet1.FontFileTableDataTable fontTable = _dataSet.FontFileTable;
					TargetRecord[] targetRecordList = fontIdList.AsParallel().Select((fontId) =>
					{
						// IndexOutOfRangeException
						DataSet1.FontFileTableRow row = fontTable.FindByid(fontId);
						return new TargetRecord()
						{
							fontId = row.id,
							fontPath = row.filepath,
							state = row.state
						};
					}).ToArray();

					int[] retList = targetRecordList.AsParallel()
						.WithCancellation(_cancelToken)
						.Select((target) => actionFunc(target))
						.ToArray();

					foreach (TargetRecord record in targetRecordList)
					{
						DataSet1.FontFileTableRow row = fontTable.FindByid(record.fontId);
						row.state = record.state;
					}

					return retList;
				}
			}

			Task<int[]> task = new Task<int[]>(Func, _cancelToken, TaskCreationOptions.LongRunning);
			task.Start();
			return task;
		}

		static private MemoryMappedFile CreateFileMapping(string file)
		{
			FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
			return MemoryMappedFile.CreateFromFile(fstream, null, 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);
		}
	}
}
