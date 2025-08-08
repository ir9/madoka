using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace madoka.ctrl
{
	class TaskCtrl
	{
		private readonly ModelMy _model;

		public TaskCtrl(ModelMy model)
		{
			_model = model;
		}

		public void AddTask(Task task)
		{
			CleanTaskQueue();
			_model.taskList.Enqueue(task);
		}

		public void StartTask(Task task)
		{
			AddTask(task);
			task.Start();
		}

		/// <returns>Queue が空になったら true</returns>
		public bool CleanTaskQueue()
		{
			Queue<Task> taskList = _model.taskList;

			while (taskList.Count > 0)
			{
				Task task = taskList.Peek();
				if (!task.IsCompleted)
					break;
				taskList.Dequeue();
			}

			return taskList.Count == 0;
		}

		public void DisposeTask()
		{
			Task.WaitAll(_model.taskList.ToArray(), 10000);
			if (!CleanTaskQueue())
			{
				_model.cancelToken.Cancel();
				Task.WaitAll(_model.taskList.ToArray(), 10000);
				_model.cancelToken.Dispose();
			}
		}
	}
}
