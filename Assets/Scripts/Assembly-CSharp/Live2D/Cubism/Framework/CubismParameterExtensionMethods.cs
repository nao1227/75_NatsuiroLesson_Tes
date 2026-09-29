using Live2D.Cubism.Core;

namespace Live2D.Cubism.Framework
{
	public static class CubismParameterExtensionMethods
	{
		public static void AddToValue(this CubismParameter parameter, float value, float weight = 1f)
		{
			if (!(parameter == null))
			{
				parameter.Value += value * weight;
			}
		}

		public static void MultiplyValueBy(this CubismParameter parameter, float value, float weight = 1f)
		{
			if (!(parameter == null))
			{
				parameter.Value *= 1f + (value - 1f) * weight;
			}
		}

		public static void BlendToValue(this CubismParameter self, CubismParameterBlendMode mode, float value, float weight = 1f)
		{
			if (!(self == null))
			{
				switch (mode)
				{
				case CubismParameterBlendMode.Additive:
					self.AddToValue(value, weight);
					break;
				case CubismParameterBlendMode.Multiply:
					self.MultiplyValueBy(value, weight);
					break;
				default:
					self.Value = self.Value * (1f - weight) + value * weight;
					break;
				}
			}
		}

		public static void BlendToValue(this CubismParameter[] self, CubismParameterBlendMode mode, float value, float weight = 1f)
		{
			if (self == null)
			{
				return;
			}
			switch (mode)
			{
			case CubismParameterBlendMode.Additive:
			{
				for (int j = 0; j < self.Length; j++)
				{
					self[j].AddToValue(value, weight);
				}
				break;
			}
			case CubismParameterBlendMode.Multiply:
			{
				for (int k = 0; k < self.Length; k++)
				{
					self[k].MultiplyValueBy(value, weight);
				}
				break;
			}
			default:
			{
				for (int i = 0; i < self.Length; i++)
				{
					self[i].Value = self[i].Value * (1f - weight) + value * weight;
				}
				break;
			}
			}
		}
	}
}
