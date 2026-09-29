using UnityEngine;

namespace Live2D.Cubism.Framework.LookAt
{
	public interface ICubismLookTarget
	{
		Vector3 GetPosition();

		bool IsActive();
	}
}
