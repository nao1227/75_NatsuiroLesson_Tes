using System;
using Live2D.Cubism.Core;

public class ParameterValue
{
	private float _value;

	private bool _showLog;

	public readonly float Value;

	protected float _min;

	protected float _max;

	private const float EPS = 0.001f;

	public float AbsoluteValue => Math.Abs(Value);

	public static float AlmostZero => 0.001f;

	public ParameterValue(float value, float _min = 0f, float _max = 1f)
	{
		this._min = _min;
		this._max = _max;
		float value2 = value;
		if (Math.Abs(value2) < 0.001f)
		{
			value2 = 0f;
		}
		if (value < this._min)
		{
			Value = this._min;
		}
		else if (value > this._max)
		{
			Value = this._max;
		}
		else
		{
			Value = value2;
		}
	}

	public ParameterValue(CubismParameter param)
	{
		if (param.Id == "ParamAngleY")
		{
			_showLog = true;
		}
		_min = param.MinimumValue;
		_max = param.MaximumValue;
		Value = param.DefaultValue;
	}

	protected ParameterValue()
	{
	}

	public static ParameterValue operator +(ParameterValue a, float b)
	{
		return new ParameterValue(a.Value + b, a._min, a._max);
	}

	public static ParameterValue operator -(ParameterValue a, float b)
	{
		return new ParameterValue(a.Value - b, a._min, a._max);
	}

	public static ParameterValue operator *(ParameterValue a, float b)
	{
		return new ParameterValue(a.Value * b, a._min, a._max);
	}

	public virtual ParameterValue Update(float newVal)
	{
		return new ParameterValue(newVal, _min, _max);
	}

	public static ParameterValue GetHandParameterValue()
	{
		return new ParameterValue(0f);
	}

	public bool IsAlmostZero()
	{
		return AbsoluteValue < AlmostZero;
	}

	public bool IsMax()
	{
		return Value == _max;
	}

	public bool IsMin()
	{
		return Value == _min;
	}
}
