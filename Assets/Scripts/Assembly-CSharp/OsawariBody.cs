using Paidia.satsuki1;
using UnityEngine;

public class OsawariBody : OsawariDoublehanded
{
	private ParameterValue _body;

	private OsawariMizugiHimo _mizugi;

	private bool _isAutoReturning;

	protected override void AutoAnimation()
	{
		if (IsAuto)
		{
			float easedValue = GetEasedValue(_lastTimeAuto);
			UpdateAutoTimeCount();
			float y = (GetEasedValue(_autoTime) - easedValue) * SensitivityY;
			Vector3 move = new Vector3(0f, y, 0f) * ((!_isAutoReturning) ? 1 : (-1));
			UpdateParamsCore(move);
			if (_body.IsMax() || _body.IsMin() || _autoTime == AutoPeriod)
			{
				ResetAutoTimeCount();
				_isAutoReturning = !_isAutoReturning;
			}
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (handType == HandType.Left)
		{
			return ParameterNumbers.GetTable()[ParameterName.LeftHandOnBody];
		}
		return ParameterNumbers.GetTable()[ParameterName.RightHandOnBody];
	}

	protected override void InitializeParams()
	{
		_body = new ParameterValue(parameters[ParameterName.WomanBodyY]);
		_mizugi = _manager.GetOsawariOf<OsawariMizugiHimo>();
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.WomanBodyY, _body);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_body += move.y / SensitivityY;
		_manHand.Appear();
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAuto)
		{
			_manHand.Disappear();
		}
	}

	protected override bool GetConstraintsCore()
	{
		return _mizugi.Phase == MizugiHodokiPhase.TakeOff;
	}

	protected override bool GetRestrictedCore()
	{
		return !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Body_Day4);
	}
}
