using System.Collections.Generic;
using UnityEngine;

namespace Live2D.Cubism.Framework
{
	public static class ComponentExtensionMethods
	{
		public static T[] GetComponentsMany<T>(this Component[] self) where T : Component
		{
			if (self == null)
			{
				return null;
			}
			List<T> list = new List<T>();
			for (int i = 0; i < self.Length; i++)
			{
				T[] components = self[i].GetComponents<T>();
				if (components != null && components.Length != 0)
				{
					list.AddRange(components);
				}
			}
			return list.ToArray();
		}

		public static T[] AddComponentEach<T>(this Component[] self) where T : Component
		{
			if (self == null)
			{
				return null;
			}
			T[] array = new T[self.Length];
			for (int i = 0; i < self.Length; i++)
			{
				array[i] = self[i].gameObject.AddComponent<T>();
			}
			return array;
		}
	}
}
