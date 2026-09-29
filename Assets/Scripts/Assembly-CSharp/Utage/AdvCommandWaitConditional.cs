using UnityEngine;

namespace Utage
{
	internal class AdvCommandWaitConditional : AdvCommand
	{
		private readonly float time;

		private float waitEndTime;

		private readonly ExpressionParser conditionalExp;

		public AdvCommandWaitConditional(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			string exp = ParseCell<string>(AdvColumnName.Arg1);
			conditionalExp = dataManager.DefaultParam.CreateExpressionBoolean(exp);
			if (conditionalExp.ErrorMsg != null)
			{
				Debug.LogError(ToErrorString(conditionalExp.ErrorMsg));
			}
			time = ParseCellOptional(AdvColumnName.Arg6, -1f);
		}

		public override void DoCommand(AdvEngine engine)
		{
			waitEndTime = engine.Time.Time + (engine.Page.CheckSkip() ? (time / engine.Config.SkipSpped) : time);
		}

		public override bool Wait(AdvEngine engine)
		{
			if (!IsWaitingTime(engine))
			{
				return engine.Param.CalcExpressionBoolean(conditionalExp);
			}
			return true;
		}

		protected virtual bool IsWaitingTime(AdvEngine engine)
		{
			if (time > 0f)
			{
				return engine.Time.Time < waitEndTime;
			}
			return false;
		}
	}
}
