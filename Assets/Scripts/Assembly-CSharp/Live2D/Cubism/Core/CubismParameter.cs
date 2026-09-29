using Live2D.Cubism.Core.Unmanaged;
using Live2D.Cubism.Framework;
using UnityEngine;

namespace Live2D.Cubism.Core
{
	[CubismDontMoveOnReimport]
	public sealed class CubismParameter : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		private int _unmanagedIndex = -1;

		[SerializeField]
		[HideInInspector]
		public float Value;

		private CubismUnmanagedParameters UnmanagedParameters { get; set; }

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

		public string Id => UnmanagedParameters.Ids[UnmanagedIndex];

		public int Type => UnmanagedParameters.Types[UnmanagedIndex];

		public float MinimumValue => UnmanagedParameters.MinimumValues[UnmanagedIndex];

		public float MaximumValue => UnmanagedParameters.MaximumValues[UnmanagedIndex];

		public float DefaultValue => UnmanagedParameters.DefaultValues[UnmanagedIndex];

		internal static GameObject CreateParameters(CubismUnmanagedModel unmanagedModel)
		{
			GameObject gameObject = new GameObject("Parameters");
			CubismParameter[] array = new CubismParameter[unmanagedModel.Parameters.Count];
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject2 = new GameObject();
				array[i] = gameObject2.AddComponent<CubismParameter>();
				array[i].transform.SetParent(gameObject.transform);
				array[i].Reset(unmanagedModel, i);
			}
			return gameObject;
		}

		internal void Revive(CubismUnmanagedModel unmanagedModel)
		{
			UnmanagedParameters = unmanagedModel.Parameters;
		}

		private void Reset(CubismUnmanagedModel unmanagedModel, int unmanagedIndex)
		{
			Revive(unmanagedModel);
			UnmanagedIndex = unmanagedIndex;
			base.name = Id;
			Value = DefaultValue;
		}
	}
}
