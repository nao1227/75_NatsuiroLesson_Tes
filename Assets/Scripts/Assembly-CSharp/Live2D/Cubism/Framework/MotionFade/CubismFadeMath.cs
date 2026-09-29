using System;

namespace Live2D.Cubism.Framework.MotionFade
{
	public static class CubismFadeMath
	{
		public static float GetEasingSine(float value)
		{
			if (value < 0f)
			{
				return 0f;
			}
			if (value > 1f)
			{
				return 1f;
			}
			return (float)(0.5 - 0.5 * Math.Cos(value * (float)Math.PI));
		}
	}
}
