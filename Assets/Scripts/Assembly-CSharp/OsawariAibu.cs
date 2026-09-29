using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariAibu : AbstractOsawari, IAibu
{
	private OsawariPiston _osawariPiston;

	private ParameterValue _aibu;

	private AverageCalculator _average;

	private FloatReactiveProperty _ave;

	public IReadOnlyReactiveProperty<float> GetAverageSpeed()
	{
		return _ave;
	}

	protected override void InitializeParams()
	{
		_osawariPiston = _manager.GetOsawariOf<OsawariPiston>();
		_aibu = new ParameterValue(parameters[ParameterName.Aibu]);
		_ave = new FloatReactiveProperty(0f);
		_average = new AverageCalculator(120);
	}

	protected override void OnLateUpdate()
	{
		float aibu = _aibu.Value * (3f / (parameters[ParameterName.Aibu].MaximumValue - parameters[ParameterName.Aibu].MinimumValue)) - (parameters[ParameterName.Aibu].MinimumValue + 1f);
		_manager.CsManager.OsawariCrossSection?.SyncroAibu(aibu);
		SetLive2D(ParameterName.Aibu, _aibu);
		_ave.Value = _average.AbsAverage;
	}

	public void CancellAnimaion()
	{
		_manager.IsAction = false;
		Cursor.visible = true;
		IsAuto = false;
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (handType == HandType.Left)
		{
			return ParameterNumbers.GetTable()[ParameterName.LeftHandOnAibu];
		}
		return ParameterNumbers.GetTable()[ParameterName.RighthandOnAibu];
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (!IsAuto)
		{
			_aibu += move.y / SensitivityY;
			_manHand.Appear(GetActiveHand());
			_average.Add(move.y / SensitivityY);
			_manager.CsManager.OsawariCrossSection.SetHand(GetActiveHand()[0].HandType);
		}
	}

	protected override bool GetConstraintsCore()
	{
		if (!_osawariPiston.IsInsert() && !_osawariPiston.IsEnter())
		{
			return _manager.IsAbleToInsert();
		}
		return false;
	}

	protected override void AutoAnimation()
	{
		float value = _aibu.Value;
		_aibu = _aibu.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) + 1f);
		_average.Add(_aibu.Value - value);
		foreach (Hand item in GetActiveHand())
		{
			_manHand.SetValue(item.HandType, 1f);
		}
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAuto)
		{
			_manHand.Get(HandType.Right);
			_manHand.Get(HandType.Left);
			_manHand.Disappear();
			_aibu -= 0.1f;
			_average.Add(0f);
		}
	}
}
