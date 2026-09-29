namespace Utage
{
	internal class AdvCommandBg : AdvCommandBgBase
	{
		public AdvCommandBg(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row, dataManager)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.GraphicManager.IsEventMode = false;
			DoCommandBgSub(engine);
		}
	}
}
