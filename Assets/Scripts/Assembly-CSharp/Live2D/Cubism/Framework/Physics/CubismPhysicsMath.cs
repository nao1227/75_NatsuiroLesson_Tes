using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	internal static class CubismPhysicsMath
	{
		public static float DegreesToRadian(float degrees)
		{
			return degrees / 180f * (float)Math.PI;
		}

		public static float RadianToDegrees(float radian)
		{
			return radian * 180f / (float)Math.PI;
		}

		public static float DirectionToRadian(Vector2 from, Vector2 to)
		{
			float q = Mathf.Atan2(to.y, to.x);
			float q2 = Mathf.Atan2(from.y, from.x);
			return GetAngleDiff(q, q2);
		}

		public static float GetAngleDiff(float q1, float q2)
		{
			float num;
			for (num = q1 - q2; num < -(float)Math.PI; num += (float)Math.PI * 2f)
			{
			}
			while (num > (float)Math.PI)
			{
				num -= (float)Math.PI * 2f;
			}
			return num;
		}

		public static float DirectionToDegrees(Vector2 from, Vector2 to)
		{
			float num = RadianToDegrees(DirectionToRadian(from, to));
			if (to.x - from.x > 0f)
			{
				num = 0f - num;
			}
			return num;
		}

		public static Vector2 RadianToDirection(float totalAngle)
		{
			Vector2 zero = Vector2.zero;
			zero.x = Mathf.Sin(totalAngle);
			zero.y = Mathf.Cos(totalAngle);
			return zero;
		}

		private static float GetRangeValue(float min, float max)
		{
			float num = Mathf.Max(min, max);
			float num2 = Mathf.Min(min, max);
			return Mathf.Abs(num - num2);
		}

		private static float GetDefaultValue(float min, float max)
		{
			return Mathf.Min(min, max) + GetRangeValue(min, max) / 2f;
		}

		public static float Normalize(CubismParameter parameter, ref float parameterValue, float normalizedMinimum, float normalizedMaximum, float normalizedDefault, bool isInverted = false)
		{
			float num = 0f;
			float num2 = Mathf.Max(parameter.MaximumValue, parameter.MinimumValue);
			if (num2 < parameterValue)
			{
				parameterValue = num2;
			}
			float num3 = Mathf.Min(parameter.MaximumValue, parameter.MinimumValue);
			if (num3 > parameterValue)
			{
				parameterValue = num3;
			}
			float num4 = Mathf.Min(normalizedMinimum, normalizedMaximum);
			float num5 = Mathf.Max(normalizedMinimum, normalizedMaximum);
			float defaultValue = GetDefaultValue(num3, num2);
			float num6 = parameterValue - defaultValue;
			switch ((int)Mathf.Sign(num6))
			{
			case 1:
			{
				float num9 = num5 - normalizedDefault;
				float num10 = num2 - defaultValue;
				if (num10 != 0f)
				{
					num = num6 * (num9 / num10);
					num += normalizedDefault;
				}
				break;
			}
			case -1:
			{
				float num7 = num4 - normalizedDefault;
				float num8 = num3 - defaultValue;
				if (num8 != 0f)
				{
					num = num6 * (num7 / num8);
					num += normalizedDefault;
				}
				break;
			}
			case 0:
				num = normalizedDefault;
				break;
			}
			if (!isInverted)
			{
				return num * -1f;
			}
			return num;
		}
	}
}
