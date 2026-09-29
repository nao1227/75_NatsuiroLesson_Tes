using System;
using Serialize;

[Serializable]
public class ScenarioNamePair : KeyAndValue<ScenarioLabel, string>
{
	public ScenarioNamePair(ScenarioLabel key, string value)
		: base(key, value)
	{
	}
}
