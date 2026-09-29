using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public class ReadLabel
	{
		public ScenarioLabel Name;

		public bool IsRead;

		public ReadLabel(ScenarioLabel name)
		{
			Name = name;
			IsRead = false;
		}

		public void SetRead()
		{
			IsRead = true;
		}
	}
}
