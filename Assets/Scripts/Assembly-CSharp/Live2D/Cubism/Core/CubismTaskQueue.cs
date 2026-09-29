namespace Live2D.Cubism.Core
{
	public static class CubismTaskQueue
	{
		public delegate void CubismTaskHandler(ICubismTask task);

		public static CubismTaskHandler OnTask;

		internal static void Enqueue(ICubismTask task)
		{
			if (OnTask == null)
			{
				task.Execute();
			}
			else
			{
				OnTask(task);
			}
		}
	}
}
