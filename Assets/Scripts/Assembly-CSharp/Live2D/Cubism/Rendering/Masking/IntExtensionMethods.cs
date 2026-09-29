using UnityEngine;

namespace Live2D.Cubism.Rendering.Masking
{
	internal static class IntExtensionMethods
	{
		public static bool IsPowerOfTwo(this int self)
		{
			return Mathf.ClosestPowerOfTwo(self) == self;
		}
	}
}
