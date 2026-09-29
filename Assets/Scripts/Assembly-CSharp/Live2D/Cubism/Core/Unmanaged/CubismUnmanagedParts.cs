using System;
using System.Runtime.InteropServices;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedParts
	{
		public int Count { get; private set; }

		public string[] Ids { get; private set; }

		public CubismUnmanagedFloatArrayView Opacities { get; private set; }

		public CubismUnmanagedIntArrayView ParentIndices { get; private set; }

		internal unsafe CubismUnmanagedParts(IntPtr modelPtr)
		{
			int num = 0;
			Count = CubismCoreDll.GetPartCount(modelPtr);
			num = CubismCoreDll.GetPartCount(modelPtr);
			Ids = new string[num];
			IntPtr* partIds = (IntPtr*)CubismCoreDll.GetPartIds(modelPtr);
			for (int i = 0; i < num; i++)
			{
				Ids[i] = Marshal.PtrToStringAnsi(partIds[i]);
			}
			num = CubismCoreDll.GetPartCount(modelPtr);
			Opacities = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetPartOpacities(modelPtr), num);
			num = CubismCoreDll.GetPartCount(modelPtr);
			ParentIndices = new CubismUnmanagedIntArrayView(CubismCoreDll.GetPartParentPartIndices(modelPtr), num);
		}
	}
}
