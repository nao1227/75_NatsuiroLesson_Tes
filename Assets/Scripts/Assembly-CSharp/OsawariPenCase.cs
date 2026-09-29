using System;
using Paidia.satsuki1;
using UnityEngine;

public class OsawariPenCase : OsawariWithoutHand
{
	private ParameterValue _keyHoler;

	private BoolParameterValue _penCaseFlag;

	protected override void AutoAnimation()
	{
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		throw new NotImplementedException();
	}

	protected override void InitializeParams()
	{
		_penCaseFlag = new BoolParameterValue(parameters[ParameterName.PenCaseFlag]);
	}

	protected override void OnLateUpdate()
	{
		_penCaseFlag = _penCaseFlag.Update(parameters[ParameterName.PenCaseFlag].Value == 0f);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
	}

	protected override void UpdateWhileNotClicked()
	{
	}

	protected override bool GetConstraintsCore()
	{
		bool flag = _penCaseFlag.AsBool();
		if (flag)
		{
			flag = SaveLoadManager.UnsavedData.Days switch
			{
				1 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_PenCase_Day1), 
				2 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_PenCase_Day2), 
				3 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_PenCase_Day3), 
				_ => true, 
			};
		}
		return flag;
	}
}
