using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class MainScenarioDict
	{
		public List<DayScenarioPair> Dictionary;

		public ScenarioLabel GetLabel(int day)
		{
			if (HasLabel(day))
			{
				return Dictionary.First((DayScenarioPair x) => x.Day == day).Label;
			}
			return ScenarioLabel.None;
		}

		public bool HasLabel(int day)
		{
			return Dictionary.Count((DayScenarioPair x) => x.Day == day) > 0;
		}

		public bool HasLabel(ScenarioLabel label)
		{
			return Dictionary.Count((DayScenarioPair x) => x.Label == label) > 0;
		}

		public bool HasGreeting(int day)
		{
			if (HasLabel(day))
			{
				return Dictionary.First((DayScenarioPair x) => x.Day == day).HasGreeting;
			}
			return false;
		}

		public bool Contains(ScenarioLabel label)
		{
			return Dictionary.Count((DayScenarioPair x) => x.Label == label) > 0;
		}
	}
}
