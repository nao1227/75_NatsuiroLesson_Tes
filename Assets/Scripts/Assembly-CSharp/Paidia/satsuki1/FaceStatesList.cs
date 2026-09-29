using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class FaceStatesList
	{
		public int DefaultCrossFadeTime = 75;

		public List<NamedWeightedStateList> List;

		private OsawariConditions _emptyConditions = OsawariConditions.Empty;

		public WeightedStateList GetFaceList(FaceListName name, OsawariManager manager)
		{
			if (List == null)
			{
				return null;
			}
			if (List.Count((NamedWeightedStateList x) => x.Name == name && x.Condition.IsFullfillCondition(manager.TemporaryStatus, _emptyConditions) && manager.ContextManager.Context == x.Mode) > 0)
			{
				return List.First((NamedWeightedStateList x) => x.Name == name && x.Condition.IsFullfillCondition(manager.TemporaryStatus, _emptyConditions) && manager.ContextManager.Context == x.Mode).List;
			}
			return new WeightedStateList();
		}

		public int GetCrossFadeTime(FaceStateName name)
		{
			if (List == null)
			{
				return DefaultCrossFadeTime;
			}
			if (List.Any((NamedWeightedStateList x) => x.List.Contains(name)))
			{
				return List.First((NamedWeightedStateList x) => x.List.Contains(name)).CrossFadeTime;
			}
			return DefaultCrossFadeTime;
		}
	}
}
