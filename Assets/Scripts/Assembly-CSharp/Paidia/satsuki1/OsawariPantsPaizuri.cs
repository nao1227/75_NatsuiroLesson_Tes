using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariPantsPaizuri : AbstractOsawariAnotherModel, IPants, IWearable
	{
		private ParameterValue _pants;

		private BoolParameterValue _pantsFlag;

		private OsawariPantsOroshiPaizuri _oroshi;

		private bool _isAutoOpening;

		public HScene4OsawariPants _osawariPants;

		private OsawariPaizuri _paizuri;

		private bool _isRotorSet;

		private bool _isVibSet;

		private Subject<bool> _onPantsChanged = new Subject<bool>();

		public HScene4OsawariGoods _goods;

		private int _pantsParameterNumber => GetParameterNumber(IsMB() ? ParameterName.MBPantsFlag : ParameterName.PantsFlag);

		private int _notShownPantsParameterNumber => GetParameterNumber(IsMB() ? ParameterName.PantsFlag : ParameterName.MBPantsFlag);

		public bool IsClosed
		{
			get
			{
				ParameterValue pants = _pants;
				if (pants == null)
				{
					return false;
				}
				return pants.Value < 0.1f;
			}
		}

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

		public IObservable<bool> OnPantsChanged => _onPantsChanged;

		public bool IsWearing()
		{
			return _pantsFlag.AsBool();
		}

		private bool IsMB()
		{
			return _manager.TemporaryStatus.Cloth == ClothName.MicroBikini;
		}

		protected override void InitializeParams()
		{
			_pants = new ParameterValue(parameters[ParameterName.Pants]);
			_pantsFlag = new BoolParameterValue(val: true);
			_oroshi = _manager.GetOsawariOf<OsawariPantsOroshiPaizuri>();
			_paizuri = _manager.OsawariPaizuri;
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Pants, _pants);
			SetLive2D(_pantsParameterNumber, _pantsFlag.Value);
			SetLive2D(_notShownPantsParameterNumber, 0f);
			_osawariPants.SynchroValue(_pantsFlag.Value, _pants.Value);
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
			_manHand.Appear(GetActiveHand());
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
			else if (flag && _pants.Value == 1f)
			{
				_isAutoOpening = true;
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
			else
			{
				_manHand.Disappear();
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (_pantsFlag.AsBool() && !_isAutoOpening && !_isRotorSet && !_isVibSet)
			{
				return !_paizuri.IsInPaizuriMode;
			}
			return false;
		}

		public virtual bool IsAbleToInsert()
		{
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
			_oroshi.ResetPantsOroshi();
			if (_goods.IsVibratorAppeared || _manager.GetOsawariOf<HScene4OsawariPiston>().GetManPenisValue() != ManPenisStatus.Unshown)
			{
				_pants = _pants.Update(1f);
			}
			else
			{
				_pants = _pants.Update(0f);
			}
			_onPantsChanged.OnNext(_pants.Value == 0f);
		}

		public void SetPantsFlag(bool flag)
		{
			if (!flag)
			{
				_pantsFlag = _pantsFlag.Update(val: false);
			}
		}

		public void OffPants()
		{
			_pants = _pants.Update(0f);
			_pantsFlag = _pantsFlag.Update(val: false);
			_onPantsChanged.OnNext(value: false);
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

		public void SynchroValue(float flag, float value)
		{
		}

		public void SetRotor(bool isSet)
		{
			_isRotorSet = isSet;
		}

		public void SetVibrator(bool isSet)
		{
			_isVibSet = isSet;
		}

		public override void SetAuto()
		{
		}
	}
}
