using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	public class OsawariGoods : MonoBehaviour
	{
		public OsawariManager Manager;

		public List<Rotor> Rotors;

		public int RotorOrbitNumber;

		public int PenisStatusNumber;

		public int CondomOnValue = 2;

		public int CondomOffValue = 1;

		public bool UseVibrator;

		public int VibratorNumber;

		public int VibratorFlag;

		public float RotorSpeed;

		public float VibratorSpeed;

		private AudioClip _rotorSoundClip;

		public List<OsawariEvent> OnRotorMoveEvents;

		public List<OsawariEvent> OnVibMoveEvents;

		protected Dictionary<int, ParameterValue> _rotors;

		protected Dictionary<RotorPlace, bool> _rotorEnabled = new Dictionary<RotorPlace, bool>
		{
			{
				RotorPlace.Breast,
				false
			},
			{
				RotorPlace.Kuri,
				false
			}
		};

		protected float _rotorOrbit;

		protected int _condom;

		protected ParameterValue _vibrator;

		protected bool _vibratorOn;

		protected bool _vibratorAppear;

		protected bool _condomEnabled = true;

		protected bool _loaded;

		public int RotorExciteIncrement = 8;

		public int RotorAtomosphereIncrement = 4;

		public int VibratorExciteIncrement = 8;

		public int VibratorAtomosphereIncrement = 4;

		protected OsawariPants _pants;

		protected bool _isBra = true;

		public Subject<bool> OnVibratorEnabled;

		public Subject<bool> OnVibratorAppear = new Subject<bool>();

		public Subject<bool> OnVibratorStart;

		public Subject<(RotorPlace, bool)> OnRotorEnabled;

		public Subject<bool> OnRotorMove;

		public Subject<(RotorPlace, bool)> OnRotorAppear;

		public Subject<bool> OnCondomEnabled;

		public Subject<bool> OnCondomSet;

		protected bool _isWearingShirt = true;

		public bool IsRotorMove { get; protected set; }

		public bool IsVibratorAppeared => _vibratorAppear;

		public bool IsCondomSet => _condom == CondomOnValue;

		public bool AnyRotorEnabled => _rotorEnabled.Any((KeyValuePair<RotorPlace, bool> x) => x.Value);

		public virtual bool IsAnimating()
		{
			return false;
		}

		public bool IsRotorAppear(RotorPlace place)
		{
			return _rotors[Rotors.First((Rotor x) => x.Place == place).RotorNumber].Value == 1f;
		}

		public virtual void ManagedStart()
		{
			OnRotorEnabled = new Subject<(RotorPlace, bool)>();
			OnVibratorEnabled = new Subject<bool>();
			OnVibratorStart = new Subject<bool>();
			OnRotorMove = new Subject<bool>();
			OnRotorAppear = new Subject<(RotorPlace, bool)>();
			OnCondomEnabled = new Subject<bool>();
			OnCondomSet = new Subject<bool>();
			_rotors = new Dictionary<int, ParameterValue>();
			OnRotorMoveEvents.ForEach(delegate(OsawariEvent x)
			{
				x.Initialize();
			});
			OnVibMoveEvents.ForEach(delegate(OsawariEvent x)
			{
				x.Initialize();
			});
			_pants = Manager.GetOsawariOf<OsawariPants>();
			foreach (Rotor rotor in Rotors)
			{
				_rotors.Add(rotor.RotorNumber, new ParameterValue(Manager.Model.Parameters[rotor.RotorNumber]));
				_rotors[rotor.RotorNumber] = _rotors[rotor.RotorNumber].Update(0f);
			}
			_rotorOrbit = 0f;
			if (UseVibrator)
			{
				_vibrator = new ParameterValue(Manager.Model.Parameters[VibratorNumber]);
			}
			_vibratorOn = false;
			_vibratorAppear = false;
			_condom = CondomOffValue;
			_loaded = true;
			IsRotorMove = false;
			LoadSE().Forget();
		}

		protected virtual async UniTask LoadSE()
		{
			_rotorSoundClip = await Addressables.LoadAssetAsync<AudioClip>("RotorSE");
		}

		public virtual void InitializeRx()
		{
			OnVibratorEnabled.OnNext(value: false);
			OnCondomEnabled.OnNext(value: false);
			_pants?.OnPantsChanged.Where((bool _) => Manager.ContextManager.Context == OsawariContext.Osawari).Subscribe(delegate(bool x)
			{
				SwitchRotorEnabled(RotorPlace.Kuri, !x);
				OnVibratorEnabled.OnNext(!x);
			}).AddTo(this);
		}

		public virtual void SetCondomEnabled(bool enabled)
		{
			_condomEnabled = enabled;
			OnCondomEnabled.OnNext(enabled);
		}

		public virtual void SwitchRotorAppear(RotorPlace place, bool appear)
		{
			if (_loaded && Rotors.Count != 0)
			{
				OnRotorAppear.OnNext((place, appear));
				int num = (appear ? 1 : 0);
				ParameterValue parameterValue = _rotors[Rotors.First((Rotor x) => x.Place == place).RotorNumber];
				_rotors[Rotors.First((Rotor x) => x.Place == place).RotorNumber] = parameterValue.Update(num);
				Manager.OsawariFellatio?.SetRotor(_rotors[Rotors.First((Rotor x) => x.Place == RotorPlace.Breast).RotorNumber].Value == 1f, _rotors[Rotors.First((Rotor x) => x.Place == RotorPlace.Kuri).RotorNumber].Value == 1f);
				if (appear && IsRotorMove)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_rotorSoundClip, AudioSetting.GetLoopedDefault(), 4);
				}
				else if (_rotors.Count((KeyValuePair<int, ParameterValue> x) => x.Value.Value == 1f) == 0 && !_vibratorOn)
				{
					SingletonManager<SoundManager>.Instance.StopSE(4);
				}
				if (_rotors.Count((KeyValuePair<int, ParameterValue> x) => x.Value.Value == 1f) == 0)
				{
					SwitchRotor(on: false);
				}
			}
		}

		public virtual void SwitchRotor(bool on)
		{
			if (_rotors.Count((KeyValuePair<int, ParameterValue> x) => x.Value.Value == 1f) == 0)
			{
				if (!_vibratorOn)
				{
					SingletonManager<SoundManager>.Instance.StopSE(4);
				}
				IsRotorMove = false;
				OnRotorMove.OnNext(value: false);
				return;
			}
			IsRotorMove = on;
			if (on)
			{
				SingletonManager<SoundManager>.Instance.PlaySE(_rotorSoundClip, AudioSetting.GetLoopedDefault(), 4);
				foreach (OsawariEvent onRotorMoveEvent in OnRotorMoveEvents)
				{
					onRotorMoveEvent.InvokeEvent(Manager.TemporaryStatus, OsawariConditions.Empty).Forget();
				}
			}
			else if (!_vibratorOn)
			{
				SingletonManager<SoundManager>.Instance.StopSE(4);
			}
			OnRotorMove.OnNext(on);
		}

		public virtual void SwitchCondom(bool on)
		{
			if (_condomEnabled)
			{
				_condom = (on ? CondomOnValue : CondomOffValue);
				OnCondomSet.OnNext(on);
				GetComponent<Live2DAnimator>().SetBool("Condom", on);
				if (!SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.AllowCondomPutOff) && !SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_Bath) && on)
				{
					SetCondomEnabled(enabled: false);
				}
			}
		}

		public virtual void SwitchVibrator(bool on)
		{
			_vibratorOn = on;
			if (on)
			{
				SingletonManager<SoundManager>.Instance.PlaySE(_rotorSoundClip, AudioSetting.GetLoopedDefault(), 4);
				foreach (OsawariEvent onVibMoveEvent in OnVibMoveEvents)
				{
					onVibMoveEvent.InvokeEvent(Manager.TemporaryStatus, OsawariConditions.Empty).Forget();
				}
			}
			else if (!IsRotorMove)
			{
				SingletonManager<SoundManager>.Instance.StopSE(4);
			}
			OnVibratorStart.OnNext(on);
		}

		public virtual void AppearVibrator(bool on, bool fromPants = false)
		{
			_vibratorAppear = on;
			if (!on && !IsRotorMove)
			{
				SingletonManager<SoundManager>.Instance.StopSE(4);
			}
			OnVibratorAppear.OnNext(on);
		}

		public virtual void SwitchContext()
		{
			if (Manager.ContextManager.Context == OsawariContext.Fellatio)
			{
				SetCondomEnabled(enabled: false);
			}
		}

		public virtual void SwitchShirt(bool wear)
		{
			if (_isWearingShirt)
			{
				ShirtOff();
			}
			else
			{
				ShirtOn();
			}
		}

		public virtual void SwitchPants()
		{
			if (_rotorEnabled[RotorPlace.Kuri])
			{
				PantsOn();
			}
			else
			{
				PantsOff();
			}
		}

		public virtual void SwitchBra()
		{
			if (!_isWearingShirt)
			{
				if (!_isBra)
				{
					BraOn();
				}
				else
				{
					BraOff();
				}
			}
		}

		public virtual void SwitchSkirt(bool on)
		{
			if (on)
			{
				SkirtOn();
			}
		}

		public virtual void ShirtOn()
		{
			SwitchRotorAppear(RotorPlace.Breast, appear: false);
			SwitchRotorEnabled(RotorPlace.Breast, on: false);
			_isWearingShirt = true;
		}

		public virtual void ShirtOff()
		{
			_isWearingShirt = false;
		}

		public virtual void PantsOn()
		{
			SwitchRotorAppear(RotorPlace.Kuri, appear: false);
			SwitchRotorEnabled(RotorPlace.Kuri, on: false);
		}

		public virtual void PantsOff()
		{
			SwitchRotorEnabled(RotorPlace.Kuri, on: true);
		}

		public virtual void BraOn()
		{
			_isBra = true;
			SwitchRotorAppear(RotorPlace.Breast, appear: false);
			SwitchRotorEnabled(RotorPlace.Breast, on: false);
		}

		public virtual void BraOff()
		{
			_isBra = false;
			SwitchRotorEnabled(RotorPlace.Breast, on: true);
		}

		public virtual void SkirtOn()
		{
			SwitchRotorAppear(RotorPlace.Kuri, appear: false);
			SwitchVibrator(on: false);
			AppearVibrator(on: false, fromPants: true);
		}

		private void Update()
		{
			if (IsRotorMove)
			{
				foreach (KeyValuePair<RotorPlace, bool> item in _rotorEnabled)
				{
					if (item.Value)
					{
						Manager.TemporaryStatus.AddExciteValue(RotorExciteIncrement);
						Manager.TemporaryStatus.Feelings.AddAtomosphere(RotorAtomosphereIncrement);
					}
				}
			}
			if (_vibratorOn)
			{
				Manager.TemporaryStatus.AddExciteValue(VibratorExciteIncrement);
				Manager.TemporaryStatus.Feelings.AddAtomosphere(VibratorAtomosphereIncrement);
			}
		}

		private void LateUpdate()
		{
			if (!_loaded)
			{
				return;
			}
			foreach (KeyValuePair<int, ParameterValue> rotor in _rotors)
			{
				Manager.Preserver.SetValue(rotor.Key, rotor.Value);
			}
			if (IsRotorMove)
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
				Manager.Preserver.SetValue(VibratorFlag, _vibratorAppear ? 1f : 0f);
				Manager.Preserver.SetValue(VibratorNumber, _vibrator);
			}
			Manager.OsawariFellatio?.SetRotorOrbit(_rotorOrbit);
		}

		protected virtual void SwitchRotorEnabled(RotorPlace place, bool on)
		{
			if (_loaded)
			{
				_rotorEnabled[place] = on;
				if (!on)
				{
					SwitchRotorAppear(place, appear: false);
				}
				OnRotorEnabled.OnNext((place, on));
			}
		}
	}
}
