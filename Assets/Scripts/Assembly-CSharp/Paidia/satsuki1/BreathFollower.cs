using Live2D.Cubism.Rendering;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class BreathFollower : MonoBehaviour
	{
		public Transform Breath;

		public CubismRenderer FollowTarget;

		public float XOffset;

		public float YOffset;

		public float Z;

		private void Update()
		{
			if (!(FollowTarget == null))
			{
				Vector3 center = FollowTarget.Mesh.bounds.center;
				Breath.position = new Vector3(center.x + XOffset, center.y + YOffset, Z);
			}
		}
	}
}
