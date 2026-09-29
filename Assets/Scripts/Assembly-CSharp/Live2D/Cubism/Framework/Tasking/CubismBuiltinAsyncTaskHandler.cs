using System.Collections.Generic;
using System.Threading;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Tasking
{
	public static class CubismBuiltinAsyncTaskHandler
	{
		private static bool _callItADay;

		private static Queue<ICubismTask> Tasks { get; set; }

		private static Thread Worker { get; set; }

		private static object Lock { get; set; }

		private static ManualResetEvent Signal { get; set; }

		private static bool CallItADay
		{
			get
			{
				lock (Lock)
				{
					return _callItADay;
				}
			}
			set
			{
				lock (Lock)
				{
					_callItADay = value;
				}
			}
		}

		public static void Activate()
		{
			if (CubismTaskQueue.OnTask != null && CubismTaskQueue.OnTask != new CubismTaskQueue.CubismTaskHandler(EnqueueTask))
			{
				Debug.LogWarning("\"CubismTaskQueue.OnTask\" already set.");
				return;
			}
			Tasks = new Queue<ICubismTask>();
			Worker = new Thread(Work);
			Lock = new object();
			Signal = new ManualResetEvent(initialState: false);
			CallItADay = false;
			CubismTaskQueue.OnTask = EnqueueTask;
			Worker.Start();
		}

		public static void Deactivate()
		{
			if (!(CubismTaskQueue.OnTask != new CubismTaskQueue.CubismTaskHandler(EnqueueTask)))
			{
				CubismTaskQueue.OnTask = null;
				CallItADay = true;
				if (Worker != null)
				{
					Signal.Set();
					Worker.Join();
				}
				Tasks = null;
				Worker = null;
				Lock = null;
				Signal = null;
			}
		}

		private static void EnqueueTask(ICubismTask task)
		{
			lock (Lock)
			{
				Tasks.Enqueue(task);
				Signal.Set();
			}
		}

		private static ICubismTask DequeueTask()
		{
			lock (Lock)
			{
				return (Tasks.Count > 0) ? Tasks.Dequeue() : null;
			}
		}

		private static void Work()
		{
			while (!CallItADay)
			{
				ICubismTask cubismTask = DequeueTask();
				if (cubismTask != null)
				{
					cubismTask.Execute();
					continue;
				}
				Signal.WaitOne();
				Signal.Reset();
			}
		}
	}
}
