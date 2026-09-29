using System;
using System.Collections.Generic;

namespace Paidia.satsuki1
{
	[Serializable]
	public class SubEvent
	{
		public List<EventCondition> Conditions;

		public ScenarioLabel Label;

		public SceneName SceneName;

		public FlagEnum UnlockFlag;
	}
}
