using Paidia.satsuki1;
using UnityEngine;

public class OsawariIrrumatio : AbstractOsawariAnotherModel
{
	private SingleThresholdParameterValue _fellatio;

	public OsawariFellatio _osawariFellatio;

	private bool _isIrrumaMoving;

	private bool _isReturning;

	private InsertController _insertController;

	protected override void AutoAnimation()
	{
		_isIrrumaMoving = true;
		float num = 0.1f;
		if (_isReturning)
		{
			num *= -1f;
		}
		_fellatio -= num;
		if (_fellatio.IsOnThresholdMax() || _fellatio.IsOnThresholdMin())
		{
			_isReturning = !_isReturning;
		}
		_osawariFellatio.SetFellatioValue(_fellatio.Value);
		_osawariFellatio.Move(move: true);
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return 0;
	}

	protected override void InitializeParams()
	{
		_fellatio = new SingleThresholdParameterValue(parameters[ParameterName.Fellatio], 0f);
		_fellatio = _fellatio.MoveToUpperSection();
		_insertController = Object.FindObjectOfType<InsertController>();
	}

	protected override void OnLateUpdate()
	{
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_isIrrumaMoving = true;
		_fellatio -= move.y / SensitivityY;
		_osawariFellatio.SetFellatioValue(_fellatio.Value);
		_osawariFellatio.Move(move: true);
	}

	protected override void UpdateWhileNotClicked()
	{
		if (_isIrrumaMoving)
		{
			_osawariFellatio.Move(move: false);
			_isIrrumaMoving = false;
		}
	}

	protected override bool GetConstraintsCore()
	{
		return _osawariFellatio.Mode == FellatioMode.PlayerControl;
	}

	public override void OnSpecial()
	{
		_insertController.AddManExtacy(99999f);
	}
}
