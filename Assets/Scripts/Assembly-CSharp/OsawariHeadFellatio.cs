using System;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariHeadFellatio : AbstractOsawariAnotherModel
{
	private ParameterValue _headX;

	private ParameterValue _headY;

	private Subject<Unit> _onStroke = new Subject<Unit>();

	public IObservable<Unit> OnStroke => _onStroke;

	protected override void InitializeParams()
	{
		_headX = new ParameterValue(parameters[ParameterName.HeadX]);
		_headY = new ParameterValue(parameters[ParameterName.HeadY]);
	}

	protected override void OnLateUpdate()
	{
		_headX = _headX.Update(Mathf.Clamp(_headX.Value, -15f, 15f));
		_headY = _headY.Update(Mathf.Clamp(_headY.Value, -10f, 0f));
		if (IsReturning())
		{
			SetLive2D(ParameterName.HeadX, _headX);
			SetLive2D(ParameterName.HeadY, _headY);
		}
		else
		{
			InactivateLive2D(ParameterName.HeadX);
			InactivateLive2D(ParameterName.HeadY);
		}
	}

	public bool IsReturning()
	{
		if (!(_headX.AbsoluteValue > ParameterValue.AlmostZero))
		{
			return _headY.AbsoluteValue > ParameterValue.AlmostZero;
		}
		return true;
	}

	protected override void AutoAnimation()
	{
		_headX = _headX.Update(15f * Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
		_headY = _headY.Update(-5f * Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) - 5f);
		foreach (Hand item in GetActiveHand())
		{
			_manHand.SetValue(item.HandType, 1f);
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return handType switch
		{
			HandType.Right => ParameterNumbers.GetTable()[ParameterName.RightHandOnHead], 
			HandType.Left => ParameterNumbers.GetTable()[ParameterName.LeftHandOnHead], 
			_ => throw new Exception(), 
		};
	}

	public override void Cancel()
	{
		base.Cancel();
		if (_headX != null)
		{
			_headX = _headX.Update(0f);
			_headY = _headY.Update(0f);
		}
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (move.magnitude > 0f)
		{
			_onStroke.OnNext(Unit.Default);
		}
		_headX += move.x / SensitivityX;
		_headY += move.y / SensitivityY;
		_manHand.Appear(GetActiveHand());
	}

	protected override void UpdateWhileNotClicked()
	{
		if (IsReturning())
		{
			_headX *= 0.9f;
			_headY *= 0.9f;
		}
		_manHand.Disappear();
	}

	protected override bool GetConstraintsCore()
	{
		OsawariFellatio osawariFellatio = _manager.OsawariFellatio;
		if ((object)osawariFellatio == null || !osawariFellatio.IsInFellatio)
		{
			OsawariFellatio osawariFellatio2 = _manager.OsawariFellatio;
			if ((object)osawariFellatio2 == null)
			{
				return true;
			}
			return !osawariFellatio2.IsSucking;
		}
		return false;
	}
}
