namespace Utage
{
	internal class AdvCommandBgEvent : AdvCommandBgBase
	{
		public AdvCommandBgEvent(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row, dataManager)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.SystemSaveData.GalleryData.AddCgLabel(label);
			engine.GraphicManager.IsEventMode = true;
			AdvGraphicOperationArg advGraphicOperationArg = DoCommandBgSub(engine);
			engine.GraphicManager.CharacterManager.FadeOutAll(advGraphicOperationArg.GetSkippedFadeTime(engine));
		}
	}
}
