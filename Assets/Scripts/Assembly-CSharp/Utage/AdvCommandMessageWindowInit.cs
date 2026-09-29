using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	internal class AdvCommandMessageWindowInit : AdvCommand
	{
		private List<string> names = new List<string>();

		public AdvCommandMessageWindowInit(StringGridRow row)
			: base(row)
		{
			if (!IsEmptyCell(AdvColumnName.Arg1))
			{
				AddName(ParseCell<string>(AdvColumnName.Arg1));
			}
			if (!IsEmptyCell(AdvColumnName.Arg2))
			{
				AddName(ParseCell<string>(AdvColumnName.Arg2));
			}
			if (!IsEmptyCell(AdvColumnName.Arg3))
			{
				AddName(ParseCell<string>(AdvColumnName.Arg3));
			}
			if (!IsEmptyCell(AdvColumnName.Arg4))
			{
				AddName(ParseCell<string>(AdvColumnName.Arg4));
			}
			if (!IsEmptyCell(AdvColumnName.Arg5))
			{
				AddName(ParseCell<string>(AdvColumnName.Arg5));
			}
			if (!IsEmptyCell(AdvColumnName.Arg6))
			{
				AddName(ParseCell<string>(AdvColumnName.Arg6));
			}
			if (names.Count <= 0)
			{
				Debug.LogError(ToErrorString("Not set data in this command"));
			}
		}

		private void AddName(string name)
		{
			if (names.Contains(name))
			{
				Debug.LogError(ToErrorString(name + " is duplicated. You cannot use the same message windows name more than once."));
			}
			else
			{
				names.Add(name);
			}
		}

		public override void InitFromPageData(AdvScenarioPageData pageData)
		{
			if (names.Count > 0)
			{
				pageData.InitMessageWindowName(this, names[0]);
			}
		}

		public override void DoCommand(AdvEngine engine)
		{
			engine.MessageWindowManager.ChangeActiveWindows(names);
			engine.MessageWindowManager.ChangeCurrentWindow(engine.Page.CurrentData.MessageWindowName);
		}
	}
}
