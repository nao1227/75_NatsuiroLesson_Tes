public class BoolParameterValue : ParameterValue
{
	public BoolParameterValue(bool val = false)
		: base(val ? 1 : 0)
	{
	}

	public bool AsBool()
	{
		if (Value != 1f)
		{
			return false;
		}
		return true;
	}

	public BoolParameterValue Update(bool val)
	{
		return new BoolParameterValue(val);
	}
}
