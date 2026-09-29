using UnityEngine;

namespace Live2D.Cubism.Rendering.Masking
{
	internal sealed class CubismMaskTilePool
	{
		private int Subdivisions { get; set; }

		private bool[] Slots { get; set; }

		public CubismMaskTilePool(int subdivisions, int channels)
		{
			Subdivisions = subdivisions;
			Slots = new bool[(int)Mathf.Pow(4f, subdivisions) * channels];
		}

		public CubismMaskTile[] AcquireTiles(int count)
		{
			CubismMaskTile[] array = new CubismMaskTile[count];
			for (int i = 0; i < count; i++)
			{
				bool flag = false;
				for (int j = 0; j < Slots.Length; j++)
				{
					if (!Slots[j])
					{
						array[i] = ToTile(j);
						Slots[j] = true;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return null;
				}
			}
			return array;
		}

		public void ReturnTiles(CubismMaskTile[] tiles)
		{
			for (int i = 0; i < tiles.Length; i++)
			{
				Slots[ToIndex(tiles[i])] = false;
			}
		}

		private CubismMaskTile ToTile(int index)
		{
			int num = (int)Mathf.Pow(4f, Subdivisions - 1);
			int num2 = (int)Mathf.Pow(2f, Subdivisions - 1);
			float size = 1f / (float)num2;
			int num3 = index / num;
			int num4 = index - num3 * num;
			int num5 = num4 / num2;
			int num6 = num4 % num2;
			return new CubismMaskTile
			{
				Channel = num3,
				Column = num5,
				Row = num6,
				Size = size
			};
		}

		private int ToIndex(CubismMaskTile tile)
		{
			int num = (int)Mathf.Pow(4f, Subdivisions - 1);
			int num2 = (int)Mathf.Pow(2f, Subdivisions - 1);
			return (int)(tile.Channel * (float)num + tile.Column * (float)num2 + tile.Row);
		}
	}
}
