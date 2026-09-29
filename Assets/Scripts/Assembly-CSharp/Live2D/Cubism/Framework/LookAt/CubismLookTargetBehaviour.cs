using UnityEngine;

namespace Live2D.Cubism.Framework.LookAt
{
	public class CubismLookTargetBehaviour : MonoBehaviour, ICubismLookTarget
	{
		Vector3 ICubismLookTarget.GetPosition()
		{
			return base.transform.position;
		}

		bool ICubismLookTarget.IsActive()
		{
			return base.isActiveAndEnabled;
		}
	}
}
