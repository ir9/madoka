using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Text;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace install_test
{
	class Program
	{
		[DllImport("gdi32.dll")]
		static extern int AddFontResourceEx(string lpszFilename, uint fl, IntPtr pdv);

		[DllImport("gdi32.dll")]
		static extern int RemoveFontResourceEx(string name, uint fl, IntPtr pdv);

		private DateTime _start;

		public Program()
		{

		}

		private int ZO(int v)
		{
			return v > 0 ? 1 : 0;
		}


		private DateTime Sequential(string[] fileList)
		{
			int addCount = 0;
			foreach (string file in fileList)
			{
				addCount += ZO(AddFontResourceEx(file, 0, IntPtr.Zero));
			}

			Console.WriteLine($"{addCount} fonts added. // { TotalSecs() }[ms]");
			DateTime checkPoint = DateTime.Now;

			int remCount = 0;
			foreach (string file in fileList)
			{
				remCount += ZO(RemoveFontResourceEx(file, 0, IntPtr.Zero));
			}
			Console.WriteLine($"{remCount} fonts removed. // { TotalSecs() }[ms]");

			return checkPoint;
		}

		private DateTime Parallel(string[] fileList)
		{
			var addCount = fileList.AsParallel().Select((file) =>
			{
				return ZO(AddFontResourceEx(file, 0, IntPtr.Zero));
			}).Sum();
			Console.WriteLine($"{addCount} fonts added. // { TotalSecs() }[ms]");

			DateTime checkPoint = DateTime.Now;
			var remCount = fileList.AsParallel().Select((file) =>
			{
				return ZO(RemoveFontResourceEx(file, 0, IntPtr.Zero));
			}).Sum();
			Console.WriteLine($"{remCount} fonts removed. // { TotalSecs() }[ms]");

			return checkPoint;
		}

		private DateTime ParallelMapped00(string[] fileList)
		{
			var addCount = fileList.AsParallel().Select((file) =>
			{
				using (MemoryMappedFile m = CreateFileMapping(file))
				{
					return ZO(AddFontResourceEx(file, 0, IntPtr.Zero));
				}
			}).Sum();
			Console.WriteLine($"{addCount} fonts added. // { TotalSecs() }[ms]");

			DateTime checkPoint = DateTime.Now;
			var remCount = fileList.AsParallel().Select((file) =>
			{
				return ZO(RemoveFontResourceEx(file, 0, IntPtr.Zero));
			}).Sum();
			Console.WriteLine($"{remCount} fonts removed. // { TotalSecs() }[ms]");

			return checkPoint;
		}

		struct Mapped01
		{
			public Mapped01(string file, MemoryMappedFile mem)
			{
				this.file = file;
				this.mem = mem;
			}

			public string file { get; }
			public MemoryMappedFile mem { get; }
		}

		class SingleTaskScheduler : TaskScheduler, IDisposable
		{
			private readonly Thread thread;
			private readonly BlockingCollection<Task> taskQueue = new BlockingCollection<Task>();

			public SingleTaskScheduler()
			{
				thread = new Thread(new ThreadStart(ExecuteTasks));
				// thread.IsBackground = true;
				thread.Start();
			}

			protected override IEnumerable<Task> GetScheduledTasks()
			{
				return taskQueue;
			}

			protected override void QueueTask(Task task)
			{
				taskQueue.Add(task);
			}

			protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
			{
				if (Thread.CurrentThread == thread)
				{
					return TryExecuteTask(task);
				}
				return false;
			}

			private void ExecuteTasks()
			{
				while (!taskQueue.IsCompleted)
				{
					if (taskQueue.TryTake(out Task task))
					{
						TryExecuteTask(task);
					}
				}
			}

			public void Complete()
			{
				taskQueue.CompleteAdding();
			}

			#region IDisposable Support
			private bool disposedValue = false; // 重複する呼び出しを検出するには

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue)
				{
					if (disposing)
					{
						// TODO: マネージド状態を破棄します (マネージド オブジェクト)。
						thread.Join(1000);
						taskQueue.Dispose();
					}

					// TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
					// TODO: 大きなフィールドを null に設定します。
					disposedValue = true;
				}
			}

			// TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
			// ~SingleTaskScheduler() {
			//   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
			//   Dispose(false);
			// }

			// このコードは、破棄可能なパターンを正しく実装できるように追加されました。
			public void Dispose()
			{
				// このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
				Dispose(true);
				// TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
				// GC.SuppressFinalize(this);
			}
			#endregion
		};

		private DateTime ParallelMapped01(string[] fileList)
		{
			using (SingleTaskScheduler single = new SingleTaskScheduler())
			{
				TaskFactory factory = new TaskFactory(single);

				async Task<int> load(string file)
				{
					using (var mem = await factory.StartNew(
						() => CreateFileMapping(file)
					))
					{
						return ZO(AddFontResourceEx(file, 0, IntPtr.Zero));
					}
				}
				int addCount = fileList.AsParallel().Select(load).Sum((task) => task.Result);
				Console.WriteLine($"{addCount} fonts added. // { TotalSecs() }[ms]");

				DateTime checkPoint = DateTime.Now;
				var remCount = fileList.AsParallel().Select((file) =>
				{
					return ZO(RemoveFontResourceEx(file, 0, IntPtr.Zero));
				}).Sum();
				Console.WriteLine($"{remCount} fonts removed // { TotalSecs() }[ms]");

				single.Complete();
				return checkPoint;
			}
		}

		private double TotalSecs()
		{
			TimeSpan span = DateTime.Now - _start;
			return span.TotalSeconds;
		}

		private MemoryMappedFile CreateFileMapping(string file)
		{
			FileStream fstream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
			return MemoryMappedFile.CreateFromFile(fstream, null, 0, MemoryMappedFileAccess.Read, null, HandleInheritability.None, false);
		}

		private void _Main(string[] args)
		{
			string[] fileList = File.ReadAllLines("fontlist.txt", Encoding.GetEncoding(932)).Select((line) => line.Trim()).ToArray();
			Console.WriteLine($"{fileList.Length} fonts found.");
			_start = DateTime.Now;

			DateTime checkPoint = DateTime.Now;
			switch (args[0])
			{
			case "-s":  checkPoint = Sequential(fileList); break;
			case "-p":  checkPoint = Parallel(fileList); break;
			case "-m0": checkPoint = ParallelMapped00(fileList); break;
			case "-m1": checkPoint = ParallelMapped01(fileList); break;
			}

			DateTime end = DateTime.Now;
			TimeSpan addSpan   = end - checkPoint;
			TimeSpan totalSpan = end - _start;

			// Console.WriteLine($"add: {addSpan.TotalSeconds:F3}[ms]");
			Console.WriteLine($"total: {totalSpan.TotalSeconds:F3}[ms]");
		}

		static void Main(string[] args)
		{
			new Program()._Main(args);
		}
	}
}