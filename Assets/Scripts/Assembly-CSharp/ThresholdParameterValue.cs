using Live2D.Cubism.Core;

public class ThresholdParameterValue
{
	public float[] thresholds;

	public int index;

	public int SectionCount => thresholds.Length - 1;

	protected float _min => thresholds[index];

	protected float _max => thresholds[index + 1];

	public float Value { get; protected set; }

	public ThresholdParameterValue(CubismParameter param, float[] _thresholds)
	{
		index = 0;
		thresholds = new float[_thresholds.Length + 2];
		thresholds[0] = param.MinimumValue;
		for (int i = 0; i < _thresholds.Length; i++)
		{
			thresholds[i + 1] = _thresholds[i];
		}
		thresholds[thresholds.Length - 1] = param.MaximumValue;
		Value = param.Value;
		SetIndex();
	}

	protected ThresholdParameterValue(float val, float[] _thresholds, int idx)
	{
		index = idx;
		thresholds = _thresholds;
		SetValue(val);
	}

	protected void SetIndex()
	{
		while (Value > thresholds[index] - float.Epsilon)
		{
			index++;
		}
	}

	private void SetValue(float val)
	{
		if (val > _max)
		{
			val = _max;
		}
		else if (val < _min)
		{
			val = _min;
		}
		Value = val;
	}

	public ThresholdParameterValue MoveSectionTo(int newIndex, bool useMinAsDefault = true)
	{
		float val = ((!useMinAsDefault) ? GetMaxValueOfSection(newIndex) : GetMinValueOfSection(newIndex));
		return new ThresholdParameterValue(val, thresholds, newIndex);
	}

	public float GetMinValueOfSection(int target)
	{
		return thresholds[target];
	}

	public float GetMaxValueOfSection(int target)
	{
		return thresholds[target + 1];
	}

	public bool IsOnThresholdMax()
	{
		return Value == _max;
	}

	public bool IsOnThresholdMin()
	{
		return Value == _min;
	}

	public override string ToString()
	{
		string text = "";
		float[] array = thresholds;
		foreach (float num in array)
		{
			text = text + num + ", ";
		}
		return $"min: {_min}, max: {_max}, val:{Value}, thresholds: {text}";
	}

	public static ThresholdParameterValue operator +(ThresholdParameterValue a, float b)
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
		return new ThresholdParameterValue(num - float.Epsilon, a.thresholds, a.index);
	}

	public static ThresholdParameterValue operator -(ThresholdParameterValue a, float b)
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
		return new ThresholdParameterValue(num - float.Epsilon, a.thresholds, a.index);
	}

	public static ThresholdParameterValue operator *(ThresholdParameterValue a, float b)
	{
		float num = a.Value * b;
		if (a._max < num)
		{
			num = a._max;
		}
		if (a._min > num)
		{
			num = a._min;
		}
		return new ThresholdParameterValue(num - float.Epsilon, a.thresholds, a.index);
	}

	private void SetValueRange()
	{
		index = thresholds.Length - 1;
		for (int i = 0; i < thresholds.Length; i++)
		{
			if (Value < thresholds[i])
			{
				index = i;
				break;
			}
		}
	}

	public ThresholdParameterValue Update(float val)
	{
		return new ThresholdParameterValue(val, thresholds, index);
	}
}
