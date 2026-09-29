using UnityEngine;

namespace Paidia.satsuki1
{
	public interface IInputProvider
	{
		bool InputGrab();

		bool InputAuto();

		bool InputSpecial();

		bool InputMouseRelease();

		bool InputSpecialRelease();

		bool InputMouse();

		float GetAxis();

		Vector3 GetPosition();

		Vector3 GetMouseMove();
	}
}
