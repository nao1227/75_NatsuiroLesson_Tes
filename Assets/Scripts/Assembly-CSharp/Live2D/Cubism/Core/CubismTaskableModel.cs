using System.Threading;
using Live2D.Cubism.Core.Unmanaged;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	internal sealed class CubismTaskableModel : ICubismTask
	{
		private enum TaskState
		{
			Idle = 0,
			Enqueued = 1,
			Executing = 2,
			Executed = 3
		}

		private CubismDynamicDrawableData[] _dynamicDrawableData;

		public CubismUnmanagedModel UnmanagedModel { get; private set; }

		public CubismMoc Moc { get; private set; }

		public CubismDynamicDrawableData[] DynamicDrawableData
		{
			get
			{
				CubismDynamicDrawableData[] result = null;
				if (Monitor.TryEnter(Lock))
				{
					result = _dynamicDrawableData;
					Monitor.Exit(Lock);
				}
				return result;
			}
			private set
			{
				_dynamicDrawableData = value;
			}
		}

		public bool IsExecuting
		{
			get
			{
				bool result = false;
				if (Monitor.TryEnter(Lock))
				{
					result = State == TaskState.Enqueued || State == TaskState.Executing;
					Monitor.Exit(Lock);
				}
				return result;
			}
		}

		public bool DidExecute
		{
			get
			{
				bool result = false;
				if (Monitor.TryEnter(Lock))
				{
					result = State == TaskState.Executed;
					Monitor.Exit(Lock);
				}
				return result;
			}
		}

		private bool ShouldReleaseUnmanaged { get; set; }

		private object Lock { get; set; }

		private TaskState State { get; set; }

		public static CubismTaskableModel CreateTaskableModel(CubismMoc moc)
		{
			return new CubismTaskableModel(moc);
		}

		public CubismTaskableModel(CubismMoc moc)
		{
			Moc = moc;
			CubismUnmanagedMoc moc2 = moc.AcquireUnmanagedMoc();
			UnmanagedModel = CubismUnmanagedModel.FromMoc(moc2);
			if (UnmanagedModel == null)
			{
				Debug.LogError("This .moc3 file version is \"Unknown\"!!\nIt may be broken or you are trying to use a higher version of Moc than Cubism Core.\nCheck the supported versions at CubismMoc.LatestVersion.\nThe \"CoreDll\" constants indicate which Moc version the numbers are assigned to.");
				return;
			}
			Lock = new object();
			State = TaskState.Idle;
			DynamicDrawableData = CubismDynamicDrawableData.CreateData(UnmanagedModel);
			ShouldReleaseUnmanaged = false;
		}

		public bool TryReadParameters(CubismParameter[] parameters)
		{
			bool result = false;
			if (Monitor.TryEnter(Lock))
			{
				try
				{
					if (State == TaskState.Executed)
					{
						parameters.ReadFrom(UnmanagedModel);
						result = true;
					}
				}
				finally
				{
					Monitor.Exit(Lock);
				}
			}
			return result;
		}

		public bool TryWriteParametersAndParts(CubismParameter[] parameters, CubismPart[] parts)
		{
			bool result = false;
			if (Monitor.TryEnter(Lock))
			{
				try
				{
					if (State != TaskState.Executing)
					{
						parameters.WriteTo(UnmanagedModel);
						parts.WriteTo(UnmanagedModel);
						result = true;
					}
				}
				finally
				{
					Monitor.Exit(Lock);
				}
			}
			return result;
		}

		public void Update()
		{
			lock (Lock)
			{
				if (State == TaskState.Enqueued || State == TaskState.Executing)
				{
					return;
				}
				State = TaskState.Enqueued;
			}
			CubismTaskQueue.Enqueue(this);
		}

		public bool UpdateNow()
		{
			lock (Lock)
			{
				if (State == TaskState.Enqueued || State == TaskState.Executing)
				{
					return false;
				}
				State = TaskState.Enqueued;
			}
			Execute();
			return true;
		}

		public void ReleaseUnmanaged()
		{
			ShouldReleaseUnmanaged = true;
			lock (Lock)
			{
				if (State == TaskState.Enqueued || State == TaskState.Executing)
				{
					return;
				}
			}
			OnReleaseUnmanaged();
			ShouldReleaseUnmanaged = false;
		}

		private void Execute()
		{
			lock (Lock)
			{
				State = TaskState.Executing;
			}
			UnmanagedModel.Update();
			DynamicDrawableData.ReadFrom(UnmanagedModel);
			lock (Lock)
			{
				State = TaskState.Executed;
				if (ShouldReleaseUnmanaged)
				{
					OnReleaseUnmanaged();
				}
			}
		}

		private void OnReleaseUnmanaged()
		{
			UnmanagedModel.Release();
			Moc.ReleaseUnmanagedMoc();
			UnmanagedModel = null;
		}

		void ICubismTask.Execute()
		{
			Execute();
		}
	}
}
