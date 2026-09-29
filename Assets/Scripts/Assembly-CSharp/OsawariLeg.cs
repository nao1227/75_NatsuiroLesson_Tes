using System;
using UniRx;
using UnityEngine;

public class OsawariLeg : OsawariDoublehanded
{
	private ParameterValue _leg;

	private OsawariSkirtOpen _skirt;

	protected Subject<Unit> _onLegOpen;

	protected Subject<Unit> _onLegOpening;

	protected Subject<Unit> _onLegClosing;

	protected Subject<Unit> _onLegClosedCompletely;

	private HScene3OsawariHelper _helper;

	private OsawariVibrator _vib;

	public bool IsSkirtOpen => _leg.Value == 1f;

	public bool IsClosed => _leg.Value == 0f;

	public bool IsOpen => _leg.Value == 1f;

	public IObservable<Unit> OnLegOpen => _onLegOpen;

	public IObservable<Unit> OnLegOpening => _onLegOpening;

	public IObservable<Unit> OnLegClosing => _onLegClosing;

	public IObservable<Unit> OnLegClosedCompletely => _onLegClosedCompletely;

	public bool IsMosaicNeeded()
	{
		if (_leg == null)
		{
			return false;
		}
		return _leg.Value >= 0.14f;
	}

	protected override void SetTouchableMeshs()
	{
	}

	protected override void InitializeParams()
	{
		_leg = new ParameterValue(parameters[ParameterName.LegOpen]);
		_skirt = _manager.GetOsawariOf<OsawariSkirtOpen>();
		_helper = _manager.GetOsawariOf<HScene3OsawariHelper>();
		_vib = _manager.GetOsawariOf<OsawariVibrator>();
		_onLegOpen = new Subject<Unit>();
		_onLegOpening = new Subject<Unit>();
		_onLegClosing = new Subject<Unit>();
		_onLegClosedCompletely = new Subject<Unit>();
	}

	protected override void OnLateUpdate()
	{
		float lastValue = _manager.Preserver.GetLastValue(GetParameterNumber(ParameterName.LegOpen));
		if (lastValue < 1f && _leg.Value == 1f)
		{
			_onLegOpen.OnNext(Unit.Default);
		}
		else if (lastValue == 1f && _leg.Value < 1f)
		{
			_onLegClosing.OnNext(Unit.Default);
		}
		else if (lastValue == 0f && _leg.Value > 0f)
		{
			_onLegOpening.OnNext(Unit.Default);
		}
		else if (lastValue > 0f && _leg.Value == 0f)
		{
			_onLegClosedCompletely.OnNext(Unit.Default);
		}
		SetLive2D(ParameterName.LegOpen, _leg);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_leg += move.x / SensitivityX;
		_manHand.Appear();
	}

	protected override void UpdateWhileNotClicked()
	{
		_manHand.Disappear();
		if (1f - _leg.Value < 0.01f)
		{
			_leg = _leg.Update(1f);
		}
		if (_leg.Value < 0.01f)
		{
			_leg = _leg.Update(0f);
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return ParameterNumbers.GetTable()[ParameterName.HandOnLeg];
	}

	protected override bool GetConstraintsCore()
	{
		if (_skirt.IsSkirtOpen || _helper.ClothStatus == ClothStatus.Naked || _helper.ClothStatus == ClothStatus.SwimSuit)
		{
			return !_vib.IsVibAppeared;
		}
		return false;
	}

	protected override void AutoAnimation()
	{
		throw new NotImplementedException();
	}
}
