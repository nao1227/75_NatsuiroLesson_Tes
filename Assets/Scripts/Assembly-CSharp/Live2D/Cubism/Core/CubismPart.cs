using Live2D.Cubism.Core.Unmanaged;
using Live2D.Cubism.Framework;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	[CubismDontMoveOnReimport]
	public sealed class CubismPart : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		private int _unmanagedIndex = -1;

		[SerializeField]
		[HideInInspector]
		public float Opacity;

		private CubismUnmanagedParts UnmanagedParts { get; set; }

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

		public string Id => UnmanagedParts.Ids[UnmanagedIndex];

		internal static GameObject CreateParts(CubismUnmanagedModel unmanagedModel)
		{
			GameObject gameObject = new GameObject("Parts");
			CubismPart[] array = new CubismPart[unmanagedModel.Parts.Count];
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject2 = new GameObject();
				array[i] = gameObject2.AddComponent<CubismPart>();
				array[i].transform.SetParent(gameObject.transform);
				array[i].Reset(unmanagedModel, i);
			}
			return gameObject;
		}

		internal void Revive(CubismUnmanagedModel unmanagedModel)
		{
			UnmanagedParts = unmanagedModel.Parts;
		}

		private void Reset(CubismUnmanagedModel unmanagedModel, int unmanagedIndex)
		{
			Revive(unmanagedModel);
			UnmanagedIndex = unmanagedIndex;
			base.name = Id;
			Opacity = UnmanagedParts.Opacities[unmanagedIndex];
		}
	}
}
