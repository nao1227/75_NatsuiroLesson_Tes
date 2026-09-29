using System;
using Paidia.satsuki1;
using UnityEngine;

namespace Utage
{
	public static class InputUtil
	{
		private static bool enableInput = true;

		private static readonly float wheelSensitive = 0.1f;

		public static bool EnableInput
		{
			get
			{
				if (enableInput)
				{
					return SingletonManager<SceneContextManager>.Instance.AllowUtageInput;
				}
				return false;
			}
			set
			{
				enableInput = value;
			}
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void OnRuntimeInitialize()
		{
			EnableInput = true;
		}

		[Obsolete("Use IsMouseRightButtonDown instead")]
		public static bool IsMousceRightButtonDown()
		{
			if (!EnableInput)
			{
				return false;
			}
			return IsMouseRightButtonDown();
		}

		public static bool EnableWebGLInput()
		{
			return Application.platform == RuntimePlatform.WebGLPlayer;
		}

		public static bool IsMouseRightButtonDown()
		{
			if (!EnableInput)
			{
				return false;
			}
			if (UtageToolKit.IsPlatformStandAloneOrEditor() || EnableWebGLInput())
			{
				return Input.GetMouseButtonDown(1);
			}
			return false;
		}

		public static bool IsInputControl()
		{
			if (!EnableInput)
			{
				return false;
			}
			if (UtageToolKit.IsPlatformStandAloneOrEditor() || EnableWebGLInput())
			{
				if (!Input.GetKey(KeyCode.LeftControl))
				{
					return Input.GetKey(KeyCode.RightControl);
				}
				return true;
			}
			return false;
		}

		public static bool IsInputScrollWheelUp()
		{
			if (!EnableInput)
			{
				return false;
			}
			if (Input.GetAxis("Mouse ScrollWheel") >= wheelSensitive)
			{
				return true;
			}
			return false;
		}

		public static bool IsInputScrollWheelDown()
		{
			if (!EnableInput)
			{
				return false;
			}
			if (Input.GetAxis("Mouse ScrollWheel") <= 0f - wheelSensitive)
			{
				return true;
			}
			return false;
		}

		public static bool IsInputKeyboadReturnDown()
		{
			if (!EnableInput)
			{
				return false;
			}
			if (UtageToolKit.IsPlatformStandAloneOrEditor() || EnableWebGLInput())
			{
				return Input.GetKeyDown(KeyCode.Return);
			}
			return false;
		}

		internal static bool GetKeyDown(KeyCode keyCode)
		{
			if (!EnableInput)
			{
				return false;
			}
			return Input.GetKeyDown(keyCode);
		}
	}
}
