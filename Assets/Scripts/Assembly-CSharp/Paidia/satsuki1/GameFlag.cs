using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public class GameFlag
	{
		public FlagEnum Name;

		public bool IsOn;

		public GameFlag(FlagEnum name)
		{
			Name = name;
			IsOn = false;
		}

		public void SetFlag(bool isOn)
		{
			IsOn = isOn;
		}
	}
}
