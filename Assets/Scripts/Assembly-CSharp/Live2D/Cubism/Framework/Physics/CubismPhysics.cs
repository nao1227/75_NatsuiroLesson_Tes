using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	public static class CubismPhysics
	{
		public static Vector2 Gravity = Vector2.down;

		public static Vector2 Wind = Vector2.zero;

		public static float AirResistance = 5f;

		public static float MaximumWeight = 100f;

		public static bool UseFixedDeltaTime = false;

		public static bool UseAngleCorrection = true;

		public const float MovementThreshold = 0.001f;

		public const float MaxDeltaTime = 5f;
	}
}
