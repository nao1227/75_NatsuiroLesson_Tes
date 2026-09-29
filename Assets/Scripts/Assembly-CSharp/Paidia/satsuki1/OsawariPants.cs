using System;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariPants : OsawariWithoutAuto, ISwitchable, IWearable, IPants
	{
		protected ParameterValue _pants;

		protected BoolParameterValue _pantsFlag;

		protected OsawariPiston _piston;

		private bool _isAutoOpening;

		private int _pantsParameterNumber;

		private bool _isPantsEnabled = true;

		protected bool _forceFromOtherParts;

		private HandType _fromOtherPartsHandType;

		private Subject<bool> _onPantsChanged = new Subject<bool>();

		public List<AbstractOsawari> CancelPartsWhenClose = new List<AbstractOsawari>();

		public bool Touchable => _pantsFlag.AsBool();

		public bool IsOpen
		{
			get
			{
				ParameterValue pants = _pants;
				if (pants == null)
				{
					return false;
				}
				return pants.Value > 0.9f;
			}
		}

		public float Value => _pants?.Value ?? 0f;

		public IObservable<bool> OnPantsChanged => _onPantsChanged;

		public bool IsWearing()
		{
			return _pantsFlag.AsBool();
		}

		protected override void InitializeParams()
		{
			_pants = new ParameterValue(parameters[ParameterName.Pants]);
			_pantsFlag = new BoolParameterValue(val: true);
			_pantsParameterNumber = ParameterNumbers.GetTable()[ParameterName.PantsFlag];
			_piston = _manager.GetOsawariOf<OsawariPiston>();
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Pants, _pants);
			if (_isPantsEnabled)
			{
				SetLive2D(_pantsParameterNumber, _pantsFlag.Value);
			}
			else
			{
				SetLive2D(_pantsParameterNumber, 0f);
			}
		}

		protected override void AutoAnimation()
		{
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			if (handType == HandType.Left)
			{
				return ParameterNumbers.GetTable()[ParameterName.LeftHandOnPants];
			}
			return ParameterNumbers.GetTable()[ParameterName.RightHandOnPants];
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (_isAutoOpening)
			{
				return;
			}
			if (_forceFromOtherParts)
			{
				_manHand.Appear(_fromOtherPartsHandType);
				SetLive2D(GetHandParamIndex(_fromOtherPartsHandType), _manHand.GetValue(_fromOtherPartsHandType));
			}
			else
			{
				_manHand.Appear(GetActiveHand());
			}
			bool num = move.x > 0f;
			bool flag = move.x < 0f;
			if (num && _pants.Value == 0f)
			{
				_isAutoOpening = true;
				DOVirtual.Float(_pants.Value, 1f, 0.5f, delegate(float x)
				{
					_pants = _pants.Update(x);
				}).OnComplete(delegate
				{
					_isAutoOpening = false;
					_onPantsChanged.OnNext(value: false);
				}).Play();
			}
			else
			{
				if (!flag || _pants.Value != 1f)
				{
					return;
				}
				_isAutoOpening = true;
				foreach (AbstractOsawari item in CancelPartsWhenClose)
				{
					item.Cancel();
				}
				DOVirtual.Float(_pants.Value, 0f, 0.5f, delegate(float x)
				{
					_pants = _pants.Update(x);
				}).OnComplete(delegate
				{
					_isAutoOpening = false;
					_onPantsChanged.OnNext(value: true);
				}).Play();
			}
		}

		protected override void UpdateWhileNotClicked()
		{
			if (_isAutoOpening)
			{
				_manHand.Appear();
			}
			else if (!_forceFromOtherParts)
			{
				_manHand.Disappear();
				SetLive2D(GetHandParamIndex(_fromOtherPartsHandType), _manHand.GetValue(_fromOtherPartsHandType));
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (_pantsFlag.AsBool() && !_isAutoOpening && _isPantsEnabled)
			{
				if (!(null == _piston))
				{
					if (!_piston.IsAnimating)
					{
						return _piston.ManPenis.Value == ManPenisStatus.Unshown;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public virtual bool IsAbleToInsert()
		{
			if (_pantsFlag == null || _pants == null)
			{
				return true;
			}
			BoolParameterValue pantsFlag = _pantsFlag;
			if (pantsFlag == null || pantsFlag.AsBool())
			{
				return IsOpen;
			}
			return true;
		}

		public virtual void OnPants()
		{
			_pantsFlag = _pantsFlag.Update(val: true);
			OsawariPiston piston = _piston;
			if ((object)piston != null && piston.IsInsert())
			{
				_pants = _pants.Update(1f);
			}
		}

		public void OffPants()
		{
			_pantsFlag = _pantsFlag.Update(val: false);
		}

		public void SwitchPants()
		{
			if (_pantsFlag.AsBool())
			{
				OffPants();
			}
			else
			{
				OnPants();
			}
		}

		public void SetPantsName(int pantsParameterNumber)
		{
			if (null == _manager)
			{
				_manager = GetComponent<OsawariManager>();
			}
			SetLive2D(pantsParameterNumber, 0f);
			_pantsParameterNumber = pantsParameterNumber;
		}

		public void InactivatePants()
		{
			_isPantsEnabled = false;
		}

		public void ForceUpdateParams(Vector3 move, AbstractOsawari osawari)
		{
			_forceFromOtherParts = true;
			_fromOtherPartsHandType = _handManager.GetHandGrabbing(osawari)[0].HandType;
			UpdateParamsCore(move);
		}

		public void EnableUpdateWhileNotClicked()
		{
			_forceFromOtherParts = false;
		}
	}
}
