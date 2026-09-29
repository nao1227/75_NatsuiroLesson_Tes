using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariEventHolder : AbstractOsawari
	{
		public int ActiveUntilDay;

		public EventType EventType;

		protected override void InitializeParams()
		{
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
		}

		protected override void AutoAnimation()
		{
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return 0;
		}

		protected override void OnLateUpdate()
		{
		}

		protected override void UpdateWhileNotClicked()
		{
		}

		protected override bool GetRestrictedCore()
		{
			return true;
		}

		protected override bool GetConstraintsCore()
		{
			if (EventType == EventType.Note)
			{
				if (SaveLoadManager.UnsavedData.Days == 1)
				{
					return !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Note_Day1);
				}
				return false;
			}
			if (EventType == EventType.Face)
			{
				return SaveLoadManager.UnsavedData.Days switch
				{
					1 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Face_Day1), 
					2 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Face_Day2), 
					3 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Face_Day3), 
					_ => false, 
				};
			}
			return SaveLoadManager.UnsavedData.Days <= ActiveUntilDay;
		}
	}
}
