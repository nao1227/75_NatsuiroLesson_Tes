using System;
using System.Collections.Generic;
using DG.Tweening;
using Live2D.Cubism.Core;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariBreastPaizuri : AbstractOsawariAnotherModel, IParticularHand, IBreast
	{
		public CubismDrawable MBMesh;

		private OsawariBreastPaizuri _another;

		private ParameterValue _breastX;

		private ParameterValue _breastY;

		public HandType HandToUse;

		private BoolReactiveProperty _grab = new BoolReactiveProperty(initialValue: false);

		private bool _breastMoving;

		private Tween _xTweener;

		private Tween _yTweener;

		private bool _synchro;

		public bool isInAction;

		private OsawariPaizuri _paizuri;

		private OsawariMizugiUpperPaizuri _mizugi;

		private bool _canceled;

		public IReadOnlyReactiveProperty<bool> GetIsGrabbing()
		{
			return _grab;
		}

		protected override void SetTouchableMeshs()
		{
			TouchableMeshs = new CubismDrawable[2] { Mesh, MBMesh };
		}

		protected override void AutoAnimation()
		{
			_breastX = _breastX.Update(0f - Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			if (HandToUse == HandType.Right)
			{
				_breastX *= -1f;
			}
			_breastY = _breastY.Update(Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			_manHand.SetValue(HandToUse, 1f);
		}

		private bool IsMB()
		{
			return _manager.TemporaryStatus.Cloth == ClothName.MicroBikini;
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return GetParameterNumber((!_mizugi.IsWearing()) ? ParameterName.HandOnBreast : (IsMB() ? ParameterName.HandOnMBBra : ParameterName.HandOnBreastWithBra));
		}

		protected override void InitializeParams()
		{
			_breastX = new ParameterValue(parameters[ParameterName.BreastX]);
			_breastY = new ParameterValue(parameters[ParameterName.BreastY]);
			List<OsawariBreastPaizuri> everyOsawariOf = _manager.GetEveryOsawariOf<OsawariBreastPaizuri>();
			if (everyOsawariOf[0] == this)
			{
				_another = everyOsawariOf[1];
			}
			else
			{
				_another = everyOsawariOf[0];
			}
			_mizugi = _manager.GetOsawariOf<OsawariMizugiUpperPaizuri>();
			_paizuri = _manager.GetOsawariOf<OsawariPaizuri>();
		}

		protected override void OnLateUpdate()
		{
			if (_breastMoving || _synchro)
			{
				SetLive2D(ParameterName.BreastX, _breastX.Value);
				SetLive2D(ParameterName.BreastY, _breastY.Value);
			}
			else if (_canceled)
			{
				_breastX = _breastX.Update(0f);
				_breastY = _breastY.Update(0f);
				SetLive2D(ParameterName.BreastX, _breastX.Value);
				SetLive2D(ParameterName.BreastY, _breastY.Value);
				_canceled = false;
			}
			else
			{
				InactivateLive2D(ParameterName.BreastX);
				InactivateLive2D(ParameterName.BreastY);
			}
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (!IsAuto)
			{
				_breastX += move.x / SensitivityX;
				_breastY += move.y / SensitivityY;
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
			_breastX -= move.x / SensitivityX;
			_breastY += move.y / SensitivityY;
			_manHand.SetValue(HandToUse, _another._manHand.GetValue(_another.HandToUse));
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!_synchro)
			{
				if (!_breastMoving && _breastX.Value != 0f && _breastY.Value != 0f)
				{
					_canceled = true;
				}
				_manHand.Disappear();
				_grab.Value = false;
			}
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

		protected override bool GetConstraintsCore()
		{
			if (HandToUse switch
			{
				HandType.Right => _handManager.GrabRight ? 1 : 0, 
				HandType.Left => _handManager.GrabLeft ? 1 : 0, 
				_ => throw new Exception(), 
			} == 0 || IsAuto)
			{
				return !_paizuri.IsInPaizuriMode;
			}
			return false;
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
				_xTweener = DOVirtual.Float(_breastX.Value, 0f, 1f, delegate(float x)
				{
					_breastX = _breastX.Update(x);
				}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).SetDelay(0.05f);
				_yTweener = DOVirtual.Float(_breastY.Value, 0f, 1f, delegate(float x)
				{
					_breastY = _breastY.Update(x);
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

		public override void Cancel()
		{
			base.Cancel();
			_synchro = false;
			_breastMoving = false;
		}
	}
}
