using UnityEngine;

namespace Utage
{
	internal class AdvCommandSetPivot : AdvCommand
	{
		private readonly string targetName;

		private readonly float pivotX;

		private readonly float pivotY;

		private readonly float x;

		private readonly float y;

		private readonly AdvGraphicObjectPivotType pivotType;

		public AdvCommandSetPivot(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			targetName = ParseCell<string>(AdvColumnName.Arg1);
			switch (ParseCell<string>(AdvColumnName.Arg2))
			{
			case "Left":
				pivotX = 0f;
				break;
			case "Center":
				pivotX = 0.5f;
				break;
			case "Right":
				pivotX = 1f;
				break;
			default:
				pivotX = ParseCell<float>(AdvColumnName.Arg2);
				break;
			}
			switch (ParseCell<string>(AdvColumnName.Arg3))
			{
			case "Bottom":
				pivotY = 0f;
				break;
			case "Center":
				pivotY = 0.5f;
				break;
			case "Top":
				pivotY = 1f;
				break;
			default:
				pivotY = ParseCell<float>(AdvColumnName.Arg3);
				break;
			}
			x = ParseCellOptional(AdvColumnName.Arg4, 0f);
			y = ParseCellOptional(AdvColumnName.Arg5, 0f);
			pivotType = ParseCellOptional(AdvColumnName.Arg6, AdvGraphicObjectPivotType.SpritePos);
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
				advGraphicObject.SetPivot(pivotX, pivotY, x, y, pivotType);
			}
		}
	}
}
