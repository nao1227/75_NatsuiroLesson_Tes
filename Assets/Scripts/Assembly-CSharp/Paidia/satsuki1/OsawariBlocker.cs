using System.Collections.Generic;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariBlocker : MonoBehaviour
	{
		public List<BlockCondition> Conditions;

		public virtual bool IsBlocked()
		{
			if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH)
			{
				return false;
			}
			foreach (BlockCondition condition in Conditions)
			{
				switch (condition.BlockType)
				{
				case BlockType.DayBefore:
					if (SaveLoadManager.UnsavedData.Days < condition.Value)
					{
						return true;
					}
					break;
				case BlockType.DayAfter:
					if (SaveLoadManager.UnsavedData.Days > condition.Value)
					{
						return true;
					}
					break;
				case BlockType.Flag:
					if (!SaveLoadManager.UnsavedData.GlobalFlags.IsOn((FlagEnum)condition.Value))
					{
						return true;
					}
					break;
				case BlockType.FlagOff:
					if (!SaveLoadManager.UnsavedData.GlobalFlags.IsOn((FlagEnum)condition.Value))
					{
						return true;
					}
					break;
				case BlockType.ScenarioRead:
					if (SaveLoadManager.UnsavedData.GlobalFlags.IsRead((ScenarioLabel)condition.Value))
					{
						return true;
					}
					break;
				case BlockType.ScenarioUnread:
					if (!SaveLoadManager.UnsavedData.GlobalFlags.IsRead((ScenarioLabel)condition.Value))
					{
						return true;
					}
					break;
				case BlockType.IsDay:
					if (SaveLoadManager.UnsavedData.Days == condition.Value)
					{
						return true;
					}
					break;
				}
			}
			return false;
		}
	}
}
