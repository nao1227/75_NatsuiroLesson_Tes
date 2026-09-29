using System;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariVibrator : AbstractOsawari
{
	public OsawariGoods OsawariGoods;

	private SingleThresholdParameterValue _vibrator;

	private bool _vibratorAppear;

	private Live2DAnimator _animator;

	private Subject<bool> _onMove;

	public bool IsInInsertAnimation;

	private bool _isAutoReturning;

	public bool IsVibAppeared
	{
		get
		{
			if (!_vibratorAppear)
			{
				return IsInInsertAnimation;
			}
			return true;
		}
	}

	public IObservable<bool> OnMove => _onMove;

	protected override void AutoAnimation()
	{
		if (IsAuto)
		{
			float easedValue = GetEasedValue(_lastTimeAuto);
			UpdateAutoTimeCount();
			float y = (GetEasedValue(_autoTime) - easedValue) * SensitivityY;
			Vector3 move = new Vector3(0f, y, 0f) * ((!_isAutoReturning) ? 1 : (-1));
			UpdateParamsCore(move);
			if (_vibrator.IsOnThresholdMax() || _vibrator.IsOnThresholdMin() || _autoTime == AutoPeriod)
			{
				ResetAutoTimeCount();
				_isAutoReturning = !_isAutoReturning;
			}
		}
	}

	protected override void InitializeParams()
	{
		_vibrator = new SingleThresholdParameterValue(parameters[ParameterName.Vibrator], 0.8f);
		_vibrator = _vibrator.MoveToLowerSection();
		_vibratorAppear = false;
		_animator = GetComponent<Live2DAnimator>();
		_onMove = new Subject<bool>();
		StateMachineObservables[] rx = _animator.GetRx();
		for (int i = 0; i < rx.Length; i++)
		{
			rx[i].OnStateExitObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Vibrator).Subscribe(delegate
			{
				_vibrator = _vibrator.MoveToUpperSection();
				_vibrator = _vibrator.Update(0.8f);
				_vibratorAppear = true;
				IsInInsertAnimation = false;
			}).AddTo(this);
		}
		OsawariGoods.OnVibratorAppear.Subscribe(delegate(bool x)
		{
			if (x)
			{
				_vibratorAppear = false;
				_animator.SetInteger("Randomizer", 1);
				_animator.SetTrigger("VibAppear");
				IsInInsertAnimation = true;
			}
			else
			{
				if (_vibratorAppear)
				{
					_animator.SetTrigger("TriggerVibEject");
				}
				_vibrator = _vibrator.MoveToLowerSection(useMinAsDefault: true);
			}
		}).AddTo(this);
	}

	protected override void OnLateUpdate()
	{
		if (_vibratorAppear)
		{
			if (_vibrator.Value <= 0.8f)
			{
				_manager.Preserver.InactivateValue(GetParameterNumber(ParameterName.Vibrator));
			}
			else
			{
				SetLive2D(ParameterName.Vibrator, _vibrator.Value);
			}
			if (_manager.Model.Parameters[GetParameterNumber(ParameterName.Vibrator)].Value <= 0.01f)
			{
				_vibratorAppear = false;
			}
		}
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_vibrator += move.y / SensitivityY;
		_onMove.OnNext(!IsInInsertAnimation);
		_manHand.Appear();
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return ParameterNumbers.GetTable()[(handType == HandType.Left) ? ParameterName.HandOnVibratorLeft : ParameterName.HandOnVibratorRight];
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAuto)
		{
			_onMove.OnNext(value: false);
			_manHand.Disappear();
		}
	}

	protected override bool GetConstraintsCore()
	{
		return _vibratorAppear;
	}
}
