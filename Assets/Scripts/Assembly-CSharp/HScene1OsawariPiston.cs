using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class HScene1OsawariPiston : OsawariPiston
{
	[Serializable]
	private class ImpactEventParameter
	{
		[SerializeField]
		private float VelocityUpper;

		[SerializeField]
		private float VelocityLower;

		[SerializeField]
		private string AnimationName;

		[SerializeField]
		private string SoundSetName;

		[SerializeField]
		private bool StopBleezing;

		[SerializeField]
		private float StopBleezingTime;

		[SerializeField]
		private float WomanExtacy;

		[SerializeField]
		private float ManExtacy;

		public ImpactEventParameter()
		{
			VelocityUpper = 0f;
			VelocityLower = 0f;
			AnimationName = "";
			SoundSetName = "";
			StopBleezing = false;
			StopBleezingTime = 0f;
			WomanExtacy = 0f;
			ManExtacy = 0f;
		}

		public float GetVelocityUpper()
		{
			return VelocityUpper;
		}

		public float GetVelocityLower()
		{
			return VelocityLower;
		}

		public string GetAnimationName()
		{
			return AnimationName;
		}

		public string GetSoundSetName()
		{
			return SoundSetName;
		}

		public bool GetStopBleezing()
		{
			return StopBleezing;
		}

		public float GetStopBleezingTime()
		{
			return StopBleezingTime;
		}

		public float GetWomanExtacy()
		{
			return WomanExtacy;
		}

		public float GetManExtacy()
		{
			return ManExtacy;
		}
	}

	[Serializable]
	private class EventParameter
	{
		[SerializeField]
		private float EventParameterUpper;

		[SerializeField]
		private float EventParameterLower;

		[SerializeField]
		private float VelocityUpper;

		[SerializeField]
		private float VelocityLower;

		[SerializeField]
		private string AnimationName;

		[SerializeField]
		private AnimeName AnimationID;

		[SerializeField]
		private bool FacialAnimation;

		[SerializeField]
		private string FaceAnimationName;

		[SerializeField]
		private string SoundSetName;

		[SerializeField]
		private bool StopBleezing;

		[SerializeField]
		private float StopBleezingTime;

		[SerializeField]
		private float WomanExtacy;

		[SerializeField]
		private float ManExtacy;

		public EventParameter()
		{
			EventParameterUpper = 0f;
			EventParameterLower = 0f;
			VelocityUpper = 0f;
			VelocityLower = 0f;
			AnimationName = "";
			FacialAnimation = false;
			FaceAnimationName = "";
			SoundSetName = "";
			StopBleezing = false;
			StopBleezingTime = 0f;
			WomanExtacy = 0f;
			ManExtacy = 0f;
		}

		public float GetEventParameterUpper()
		{
			return EventParameterUpper;
		}

		public float GetEventParameterLower()
		{
			return EventParameterLower;
		}

		public float GetVelocityUpper()
		{
			return VelocityUpper;
		}

		public float GetVelocityLower()
		{
			return VelocityLower;
		}

		public string GetAnimationName()
		{
			return AnimationName;
		}

		public AnimeName GetAnimationID()
		{
			return AnimationID;
		}

		public bool GetFacialAnimation()
		{
			return FacialAnimation;
		}

		public string GetFaceAnimationName()
		{
			return FaceAnimationName;
		}

		public string GetSoundSetName()
		{
			return SoundSetName;
		}

		public bool GetStopBleezing()
		{
			return StopBleezing;
		}

		public float GetStopBleezingTime()
		{
			return StopBleezingTime;
		}

		public float GetWomanExtacy()
		{
			return WomanExtacy;
		}

		public float GetManExtacy()
		{
			return ManExtacy;
		}
	}

	private OsawariPants _osawariPants;

	private OsawariCosplay _osawariCosplay;

	private OsawariKuri _osawariKuri;

	private OsawariAibu _osawariAibu;

	private OsawariGoods _goods;

	private float _EPS = 0.01f;

	private bool _isInsert;

	private bool _isEnter;

	private ThresholdParameterValue _piston;

	public float AutoPistonThreashold = 1f;

	public float AutoPistonSpeed = 0.1f;

	public float ShakePower;

	public float ShakeDecreaseRate;

	private ManPenisStatus _lastPenisStatus;

	private bool _isInCorrectedPiston;

	private float _autoPistonValue;

	[SerializeField]
	private float PistonRealmUpper = 2f;

	[SerializeField]
	private float PistonRealmLower;

	[SerializeField]
	private float PistonInsertRealmUpper;

	[SerializeField]
	private float PistonInsertRealmLower = -1f;

	[SerializeField]
	private float PistonInvisible = -2f;

	[SerializeField]
	private float InsertThresholdVelocity = 0.01f;

	[SerializeField]
	private EventParameter[] eventParameter;

	[SerializeField]
	private ImpactEventParameter[] impactEventParameter;

	private PenisFollower _penisFollower;

	protected override void InitializeParams()
	{
		base.InitializeParams();
		_insertController = GetComponent<InsertController>();
		_osawariPants = _manager.GetOsawariOf<OsawariPants>();
		_osawariAibu = _manager.GetOsawariOf<OsawariAibu>();
		_osawariKuri = _manager.GetOsawariOf<OsawariKuri>();
		_osawariCosplay = _manager.GetOsawariOf<OsawariCosplay>();
		_goods = GetComponent<OsawariGoods>();
		_faceController = GetComponent<FaceController>();
		_penisFollower = UnityEngine.Object.FindObjectOfType<PenisFollower>();
		float[] thresholds = new float[2] { PistonInsertRealmLower, PistonRealmLower };
		_piston = new ThresholdParameterValue(parameters[ParameterName.Piston], thresholds);
		_piston = _piston.MoveSectionTo(0);
		_lastPistonValue = _piston.Value;
		_impact = 0f;
		base.IsMoving = false;
		StateMachineObservables[] rx = animator.GetRx();
		for (int i = 0; i < rx.Length; i++)
		{
			rx[i].OnStateExitObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Impact).Subscribe(delegate
			{
				_allowImpactEvent = true;
			}).AddTo(this);
		}
	}

	protected override void OnLateUpdate()
	{
		if (IsAnimating)
		{
			InactivateLive2D(ParameterName.Piston);
			_manager.CsManager.OsawariCrossSection.SyncroPiston(parameters[ParameterName.Piston].Value);
		}
		else
		{
			SetLive2D(ParameterName.Piston, _piston.Value);
			_manager.CsManager.OsawariCrossSection.SyncroPiston(_piston.Value);
		}
		_averageSpd.Value = _averageCalculator.AbsAverage;
	}

	public override bool IsInsert()
	{
		return _isInsert;
	}

	public override bool IsEnter()
	{
		return _isEnter;
	}

	public bool IsAbleToTakeOffPants()
	{
		bool result = false;
		if (!_isInsert && _piston.Value < PistonInsertRealmLower / 2f)
		{
			result = true;
		}
		else if (!_isInsert && _piston.Value > PistonInsertRealmLower / 2f)
		{
			result = false;
		}
		return result;
	}

	public override async UniTask Eject()
	{
		while (_piston.Value > PistonRealmLower)
		{
			_piston -= 0.05f;
			_lastPistonValue = _piston.Value;
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
		}
		_piston = _piston.Update(PistonRealmLower);
		_manager.CsManager.OsawariCrossSection.EjectPenis();
		await EjectAnimation();
		await UniTask.Yield();
		_piston = _piston.Update(_manager.Model.Parameters[GetParameterNumber(ParameterName.Piston)].Value);
		_lastPistonValue = _piston.Value;
	}

	public override bool IsAbleToInsert()
	{
		OsawariPants osawariPants = _osawariPants;
		if ((object)osawariPants == null || !osawariPants.IsAbleToInsert())
		{
			return _osawariCosplay?.IsAbleToInsert() ?? false;
		}
		return true;
	}

	public override bool CanManEnter()
	{
		if (!IsAnimating)
		{
			if (!IsAbleToInsert())
			{
				return _isInsert;
			}
			return true;
		}
		return false;
	}

	public override async UniTask ManEnter()
	{
		if (IsAnimating)
		{
			return;
		}
		if (_manager.ContextManager.Context == OsawariContext.Osawari)
		{
			if (IsAbleToInsert() && Mathf.Abs(_piston.Value - PistonInvisible) < _EPS)
			{
				_osawariAibu.CancellAnimaion();
				_osawariKuri.CancellAnimaion();
				_penisFollower.SetPistonValue(-1f);
				await StartAnimation(AnimeName.Enter, on: true, singleTime: true);
				_piston = _piston.MoveSectionTo(1);
				_lastPistonValue = _piston.Value;
				_isEnter = true;
				_manPenis.Value = ManPenisStatus.Enter;
				_goods.SetCondomEnabled(enabled: true);
			}
		}
		else
		{
			IsAnimating = true;
			_manager.OsawariFellatio.EnterPenis(delegate
			{
				IsAnimating = false;
				_manPenis.Value = ManPenisStatus.Enter;
			});
		}
		_onPenisShown.OnNext(value: true);
	}

	public override async UniTask<bool> OnNextEnter()
	{
		if (IsAnimating)
		{
			return false;
		}
		if (_manager.ContextManager.Context == OsawariContext.Osawari)
		{
			if (!_isInsert && _isEnter)
			{
				await StartAnimation(AnimeName.Exit, on: true, singleTime: true);
				_piston = _piston.MoveSectionTo(0);
				_lastPistonValue = _piston.Value;
				_isEnter = false;
				_manPenis.Value = ManPenisStatus.Unshown;
				_goods.SetCondomEnabled(enabled: false);
				_penisFollower.SetPistonValue(_piston.Value);
			}
		}
		else
		{
			IsAnimating = true;
			_manager.OsawariFellatio.ExitPenis(delegate
			{
				IsAnimating = false;
				_manPenis.Value = ManPenisStatus.Unshown;
			});
		}
		_onPenisShown.OnNext(value: false);
		return true;
	}

	private IEnumerator _StateListener()
	{
		yield return null;
	}

	public void StateListener()
	{
		StartCoroutine(_StateListener());
	}

	protected override void AutoAnimation()
	{
		base.AutoAnimation();
		if (IsAuto && !IsAnimating)
		{
			float easedValue = GetEasedValue(_lastTimeAuto);
			UpdateAutoTimeCount();
			float y = (GetEasedValue(_autoTime) - easedValue) * SensitivityY;
			Vector3 move = new Vector3(0f, y, 0f) * ((!_isPistonAutoReturning) ? 1 : (-1));
			UpdateParamsCore(move);
			if (_piston.IsOnThresholdMax() || _piston.IsOnThresholdMin() || _autoTime == AutoPeriod)
			{
				ResetAutoTimeCount();
				_isPistonAutoReturning = !_isPistonAutoReturning;
			}
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return 0;
	}

	private async UniTask CorrectedPiston(float startValue)
	{
		if (_isInCorrectedPiston)
		{
			return;
		}
		_autoPistonValue = startValue;
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		float spd = 0f;
		while (_autoPistonValue < PistonRealmUpper)
		{
			_autoPistonValue += spd;
			spd += AutoPistonSpeed / 30f;
			if (spd > AutoPistonSpeed)
			{
				spd = AutoPistonSpeed;
			}
			await UniTask.Yield(token);
		}
		while (_autoPistonValue > PistonRealmLower)
		{
			_autoPistonValue -= AutoPistonSpeed;
			await UniTask.Yield(token);
		}
		await UniTask.Delay(10);
		_isInCorrectedPiston = false;
	}

	protected override async void UpdateParamsCore(Vector3 move)
	{
		if (IsAnimating)
		{
			return;
		}
		float magnitude = (_manager.GetCurrentMousePosition() - _mousePositionOnLastFrame).magnitude;
		if (_averageCalculator.AbsAverage > AutoPistonThreashold && magnitude > 0f && !_isInCorrectedPiston)
		{
			CorrectedPiston(_piston.Value).Forget();
			_isInCorrectedPiston = true;
		}
		else
		{
			_piston += move.y / SensitivityY;
		}
		if (_isInsert)
		{
			CheckIsInPiston(move);
			if (_isInCorrectedPiston)
			{
				_piston = _piston.Update(_autoPistonValue);
			}
			if (_piston.Value >= PistonRealmUpper)
			{
				float time = Time.time;
				if (time > _pastImpactTime)
				{
					float impact = move.y / SensitivityY;
					_impact = impact;
					_impactFlag = true;
					_pastImpactTime = time;
					if (_isInCorrectedPiston)
					{
						_manager.cameraManager.Shake(ShakePower, ShakeDecreaseRate);
					}
				}
				_piston = _piston.Update(PistonRealmUpper);
			}
			_averageCalculator.Add(move.y / SensitivityY);
			if (_impactFlag && _allowImpactEvent)
			{
				foreach (IImpactTrigger item in _manager.ActionManager.GetActionOf<IImpactTrigger>())
				{
					item.OnImpact(_impact);
				}
				foreach (ImpactEvent item2 in ImpactEvents.Where((ImpactEvent x) => x.IsFullfillCondition(_manager.TemporaryStatus, _conditions) && x.IsInRange(_piston.Value)))
				{
					_allowImpactEvent = false;
					item2.InvokeEvent(_manager.TemporaryStatus, _conditions);
				}
				_impactFlag = false;
			}
			if (_isInsert)
			{
				_insertController.AddManExtacy(EXTACY_MAN * move.magnitude);
			}
		}
		else if (_piston.Value - _lastPistonValue > InsertThresholdVelocity && Mathf.Abs(_piston.Value - PistonRealmLower) <= 0.1f && IsAbleToInsert())
		{
			_isInsert = true;
			_manager.CsManager.OsawariCrossSection.ResetPenisStatus();
			InsertEvent?.InvokeEvent(_manager.TemporaryStatus, _conditions);
			_onInsert.OnNext(value: true);
			await StartAnimation(AnimeName.Insert, on: true, singleTime: true);
			_piston = _piston.MoveSectionTo(2);
			_lastPistonValue = _piston.Value;
			_faceController.SetSight(Bool: false);
			_manPenis.Value = ManPenisStatus.Insert;
		}
		if (_isInsert)
		{
			Enforce(_piston, move);
		}
		_lastPistonValue = _piston.Value;
		_penisFollower.SetPistonValue(_piston.Value);
	}

	private async UniTask EjectAnimation()
	{
		await SetAnimationStatusOff(AnimeName.Insert);
		await StartAnimation(AnimeName.Eject, on: true, singleTime: true);
		_onInsert.OnNext(value: false);
		_piston = _piston.MoveSectionTo(1);
		_lastPistonValue = _piston.Value;
		_faceController.FacialAnimation(StringsManager.GetAnimeName(AnimeName.Eject));
		_isInsert = false;
		_manPenis.Value = ManPenisStatus.Enter;
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAuto)
		{
			_averageCalculator.Add(0f);
			_allowImpactEvent = true;
			if (_averageSpd.Value == 0f && IsInPiston)
			{
				IsInPiston = false;
				_onPiston.OnNext(value: false);
				_movedDistanceWhilePiston = 0f;
			}
		}
	}

	protected override void UpdateFeelingParams()
	{
		if (_isInsert)
		{
			base.UpdateFeelingParams();
		}
	}

	protected override bool GetConstraintsCore()
	{
		if (!_isEnter)
		{
			return _isInsert;
		}
		return true;
	}

	public override void OnClick(CubismDrawable targetMesh, bool isFirst)
	{
		base.IsMoving = true;
		base.OnClick(targetMesh, isFirst);
	}

	public override void OnMouseUp(bool fromCancel = false)
	{
		base.OnMouseUp(fromCancel);
		base.IsMoving = false;
	}

	public override void SwitchContext()
	{
		if (_manager.ContextManager.Context == OsawariContext.Fellatio)
		{
			_lastPenisStatus = _manPenis.Value;
			_manPenis.Value = _manager.OsawariFellatio.PenisStatus;
			return;
		}
		_manPenis.Value = _lastPenisStatus;
		if (!SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.AllowCondomPutOff))
		{
			_goods.SetCondomEnabled(!_goods.IsCondomSet);
		}
		else
		{
			_goods.SetCondomEnabled(_manPenis.Value != ManPenisStatus.Unshown);
		}
	}

	protected override bool GetRestrictedCore()
	{
		if (!_goods.IsCondomSet)
		{
			return !SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.AllowCondomPutOff);
		}
		return false;
	}
}
