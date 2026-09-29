using UnityEngine;

namespace Live2D.Cubism.Rendering.Masking
{
	public struct CubismMaskTile
	{
		public float Channel;

		public float Column;

		public float Row;

		public float Size;

		public static implicit operator Vector4(CubismMaskTile value)
		{
			return new Vector4
			{
				x = value.Channel,
				y = value.Column,
				z = value.Row,
				w = value.Size
			};
		}
	}
}
