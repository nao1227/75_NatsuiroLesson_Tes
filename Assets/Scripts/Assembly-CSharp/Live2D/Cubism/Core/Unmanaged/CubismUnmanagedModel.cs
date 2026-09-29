using System;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedModel
	{
		public CubismUnmanagedParameters Parameters { get; private set; }

		public CubismUnmanagedParts Parts { get; private set; }

		public CubismUnmanagedDrawables Drawables { get; private set; }

		public CubismUnmanagedCanvasInformation CanvasInformation { get; private set; }

		public IntPtr Ptr { get; private set; }

		public static CubismUnmanagedModel FromMoc(CubismUnmanagedMoc moc)
		{
			if (moc == null)
			{
				return null;
			}
			CubismUnmanagedModel cubismUnmanagedModel = new CubismUnmanagedModel(moc);
			if (!(cubismUnmanagedModel.Ptr != IntPtr.Zero))
			{
				return null;
			}
			return cubismUnmanagedModel;
		}

		public void Update()
		{
			if (!(Ptr == IntPtr.Zero))
			{
				CubismCoreDll.UpdateModel(Ptr);
			}
		}

		public void Release()
		{
			if (!(Ptr == IntPtr.Zero))
			{
				CubismUnmanagedMemory.Deallocate(Ptr);
				Ptr = IntPtr.Zero;
			}
		}

		private CubismUnmanagedModel(CubismUnmanagedMoc moc)
		{
			uint sizeofModel = CubismCoreDll.GetSizeofModel(moc.Ptr);
			IntPtr intPtr = CubismUnmanagedMemory.Allocate((int)sizeofModel, 16);
			if (!(intPtr == IntPtr.Zero))
			{
				Ptr = CubismCoreDll.InitializeModelInPlace(moc.Ptr, intPtr, sizeofModel);
				if (Ptr == IntPtr.Zero)
				{
					CubismUnmanagedMemory.Deallocate(intPtr);
					return;
				}
				Parameters = new CubismUnmanagedParameters(Ptr);
				Parts = new CubismUnmanagedParts(Ptr);
				Drawables = new CubismUnmanagedDrawables(Ptr);
				CanvasInformation = new CubismUnmanagedCanvasInformation(Ptr);
			}
		}
	}
}
