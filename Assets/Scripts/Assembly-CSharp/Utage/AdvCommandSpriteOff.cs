using UnityEngine;

namespace Utage
{
	internal class AdvCommandSpriteOff : AdvCommand
	{
		private string name;

		private float fadeTime = 0.2f;

		public AdvCommandSpriteOff(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "AllSpriteLayers");
			fadeTime = ParseCellOptional(AdvColumnName.Arg6, fadeTime);
		}

		public override void DoCommand(AdvEngine engine)
		{
			float num = engine.Page.ToSkippedTime(fadeTime);
			switch (name)
			{
			case "AllSpriteLayers":
				engine.GraphicManager.SpriteManager.FadeOutAll(num);
				return;
			case "AllSpriteObjects":
				engine.GraphicManager.FadeOutAllObjects(AdvGraphicObjectType.Sprite, num);
				return;
			}
			AdvGraphicLayer advGraphicLayer = engine.GraphicManager.FindLayerByObjectName(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOut(name, num);
				return;
			}
			advGraphicLayer = engine.GraphicManager.FindLayer(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOutAllObjects(AdvGraphicObjectType.Sprite, num);
			}
			else
			{
				Debug.LogError("Not found " + name + " Please input sprite name or layer name");
			}
		}
	}
}
