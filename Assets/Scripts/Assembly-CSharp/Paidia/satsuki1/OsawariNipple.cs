using System;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariNipple : AbstractOsawari, IParticularHand
	{
		[SerializeField]
		protected HandType HandToUse;

		private OsawariArms _osawariArms;

		private OsawariShirt _osawariShirt;

		private OsawariClothes _osawariClothes;

		protected ParameterValue _nipple;

		protected OsawariGoods _goods;

		private OsawariCosplay _cosplay;

		protected OsawariNipple _another;

		private bool _synchro;

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
				if (GetConstraintsCore() && !GetParticularHand().IsGrabbing)
				{
					OnFirstClick();
					_handManager.Grab(GetParticularHand().HandType, this);
				}
			}
			if (_handManager.IsGrabbing(this))
			{
				UpdateParams(ConvertMovementVec3ForParams());
			}
		}

		protected override void AutoAnimation()
		{
			_nipple = _nipple.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			_manHand.SetValue(HandToUse, 1f);
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return ParameterNumbers.GetTable()[ParameterName.NippleHand];
		}

		protected override void InitializeParams()
		{
			_osawariArms = _manager.GetOsawariOf<OsawariArms>();
			_osawariClothes = _manager.GetOsawariOf<OsawariClothes>();
			_osawariShirt = _manager.GetOsawariOf<OsawariShirt>();
			_nipple = new ParameterValue(parameters[ParameterName.Nipple]);
			_goods = GetComponent<OsawariGoods>();
			_cosplay = _manager.GetOsawariOf<OsawariCosplay>();
			_goods.OnRotorAppear.Where(((RotorPlace, bool) x) => x.Item1 == RotorPlace.Breast && x.Item2).Subscribe(delegate
			{
				Cancel();
			}).AddTo(this);
			List<OsawariNipple> everyOsawariOf = _manager.GetEveryOsawariOf<OsawariNipple>();
			if (everyOsawariOf[0] == this)
			{
				_another = everyOsawariOf[1];
			}
			else
			{
				_another = everyOsawariOf[0];
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (!_osawariArms.IsArmClosed && _osawariShirt.NippleAllowed && !_osawariClothes.IsWearingBra && !_goods.IsRotorAppear(RotorPlace.Breast))
			{
				return !_cosplay.IsWearingBra();
			}
			return false;
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Nipple, _nipple);
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			_nipple += move.x / SensitivityX;
			_manHand.Appear(GetActiveHand());
			if (_synchro)
			{
				_another.UpdateSingleParam(move);
			}
		}

		protected void UpdateSingleParam(Vector3 move)
		{
			_nipple += move.x / SensitivityX;
			_manHand.Appear(GetActiveHand());
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!_synchro)
			{
				_manHand.Disappear();
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

		public override void OnMouseUp(bool fromCancel = false)
		{
			if (!IsAuto)
			{
				if (_synchro)
				{
					_synchro = false;
					_another._synchro = false;
				}
				base.OnMouseUp(fromCancel);
			}
		}

		public override void OnSpecial()
		{
			if (!_synchro)
			{
				if (_manager.CanGrab(_another))
				{
					_handManager.Grab(_another.HandToUse, _another);
					_synchro = true;
					_another._synchro = true;
					_another.UpdateSingleParam(ConvertMovementVec3ForParams());
				}
				base.OnSpecial();
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
	}
}
