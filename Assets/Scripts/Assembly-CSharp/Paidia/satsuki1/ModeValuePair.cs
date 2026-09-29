using Live2D.Cubism.Framework;

namespace Paidia.satsuki1
{
	public struct ModeValuePair
	{
		public CubismParameterBlendMode Mode;

		public float Value;

		public ModeValuePair(float value, CubismParameterBlendMode mode)
		{
			Mode = mode;
			Value = value;
		}

		public override string ToString()
		{
			return $"Mode: {Mode}. Value: {Value}";
		}
	}
}
