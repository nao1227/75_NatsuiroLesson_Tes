using UniRx;
using UnityEngine;

public class BreathFrequency
{
	private FloatReactiveProperty _val;

	public IReadOnlyReactiveProperty<float> Value => _val;

	public BreathFrequency(float freq)
	{
		_val = new FloatReactiveProperty(freq);
		_val.Value += 1f;
	}

	public float GetBreathSpeed()
	{
		if (Mathf.Abs(_val.Value) > 0.0001f)
		{
			return 1f / _val.Value;
		}
		return 0f;
	}

	public void SetFrequency(float val)
	{
		_val.Value = val + 1f;
	}

	public float GetValue()
	{
		return Value.Value - 1f;
	}
}
