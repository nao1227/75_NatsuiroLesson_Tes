using UnityEngine;
using Stubs;

namespace Paidia.satsuki1
{
	public class MouseInputProvider : IInputProvider
	{
		private UtageManager _utage;

		public MouseInputProvider(UtageManager utage)
		{
			_utage = utage;
		}

		public bool InputGrab()
		{
			if (Input.GetMouseButton(SaveLoadManager.GlobalData.GameOption.MouseButtonDecision))
			{
				return IsUtageNotPlayingOrNull();
			}
			return false;
		}

		public bool InputMouse()
		{
			return Input.GetMouseButtonDown(0);
		}

		public bool InputAuto()
		{
			return Input.GetMouseButton(SaveLoadManager.GlobalData.GameOption.MouseButtonAuto);
		}

		public bool InputSpecial()
		{
			return Input.GetMouseButton(SaveLoadManager.GlobalData.GameOption.MouseButtonSpecial);
		}

		public Vector3 GetPosition()
		{
			return Input.mousePosition;
		}

		public Vector3 GetMouseMove()
		{
			float axis = Input.GetAxis("Mouse X");
			float axis2 = Input.GetAxis("Mouse Y");
			return new Vector3(axis, axis2, 0f) * SaveLoadManager.GlobalData.GetMouseSensitivityFactor();
		}

		public bool InputMouseRelease()
		{
			if (!Input.GetMouseButtonUp(SaveLoadManager.GlobalData.GameOption.MouseButtonDecision))
			{
				return IsUtagePlaying();
			}
			return true;
		}

		public bool InputSpecialRelease()
		{
			if (!Input.GetMouseButtonUp(SaveLoadManager.GlobalData.GameOption.MouseButtonSpecial))
			{
				return IsUtagePlaying();
			}
			return true;
		}

		public float GetAxis()
		{
			return Input.GetAxis("Mouse ScrollWheel");
		}

		private bool IsUtageNotPlayingOrNull()
		{
			if (!(null == _utage))
			{
				return !_utage.IsPlaying;
			}
			return true;
		}

		private bool IsUtagePlaying()
		{
			if (null != _utage)
			{
				return _utage.IsPlaying;
			}
			return false;
		}
	}
}
