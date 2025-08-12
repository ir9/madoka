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
		private class TargetRecord
		{
			public int fontId;
			public string fontPath;
			public int state; // [i/o]
		};

		public const int NO_OP = -1;
		private delegate int FontActionFunc(TargetRecord target);

		private readonly InstallingDialogModel _model;
		private readonly object _lockObject = new object();

		public FontInstallationCtrl(InstallingDialogModel model)
		{
			_model = model;
		}

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
					ret = _model.api.AddFontResourceEx(fontPath, 0, IntPtr.Zero);
				}

				target.state = ret > 0 ? K.FONTSTATE_INSTALLED : K.FONTSTATE_ERROR;
				return ret;
			}
			finally
			{
				Interlocked.Increment(ref _model.progressCompletedCount);
			}
		}

		private int _UninstallFont(TargetRecord target)
		{
			try
			{
				if (target.state == 0) // FONTSTATE_ERROR でもまぁ試してみる
					return NO_OP;

				string fontPath = target.fontPath;
				int ret = _model.api.RemoveFontResourceEx(fontPath, 0, IntPtr.Zero);

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
			finally
			{
				Interlocked.Increment(ref _model.progressCompletedCount);
			}
		}

		private Task<int[]> ApplyActionTolFontList(int[] fontIdList, FontActionFunc actionFunc)
		{
			DataSet1.FontFileTableDataTable fontTable = _model.dataSet.FontFileTable;
			TargetRecord Convert(int fontId)
			{
				DataSet1.FontFileTableRow row = fontTable.FindByid(fontId);
				try
				{
					return new TargetRecord()
					{
						fontId = row.id,
						fontPath = row.filepath,
						state = row.state
					};
				}
				catch (IndexOutOfRangeException)
				{
					return null;
				}
			}

			int[] Func()
			{
				using (_model.dataSet.GetWriteLocker())
				{
					// extract values
					var it = from fontId in fontIdList.AsParallel().WithCancellation(_model.cancelToken.Token)
							 let ret = Convert(fontId)
							 where ret != null
							 select ret;
					TargetRecord[] targetRecordList = it.ToArray();

					// action
					int[] retList = targetRecordList.AsParallel()
						.WithCancellation(_model.cancelToken.Token)
						.Select((target) => actionFunc(target))
						.ToArray();

					// reflect results to original records
					foreach (TargetRecord record in targetRecordList)
					{
						DataSet1.FontFileTableRow row = fontTable.FindByid(record.fontId);
						row.state = record.state;
					}

					return retList;
				}
			}

			Task<int[]> task = new Task<int[]>(Func, _model.cancelToken.Token, TaskCreationOptions.LongRunning);
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
