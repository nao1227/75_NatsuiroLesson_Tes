using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public struct FeelingParameterRange
	{
		public int Upper;

		public int Lower;

		public bool IsInRange(int val)
		{
			if (Lower <= val)
			{
				return Upper >= val;
			}
			return false;
		}
	}
}
