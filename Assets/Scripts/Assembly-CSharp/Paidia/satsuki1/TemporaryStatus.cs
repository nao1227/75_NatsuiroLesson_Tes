using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class TemporaryStatus
	{
		private AtomospherereValues _atomosphere;

		private FloatReactiveProperty _manExtacy = new FloatReactiveProperty(0f);

		public bool IsStimulusDecreasable = true;

		public ClothName Cloth;

		public int ExciteCount;

		public int EjaculateCount;

		[SerializeField]
		private int _manExtacyMax = 100;

		private bool _isManExtacyReducing;

		private CancellationTokenSource _manExtacyTokenSource;

		public float ManExtacyReduceByTime = 0.01f;

		public int ManExtacyReduceWaitTime = 1000;

		public FeelingParams Feelings { get; protected set; }

		public IReadOnlyReactiveProperty<float> ManExtacy => _manExtacy;

		public bool IsWomanExtacy => Feelings.Excite == 100000;

		public int ManExtacyMax => _manExtacyMax;

		public void CopyValues(TemporaryStatus other)
		{
			_manExtacy.Value = other.ManExtacy.Value;
			Cloth = other.Cloth;
			_atomosphere = other._atomosphere;
			Feelings.CopyValues(other.Feelings);
		}

		public AtomosphereName GetCurrentAtomosphere()
		{
			return Feelings.AtomosphereName;
		}

		public void AddExciteValue(int val)
		{
			BaseScene baseScene = UnityEngine.Object.FindObjectOfType<BaseScene>();
			if (!(null == baseScene))
			{
				Feelings.AddExciteValue(val);
			}
		}

		public void AddNervous(int val)
		{
			_atomosphere.Nervous += val;
		}

		public void AddExcited(int val)
		{
			_atomosphere.Excited += val;
		}

		public void AddRut(int val)
		{
			_atomosphere.Rut += val;
		}

		public void AddManExtacy(float val)
		{
			if (SaveLoadManager.UnsavedData.Days > 5)
			{
				val *= SaveLoadManager.GlobalData.GameOption.EjaculationGaugeCorrection;
				_isManExtacyReducing = false;
				_manExtacy.Value += Mathf.Min(val, (float)ManExtacyMax - _manExtacy.Value);
				_manExtacyTokenSource?.Cancel();
				_manExtacyTokenSource = new CancellationTokenSource();
				CountUntilManExtacyDown(_manExtacyTokenSource.Token).Forget();
			}
		}

		private async UniTask CountUntilManExtacyDown(CancellationToken token)
		{
			try
			{
				await UniTask.Delay(ManExtacyReduceWaitTime, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				ReduceManExtacy(token).Forget();
				_isManExtacyReducing = true;
			}
			catch (OperationCanceledException)
			{
			}
		}

		private async UniTask ReduceManExtacy(CancellationToken token)
		{
			if (_isManExtacyReducing)
			{
				return;
			}
			try
			{
				while (_manExtacy.Value > 0f)
				{
					_manExtacy.Value -= ManExtacyReduceByTime;
					await UniTask.Yield(token);
				}
				_manExtacy.Value = 0f;
			}
			catch (OperationCanceledException)
			{
			}
			_isManExtacyReducing = false;
		}

		public void ResetExtacyMan()
		{
			_manExtacy.Value = 0f;
		}

		public async UniTask ResetExtacyManGradually()
		{
			while (_manExtacy.Value > 0f)
			{
				_manExtacy.Value -= 2f;
				await UniTask.Yield();
			}
			_manExtacy.Value = 0f;
		}

		public void ResetExtacyWoman()
		{
			Feelings.ResetExcite();
		}

		public void Update(int exciteExtinction, int atomosphereExtinction, int stimulusExtinction)
		{
			Feelings.CountupTimer();
			AddExciteValue(-exciteExtinction);
			Feelings.AddAtomosphere(-atomosphereExtinction);
			if (IsStimulusDecreasable)
			{
				Feelings.ExtinctStimulus(stimulusExtinction);
			}
		}

		public void SetCloth(ClothName cloth)
		{
			Cloth = cloth;
		}

		public TemporaryStatus()
		{
			Feelings = new FeelingParams();
			_atomosphere = default(AtomospherereValues);
		}
	}
}
