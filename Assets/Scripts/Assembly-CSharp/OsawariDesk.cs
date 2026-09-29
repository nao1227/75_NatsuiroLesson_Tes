using UnityEngine;

public class OsawariDesk : OsawariWithoutHand
{
	private ParameterValue _desk;

	protected override void AutoAnimation()
	{
	}

	protected override void InitializeParams()
	{
		_desk = new ParameterValue(parameters[ParameterName.Desk]);
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.Desk, _desk);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_desk += move.x / SensitivityX;
	}

	protected override void UpdateWhileNotClicked()
	{
	}
}
