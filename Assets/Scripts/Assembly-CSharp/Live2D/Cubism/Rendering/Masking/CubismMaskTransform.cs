using UnityEngine;

namespace Live2D.Cubism.Rendering.Masking
{
	public struct CubismMaskTransform
	{
		private static int _uniqueId;

		public Vector2 Offset;

		public float Scale;

		private static int UniqueId
		{
			get
			{
				if (_uniqueId > 1024)
				{
					_uniqueId = 0;
				}
				return ++_uniqueId;
			}
		}

		public static implicit operator Vector4(CubismMaskTransform value)
		{
			return new Vector4
			{
				x = value.Offset.x,
				y = value.Offset.y,
				z = value.Scale,
				w = UniqueId
			};
		}
	}
}
