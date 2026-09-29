using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class HScene3OsawariGoods : OsawariGoods
	{
		private HScene3OsawariHelper _helper;

		private OsawariLeg _leg;

		private OsawariVibrator _osawariVibrator;

		public override bool IsAnimating()
		{
			return _osawariVibrator.IsInInsertAnimation;
		}

		public override void ManagedStart()
		{
			base.ManagedStart();
			_osawariVibrator = Manager.GetOsawariOf<OsawariVibrator>();
			_pants = Manager.GetOsawariOf<HScene3OsawariPants>();
		}

		public override void InitializeRx()
		{
			_leg = Manager.GetOsawariOf<OsawariLeg>();
			_helper = Manager.GetOsawariOf<HScene3OsawariHelper>();
			_leg.OnLegOpen.Subscribe(delegate
			{
				if (_helper.ClothStatus == ClothStatus.Naked)
				{
					SwitchRotorEnabled(RotorPlace.Kuri, on: true);
					OnVibratorEnabled.OnNext(value: true);
				}
			}).AddTo(this);
			_leg.OnLegClosing.Subscribe(delegate
			{
				SwitchRotorEnabled(RotorPlace.Kuri, on: false);
				OnVibratorEnabled.OnNext(value: false);
			}).AddTo(this);
			SwitchRotorEnabled(RotorPlace.Breast, on: false);
			SwitchRotorEnabled(RotorPlace.Kuri, on: false);
			if (_helper.ClothStatus == ClothStatus.Naked)
			{
				SwitchRotorEnabled(RotorPlace.Breast, on: true);
			}
			OnVibratorEnabled.OnNext(value: false);
		}

		public override void ShirtOn()
		{
			if (_helper.ClothStatus != ClothStatus.Naked)
			{
				base.ShirtOn();
			}
		}

		public override void BraOn()
		{
			if (_helper.ClothStatus != ClothStatus.Naked)
			{
				base.BraOn();
			}
		}

		public override void PantsOn()
		{
			if (_helper.ClothStatus != ClothStatus.Naked)
			{
				base.PantsOn();
			}
		}

		public override void SkirtOn()
		{
			if (_helper.ClothStatus != ClothStatus.Naked)
			{
				base.SkirtOn();
			}
		}

		private void LateUpdate()
		{
			if (!_loaded || null == _helper)
			{
				return;
			}
			foreach (KeyValuePair<int, ParameterValue> rotor in _rotors)
			{
				Manager.Preserver.SetValue(rotor.Key, rotor.Value);
			}
			if (base.IsRotorMove)
			{
				_rotorOrbit += RotorSpeed * Time.deltaTime;
			}
			while (_rotorOrbit > 1f)
			{
				_rotorOrbit -= 1f;
			}
			if (UseVibrator && _vibratorOn)
			{
				_vibrator = _vibrator.Update(_vibrator.Value + VibratorSpeed * Time.deltaTime);
				if (_vibrator.Value == 1f)
				{
					_vibrator = _vibrator.Update(-1f);
				}
			}
			Manager.Preserver.SetValue(RotorOrbitNumber, _rotorOrbit);
			Manager.Preserver.SetValue(PenisStatusNumber, _condom);
			if (UseVibrator)
			{
				Manager.Preserver.SetValue(VibratorNumber, _vibrator);
			}
			Manager.OsawariFellatio?.SetRotorOrbit(_rotorOrbit);
			if (_helper.ClothStatus != ClothStatus.Naked)
			{
				OnVibratorEnabled.OnNext(_pants.IsAbleToInsert());
			}
		}

		public override void AppearVibrator(bool on, bool fromPants = false)
		{
			if (on)
			{
				Manager.GetOsawariOf<HScene3OsawariAibu>()?.Cancel();
			}
			base.AppearVibrator(on, fromPants);
		}
	}
}
