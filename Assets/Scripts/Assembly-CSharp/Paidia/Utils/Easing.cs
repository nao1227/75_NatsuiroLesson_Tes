using System;
using UnityEngine;

namespace Paidia.Utils
{
	public static class Easing
	{
		public enum Ease
		{
			Linear = 0,
			InSine = 1,
			OutSine = 2,
			InOutSine = 3,
			InQuad = 4,
			OutQuad = 5,
			InOutQuad = 6,
			InCubic = 7,
			OutCubic = 8,
			InOutCubic = 9,
			InQuart = 10,
			OutQuart = 11,
			InOutQuart = 12,
			InQuint = 13,
			OutQuint = 14,
			InOutQuint = 15,
			InExpo = 16,
			OutExpo = 17,
			InOutExpo = 18,
			InCirc = 19,
			OutCirc = 20,
			InOutCirc = 21,
			InBack = 22,
			OutBack = 23,
			InOutBack = 24,
			InElastic = 25,
			OutElastic = 26,
			InOutElastic = 27,
			InBounce = 28,
			OutBounce = 29,
			InOutBounce = 30
		}

		public static Func<float, float> GetEasingMethod(Ease ease)
		{
			return ease switch
			{
				Ease.Linear => Linear, 
				Ease.InSine => EaseInSine, 
				Ease.OutSine => EaseOutSine, 
				Ease.InOutSine => EaseInOutSine, 
				Ease.InQuad => EaseInQuad, 
				Ease.OutQuad => EaseOutQuad, 
				Ease.InOutQuad => EaseInOutQuad, 
				Ease.InCubic => EaseInCubic, 
				Ease.OutCubic => EasesOutCubic, 
				Ease.InOutCubic => EasesInOutCubic, 
				Ease.InQuart => EaseInQuart, 
				Ease.OutQuart => EaseOutQuart, 
				Ease.InOutQuart => EaseInOutQuart, 
				Ease.InQuint => EaseInQuint, 
				Ease.OutQuint => EaseOutQuint, 
				Ease.InOutQuint => EaseInOutQuint, 
				Ease.InExpo => EaseInExpo, 
				Ease.OutExpo => EaseOutExpo, 
				Ease.InOutExpo => EaseInOutExpo, 
				Ease.InCirc => EaseInCirc, 
				Ease.OutCirc => EaseOutCirc, 
				Ease.InOutCirc => EaseInOutCirc, 
				Ease.InBack => EaseInBack, 
				Ease.OutBack => EaseOutBack, 
				Ease.InOutBack => EaseInOutBack, 
				Ease.InElastic => EaseInElastic, 
				Ease.OutElastic => EaseOutElastic, 
				Ease.InOutElastic => EaseInOutElastic, 
				Ease.InBounce => EaseInBounce, 
				Ease.OutBounce => EaseOutBounce, 
				Ease.InOutBounce => EaseInOutBounce, 
				_ => Linear, 
			};
		}

		public static float Linear(float x)
		{
			return x;
		}

		public static float EaseInSine(float x)
		{
			return 1f - Mathf.Cos(x * (float)Math.PI / 2f);
		}

		public static float EaseOutSine(float x)
		{
			return Mathf.Sin(x * (float)Math.PI / 2f);
		}

		public static float EaseInOutSine(float x)
		{
			return (0f - (Mathf.Cos((float)Math.PI * x) - 1f)) / 2f;
		}

		public static float EaseInQuad(float x)
		{
			return x * x;
		}

		public static float EaseOutQuad(float x)
		{
			return 1f - (1f - x) * (1f - x);
		}

		public static float EaseInOutQuad(float x)
		{
			if (!(x < 0.5f))
			{
				return 1f - Mathf.Pow(-2f * x + 2f, 2f) / 2f;
			}
			return 2f * x * x;
		}

		public static float EaseInCubic(float x)
		{
			return x * x * x;
		}

		public static float EasesOutCubic(float x)
		{
			return 1f - Mathf.Pow(1f - x, 3f);
		}

		public static float EasesInOutCubic(float x)
		{
			if (!(x < 0.5f))
			{
				return 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
			}
			return 4f * x * x * x;
		}

		public static float EaseInQuart(float x)
		{
			return x * x * x * x;
		}

		public static float EaseOutQuart(float x)
		{
			return 1f - Mathf.Pow(1f - x, 4f);
		}

		public static float EaseInOutQuart(float x)
		{
			if (!(x < 0.5f))
			{
				return 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f;
			}
			return 8f * x * x * x * x;
		}

		public static float EaseInQuint(float x)
		{
			return x * x * x * x * x;
		}

		public static float EaseOutQuint(float x)
		{
			return 1f - Mathf.Pow(1f - x, 5f);
		}

		public static float EaseInOutQuint(float x)
		{
			if (!(x < 0.5f))
			{
				return 1f - Mathf.Pow(-2f * x + 2f, 5f) / 2f;
			}
			return 16f * x * x * x * x * x;
		}

		public static float EaseInExpo(float x)
		{
			if (x != 0f)
			{
				return Mathf.Pow(2f, 10f * x - 10f);
			}
			return 0f;
		}

		public static float EaseOutExpo(float x)
		{
			if (x != 1f)
			{
				return 1f - Mathf.Pow(2f, -10f * x);
			}
			return 1f;
		}

		public static float EaseInOutExpo(float x)
		{
			if (x != 0f)
			{
				if (x != 1f)
				{
					if (!(x < 0.5f))
					{
						return (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
					}
					return Mathf.Pow(2f, 20f * x - 10f) / 2f;
				}
				return 1f;
			}
			return 0f;
		}

		public static float EaseInCirc(float x)
		{
			return 1f - Mathf.Sqrt(1f - Mathf.Pow(x, 2f));
		}

		public static float EaseOutCirc(float x)
		{
			return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
		}

		public static float EaseInOutCirc(float x)
		{
			if (!(x < 0.5f))
			{
				return (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
			}
			return (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f;
		}

		public static float EaseInBack(float x)
		{
			float num = 1.70158f;
			return (num + 1f) * x * x * x - num * x * x;
		}

		public static float EaseOutBack(float x)
		{
			float num = 1.70158f;
			float num2 = num + 1f;
			return 1f + num2 * Mathf.Pow(x - 1f, 3f) + num * Mathf.Pow(x - 1f, 2f);
		}

		public static float EaseInOutBack(float x)
		{
			float num = 1.70158f * 1.525f;
			if (!(x < 0.5f))
			{
				return (Mathf.Pow(2f * x - 2f, 2f) * ((num + 1f) * (x * 2f - 2f) + num) + 2f) / 2f;
			}
			return Mathf.Pow(2f * x, 2f) * ((num + 1f) * 2f * x - num) / 2f;
		}

		public static float EaseInElastic(float x)
		{
			float num = (float)Math.PI * 2f / 3f;
			if (x != 0f)
			{
				if (x != 1f)
				{
					return (0f - Mathf.Pow(2f, 10f * x - 10f)) * Mathf.Sin((x * 10f - 10.75f) * num);
				}
				return 1f;
			}
			return 0f;
		}

		public static float EaseOutElastic(float x)
		{
			float num = (float)Math.PI * 2f / 3f;
			if (x != 0f)
			{
				if (x != 1f)
				{
					return Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * num) + 1f;
				}
				return 1f;
			}
			return 0f;
		}

		public static float EaseInOutElastic(float x)
		{
			float num = (float)Math.PI * 4f / 9f;
			if (x != 0f)
			{
				if (x != 1f)
				{
					if (!(x < 0.5f))
					{
						return Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - 11.125f) * num) / 2f + 1f;
					}
					return (0f - Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - 11.125f) * num)) / 2f;
				}
				return 1f;
			}
			return 0f;
		}

		public static float EaseInBounce(float x)
		{
			return 1f - EaseOutBounce(1f - x);
		}

		public static float EaseOutBounce(float x)
		{
			float num = 7.5625f;
			float num2 = 2.75f;
			if (x < 1f / num2)
			{
				return num * x * x;
			}
			if (x < 2f / num2)
			{
				float num3 = x - 1.5f / num2;
				return num * num3 * num3 + 0.75f;
			}
			if ((double)x < 2.5 / (double)num2)
			{
				float num4 = x - 2.25f / num2;
				return num * num4 * num4 + 0.9375f;
			}
			float num5 = x - 2.625f / num2;
			return num * num5 * num5 + 63f / 64f;
		}

		public static float EaseInOutBounce(float x)
		{
			if (!(x < 0.5f))
			{
				return (1f + EaseOutBounce(2f * x - 1f)) / 2f;
			}
			return (1f - EaseOutBounce(1f - 2f * x)) / 2f;
		}
	}
}
