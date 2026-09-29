using Paidia.satsuki1;
using UnityEngine;

public class OsawariThigh : OsawariDoublehanded
{
	private ParameterValue _thigh;

	private OsawariPiston _piston;

	protected override void AutoAnimation()
	{
		Vector3 move = new Vector3(0f, Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) - _thigh.Value);
		UpdateParamsCore(move);
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (handType == HandType.Left)
		{
			return ParameterNumbers.GetTable()[ParameterName.LeftHandOnThigh];
		}
		return ParameterNumbers.GetTable()[ParameterName.RightHandOnThigh];
	}

	protected override void InitializeParams()
	{
		_thigh = new ParameterValue(parameters[ParameterName.Thigh]);
		_piston = _manager.GetOsawariOf<OsawariPiston>();
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.Thigh, _thigh);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_thigh += move.y / SensitivityY;
		_manHand.Appear();
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAuto)
		{
			_manHand.Disappear();
			_thigh *= 0.9f;
		}
	}

	protected override bool GetConstraintsCore()
	{
		if (base.GetConstraintsCore())
		{
			return !_piston.IsEnter();
		}
		return false;
	}
}
