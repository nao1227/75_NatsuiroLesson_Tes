using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class PartBCScenarioList
	{
		public List<PartBCScenario> List;

		public bool HasScenario(int day)
		{
			return List.Any((PartBCScenario x) => x.Day == day);
		}

		public bool HasScenario(ScenarioLabel label)
		{
			return List.Any((PartBCScenario x) => x.ScenarioLabel == label);
		}

		public PartBCScenario Get(int day)
		{
			return List.First((PartBCScenario x) => x.Day == day);
		}

		public PartBCScenario Get(ScenarioLabel label)
		{
			return List.First((PartBCScenario x) => x.ScenarioLabel == label);
		}
	}
}
