using Live2D.Cubism.Core.Unmanaged;
using Live2D.Cubism.Framework;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	[CubismDontMoveOnReimport]
	public sealed class CubismDrawable : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		private int _unmanagedIndex = -1;

		private CubismUnmanagedDrawables UnmanagedDrawables { get; set; }

		internal int UnmanagedIndex
		{
			get
			{
				return _unmanagedIndex;
			}
			private set
			{
				_unmanagedIndex = value;
			}
		}

		public string Id => UnmanagedDrawables.Ids[UnmanagedIndex];

		public int TextureIndex => UnmanagedDrawables.TextureIndices[UnmanagedIndex];

		public Color MultiplyColor
		{
			get
			{
				int num = UnmanagedIndex * 4;
				return new Color(UnmanagedDrawables.MultiplyColors[num], UnmanagedDrawables.MultiplyColors[num + 1], UnmanagedDrawables.MultiplyColors[num + 2], UnmanagedDrawables.MultiplyColors[num + 3]);
			}
		}

		public Color ScreenColor
		{
			get
			{
				int num = UnmanagedIndex * 4;
				return new Color(UnmanagedDrawables.ScreenColors[num], UnmanagedDrawables.ScreenColors[num + 1], UnmanagedDrawables.ScreenColors[num + 2], UnmanagedDrawables.ScreenColors[num + 3]);
			}
		}

		public int ParentPartIndex => UnmanagedDrawables.ParentPartIndices[UnmanagedIndex];

		public CubismDrawable[] Masks
		{
			get
			{
				CubismDrawable[] drawables = this.FindCubismModel(includeParents: true).Drawables;
				CubismUnmanagedIntArrayView maskCounts = UnmanagedDrawables.MaskCounts;
				CubismUnmanagedIntArrayView[] masks = UnmanagedDrawables.Masks;
				CubismDrawable[] array = new CubismDrawable[maskCounts[UnmanagedIndex]];
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < drawables.Length; j++)
					{
						if (drawables[j].UnmanagedIndex == masks[UnmanagedIndex][i])
						{
							array[i] = drawables[j];
							break;
						}
					}
				}
				return array;
			}
		}

		public Vector3[] VertexPositions
		{
			get
			{
				CubismUnmanagedIntArrayView vertexCounts = UnmanagedDrawables.VertexCounts;
				CubismUnmanagedFloatArrayView[] vertexPositions = UnmanagedDrawables.VertexPositions;
				Vector3[] array = new Vector3[vertexCounts[UnmanagedIndex]];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Vector3(vertexPositions[UnmanagedIndex][i * 2], vertexPositions[UnmanagedIndex][i * 2 + 1]);
				}
				return array;
			}
		}

		public Vector2[] VertexUvs
		{
			get
			{
				CubismUnmanagedIntArrayView vertexCounts = UnmanagedDrawables.VertexCounts;
				CubismUnmanagedFloatArrayView[] vertexUvs = UnmanagedDrawables.VertexUvs;
				Vector2[] array = new Vector2[vertexCounts[UnmanagedIndex]];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = new Vector2(vertexUvs[UnmanagedIndex][i * 2], vertexUvs[UnmanagedIndex][i * 2 + 1]);
				}
				return array;
			}
		}

		public int[] Indices
		{
			get
			{
				CubismUnmanagedIntArrayView indexCounts = UnmanagedDrawables.IndexCounts;
				CubismUnmanagedUshortArrayView[] indices = UnmanagedDrawables.Indices;
				int[] array = new int[indexCounts[UnmanagedIndex]];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = indices[UnmanagedIndex][i];
				}
				return array;
			}
		}

		public bool IsDoubleSided => UnmanagedDrawables.ConstantFlags[UnmanagedIndex].HasIsDoubleSidedFlag();

		public bool IsMasked => UnmanagedDrawables.MaskCounts[UnmanagedIndex] > 0;

		public bool IsInverted => UnmanagedDrawables.ConstantFlags[UnmanagedIndex].HasIsInvertedMaskFlag();

		public bool BlendAdditive => UnmanagedDrawables.ConstantFlags[UnmanagedIndex].HasBlendAdditiveFlag();

		public bool MultiplyBlend => UnmanagedDrawables.ConstantFlags[UnmanagedIndex].HasBlendMultiplicativeFlag();

		internal static GameObject CreateDrawables(CubismUnmanagedModel unmanagedModel)
		{
			GameObject gameObject = new GameObject("Drawables");
			CubismDrawable[] array = new CubismDrawable[unmanagedModel.Drawables.Count];
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject2 = new GameObject();
				array[i] = gameObject2.AddComponent<CubismDrawable>();
				array[i].transform.SetParent(gameObject.transform);
				array[i].Reset(unmanagedModel, i);
			}
			return gameObject;
		}

		internal void Revive(CubismUnmanagedModel unmanagedModel)
		{
			UnmanagedDrawables = unmanagedModel.Drawables;
		}

		private void Reset(CubismUnmanagedModel unmanagedModel, int unmanagedIndex)
		{
			Revive(unmanagedModel);
			UnmanagedIndex = unmanagedIndex;
			base.name = Id;
		}
	}
}
