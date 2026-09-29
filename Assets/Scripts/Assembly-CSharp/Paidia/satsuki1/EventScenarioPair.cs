using System;
using System.Collections.Generic;

namespace Paidia.satsuki1
{
	[Serializable]
	public class EventScenarioPair
	{
		public List<EventCondition> Conditions;

		public ScenarioLabel Label;

		public Timing Timing;
	}
}
