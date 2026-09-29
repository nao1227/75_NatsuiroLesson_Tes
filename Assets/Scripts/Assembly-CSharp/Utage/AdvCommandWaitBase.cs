namespace Utage
{
	public abstract class AdvCommandWaitBase : AdvCommand
	{
		public AdvCommandWaitType WaitType { get; protected set; }

		protected AdvCommandWaitBase(StringGridRow row)
			: base(row)
		{
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.WaitManager.StartCommand(this);
			OnStart(engine, base.CurrentTread);
		}

		public override bool Wait(AdvEngine engine)
		{
			switch (WaitType)
			{
			case AdvCommandWaitType.Default:
				return base.CurrentTread.WaitManager.IsWaitingDefault;
			case AdvCommandWaitType.Skippable:
				if (!base.CurrentTread.WaitManager.IsWaitingDefault)
				{
					return false;
				}
				if (engine.Page.CheckSkip() || engine.UiManager.IsInputTrig)
				{
					base.CurrentTread.WaitManager.SkipEffectCommand();
				}
				return true;
			case AdvCommandWaitType.SkippableOnWaitThread:
				if (!base.CurrentTread.WaitManager.IsWaitingOnThread)
				{
					return false;
				}
				if (base.CurrentTread.ParenetThread.IsWaitingSubTread(base.CurrentTread.ThreadName) && (engine.Page.CheckSkip() || engine.UiManager.IsInputTrig))
				{
					base.CurrentTread.WaitManager.SkipEffectCommandOnWaitThread();
				}
				return true;
			default:
				return false;
			}
		}

		protected abstract void OnStart(AdvEngine engine, AdvScenarioThread thread);

		internal virtual void OnComplete(AdvScenarioThread thread)
		{
			thread.WaitManager.CompleteCommand(this);
		}
	}
}
