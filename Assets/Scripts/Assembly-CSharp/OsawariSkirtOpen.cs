using UnityEngine;

public class OsawariSkirtOpen : OsawariWithoutAuto, IDoubleHanded
{
	private ParameterValue _skirt;

	private HScene3OsawariHelper _helper;

	private OsawariLeg _leg;

	public bool IsSkirtOpen => _skirt.Value == 1f;

	protected override void InitializeParams()
	{
		_skirt = new ParameterValue(parameters[ParameterName.Skirt]);
		_helper = _manager.GetOsawariOf<HScene3OsawariHelper>();
		_leg = _manager.GetOsawariOf<OsawariLeg>();
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.Skirt, _skirt);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_skirt += move.y / SensitivityY;
		_manHand.Appear();
	}

	protected override void UpdateWhileNotClicked()
	{
		_manHand.Disappear();
		if (1f - _skirt.Value < 0.01f)
		{
			_skirt = _skirt.Update(1f);
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return ParameterNumbers.GetTable()[ParameterName.HandOnSkirt];
	}

	protected override bool GetConstraintsCore()
	{
		if (_helper.ClothStatus == ClothStatus.WearAll || _helper.ClothStatus == ClothStatus.WearHalf)
		{
			return _leg.IsClosed;
		}
		return false;
	}
}
