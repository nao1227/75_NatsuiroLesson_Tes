using System;
using Live2D.Cubism.Core.Unmanaged;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	public static class ArrayExtensionMethods
	{
		public static CubismParameter FindById(this CubismParameter[] self, string id)
		{
			if (self == null)
			{
				return null;
			}
			for (int i = 0; i < self.Length; i++)
			{
				if (!(self[i].name != id))
				{
					return self[i];
				}
			}
			return null;
		}

		internal static void Revive(this CubismParameter[] self, CubismUnmanagedModel model)
		{
			Array.Sort(self, (CubismParameter a, CubismParameter b) => a.UnmanagedIndex - b.UnmanagedIndex);
			for (int num = 0; num < self.Length; num++)
			{
				self[num].Revive(model);
			}
		}

		internal static void WriteTo(this CubismParameter[] self, CubismUnmanagedModel unmanagedModel)
		{
			CubismUnmanagedFloatArrayView values = unmanagedModel.Parameters.Values;
			for (int i = 0; i < self.Length; i++)
			{
				values[self[i].UnmanagedIndex] = self[i].Value;
			}
		}

		internal static void ReadFrom(this CubismParameter[] self, CubismUnmanagedModel unmanagedModel)
		{
			CubismUnmanagedFloatArrayView values = unmanagedModel.Parameters.Values;
			for (int i = 0; i < self.Length; i++)
			{
				self[i].Value = values[self[i].UnmanagedIndex];
			}
		}

		public static CubismPart FindById(this CubismPart[] self, string id)
		{
			if (self == null)
			{
				return null;
			}
			for (int i = 0; i < self.Length; i++)
			{
				if (!(self[i].name != id))
				{
					return self[i];
				}
			}
			return null;
		}

		internal static void Revive(this CubismPart[] self, CubismUnmanagedModel model)
		{
			Array.Sort(self, (CubismPart a, CubismPart b) => a.UnmanagedIndex - b.UnmanagedIndex);
			for (int num = 0; num < self.Length; num++)
			{
				self[num].Revive(model);
			}
		}

		internal static void WriteTo(this CubismPart[] self, CubismUnmanagedModel unmanagedModel)
		{
			CubismUnmanagedFloatArrayView opacities = unmanagedModel.Parts.Opacities;
			for (int i = 0; i < self.Length; i++)
			{
				opacities[self[i].UnmanagedIndex] = self[i].Opacity;
			}
		}

		public static CubismDrawable FindById(this CubismDrawable[] self, string id)
		{
			if (self == null)
			{
				return null;
			}
			for (int i = 0; i < self.Length; i++)
			{
				if (!(self[i].name != id))
				{
					return self[i];
				}
			}
			return null;
		}

		internal static void Revive(this CubismDrawable[] self, CubismUnmanagedModel model)
		{
			Array.Sort(self, (CubismDrawable a, CubismDrawable b) => a.UnmanagedIndex - b.UnmanagedIndex);
			for (int num = 0; num < self.Length; num++)
			{
				self[num].Revive(model);
			}
		}

		internal unsafe static void ReadFrom(this CubismDynamicDrawableData[] self, CubismUnmanagedModel unmanagedModel)
		{
			CubismUnmanagedDrawables drawables = unmanagedModel.Drawables;
			CubismUnmanagedByteArrayView dynamicFlags = drawables.DynamicFlags;
			CubismUnmanagedFloatArrayView opacities = drawables.Opacities;
			CubismUnmanagedIntArrayView drawOrders = drawables.DrawOrders;
			CubismUnmanagedIntArrayView renderOrders = drawables.RenderOrders;
			CubismUnmanagedFloatArrayView[] vertexPositions = drawables.VertexPositions;
			CubismUnmanagedFloatArrayView multiplyColors = drawables.MultiplyColors;
			CubismUnmanagedFloatArrayView screenColors = drawables.ScreenColors;
			for (int i = 0; i < self.Length; i++)
			{
				CubismDynamicDrawableData cubismDynamicDrawableData = self[i];
				cubismDynamicDrawableData.Flags = dynamicFlags[i];
				cubismDynamicDrawableData.Opacity = opacities[i];
				cubismDynamicDrawableData.DrawOrder = drawOrders[i];
				cubismDynamicDrawableData.RenderOrder = renderOrders[i];
				if (!cubismDynamicDrawableData.AreVertexPositionsDirty)
				{
					continue;
				}
				fixed (Vector3* vertexPositions2 = cubismDynamicDrawableData.VertexPositions)
				{
					for (int j = 0; j < cubismDynamicDrawableData.VertexPositions.Length; j++)
					{
						vertexPositions2[j].x = vertexPositions[i][j * 2];
						vertexPositions2[j].y = vertexPositions[i][j * 2 + 1];
					}
				}
				if (cubismDynamicDrawableData.IsBlendColorDirty)
				{
					int num = i * 4;
					cubismDynamicDrawableData.MultiplyColor = new Color(multiplyColors[num], multiplyColors[num + 1], multiplyColors[num + 2], multiplyColors[num + 3]);
					cubismDynamicDrawableData.ScreenColor = new Color(screenColors[num], screenColors[num + 1], screenColors[num + 2], screenColors[num + 3]);
				}
			}
			drawables.ResetDynamicFlags();
		}
	}
}
