using UnityEngine;

namespace Utage
{
	internal abstract class AdvCommandBgBase : AdvCommand
	{
		protected string label;

		protected AdvGraphicInfoList graphic;

		protected string layerName;

		protected float fadeTime;

		protected AdvCommandBgBase(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.TextureSetting.ContainsLabel(label))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}
			graphic = dataManager.TextureSetting.LabelToGraphic(label);
			AddLoadGraphic(graphic);
			layerName = ParseCellOptional(AdvColumnName.Arg3, "");
			if (!string.IsNullOrEmpty(layerName) && !dataManager.LayerSetting.Contains(layerName, AdvLayerSettingData.LayerType.Bg))
			{
				Debug.LogError(ToErrorString(layerName + " is not contained in layer setting"));
			}
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
		}

		protected virtual AdvGraphicOperationArg DoCommandBgSub(AdvEngine engine)
		{
			AdvGraphicOperationArg advGraphicOperationArg = new AdvGraphicOperationArg(this, graphic.Main, fadeTime);
			if (string.IsNullOrEmpty(layerName))
			{
				engine.GraphicManager.BgManager.DrawToDefault(engine.GraphicManager.BgSpriteName, advGraphicOperationArg);
			}
			else
			{
				engine.GraphicManager.BgManager.Draw(layerName, engine.GraphicManager.BgSpriteName, advGraphicOperationArg);
			}
			AdvGraphicObject advGraphicObject = engine.GraphicManager.BgManager.FindObject(engine.GraphicManager.BgSpriteName);
			if (advGraphicObject != null)
			{
				advGraphicObject.SetCommandPostion(this);
				advGraphicObject.TargetObject.SetCommandArg(this);
			}
			return advGraphicOperationArg;
		}
	}
}
