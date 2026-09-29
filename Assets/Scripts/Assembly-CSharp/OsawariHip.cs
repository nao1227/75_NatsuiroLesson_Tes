using System;
using DG.Tweening;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariHip : AbstractOsawari, IParticularHand
{
	private ParameterValue _hipRubX;

	private ParameterValue _hipRubY;

	private ParameterValue _hipStrokeX;

	private ParameterValue _hipStrokeY;

	private OsawariPiston _piston;

	public HandType HandToUse;

	private bool _showHand;

	private bool _clicked;

	private Tweener _xTweener;

	private Tweener _yTweener;

	private bool _hipMoving;

	private bool _rubMode
	{
		get
		{
			if (null != _piston)
			{
				return _piston.IsEnter();
			}
			return false;
		}
	}

	protected override void AutoAnimation()
	{
		if (_rubMode)
		{
			_hipRubX = _hipRubX.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			if (HandToUse == HandType.Right)
			{
				_hipRubX *= -1f;
			}
			_hipRubY = _hipRubY.Update(Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
		}
		else
		{
			_hipStrokeX = _hipStrokeX.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			if (HandToUse == HandType.Right)
			{
				_hipStrokeX *= -1f;
			}
			_hipStrokeY = _hipStrokeY.Update(Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
		}
		_manHand.SetValue(HandToUse, 1f);
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (_rubMode)
		{
			return ParameterNumbers.GetTable()[ParameterName.HipRubHand];
		}
		return ParameterNumbers.GetTable()[ParameterName.HipStrokeHand];
	}

	protected override void InitializeParams()
	{
		_piston = _manager.GetOsawariOf<OsawariPiston>();
		_hipStrokeX = new ParameterValue(parameters[ParameterName.HipStrokeX]);
		_hipStrokeY = new ParameterValue(parameters[ParameterName.HipStrokeY]);
		_hipRubX = new ParameterValue(parameters[ParameterName.HipRubX]);
		_hipRubY = new ParameterValue(parameters[ParameterName.HipRubY]);
		UtageManager utageManager = UnityEngine.Object.FindObjectOfType<UtageManager>();
		utageManager.OnStartPlaying.Where((ScenarioLabel x) => x == ScenarioLabel.Click_Hip_Day5_2).Subscribe(delegate
		{
			_showHand = true;
		}).AddTo(this);
		utageManager.OnFinishPlaying.Where((ScenarioLabel x) => x == ScenarioLabel.Click_Hip_Day5_2 || x == ScenarioLabel.Click_Hip_Day5_1).Subscribe(delegate
		{
			_clicked = false;
			_showHand = false;
			InactivateLive2D(ParameterName.HipRubHand);
		}).AddTo(this);
	}

	protected override void OnLateUpdate()
	{
		if (_rubMode)
		{
			SetLive2D(ParameterName.HipRubX, _hipRubX);
			SetLive2D(ParameterName.HipRubY, _hipRubY);
			SetLive2D(ParameterName.HipStrokeX, 0f);
			SetLive2D(ParameterName.HipStrokeY, 0f);
		}
		else
		{
			SetLive2D(ParameterName.HipRubX, 0f);
			SetLive2D(ParameterName.HipRubY, 0f);
			SetLive2D(ParameterName.HipStrokeX, _hipStrokeX);
			SetLive2D(ParameterName.HipStrokeY, _hipStrokeY);
		}
		if (_showHand && _clicked)
		{
			SetLive2D(ParameterName.HipRubHand, 1f);
		}
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		float num = move.x / SensitivityX;
		float num2 = move.y / SensitivityY;
		if (_rubMode)
		{
			_hipRubX += num;
			_hipRubY += num2;
		}
		else
		{
			_hipStrokeX += num;
			_hipStrokeY += num2;
		}
		_manHand.Appear();
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAuto)
		{
			_hipRubX *= 0.9f;
			_hipRubY *= 0.9f;
			_hipStrokeX *= 0.9f;
			_hipStrokeY *= 0.9f;
			if (_showHand)
			{
				_manHand.Appear();
			}
			else
			{
				_manHand.Disappear();
			}
		}
	}

	protected override bool GetRestrictedCore()
	{
		if (!SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Hip_Day5_2))
		{
			return SaveLoadManager.UnsavedData.Days < 6;
		}
		return false;
	}

	protected override void OnFirstClickCore()
	{
		_clicked = true;
		_xTweener?.Kill();
		_yTweener?.Kill();
	}

	protected override bool GetConstraintsCore()
	{
		bool flag = base.GetConstraintsCore();
		if (flag)
		{
			bool flag2 = SaveLoadManager.UnsavedData.Days != 4 || !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Hip_Day4);
			flag = flag2;
		}
		return flag;
	}

	public override void OnMouseUp(bool fromCancel = false)
	{
		if (IsAuto)
		{
			return;
		}
		if (_rubMode)
		{
			_xTweener = DOVirtual.Float(_hipRubX.Value, 0f, 1f, delegate(float x)
			{
				_hipRubX = _hipRubX.Update(x);
			}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).SetDelay(0.05f);
			_yTweener = DOVirtual.Float(_hipRubY.Value, 0f, 1f, delegate(float x)
			{
				_hipRubY = _hipRubY.Update(x);
			}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).OnComplete(delegate
			{
				_hipMoving = false;
			});
		}
		else
		{
			_xTweener = DOVirtual.Float(_hipStrokeX.Value, 0f, 1f, delegate(float x)
			{
				_hipStrokeX = _hipStrokeX.Update(x);
			}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).SetDelay(0.05f);
			_yTweener = DOVirtual.Float(_hipStrokeY.Value, 0f, 1f, delegate(float x)
			{
				_hipStrokeY = _hipStrokeY.Update(x);
			}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).OnComplete(delegate
			{
				_hipMoving = false;
			});
		}
		_hipMoving = true;
		_xTweener.Play();
		_yTweener.Play();
		base.OnMouseUp(fromCancel);
	}

	public Hand GetParticularHand()
	{
		return HandToUse switch
		{
			HandType.Right => _handManager.RightHand, 
			HandType.Left => _handManager.LeftHand, 
			_ => throw new Exception(), 
		};
	}
}
