using System;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedCanvasInformation
	{
		public float CanvasWidth { get; private set; }

		public float CanvasHeight { get; private set; }

		public float CanvasOriginX { get; private set; }

		public float CanvasOriginY { get; private set; }

		public float PixelsPerUnit { get; private set; }

		internal unsafe CubismUnmanagedCanvasInformation(IntPtr modelPtr)
		{
			if (modelPtr == IntPtr.Zero)
			{
				return;
			}
			float[] array = new float[2];
			float[] array2 = new float[2];
			float[] array3 = new float[1];
			fixed (float* ptr = array)
			{
				fixed (float* ptr2 = array2)
				{
					fixed (float* ptr3 = array3)
					{
						CubismCoreDll.ReadCanvasInfo(modelPtr, (IntPtr)ptr, (IntPtr)ptr2, (IntPtr)ptr3);
						CanvasWidth = array[0];
						CanvasHeight = array[1];
						CanvasOriginX = array2[0];
						CanvasOriginY = array2[1];
						PixelsPerUnit = array3[0];
					}
				}
			}
		}
	}
}
