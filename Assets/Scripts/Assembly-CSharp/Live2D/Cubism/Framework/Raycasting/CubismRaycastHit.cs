using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Raycasting
{
	public struct CubismRaycastHit
	{
		public CubismDrawable Drawable;

		public float Distance;

		public Vector3 LocalPosition;

		public Vector3 WorldPosition;
	}
}
