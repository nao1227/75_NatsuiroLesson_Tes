using UnityEngine;

namespace Utage
{
	internal class AdvCommandResetPivot : AdvCommand
	{
		private readonly string targetName;

		public AdvCommandResetPivot(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			targetName = ParseCell<string>(AdvColumnName.Arg1);
		}

		public override void DoCommand(AdvEngine engine)
		{
			AdvGraphicObject advGraphicObject = engine.GraphicManager.FindObject(targetName);
			if (advGraphicObject == null)
			{
				Debug.LogError(targetName + " is not found");
			}
			else
			{
				advGraphicObject.ResetPivot();
			}
		}
	}
}
