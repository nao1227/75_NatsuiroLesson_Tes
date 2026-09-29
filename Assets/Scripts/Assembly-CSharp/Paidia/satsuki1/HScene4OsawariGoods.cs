using System;
using System.Collections.Generic;
using Live2D.Cubism.Framework;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class HScene4OsawariGoods : OsawariGoods, IPiston, IInsertable
	{
		private OsawariMizugiUpperPaizuri _bra;

		private OsawariPantsPaizuri _paizuriPants;

		private Live2DAnimator _animator;

		public Live2DAnimator PaizuriAnimator;

		private bool _isVibAppeared;

		private bool _isVibEntered;

		private Subject<bool> _onPiston = new Subject<bool>();

		private Subject<bool> _onInsert = new Subject<bool>();

		public IObservable<bool> OnPiston()
		{
			return _onPiston;
		}

		public IObservable<bool> OnInsert()
		{
			return _onInsert;
		}

		public override bool IsAnimating()
		{
			if (_isVibAppeared)
			{
				return !_isVibEntered;
			}
			return false;
		}

		public override void ManagedStart()
		{
			_animator = GetComponent<Live2DAnimator>();
			_bra = Manager.GetOsawariOf<OsawariMizugiUpperPaizuri>();
			_paizuriPants = Manager.GetOsawariOf<OsawariPantsPaizuri>();
			base.ManagedStart();
		}

		public override void InitializeRx()
		{
			OnRotorEnabled.OnNext((RotorPlace.Breast, false));
			OnRotorEnabled.OnNext((RotorPlace.Kuri, false));
			OnVibratorEnabled.OnNext(value: false);
			_isWearingShirt = false;
			_bra.OnBraChanged.Where((bool _) => Manager.ContextManager.Context == OsawariContext.Paizuri).Subscribe(delegate(bool x)
			{
				SwitchRotorEnabled(RotorPlace.Breast, !x && !Manager.OsawariPaizuri.IsInPaizuriMode);
			}).AddTo(this);
			_paizuriPants.OnPantsChanged.Where((bool _) => Manager.ContextManager.Context == OsawariContext.Paizuri).Subscribe(delegate(bool x)
			{
				SwitchRotorEnabled(RotorPlace.Kuri, !x);
				OnVibratorEnabled.OnNext(!x);
			}).AddTo(this);
			StateMachineObservables[] rx = PaizuriAnimator.GetRx();
			foreach (StateMachineObservables stateMachineObservables in rx)
			{
				if (stateMachineObservables.ID == StateID.Entry)
				{
					(from x in stateMachineObservables.OnStateEnterObservable
						where Manager.ContextManager.Context == OsawariContext.Paizuri
						where x.Item2.IsName("VibEntry")
						select x).Subscribe(delegate
					{
						_isVibAppeared = false;
					});
				}
				if (stateMachineObservables.ID == StateID.VibIdle)
				{
					stateMachineObservables.OnStateEnterObservable.Subscribe(delegate
					{
						_onInsert.OnNext(value: true);
						_vibratorAppear = true;
						_isVibEntered = true;
					}).AddTo(this);
				}
				if (stateMachineObservables.ID == StateID.VibEnter)
				{
					stateMachineObservables.OnStateEnterObservable.Subscribe(delegate
					{
						_onInsert.OnNext(value: true);
					}).AddTo(this);
				}
				if (stateMachineObservables.ID == StateID.VibExit)
				{
					stateMachineObservables.OnStateEnterObservable.Subscribe(delegate
					{
						_isVibEntered = false;
					}).AddTo(this);
				}
			}
			Manager.OsawariPaizuri.OnPaizuriChanged.Where((float x) => Manager.ContextManager.Context == OsawariContext.Paizuri).Subscribe(delegate(float x)
			{
				if (x > 0f)
				{
					SwitchRotorEnabled(RotorPlace.Breast, on: false);
				}
				else
				{
					SwitchRotorEnabled(RotorPlace.Breast, !_isBra);
				}
			}).AddTo(this);
		}

		public override void BraOff()
		{
			_isBra = false;
		}

		private void LateUpdate()
		{
			if (!_loaded)
			{
				return;
			}
			foreach (KeyValuePair<int, ParameterValue> rotor in _rotors)
			{
				Manager.Preserver.SetValue(rotor.Key, rotor.Value.Value, CubismParameterBlendMode.Override, 1);
			}
			if (base.IsRotorMove)
			{
				_rotorOrbit += RotorSpeed * Time.deltaTime;
			}
			while (_rotorOrbit > 1f)
			{
				_rotorOrbit -= 1f;
			}
			if (_isVibEntered && _vibratorOn)
			{
				_vibrator = _vibrator.Update(_vibrator.Value + VibratorSpeed * Time.deltaTime);
				if (_vibrator.Value == 1f)
				{
					_vibrator = _vibrator.Update(-1f);
				}
			}
			Manager.Preserver.SetValue(RotorOrbitNumber, _rotorOrbit, CubismParameterBlendMode.Override, 1);
			Manager.Preserver.SetValue(PenisStatusNumber, _condom);
			if (_isVibEntered)
			{
				Manager.Preserver.SetValue(VibratorNumber, _vibrator.Value, CubismParameterBlendMode.Override, 1);
			}
			else
			{
				Manager.Preserver.InactivateValue(VibratorNumber, 1);
			}
		}

		public override void SwitchContext()
		{
			if (Manager.ContextManager.Context == OsawariContext.Osawari)
			{
				SingletonManager<SoundManager>.Instance.StopBGS(1);
				SwitchRotorEnabled(RotorPlace.Breast, on: false);
				SwitchRotorEnabled(RotorPlace.Kuri, on: false);
				OnVibratorEnabled.OnNext(value: false);
				SwitchVibrator(on: false);
				OnCondomEnabled.OnNext(SaveLoadManager.UnsavedData.PersistantStatus.Relationship == Relationship.LoveyDovey);
			}
			else
			{
				SwitchRotorEnabled(RotorPlace.Breast, !Manager.OsawariPaizuri.IsInPaizuriMode && !_isBra);
				SwitchRotorEnabled(RotorPlace.Kuri, _paizuriPants.IsAbleToInsert());
				OnVibratorEnabled.OnNext(_paizuriPants.IsAbleToInsert());
				OnCondomEnabled.OnNext(value: false);
			}
		}

		protected override void SwitchRotorEnabled(RotorPlace place, bool on)
		{
			if (Manager.ContextManager.Context == OsawariContext.Osawari)
			{
				base.SwitchRotorEnabled(RotorPlace.Kuri, on: false);
				base.SwitchRotorEnabled(RotorPlace.Breast, on: false);
			}
			else
			{
				base.SwitchRotorEnabled(place, on);
			}
		}

		public override void AppearVibrator(bool on, bool fromPants = false)
		{
			if (_isVibAppeared == on)
			{
				return;
			}
			if (on)
			{
				PaizuriAnimator.SetTrigger("TriggerVibEnter");
				_isVibAppeared = true;
			}
			else
			{
				if (fromPants)
				{
					PaizuriAnimator.SetTrigger("TriggerVibEjectNoAnim");
					_isVibAppeared = false;
				}
				else
				{
					PaizuriAnimator.SetTrigger("TriggerVibEject");
				}
				SwitchVibrator(on: false);
			}
			if (!on && !base.IsRotorMove)
			{
				SingletonManager<SoundManager>.Instance.StopSE(4);
			}
			_paizuriPants.SetVibrator(on);
			OnVibratorAppear.OnNext(on);
		}

		public override void SwitchRotorAppear(RotorPlace place, bool on)
		{
			base.SwitchRotorAppear(place, on);
			switch (place)
			{
			case RotorPlace.Breast:
				_bra.SetRotor(on);
				break;
			case RotorPlace.Kuri:
				_paizuriPants.SetRotor(on);
				break;
			}
			_ = Rotors.Count;
		}

		public override void SwitchVibrator(bool on)
		{
			base.SwitchVibrator(on);
			_onPiston.OnNext(on);
		}

		public override void SwitchPants()
		{
		}
	}
}
