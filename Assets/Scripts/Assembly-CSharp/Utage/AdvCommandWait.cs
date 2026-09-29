namespace Utage
{
	internal class AdvCommandWait : AdvCommand
	{
		private float time;

		private float waitEndTime;

		public AdvCommandWait(StringGridRow row)
			: base(row)
		{
			time = ParseCell<float>(AdvColumnName.Arg6);
		}

		public override void DoCommand(AdvEngine engine)
		{
			waitEndTime = engine.Time.Time + (engine.Page.CheckSkip() ? (time / engine.Config.SkipSpped) : time);
		}

		public override bool Wait(AdvEngine engine)
		{
			return engine.Time.Time < waitEndTime;
		}
	}
}
