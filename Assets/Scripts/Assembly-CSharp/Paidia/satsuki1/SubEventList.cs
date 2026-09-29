using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class SubEventList
	{
		public List<SubEvent> List;

		public SubEvent GetSubEvent(SceneName name)
		{
			List<SubEvent> list = List.Where((SubEvent x) => x.SceneName == name).ToList();
			if (list.Count > 0)
			{
				List<SubEvent> list2 = list.Where((SubEvent x) => x.Conditions.All((EventCondition y) => y.IsFUllfilSubEventCondition()) && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(x.Label)).ToList();
				if (list2.Count > 0)
				{
					string text = "TEST labels: ";
					foreach (SubEvent item in list2)
					{
						text = text + item.Label.ToString() + ", ";
					}
					return list2[0];
				}
			}
			return null;
		}

		public bool Contains(ScenarioLabel label)
		{
			return List.Any((SubEvent x) => x.Label == label);
		}

		public SubEvent GetSubEventByLabel(ScenarioLabel label)
		{
			return List.First((SubEvent x) => x.Label == label);
		}
	}
}
