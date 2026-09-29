using UnityEngine;

namespace Live2D.Cubism.Framework.Pose
{
	public sealed class CubismPosePart : MonoBehaviour
	{
		[SerializeField]
		public int GroupIndex;

		[SerializeField]
		public int PartIndex;

		[SerializeField]
		public string[] Link;
	}
}
