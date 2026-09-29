using System;
using System.Runtime.InteropServices;

namespace Live2D.Cubism.Core.Unmanaged
{
	public sealed class CubismUnmanagedDrawables
	{
		public int Count { get; private set; }

		public string[] Ids { get; private set; }

		public CubismUnmanagedByteArrayView ConstantFlags { get; private set; }

		public CubismUnmanagedByteArrayView DynamicFlags { get; private set; }

		public CubismUnmanagedIntArrayView TextureIndices { get; private set; }

		public CubismUnmanagedIntArrayView DrawOrders { get; private set; }

		public CubismUnmanagedIntArrayView RenderOrders { get; private set; }

		public CubismUnmanagedFloatArrayView Opacities { get; private set; }

		public CubismUnmanagedIntArrayView MaskCounts { get; private set; }

		public CubismUnmanagedIntArrayView[] Masks { get; private set; }

		public CubismUnmanagedIntArrayView VertexCounts { get; private set; }

		public CubismUnmanagedFloatArrayView[] VertexPositions { get; private set; }

		public CubismUnmanagedFloatArrayView[] VertexUvs { get; private set; }

		public CubismUnmanagedIntArrayView IndexCounts { get; private set; }

		public CubismUnmanagedUshortArrayView[] Indices { get; private set; }

		public CubismUnmanagedFloatArrayView MultiplyColors { get; private set; }

		public CubismUnmanagedFloatArrayView ScreenColors { get; private set; }

		public CubismUnmanagedIntArrayView ParentPartIndices { get; private set; }

		private IntPtr ModelPtr { get; set; }

		public void ResetDynamicFlags()
		{
			CubismCoreDll.ResetDrawableDynamicFlags(ModelPtr);
		}

		internal unsafe CubismUnmanagedDrawables(IntPtr modelPtr)
		{
			ModelPtr = modelPtr;
			int num = 0;
			Count = CubismCoreDll.GetDrawableCount(modelPtr);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			Ids = new string[num];
			IntPtr* drawableIds = (IntPtr*)CubismCoreDll.GetDrawableIds(modelPtr);
			for (int i = 0; i < num; i++)
			{
				Ids[i] = Marshal.PtrToStringAnsi(drawableIds[i]);
			}
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			ConstantFlags = new CubismUnmanagedByteArrayView(CubismCoreDll.GetDrawableConstantFlags(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			DynamicFlags = new CubismUnmanagedByteArrayView(CubismCoreDll.GetDrawableDynamicFlags(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			TextureIndices = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableTextureIndices(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			DrawOrders = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableDrawOrders(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			RenderOrders = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableRenderOrders(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			Opacities = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetDrawableOpacities(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			MaskCounts = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableMaskCounts(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			VertexCounts = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableVertexCounts(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			IndexCounts = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableIndexCounts(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			MultiplyColors = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetDrawableMultiplyColors(modelPtr), num * 4);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			ScreenColors = new CubismUnmanagedFloatArrayView(CubismCoreDll.GetDrawableScreenColors(modelPtr), num * 4);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			ParentPartIndices = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableParentPartIndices(modelPtr), num);
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			CubismUnmanagedIntArrayView cubismUnmanagedIntArrayView = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableMaskCounts(modelPtr), num);
			Masks = new CubismUnmanagedIntArrayView[num];
			IntPtr* drawableMasks = (IntPtr*)CubismCoreDll.GetDrawableMasks(modelPtr);
			for (int j = 0; j < num; j++)
			{
				Masks[j] = new CubismUnmanagedIntArrayView(drawableMasks[j], cubismUnmanagedIntArrayView[j]);
			}
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			cubismUnmanagedIntArrayView = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableVertexCounts(modelPtr), num);
			VertexPositions = new CubismUnmanagedFloatArrayView[num];
			IntPtr* drawableVertexPositions = (IntPtr*)CubismCoreDll.GetDrawableVertexPositions(modelPtr);
			for (int k = 0; k < num; k++)
			{
				VertexPositions[k] = new CubismUnmanagedFloatArrayView(drawableVertexPositions[k], cubismUnmanagedIntArrayView[k] * 2);
			}
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			cubismUnmanagedIntArrayView = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableVertexCounts(modelPtr), num);
			VertexUvs = new CubismUnmanagedFloatArrayView[num];
			IntPtr* drawableVertexUvs = (IntPtr*)CubismCoreDll.GetDrawableVertexUvs(modelPtr);
			for (int l = 0; l < num; l++)
			{
				VertexUvs[l] = new CubismUnmanagedFloatArrayView(drawableVertexUvs[l], cubismUnmanagedIntArrayView[l] * 2);
			}
			num = CubismCoreDll.GetDrawableCount(modelPtr);
			cubismUnmanagedIntArrayView = new CubismUnmanagedIntArrayView(CubismCoreDll.GetDrawableIndexCounts(modelPtr), num);
			Indices = new CubismUnmanagedUshortArrayView[num];
			IntPtr* drawableIndices = (IntPtr*)CubismCoreDll.GetDrawableIndices(modelPtr);
			for (int m = 0; m < num; m++)
			{
				Indices[m] = new CubismUnmanagedUshortArrayView(drawableIndices[m], cubismUnmanagedIntArrayView[m]);
			}
		}
	}
}
