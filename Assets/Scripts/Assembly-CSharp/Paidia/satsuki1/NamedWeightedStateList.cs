using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public class NamedWeightedStateList
	{
		public FaceListName Name;

		public WeightedStateList List;

		public EventCondition Condition;

		public OsawariContext Mode;

		public bool Loop = true;

		public int CrossFadeTime = 75;
	}
}
