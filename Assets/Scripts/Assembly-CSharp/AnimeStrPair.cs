using System;
using Serialize;

[Serializable]
public class AnimeStrPair : KeyAndValue<AnimeName, string>
{
	public AnimeStrPair(AnimeName key, string value)
		: base(key, value)
	{
	}
}
