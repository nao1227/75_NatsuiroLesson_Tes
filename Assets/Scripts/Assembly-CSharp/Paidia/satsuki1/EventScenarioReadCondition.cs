using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class EventScenarioReadCondition
	{
		public List<ScenarioLabel> ScenarioLabels;

		public bool IsFullfillCondition()
		{
			if (ScenarioLabels.Count == 0)
			{
				return true;
			}
			return ScenarioLabels.Count((ScenarioLabel x) => SaveLoadManager.UnsavedData.GlobalFlags.IsRead(x)) == ScenarioLabels.Count;
		}
	}
}
