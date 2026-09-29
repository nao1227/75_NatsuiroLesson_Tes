using System;
using Serialize;

[Serializable]
public class ParameterIntPair : KeyAndValue<ParameterName, int>
{
	public ParameterIntPair(ParameterName key, int value)
		: base(key, value)
	{
	}
}
