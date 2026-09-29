namespace Utage
{
	internal class AdvCommandWaitInput : AdvCommand
	{
		protected float time;

		protected float waitEndTime;

		public AdvCommandWaitInput(StringGridRow row)
			: base(row)
		{
			time = ParseCellOptional(AdvColumnName.Arg6, -1f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (base.CurrentTread.IsMainThread)
			{
				engine.Page.IsWaitingInputCommand = true;
			}
			waitEndTime = engine.Time.Time + (engine.Page.CheckSkip() ? (time / engine.Config.SkipSpped) : time);
		}

		public override bool Wait(AdvEngine engine)
		{
			if (IsWaitng(engine))
			{
				return true;
			}
			if (engine.Config.VoiceStopType == VoiceStopType.OnClick)
			{
				engine.SoundManager.StopVoiceIgnoreLoop();
			}
			engine.UiManager.ClearPointerDown();
			if (base.CurrentTread.IsMainThread)
			{
				engine.Page.IsWaitingInputCommand = false;
			}
			return false;
		}

		protected virtual bool IsWaitng(AdvEngine engine)
		{
			if (engine.Page.CheckSkip())
			{
				return false;
			}
			if (engine.UiManager.IsInputTrig)
			{
				return false;
			}
			if (time > 0f)
			{
				return engine.Time.Time < waitEndTime;
			}
			return true;
		}
	}
}
