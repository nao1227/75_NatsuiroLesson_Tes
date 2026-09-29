namespace Utage
{
	internal class AdvCommandWaitEffectTime : AdvCommandWaitBase, IAdvCommandEffect, IAdvCommandUpdateWait
	{
		private float time;

		private float waitEndTime;

		private AdvEngine Engine { get; set; }

		private AdvScenarioThread Thread { get; set; }

		internal AdvCommandWaitEffectTime(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			time = ParseCell<float>(AdvColumnName.Arg6);
			base.WaitType = ParseCellOptional(AdvColumnName.WaitType, AdvCommandWaitType.Default);
		}

		protected override void OnStart(AdvEngine engine, AdvScenarioThread thread)
		{
			waitEndTime = engine.Time.Time + (engine.Page.CheckSkip() ? (time / engine.Config.SkipSpped) : time);
			Engine = engine;
			Thread = thread;
		}

		public bool UpdateCheckWait()
		{
			return Engine.Time.Time < waitEndTime;
		}

		public void OnEffectFinalize()
		{
			Engine = null;
			Thread = null;
		}

		public void OnEffectSkip()
		{
			waitEndTime = 0f;
		}
	}
}
