using Live2D.Cubism.Core;

public class SingleThresholdParameterValue : ThresholdParameterValue
{
	public SingleThresholdParameterValue(CubismParameter param, float threshold)
		: base(param, new float[1] { threshold })
	{
	}

	private SingleThresholdParameterValue(float val, float[] thresholds, int idx)
		: base(val, thresholds, idx)
	{
	}

	public static SingleThresholdParameterValue operator +(SingleThresholdParameterValue a, float b)
	{
		float num = a.Value + b;
		if (a._max < num)
		{
			num = a._max;
		}
		if (a._min > num)
		{
			num = a._min;
		}
		return new SingleThresholdParameterValue(num - float.Epsilon, a.thresholds, a.index);
	}

	public static SingleThresholdParameterValue operator -(SingleThresholdParameterValue a, float b)
	{
		float num = a.Value - b;
		if (a._max < num)
		{
			num = a._max;
		}
		if (a._min > num)
		{
			num = a._min;
		}
		return new SingleThresholdParameterValue(num - float.Epsilon, a.thresholds, a.index);
	}

	public new SingleThresholdParameterValue Update(float val)
	{
		return new SingleThresholdParameterValue(val, thresholds, index);
	}

	private new SingleThresholdParameterValue MoveSectionTo(int newIndex, bool useMinAsDefault = true)
	{
		float val = ((!useMinAsDefault) ? GetMaxValueOfSection(newIndex) : GetMinValueOfSection(newIndex));
		return new SingleThresholdParameterValue(val, thresholds, newIndex);
	}

	public SingleThresholdParameterValue MoveToUpperSection(bool useMinAsDefault = true)
	{
		return MoveSectionTo(1, useMinAsDefault);
	}

	public SingleThresholdParameterValue MoveToLowerSection(bool useMinAsDefault = false)
	{
		return MoveSectionTo(0, useMinAsDefault);
	}
}
