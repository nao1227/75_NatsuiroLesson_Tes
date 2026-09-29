using UnityEngine;

namespace Live2D.Cubism.Framework
{
	internal static class ObjectExtensionMethods
	{
		public static T GetInterface<T>(this Object self) where T : class
		{
			T val = self as T;
			if (val != null)
			{
				return val;
			}
			GameObject gameObject = self as GameObject;
			if (gameObject != null)
			{
				val = gameObject.GetComponent<T>();
			}
			if (self != null && val == null)
			{
				Debug.LogWarning(self?.ToString() + " doesn't expose requested interface of type \"" + typeof(T)?.ToString() + "\".");
			}
			return val;
		}

		public static Object ToNullUnlessImplementsInterface<T>(this Object self) where T : class
		{
			bool flag = self.ImplementsInterface<T>();
			if (self != null && !flag)
			{
				Debug.LogWarning(self?.ToString() + " doesn't expose requested interface of type \"" + typeof(T)?.ToString() + "\".");
			}
			if (!flag)
			{
				return null;
			}
			return self;
		}

		public static bool ImplementsInterface<T>(this Object self)
		{
			if (self is T)
			{
				return true;
			}
			GameObject gameObject = self as GameObject;
			if (gameObject != null)
			{
				return gameObject.GetComponents<T>().Length != 0;
			}
			return false;
		}
	}
}
