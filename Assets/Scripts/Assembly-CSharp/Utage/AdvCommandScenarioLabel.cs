namespace Utage
{
	public class AdvCommandScenarioLabel : AdvCommand
	{
		public enum ScenarioLabelType
		{
			None = 0,
			SavePoint = 1
		}

		public string ScenarioLabel { get; protected set; }

		public ScenarioLabelType Type { get; protected set; }

		public string Title
		{
			get
			{
				if (string.IsNullOrEmpty(ParseCellOptional(AdvColumnName.Arg2, "")))
				{
					return "";
				}
				return ParseCellLocalized(AdvColumnName.Arg2.QuickToString());
			}
		}

		public AdvCommandScenarioLabel(StringGridRow row)
			: base(row)
		{
			ScenarioLabel = ParseScenarioLabel(AdvColumnName.Command);
			Type = ParseCellOptional(AdvColumnName.Arg1, ScenarioLabelType.None);
		}

		public override void DoCommand(AdvEngine engine)
		{
		}
	}
}
