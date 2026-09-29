using System;

[Serializable]
public struct SpeedRange
{
	public float Upper;

	public float Lower;

	public bool IsInRange(float val)
	{
		if (Upper <= Lower)
		{
			return false;
		}
		if (Upper >= val)
		{
			return Lower < val;
		}
		return false;
	}
}
