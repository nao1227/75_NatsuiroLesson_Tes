using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class EventFlagCondition
	{
		public List<FlagEnum> Flags;

		public bool IsFullfillCondition()
		{
			if (Flags.Count == 0)
			{
				return true;
			}
			return Flags.Count((FlagEnum x) => SaveLoadManager.UnsavedData.GlobalFlags.IsOn(x)) == Flags.Count;
		}
	}
}
