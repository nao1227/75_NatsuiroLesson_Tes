using Live2D.Cubism.Core.Unmanaged;
using Live2D.Cubism.Framework;

namespace Live2D.Cubism.Core
{
	[CubismDontMoveOnReimport]
	public sealed class CubismCanvasInformation
	{
		private CubismUnmanagedCanvasInformation UnmanagedCanvasInformation { get; set; }

		public float CanvasWidth => UnmanagedCanvasInformation.CanvasWidth;

		public float CanvasHeight => UnmanagedCanvasInformation.CanvasHeight;

		public float CanvasOriginX => UnmanagedCanvasInformation.CanvasOriginX;

		public float CanvasOriginY => UnmanagedCanvasInformation.CanvasOriginY;

		public float PixelsPerUnit => UnmanagedCanvasInformation.PixelsPerUnit;

		public CubismCanvasInformation(CubismUnmanagedModel unmanagedModel)
		{
			Reset(unmanagedModel);
		}

		internal void Revive(CubismUnmanagedModel unmanagedModel)
		{
			UnmanagedCanvasInformation = unmanagedModel.CanvasInformation;
		}

		private void Reset(CubismUnmanagedModel unmanagedModel)
		{
			Revive(unmanagedModel);
		}
	}
}
