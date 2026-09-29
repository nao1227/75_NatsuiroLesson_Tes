using UnityEngine;

namespace Live2D.Cubism.Rendering.Masking
{
	internal static class CubismMaskRendererExtensionMethods
	{
		public static Bounds GetBounds(this CubismMaskRenderer[] self)
		{
			Vector3 min = self[0].MeshBounds.min;
			Vector3 max = self[0].MeshBounds.max;
			for (int i = 1; i < self.Length; i++)
			{
				Bounds meshBounds = self[i].MeshBounds;
				if (meshBounds.min.x < min.x)
				{
					min.x = meshBounds.min.x;
				}
				if (meshBounds.max.x > max.x)
				{
					max.x = meshBounds.max.x;
				}
				if (meshBounds.min.y < min.y)
				{
					min.y = meshBounds.min.y;
				}
				if (meshBounds.max.y > max.y)
				{
					max.y = meshBounds.max.y;
				}
			}
			return new Bounds
			{
				min = min,
				max = max
			};
		}
	}
}
