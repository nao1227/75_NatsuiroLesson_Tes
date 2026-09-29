using UtageExtensions;

namespace Utage
{
	internal class AdvCommandWaitFadeObjects : AdvCommandWaitBase, IAdvCommandEffect, IAdvCommandUpdateWait
	{
		private string[] Targets { get; set; }

		private AdvEngine Engine { get; set; }

		internal AdvCommandWaitFadeObjects(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			Targets = ParseCellOptionalArray(AdvColumnName.Arg1, new string[1] { "All" });
			base.WaitType = ParseCellOptional(AdvColumnName.WaitType, AdvCommandWaitType.Default);
		}

		protected override void OnStart(AdvEngine engine, AdvScenarioThread thread)
		{
			Engine = engine;
		}

		public bool UpdateCheckWait()
		{
			string[] targets = Targets;
			foreach (string targetName in targets)
			{
				if (CheckTargetWait(targetName))
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckTargetWait(string targetName)
		{
			if (targetName.IsNullOrEmpty())
			{
				return false;
			}
			AdvGraphicManager graphicManager = Engine.GraphicManager;
			switch (targetName)
			{
			case "AllBgLayers":
				return graphicManager.BgManager.IsFading;
			case "AllCharacterLayers":
				return graphicManager.CharacterManager.IsFading;
			case "AllSpriteLayers":
				return graphicManager.SpriteManager.IsFading;
			case "AllBgObjects":
				return graphicManager.IsFadingObjects(AdvGraphicObjectType.Bg);
			case "AllCharacterObjects":
				return graphicManager.IsFadingObjects(AdvGraphicObjectType.Character);
			case "AllSpriteObjects":
				return graphicManager.IsFadingObjects(AdvGraphicObjectType.Sprite);
			case "All":
				if (!graphicManager.BgManager.IsFading && !graphicManager.CharacterManager.IsFading)
				{
					return graphicManager.SpriteManager.IsFading;
				}
				return true;
			default:
				return graphicManager.IsFading(targetName);
			}
		}

		public void OnEffectFinalize()
		{
			Engine = null;
		}

		public void OnEffectSkip()
		{
			string[] targets = Targets;
			foreach (string targetName in targets)
			{
				OnEffectSkip(targetName);
			}
		}

		private void OnEffectSkip(string targetName)
		{
			AdvGraphicManager graphicManager = Engine.GraphicManager;
			switch (targetName)
			{
			case "AllBgLayers":
				graphicManager.BgManager.SkipFade();
				break;
			case "AllCharacterLayers":
				graphicManager.CharacterManager.SkipFade();
				break;
			case "AllSpriteLayers":
				graphicManager.SpriteManager.SkipFade();
				break;
			case "AllBgObjects":
				graphicManager.SkipFadeObjects(AdvGraphicObjectType.Bg);
				break;
			case "AllCharacterObjects":
				graphicManager.SkipFadeObjects(AdvGraphicObjectType.Character);
				break;
			case "AllSpriteObjects":
				graphicManager.SkipFadeObjects(AdvGraphicObjectType.Sprite);
				break;
			case "All":
				graphicManager.BgManager.SkipFade();
				graphicManager.CharacterManager.SkipFade();
				graphicManager.SpriteManager.SkipFade();
				break;
			default:
				graphicManager.SkipFade(targetName);
				break;
			}
		}
	}
}
