using UnityEngine;

namespace Utage
{
	internal class AdvCommandChangeLayer : AdvCommand
	{
		private readonly string objectName;

		private readonly string layerName;

		private readonly AdvChangeLayerRepositionType repositionType;

		private readonly float fadeTime;

		public AdvCommandChangeLayer(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			objectName = ParseCell<string>(AdvColumnName.Arg1);
			repositionType = ParseCellOptional(AdvColumnName.Arg2, AdvChangeLayerRepositionType.KeepGlobal);
			layerName = ParseCell<string>(AdvColumnName.Arg3);
			if (!dataManager.LayerSetting.Contains(layerName))
			{
				Debug.LogError(row.ToErrorString("Not found " + layerName + " Please input Layer name"));
			}
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.GraphicManager.ChangeLayer(objectName, layerName, repositionType, engine.Page.ToSkippedTime(fadeTime));
		}
	}
}
