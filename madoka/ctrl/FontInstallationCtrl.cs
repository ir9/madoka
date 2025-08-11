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
		public const int NO_OP = -1;
		private delegate int FontActionFunc(int fontId);

		private readonly DataSet1 _dataSet;
		private readonly CancellationToken _cancelToken;

		private int _completedCount = 0;

		public FontInstallationCtrl(DataSet1 dataSet, CancellationToken cancelToken)
		{
			_dataSet = dataSet;
			_cancelToken = cancelToken;
		}

		public int CompletedCount => _completedCount;

		public Task<int[]> InstallFontsAsync(int[] fontIdList)
		{
			Task<int[]> task = new Task<int[]>(
				() => ApplyActionTolFontList(fontIdList, _InstallFont),
				TaskCreationOptions.LongRunning
			);
			task.Start();

			return task;
		}

		public Task<int[]> UninstallFontsAsync(int[] fontIdList)
		{
			return ApplyActionTolFontList(fontIdList, _UninstallFont);
		}

		private int _InstallFont(int fontId)
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
			finally
			{
				Interlocked.Increment(ref _completedCount);
			}
		}

		private int _UninstallFont(int fontId)
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
			finally
			{
				Interlocked.Increment(ref _completedCount);
			}
		}

		private int[] ApplyActionTolFontList(int[] fontIdList, FontActionFunc func)
		{
			using (_dataSet.GetWriteLocker())
			{
				return fontIdList.AsParallel()
					.WithCancellation(_cancelToken)
					.Select((fontId) => func(fontId))
					.ToArray();
			}
		}

		static private MemoryMappedFile CreateFileMapping(string file)
		{
			FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
			return MemoryMappedFile.CreateFromFile(fstream, null, 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);
		}
	}
}
