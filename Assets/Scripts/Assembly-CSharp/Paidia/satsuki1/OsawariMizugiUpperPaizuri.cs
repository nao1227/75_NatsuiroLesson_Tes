using System;
using Live2D.Cubism.Core;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariMizugiUpperPaizuri : AbstractOsawariAnotherModel, IBra, IWearable
	{
		public CubismDrawable MBMesh;

		private ParameterValue _mizugiUpper;

		private BoolParameterValue _breastFlag;

		private ParameterValue _bra;

		private bool _isPuttingOffAnimation;

		private bool _isPreparingPuttingOffAnimation;

		private Live2DAnimator _animator;

		public OsawariMizugiHimo Mizugi;

		private bool _isRotorSet;

		private Subject<bool> _onBraChanged = new Subject<bool>();

		private OsawariPaizuri _paizuri;

		public IObservable<bool> OnBraChanged => _onBraChanged;

		public bool IsWearing()
		{
			return _bra.Value <= 0.1f;
		}

		protected override void AutoAnimation()
		{
		}

		protected override void SetTouchableMeshs()
		{
			TouchableMeshs = new CubismDrawable[1];
			TouchableMeshs[0] = (IsMB() ? MBMesh : Mesh);
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return GetParameterNumber(IsMB() ? ParameterName.ManHandTakeOffMB : ParameterName.ManHandTakeOffBra);
		}

		protected override void InitializeParams()
		{
			_mizugiUpper = new ParameterValue(parameters[ParameterName.WetSuitUpperFlag]);
			_breastFlag = new BoolParameterValue(val: true);
			_bra = new ParameterValue(parameters[ParameterName.Bra]);
			_paizuri = _manager.GetOsawariOf<OsawariPaizuri>();
			_animator = GetComponent<Live2DAnimator>();
		}

		protected override void OnLateUpdate()
		{
			if (_isPuttingOffAnimation)
			{
				if (IsMB())
				{
					_isPuttingOffAnimation = false;
					TakeOffBra(exc: true);
					return;
				}
				if (AnotherModel.Parameters[GetParameterNumber(ParameterName.Bra)].Value >= 0.82f)
				{
					InactivateLive2D(ParameterName.WetSuitUpperFlag);
					InactivateLive2D(ParameterName.Bra);
					InactivateLive2D(ParameterName.BreastFlag);
				}
				if (AnotherModel.Parameters[GetParameterNumber(ParameterName.Bra)].Value == 1f)
				{
					_breastFlag = _breastFlag.Update(val: false);
					_mizugiUpper = _mizugiUpper.Update(-1f);
					_isPuttingOffAnimation = false;
					TakeOffBra(exc: true);
				}
				return;
			}
			if (_isPreparingPuttingOffAnimation)
			{
				_manHand.Appear();
				_bra += 0.82f * Time.deltaTime * 3f;
				if (IsMB())
				{
					if (_bra.Value >= 1f)
					{
						_bra = _bra.Update(1f);
						_isPreparingPuttingOffAnimation = false;
						_isPuttingOffAnimation = true;
					}
				}
				else if (_bra.Value >= 0.82f)
				{
					_bra = _bra.Update(0.82f);
					_isPreparingPuttingOffAnimation = false;
					_isPuttingOffAnimation = true;
					_animator.SetTrigger("PutOffMizugiUpper");
				}
			}
			if (IsMB())
			{
				SetLive2D(ParameterName.Bra, 0f);
				SetLive2D(ParameterName.TakeOffMB, _bra);
				SetLive2D(ParameterName.BreastFlag, 0f);
				SetLive2D(ParameterName.WetSuitUpperFlag, 0f);
				SetLive2D(ParameterName.MicroBikiniUpper, _mizugiUpper);
			}
			else
			{
				SetLive2D(ParameterName.Bra, _bra);
				SetLive2D(ParameterName.TakeOffMB, 0f);
				SetLive2D(ParameterName.BreastFlag, (!(_bra.Value >= 0.82f)) ? 1 : 0);
				SetLive2D(ParameterName.WetSuitUpperFlag, _mizugiUpper.Value);
				SetLive2D(ParameterName.MicroBikiniUpper, 0f);
			}
			Mizugi.SyncroValue(1f - _bra.Value);
		}

		private bool IsMB()
		{
			return _manager.TemporaryStatus.Cloth == ClothName.MicroBikini;
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (!_isPuttingOffAnimation && !_isPreparingPuttingOffAnimation)
			{
				if (move.y > 0f && _bra.Value < 0.82f)
				{
					_isPreparingPuttingOffAnimation = true;
				}
				else
				{
					_manHand.Appear();
				}
			}
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!_isPuttingOffAnimation && !_isPreparingPuttingOffAnimation)
			{
				_manHand.Disappear();
			}
		}

		public void SyncroValue(float flagVal, float val)
		{
		}

		protected override bool GetConstraintsCore()
		{
			if (!_paizuri.IsInPaizuriMode)
			{
				return !_isRotorSet;
			}
			return false;
		}

		public void TakeOnBra()
		{
			_mizugiUpper = _mizugiUpper.Update(1f);
			_bra = _bra.Update(0f);
			_onBraChanged.OnNext(value: true);
		}

		public void TakeOffBra(bool exc)
		{
			_mizugiUpper = _mizugiUpper.Update(0f);
			_bra = _bra.Update(1f);
			_onBraChanged.OnNext(value: false);
			Mizugi.TakeOffBra(exc);
		}

		public void SwitchBra()
		{
			if (_bra.Value == 0f)
			{
				TakeOffBra(exc: true);
			}
			else
			{
				TakeOnBra();
			}
		}

		public void SetRotor(bool on)
		{
			_isRotorSet = on;
		}
	}
}
