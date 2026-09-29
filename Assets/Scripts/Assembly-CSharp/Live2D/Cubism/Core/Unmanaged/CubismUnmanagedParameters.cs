using System;
using System.Runtime.InteropServices;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedParameters
	{
		public int Count { get; private set; }

		public string[] Ids { get; private set; }

		public CubismUnmanagedFloatArrayView MinimumValues { get; private set; }

		public CubismUnmanagedIntArrayView Types { get; private set; }

		public CubismUnmanagedFloatArrayView MaximumValues { get; private set; }

		public CubismUnmanagedFloatArrayView DefaultValues { get; private set; }

		public CubismUnmanagedFloatArrayView Values { get; private set; }

		public CubismUnmanagedIntArrayView KeyCounts { get; private set; }

		public CubismUnmanagedFloatArrayView[] KeyValues { get; private set; }

		internal unsafe CubismUnmanagedParameters(IntPtr modelPtr)
		{
			int num = 0;
			Count = CubismCoreDll.GetParameterCount(modelPtr);
			num = CubismCoreDll.GetParameterCount(modelPtr);
			Ids = new string[num];
			IntPtr* parameterIds = (IntPtr*)CubismCoreDll.GetParameterIds(modelPtr);
			for (int i = 0; i < num; i++)
			{
				Ids[i] = Marshal.PtrToStringAnsi(parameterIds[i]);
			}
			num = CubismCoreDll.GetParameterCount(modelPtr);
			MinimumValues = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetParameterMinimumValues(modelPtr), num);
			num = CubismCoreDll.GetParameterCount(modelPtr);
			Types = new CubismUnmanagedIntArrayView(CubismCoreDll.GetParameterTypes(modelPtr), num);
			num = CubismCoreDll.GetParameterCount(modelPtr);
			MaximumValues = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetParameterMaximumValues(modelPtr), num);
			num = CubismCoreDll.GetParameterCount(modelPtr);
			DefaultValues = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetParameterDefaultValues(modelPtr), num);
			num = CubismCoreDll.GetParameterCount(modelPtr);
			Values = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetParameterValues(modelPtr), num);
			num = CubismCoreDll.GetParameterCount(modelPtr);
			KeyCounts = new CubismUnmanagedIntArrayView(CubismCoreDll.GetParameterKeyCounts(modelPtr), num);
			num = CubismCoreDll.GetParameterCount(modelPtr);
			CubismUnmanagedIntArrayView cubismUnmanagedIntArrayView = new CubismUnmanagedIntArrayView(CubismCoreDll.GetParameterKeyCounts(modelPtr), num);
			KeyValues = new CubismUnmanagedFloatArrayView[num];
			IntPtr* parameterKeyValues = (IntPtr*)CubismCoreDll.GetParameterKeyValues(modelPtr);
			for (int j = 0; j < num; j++)
			{
				KeyValues[j] = new CubismUnmanagedFloatArrayView(parameterKeyValues[j], cubismUnmanagedIntArrayView[j]);
			}
		}
	}
}
