using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class FeelingParams
	{
		public const int MAX = 100000;

		private const int CHANGE_FRAME = 20;

		private const int TIMER = 1000;

		public ExcitePhase ExcitePhase;

		public Subject<AtomosphereName> OnAtomosphereChanged = new Subject<AtomosphereName>();

		private Subject<int> _onExtraExcite = new Subject<int>();

		public bool IsExtacyAllowed = true;

		private int _atomosphereAdded;

		private IntReactiveProperty _excite = new IntReactiveProperty(0);

		private IntReactiveProperty _atomosphere = new IntReactiveProperty(0);

		private int _stimulus;

		private int _exciteTimer;

		private int _atomosphereTimer;

		private int _stimulusTimer;

		public bool IsStimulusExtinction;

		public bool IsExciteLocked;

		public int TemporaryAtomosphereMinimum;

		private int _atomosphereSurplusVal;

		private GlobalFlags _flags;

		public IObservable<int> OnExtraExcite => _onExtraExcite;

		public bool IsAtomosphereMax => _atomosphere.Value == _atomosphereMax;

		public int Excite
		{
			get
			{
				if (_excite.Value < _exciteMin)
				{
					_excite.Value = _exciteMin;
				}
				return _excite.Value;
			}
			protected set
			{
				if (ExcitePhase == ExcitePhase.Locked)
				{
					_onExtraExcite.OnNext(value - 100000);
					return;
				}
				if (value > _excite.Value)
				{
					if (ExcitePhase != ExcitePhase.Normal)
					{
						return;
					}
					_exciteTimer = 1000;
				}
				else if (!CanReduceExcite)
				{
					return;
				}
				if (value > 100000)
				{
					if (IsExtacyAllowed)
					{
						_excite.Value = 100000;
					}
					else
					{
						_excite.Value = Mathf.Max(95000, _excite.Value);
					}
				}
				else if (value <= _exciteMin)
				{
					_excite.Value = _exciteMin;
					ExcitePhase = ExcitePhase.Normal;
				}
				else if (ExcitePhase == ExcitePhase.OnlyDown)
				{
					_excite.Value -= 500;
				}
				else
				{
					_excite.Value = value;
				}
			}
		}

		public IReadOnlyReactiveProperty<int> ExciteRx => _excite;

		public IReadOnlyReactiveProperty<int> Atomosphere
		{
			get
			{
				if (_atomosphere.Value < _atomosphereMin)
				{
					_atomosphere.Value = _atomosphereMin;
				}
				return _atomosphere;
			}
		}

		public int Stimulus
		{
			get
			{
				if (_stimulus < _stimulusMin)
				{
					_stimulus = _stimulusMin;
				}
				return _stimulus;
			}
			set
			{
				if (!IsStimulusExtinction)
				{
					_stimulusTimer = 1000;
				}
				if (value < _stimulus && IsStimulusExtinction)
				{
					IsStimulusExtinction = false;
					if (!CanReduceStimulus)
					{
						return;
					}
				}
				if (value > 100000)
				{
					_stimulus = 100000;
				}
				else if (value < _stimulusMin)
				{
					_stimulus = _stimulusMin;
				}
				else
				{
					_stimulus = value;
				}
			}
		}

		public int AtomosphereAdded => _atomosphereAdded;

		public bool CanReduceStimulus => _stimulusTimer == 0;

		public bool CanReduceExcite => _exciteTimer == 0;

		public bool CanReduceAtomosphere => _atomosphereTimer == 0;

		public AtomosphereName AtomosphereName => GetAtomosphereName(_atomosphere.Value);

		private int _exciteMin
		{
			get
			{
				if (_flags.IsOn(FlagEnum.Mock))
				{
					return 500;
				}
				return 0;
			}
		}

		private int _atomosphereSurplus
		{
			get
			{
				return _atomosphereSurplusVal;
			}
			set
			{
				_atomosphereSurplusVal = Mathf.Clamp(value, 0, 1000);
			}
		}

		private int _atomosphereMin => Mathf.Max(0, TemporaryAtomosphereMinimum);

		private int _atomosphereMax
		{
			get
			{
				if (SaveLoadManager.UnsavedData.PersistantStatus.Relationship == Relationship.LoveyDovey)
				{
					return 80000;
				}
				if (!SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Relation_2A))
				{
					return 20000;
				}
				if (!SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Relation_4A))
				{
					return 50000;
				}
				return 80000;
			}
		}

		private int _stimulusMin
		{
			get
			{
				if (_flags.IsOn(FlagEnum.Mock))
				{
					return 500;
				}
				return 0;
			}
		}

		public void SetAtomosphere(int value, bool forceUpdate = false)
		{
			int value2 = _atomosphere.Value;
			if (value > _atomosphere.Value)
			{
				_atomosphereTimer = 1000;
			}
			else if (!CanReduceAtomosphere)
			{
				return;
			}
			int num = value;
			if (value > _atomosphereMax)
			{
				_atomosphereSurplus = value - _atomosphereMax;
				num = _atomosphereMax;
			}
			else if (value < _atomosphereMin)
			{
				num = _atomosphereMin;
			}
			if (AtomosphereName != GetAtomosphereName(num) || forceUpdate)
			{
				OnAtomosphereChanged.OnNext(GetAtomosphereName(num));
			}
			_atomosphere.Value = num;
			if (!forceUpdate)
			{
				_atomosphereAdded += num - value2;
			}
		}

		public void AddAtomosphere(int value)
		{
			if (SaveLoadManager.UnsavedData.Days <= 5)
			{
				return;
			}
			int num = (int)((float)value * ((float)SaveLoadManager.UnsavedData.PersistantStatus.GetAtomosphereBonusByLevel() * 0.01f + 1f) * SaveLoadManager.GlobalData.GameOption.AtomosphereGaugeCorrection);
			if (_atomosphere.Value + num > _atomosphereMax)
			{
				_atomosphereSurplus = Mathf.Max(1000, num - (_atomosphereMax - _atomosphere.Value));
				SetAtomosphere(_atomosphereMax);
				return;
			}
			if (value < 0)
			{
				int num2 = Mathf.Min(-value, _atomosphereSurplus);
				_atomosphereSurplus -= num2;
				num = value + num2;
			}
			SetAtomosphere(_atomosphere.Value + num);
		}

		public float GetAtomosphereRate()
		{
			if (_atomosphere.Value == _atomosphereMax)
			{
				return 1f;
			}
			if (_atomosphere.Value < 20000)
			{
				return (float)_atomosphere.Value / 20000f;
			}
			if (_atomosphere.Value < 50000)
			{
				return (float)(_atomosphere.Value - 20000) / 30000f;
			}
			if (_atomosphere.Value < 80000)
			{
				return (float)(_atomosphere.Value - 50000) / 30000f;
			}
			return 1f;
		}

		public AtomosphereName GetAtomosphereName(int value)
		{
			int num = Mathf.Max(_atomosphereMin, value);
			if (num < 20000)
			{
				return AtomosphereName.Nervous;
			}
			if (num < 50000)
			{
				return AtomosphereName.Relief;
			}
			if (num < 80000)
			{
				return AtomosphereName.Excited;
			}
			return AtomosphereName.Rut;
		}

		public FeelingParams()
		{
			_flags = SaveLoadManager.UnsavedData.GlobalFlags;
			_atomosphereAdded = 0;
		}

		public void CountupTimer()
		{
			int num = Mathf.RoundToInt(Time.deltaTime * 1000f);
			_exciteTimer = Mathf.Max(0, _exciteTimer - num);
			_atomosphereTimer = Mathf.Max(0, _atomosphereTimer - num);
			_stimulusTimer = Mathf.Max(0, _stimulusTimer - num);
		}

		public async UniTask SetExciteWithTimer(int val, CancellationToken token)
		{
			int separated = val / 20;
			for (int i = 0; i < 20; i++)
			{
				Excite += separated;
				await UniTask.Yield(token);
			}
			Excite += val - 20 * separated;
		}

		public async UniTask SetAtomosphereWithTimer(int val, CancellationToken token)
		{
			int separated = val / 20;
			for (int i = 0; i < 20; i++)
			{
				AddAtomosphere(separated);
				await UniTask.Yield(token);
			}
			AddAtomosphere(val - 20 * separated);
		}

		public async UniTask SetStimulusWithTimer(int val, CancellationToken token)
		{
			int separated = val / 20;
			for (int i = 0; i < 20; i++)
			{
				Stimulus += separated;
				await UniTask.Yield(token);
			}
			Stimulus += val - 20 * separated;
		}

		public void ExtinctStimulus(int val)
		{
			IsStimulusExtinction = true;
			Stimulus -= val;
		}

		public void CopyValues(FeelingParams other)
		{
			_atomosphere.Value = other._atomosphere.Value;
			_excite.Value = other._excite.Value;
			_stimulus = other._stimulus;
		}

		public void ResetExcite()
		{
			_excite.Value = 0;
		}

		public void AddExciteValue(int value)
		{
			if (SaveLoadManager.UnsavedData.Days > 5)
			{
				float num = SaveLoadManager.GlobalData.GameOption.HeartGaugeCorrection;
				if (AtomosphereName == AtomosphereName.Relief)
				{
					num *= 2f;
				}
				else if (AtomosphereName == AtomosphereName.Excited)
				{
					num *= 3f;
				}
				else if (AtomosphereName == AtomosphereName.Rut)
				{
					num *= 4f;
				}
				Excite += (int)((float)value * num * ((float)SaveLoadManager.UnsavedData.PersistantStatus.GetExciteBonusByLevel() * 0.01f + 1f));
			}
		}
	}
}
