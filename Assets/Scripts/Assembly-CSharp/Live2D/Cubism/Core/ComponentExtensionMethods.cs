using UnityEngine;

namespace Live2D.Cubism.Core
{
	public static class ComponentExtensionMethods
	{
		public static CubismModel FindCubismModel(this Component self, bool includeParents = false)
		{
			if (self == null)
			{
				return null;
			}
			CubismModel component = self.GetComponent<CubismModel>();
			if (component != null)
			{
				return component;
			}
			if (includeParents)
			{
				Transform parent = self.transform.parent;
				while (parent != null)
				{
					component = parent.GetComponent<CubismModel>();
					if ((bool)component)
					{
						return component;
					}
					parent = parent.parent;
				}
			}
			return null;
		}
	}
}
