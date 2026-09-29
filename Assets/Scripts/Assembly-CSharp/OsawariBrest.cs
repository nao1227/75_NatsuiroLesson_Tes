using System;
using System.Collections.Generic;
using DG.Tweening;
using Live2D.Cubism.Core;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariBrest : AbstractOsawari, IParticularHand, IBreast
{
	[SerializeField]
	private HandType HandToUse;

	private OsawariClothes _osawariClothes;

	private OsawariPiston _osawariPiston;

	private OsawariArms _osawariArms;

	private OsawariCosplay _cosplay;

	private bool _breastMoving;

	private bool _synchro;

	private OsawariBrest _another;

	protected BoolReactiveProperty _grab;

	public bool IsMulti;

	protected ParameterValue breastParamX;

	protected ParameterValue breastParamY;

	private bool isInAction;

	private Tweener _xTweener;

	private Tweener _yTweener;

	protected virtual bool _isBraOn => _osawariClothes?.GetBraFlag() ?? false;

	private bool _noPiston
	{
		get
		{
			if (null == _osawariPiston)
			{
				return true;
			}
			if (!_osawariPiston.IsInsert())
			{
				return !_osawariPiston.IsEnter();
			}
			return false;
		}
	}

	private bool _isPistonMoving
	{
		get
		{
			if (null == _osawariPiston)
			{
				return false;
			}
			return _osawariPiston.IsMoving;
		}
	}

	public IReadOnlyReactiveProperty<bool> GetIsGrabbing()
	{
		return _grab;
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

	protected override void InitializeParams()
	{
		_osawariClothes = _manager.GetOsawariOf<OsawariClothes>();
		_osawariPiston = _manager.GetOsawariOf<OsawariPiston>();
		_osawariArms = _manager.GetOsawariOf<OsawariArms>();
		_cosplay = _manager.GetOsawariOf<OsawariCosplay>();
		breastParamX = new ParameterValue(parameters[ParameterName.BreastX]);
		breastParamY = new ParameterValue(parameters[ParameterName.BreastY]);
		_grab = new BoolReactiveProperty(initialValue: false);
		List<OsawariBrest> everyOsawariOf = _manager.GetEveryOsawariOf<OsawariBrest>();
		if (IsMulti)
		{
			if (everyOsawariOf[0] == this)
			{
				_another = everyOsawariOf[1];
			}
			else
			{
				_another = everyOsawariOf[0];
			}
		}
	}

	protected override void AutoAnimation()
	{
		ActivatePhysicsCalculator();
		breastParamX = breastParamX.Update(0f - Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
		if (HandToUse == HandType.Right)
		{
			breastParamX *= -1f;
		}
		breastParamY = breastParamY.Update(Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
		_manHand.SetValue(HandToUse, 1f);
	}

	protected override bool GetConstraintsCore()
	{
		bool flag = HandToUse switch
		{
			HandType.Right => _handManager.GrabRight, 
			HandType.Left => _handManager.GrabLeft, 
			_ => throw new Exception(), 
		};
		if (!_manager.IsAction && (!flag || IsAuto))
		{
			OsawariClothes osawariClothes = _osawariClothes;
			if (((object)osawariClothes == null || !osawariClothes.IsBraTakingOff) && (null == _osawariClothes || !_osawariClothes.IsAnimePlaying()))
			{
				if (!(null == _osawariArms))
				{
					return !_osawariArms.IsArmClosed;
				}
				return true;
			}
		}
		return false;
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (_isBraOn)
		{
			return ParameterNumbers.GetTable()[ParameterName.HandOnBreastWithBra];
		}
		if (_manager.TemporaryStatus.Cloth == ClothName.Bunny)
		{
			OsawariCosplay cosplay = _cosplay;
			if ((object)cosplay != null && cosplay.IsWearingBunnyBra)
			{
				return ParameterNumbers.GetTable()[ParameterName.BunnyHand];
			}
		}
		return ParameterNumbers.GetTable()[ParameterName.HandOnBreast];
	}

	protected override void OnLateUpdate()
	{
		if (isInAction)
		{
			_ = 1;
		}
		else
			_ = _noPiston;
		if (_breastMoving || _synchro)
		{
			if (_isBraOn)
			{
				SetLive2D(ParameterName.BraBreastX, breastParamX);
				SetLive2D(ParameterName.BraBreastY, breastParamY);
			}
			else if (null != _cosplay && _cosplay.IsWearingBunnyBra)
			{
				SetLive2D(ParameterName.BunnyBreastX, breastParamX);
				SetLive2D(ParameterName.BunnyBreastY, breastParamY);
			}
			else if (!_isPistonMoving || IsAuto)
			{
				SetLive2D(ParameterName.BreastX, breastParamX);
				SetLive2D(ParameterName.BreastY, breastParamY);
			}
		}
		else
		{
			InactivateLive2D(ParameterName.BreastX);
			InactivateLive2D(ParameterName.BreastY);
			InactivateLive2D(ParameterName.BraBreastX);
			InactivateLive2D(ParameterName.BraBreastY);
		}
		foreach (Hand item in GetActiveHand())
		{
			GetHandParameter(item.HandType).Value = _manHand.Get(item.HandType).Value;
		}
		if (null != _osawariArms && _osawariArms.IsArmClosed)
		{
			IsAuto = false;
		}
		isInAction = false;
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (!IsAuto)
		{
			ActivatePhysicsCalculator();
			breastParamX += move.x / SensitivityX;
			breastParamY += move.y / SensitivityY;
			_manHand.Appear(GetActiveHand());
			if (_synchro)
			{
				_another.UpdateSingleParam(move);
			}
		}
		_grab.Value = true;
	}

	private void UpdateSingleParam(Vector3 move)
	{
		breastParamX -= move.x / SensitivityX;
		breastParamY += move.y / SensitivityY;
		_manHand.SetValue(HandToUse, _another._manHand.GetValue(_another.HandToUse));
	}

	protected override void OnWhileNotClicked()
	{
		if (!_synchro)
		{
			base.OnWhileNotClicked();
		}
	}

	protected override void UpdateWhileNotClicked()
	{
		ActivatePhysicsCalculator(IsAuto);
		_manHand.Disappear();
		_grab.Value = false;
	}

	private void ActivatePhysicsCalculator(bool active = true)
	{
		foreach (KeyValuePair<ParameterName, int> item in ParameterNumbers.GetTable())
		{
			_ = item;
		}
	}

	public override void OnClick(CubismDrawable targetMesh, bool isFirst)
	{
		if (isFirst)
		{
			if (IsAuto)
			{
				IsAuto = false;
				if (_synchro)
				{
					_another.IsAuto = false;
				}
			}
			if (GetConstraints() && !GetParticularHand().IsGrabbing)
			{
				OnFirstClick();
				if (!GetRestricted())
				{
					SaveLoadManager.UnsavedData.BreastCount++;
				}
				_handManager.Grab(GetParticularHand().HandType, this);
			}
		}
		if (_handManager.IsGrabbing(this))
		{
			UpdateParams(ConvertMovementVec3ForParams());
			isInAction = true;
		}
	}

	public override void SetAuto()
	{
		if (_synchro)
		{
			_another.IsAuto = true;
		}
		base.SetAuto();
	}

	public override void OnSpecial()
	{
		if (!_synchro && _manager.CanGrab(_another))
		{
			_handManager.Grab(_another.HandToUse, _another);
			_synchro = true;
			_another._synchro = true;
			_another.OnFirstClick();
			_another.UpdateSingleParam(ConvertMovementVec3ForParams());
		}
	}

	public override void OnMouseUp(bool fromCancel = false)
	{
		if (!IsAuto)
		{
			if (_synchro)
			{
				_another._synchro = false;
				_another.OnMouseUp();
			}
			_synchro = false;
			_xTweener = DOVirtual.Float(breastParamX.Value, 0f, 1f, delegate(float x)
			{
				breastParamX = breastParamX.Update(x);
			}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).SetDelay(0.05f);
			_yTweener = DOVirtual.Float(breastParamY.Value, 0f, 1f, delegate(float x)
			{
				breastParamY = breastParamY.Update(x);
			}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).OnComplete(delegate
			{
				_breastMoving = false;
			});
			_breastMoving = true;
			_xTweener.Play();
			_yTweener.Play();
			base.OnMouseUp(fromCancel);
		}
	}

	protected override void OnFirstClickCore()
	{
		base.OnFirstClickCore();
		_xTweener?.Kill();
		_yTweener?.Kill();
		_breastMoving = true;
	}
}
