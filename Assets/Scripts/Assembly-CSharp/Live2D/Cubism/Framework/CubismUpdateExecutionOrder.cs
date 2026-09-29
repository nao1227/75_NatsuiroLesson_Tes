using System.Collections.Generic;

namespace Live2D.Cubism.Framework
{
	public static class CubismUpdateExecutionOrder
	{
		public static readonly int CubismFadeController = 100;

		public static readonly int CubismParameterStoreSaveParameters = 150;

		public static readonly int CubismPoseController = 200;

		public static readonly int CubismExpressionController = 300;

		public static readonly int CubismEyeBlinkController = 400;

		public static readonly int CubismMouthController = 500;

		public static readonly int CubismHarmonicMotionController = 600;

		public static readonly int CubismLookController = 700;

		public static readonly int CubismPhysicsController = 800;

		public static readonly int CubismRenderController = 10000;

		public static readonly int CubismMaskController = 10100;

		public static void SortByExecutionOrder(List<ICubismUpdatable> updatables)
		{
			updatables.Sort(CompareByExecutionOrder);
		}

		private static int CompareByExecutionOrder(ICubismUpdatable a, ICubismUpdatable b)
		{
			return a.ExecutionOrder - b.ExecutionOrder;
		}
	}
}
