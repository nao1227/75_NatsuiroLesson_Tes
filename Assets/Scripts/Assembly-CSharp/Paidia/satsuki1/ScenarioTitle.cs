using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public class ScenarioTitle
	{
		public ScenarioLabel Label;

		public string Title;

		public FreeSccenarioCategory Category;

		public bool UsedInFreeScenario = true;
	}
}
