using System;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariNipplePaizuri : AbstractOsawariAnotherModel, IParticularHand
	{
		private OsawariNipplePaizuri _another;

		private ParameterValue _nipple;

		public HandType HandToUse;

		private bool _synchro;

		public bool isInAction;

		private OsawariMizugiUpperPaizuri _mizugi;

		private OsawariPaizuri _paizuri;

		private HScene4OsawariGoods _goods;

		protected override void AutoAnimation()
		{
			_nipple = _nipple.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			_manHand.SetValue(HandToUse, 1f);
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return GetParameterNumber(ParameterName.NippleHand);
		}

		protected override void InitializeParams()
		{
			_nipple = new ParameterValue(parameters[ParameterName.Nipple]);
			_paizuri = _manager.GetOsawariOf<OsawariPaizuri>();
			_goods = _manager.GetComponent<HScene4OsawariGoods>();
			List<OsawariNipplePaizuri> everyOsawariOf = _manager.GetEveryOsawariOf<OsawariNipplePaizuri>();
			if (everyOsawariOf[0] == this)
			{
				_another = everyOsawariOf[1];
			}
			else
			{
				_another = everyOsawariOf[0];
			}
			_mizugi = _manager.GetOsawariOf<OsawariMizugiUpperPaizuri>();
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Nipple, _nipple.Value);
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (!IsAuto)
			{
				_nipple += move.x / SensitivityX;
				_manHand.Appear(GetActiveHand());
				if (_synchro)
				{
					_another.UpdateSingleParam(move);
				}
			}
		}

		private void UpdateSingleParam(Vector3 move)
		{
			_nipple -= move.x / SensitivityX;
			_manHand.SetValue(HandToUse, _another._manHand.GetValue(_another.HandToUse));
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!IsAuto && !_synchro)
			{
				_manHand.Disappear();
				_nipple *= 0.9f;
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
			if ((HandToUse switch
			{
				HandType.Right => _handManager.GrabRight ? 1 : 0, 
				HandType.Left => _handManager.GrabLeft ? 1 : 0, 
				_ => throw new Exception(), 
			} == 0 || IsAuto) && !_mizugi.IsWearing() && !_paizuri.IsInPaizuriMode)
			{
				return !_goods.IsRotorAppear(RotorPlace.Breast);
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
						_another._synchro = false;
						_synchro = false;
					}
				}
				if (GetConstraints() && !GetParticularHand().IsGrabbing)
				{
					OnFirstClick();
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
				base.OnMouseUp(fromCancel);
			}
		}
	}
}
