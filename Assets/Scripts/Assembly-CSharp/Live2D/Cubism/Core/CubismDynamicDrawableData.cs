using Live2D.Cubism.Core.Unmanaged;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	public sealed class CubismDynamicDrawableData
	{
		internal byte Flags { private get; set; }

		public float Opacity { get; internal set; }

		public int DrawOrder { get; internal set; }

		public int RenderOrder { get; internal set; }

		public Vector3[] VertexPositions { get; internal set; }

		public Color MultiplyColor { get; internal set; }

		public Color ScreenColor { get; internal set; }

		public bool IsVisible => Flags.HasIsVisibleFlag();

		public bool IsVisibilityDirty => Flags.HasVisibilityDidChangeFlag();

		public bool IsOpacityDirty => Flags.HasOpacityDidChangeFlag();

		public bool IsDrawOrderDirty => Flags.HasDrawOrderDidChangeFlag();

		public bool IsRenderOrderDirty => Flags.HasRenderOrderDidChangeFlag();

		public bool AreVertexPositionsDirty => Flags.HasVertexPositionsDidChangeFlag();

		public bool IsBlendColorDirty => Flags.HasBlendColorDidChangeFlag();

		public bool IsAnyDirty => Flags != 0;

		internal static CubismDynamicDrawableData[] CreateData(CubismUnmanagedModel unmanagedModel)
		{
			CubismUnmanagedDrawables drawables = unmanagedModel.Drawables;
			CubismDynamicDrawableData[] array = new CubismDynamicDrawableData[drawables.Count];
			CubismUnmanagedIntArrayView vertexCounts = drawables.VertexCounts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new CubismDynamicDrawableData
				{
					VertexPositions = new Vector3[vertexCounts[i]]
				};
			}
			return array;
		}
	}
}
