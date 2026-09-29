using UnityEngine;

namespace Live2D.Cubism.Rendering
{
	public static class ArrayExtensionMethods
	{
		public static Bounds GetMeshRendererBounds(this CubismRenderer[] self)
		{
			Vector3 min = self[0].MeshRenderer.bounds.min;
			Vector3 max = self[0].MeshRenderer.bounds.max;
			for (int i = 1; i < self.Length; i++)
			{
				Bounds bounds = self[i].MeshRenderer.bounds;
				if (bounds.min.x < min.x)
				{
					min.x = bounds.min.x;
				}
				if (bounds.max.x > max.x)
				{
					max.x = bounds.max.x;
				}
				if (bounds.min.y < min.y)
				{
					min.y = bounds.min.y;
				}
				if (bounds.max.y > max.y)
				{
					max.y = bounds.max.y;
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
