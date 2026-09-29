using UnityEngine;

namespace Live2D.Cubism.Core
{
	public static class GameObjectExtensionMethods
	{
		public static CubismModel FindCubismModel(this GameObject self, bool includeParents = false)
		{
			if (self == null)
			{
				return null;
			}
			return self.transform.FindCubismModel(includeParents);
		}
	}
}
