namespace Utage
{
	internal class AdvCommandThread : AdvCommand
	{
		private string label;

		public AdvCommandThread(StringGridRow row)
			: base(row)
		{
			label = ParseScenarioLabel(AdvColumnName.Arg1);
		}

		public override void DoCommand(AdvEngine engine)
		{
			base.CurrentTread.StartSubThread(label);
		}
	}
}
