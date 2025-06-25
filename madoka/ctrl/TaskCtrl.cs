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

		public void CleanTaskQueue()
		{
			Queue<Task> taskList = _model.taskList;

			if (taskList.Count > 0)
			{   // remove completed tasks
				Task prev = taskList.Peek();
				while (prev.IsCompleted)
				{
					taskList.Dequeue();
					prev = taskList.Peek();
				}
			}
		}

		public void DisposeTask()
		{
			_model.cancelToken.Cancel();
			Task.WaitAll(_model.taskList.ToArray(), 10000);
			_model.cancelToken.Dispose();
		}
	}
}
