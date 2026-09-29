using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public class PartBCScenario
	{
		public int Day;

		public ScenarioLabel ScenarioLabel;

		public SceneName MoveScene;

		public bool IsDayEnd;

		public bool GoToHScene;

		public bool NoFading;
	}
}
