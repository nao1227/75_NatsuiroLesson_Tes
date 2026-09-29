using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariFellatio : MonoBehaviour, IWearable
	{
		public CubismModel Model;

		public Live2DAnimator Animator;

		public float AutoFellatioSpeed;

		public bool IsAnimating;

		private StateMachineObservables[] _observable;

		protected FPSChecker fpsChecker;

		protected Dictionary<ParameterName, CubismParameter> parameters;

		[SerializeField]
		protected ParameterDictionary ParameterNumbers;

		public List<SpermParameterGroup> SpermGroups;

		[NonSerialized]
		public ManPenisStatus PenisStatus;

		public Subject<bool> OnWaitingForDrinkOrder;

		private IntReactiveProperty _spermInMouth;

		public FellatioMode Mode;

		public int ExciteIncrement;

		public int ExciteIncrementFast;

		public int AtomosphereIncrement;

		public int AtomosphereIncrementFast;

		private bool _moving;

		private bool _moveFast;

		private bool _drinkRestricted;

		private bool _isFellatioMode;

		private ParameterValue _fellatio;

		private ParameterValue _manHand;

		private ParameterValue _penis;

		private ParameterValue _irama;

		private ParameterValue _bunny;

		private ParameterValue _wetSuit;

		private ParameterValue _sm;

		private ParameterValue _smNipples;

		private ParameterValue _smBindfold;

		private ParameterValue _cat;

		private ParameterValue _shirt;

		private ParameterValue _skirt;

		private ParameterValue _bra;

		private ParameterValue _pants;

		private BoolParameterValue _rotorBreast;

		private BoolParameterValue _rotorKuri;

		private ParameterValue _rotorVibration;

		private float _lastFellatioValue;

		private OsawariManager _manager;

		private InsertController _insertController;

		public OsawariGoods OsawariGoods;

		private OsawariHeadFellatio _head;

		private Subject<bool> _onFellatioMoveEnabled;

		private Subject<bool> _onFellatio;

		private Subject<bool> _onSuck;

		private Subject<bool> _onMoveFast;

		private Subject<bool> _onMouthOpen;

		private Subject<bool> _onRestrictDrink;

		private Subject<Unit> _onEjaculation;

		private Subject<bool> _onFellatioMove;

		private Subject<FellatioMode> _onModeChange;

		private Subject<bool> _onEnableManPenis = new Subject<bool>();

		public int WaitMillSecAfterFirst;

		public int WaitMillSEcAfterSecond;

		public PhysicsCalculater PhysicsCalculater;

		public float EnforcementFactor;

		public float PullFactor;

		private bool _isAnimating;

		public PenisFollowerFellatio _follower;

		private OsawariMasturbate _masturbate;

		public FaceAnimationControllerOnOsawariScene1 FaceAnimationController;

		private bool _isRestrictDrink;

		protected float fps => fpsChecker.GetFps();

		public IReadOnlyReactiveProperty<int> SpermInMouth => _spermInMouth;

		public float BraValue => _bra.Value;

		public IObservable<bool> OnFellatio => _onFellatio;

		public IObservable<bool> OnSuck => _onSuck;

		public IObservable<bool> OnMoveFast => _onMoveFast;

		public IObservable<bool> OnMouthOpen => _onMouthOpen;

		public IObservable<bool> OnRestrictDrink => _onRestrictDrink;

		public IObservable<Unit> OnEjaculation => _onEjaculation;

		public IObservable<bool> OnFellatioMove => _onFellatioMove;

		public IObservable<bool> OnFellatioMoveEnabled => _onFellatioMoveEnabled;

		public IObservable<bool> OnEnableManPenis => _onEnableManPenis;

		public IObservable<FellatioMode> OnModeChange => _onModeChange;

		public bool IsRestrictButton => _isAnimating;

		public bool IsInFellatio { get; private set; }

		public bool IsSucking { get; private set; }

		public void ManagedStart(OsawariManager manager)
		{
			fpsChecker = GameObject.Find("UI_fps").GetComponent<FPSChecker>();
			Mode = FellatioMode.Auto;
			parameters = new Dictionary<ParameterName, CubismParameter>();
			_spermInMouth = new IntReactiveProperty();
			Animator.ManagedStart();
			_observable = Animator.GetRx();
			OnWaitingForDrinkOrder = new Subject<bool>();
			foreach (KeyValuePair<ParameterName, int> item in ParameterNumbers.GetTable())
			{
				parameters[item.Key] = Model.Parameters[item.Value];
			}
			_fellatio = new ParameterValue(parameters[ParameterName.Fellatio]);
			_penis = new ParameterValue(parameters[ParameterName.Penis]);
			_manHand = new ParameterValue(parameters[ParameterName.FellatioHandRight]);
			_irama = new ParameterValue(parameters[ParameterName.IramaFlag]);
			_bunny = new ParameterValue(parameters[ParameterName.BunnyFlag]);
			_wetSuit = new ParameterValue(parameters[ParameterName.WetSuitUpperFlag]);
			_sm = new ParameterValue(parameters[ParameterName.SMFlag]);
			_smNipples = new ParameterValue(parameters[ParameterName.SM_Nipples]);
			_smBindfold = new ParameterValue(parameters[ParameterName.SM_Bindfold]);
			_cat = new ParameterValue(parameters[ParameterName.CatFlag]);
			_shirt = new ParameterValue(parameters[ParameterName.ShirtFlag]);
			_shirt = _shirt.Update(1f);
			_skirt = new ParameterValue(parameters[ParameterName.SkirtFlag]);
			_skirt = _skirt.Update(1f);
			_bra = new ParameterValue(parameters[ParameterName.Bra]);
			_bra = _bra.Update(1f);
			_pants = new ParameterValue(parameters[ParameterName.PantsFlag]);
			_pants = _pants.Update(1f);
			_rotorBreast = new BoolParameterValue();
			_rotorKuri = new BoolParameterValue();
			_rotorVibration = new ParameterValue(parameters[ParameterName.RotorVibration]);
			_onFellatio = new Subject<bool>();
			_onSuck = new Subject<bool>();
			_onMoveFast = new Subject<bool>();
			_onMouthOpen = new Subject<bool>();
			_onRestrictDrink = new Subject<bool>();
			_onEjaculation = new Subject<Unit>();
			_onFellatioMove = new Subject<bool>();
			_onModeChange = new Subject<FellatioMode>();
			_onFellatioMoveEnabled = new Subject<bool>();
			_manager = manager;
			_head = _manager.GetOsawariOf<OsawariHeadFellatio>();
			_masturbate = _manager.GetOsawariOf<OsawariMasturbate>();
			_insertController = manager.GetComponent<InsertController>();
			foreach (SpermParameterGroup spermGroup in SpermGroups)
			{
				spermGroup.Initialize();
			}
			_spermInMouth.Subscribe(delegate
			{
				Animator.SetInteger("SpermInMouth", _spermInMouth.Value);
			}).AddTo(this);
			_insertController.OnEjaculate.Where((Unit _) => _manager.ContextManager.Context == OsawariContext.Fellatio).Subscribe(delegate
			{
				if (_isRestrictDrink)
				{
					OpenMouth(open: false);
				}
				if (Animator.GetInteger("Mode") == 1)
				{
					_spermInMouth.Value++;
				}
				Animator.SetInteger("SpermGroup1", SpermGroups[0].GetRandomSpermparameter());
				Animator.SetInteger("SpermGroup2", SpermGroups[1].GetRandomSpermparameter());
				Animator.SetInteger("SpermGroup3", SpermGroups[2].GetRandomSpermparameter());
				_isAnimating = true;
				Animator.SetTrigger("TriggerEjaculate");
				TriggerWaitBetweenEjaculation().Forget();
				if (Animator.GetInteger("Mode") == 1)
				{
					StartFellatioMove(active: false);
				}
				else if (Animator.GetInteger("Mode") == 2)
				{
					StartSuck(active: true);
				}
				_onEjaculation.OnNext(Unit.Default);
			}).AddTo(this);
			for (int num = 0; num < _observable.Length; num++)
			{
				_observable[num].OnStateEnterObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
					if (x.Item1 == StateID.FellatioIdle)
					{
						_isFellatioMode = true;
					}
					if (x.Item1 == StateID.WaitForDrinkSperm)
					{
						OnWaitingForDrinkOrder.OnNext(value: true);
					}
					if (x.Item1 == StateID.FreeMouth)
					{
						_isFellatioMode = false;
					}
					if (x.Item1 == StateID.Ejaculation)
					{
						_isAnimating = true;
					}
					if (x.Item1 == StateID.FellaStart || x.Item1 == StateID.FellaEnd || x.Item1 == StateID.SuckEnd)
					{
						_isAnimating = true;
					}
					if (x.Item1 == StateID.SuckStart)
					{
						InactivateLive2D(ParameterName.SuckFlag);
						_isAnimating = true;
					}
					if (x.Item1 == StateID.OpenDrink)
					{
						_spermInMouth.Value--;
						if (_spermInMouth.Value == 0)
						{
							OnWaitingForDrinkOrder.OnNext(value: false);
						}
					}
					if (x.Item1 == StateID.DrinkSperm)
					{
						_spermInMouth.Value--;
					}
				}).AddTo(this);
				_observable[num].OnStateExitObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
					if (x.Item1 == StateID.WaitForDrinkSperm)
					{
						OnWaitingForDrinkOrder.OnNext(value: false);
					}
					if (x.Item1 == StateID.SpermGroup1)
					{
						SpermGroups[0].FinishEjaculate();
					}
					if (x.Item1 == StateID.SpermGroup2)
					{
						SpermGroups[1].FinishEjaculate();
					}
					if (x.Item1 == StateID.SpermGroup3)
					{
						SpermGroups[2].FinishEjaculate();
						_isAnimating = false;
					}
					if (x.Item1 == StateID.Ejaculation)
					{
						_isAnimating = false;
					}
					if (x.Item1 == StateID.FellaEnd)
					{
						_isAnimating = false;
					}
					if (x.Item1 == StateID.FellaStart || x.Item1 == StateID.SuckStart)
					{
						_isAnimating = false;
					}
					if (x.Item1 == StateID.SuckEnd)
					{
						_follower.SetSucking(val: false);
						_isAnimating = false;
						SetLive2D(ParameterName.SuckFlag, 0f);
					}
				}).AddTo(this);
			}
			PenisStatus = ManPenisStatus.Unshown;
			OpenMouth(open: true);
		}

		private async UniTask TriggerWaitBetweenEjaculation()
		{
			_ = 1;
			try
			{
				await UniTask.Delay(WaitMillSecAfterFirst, ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				Animator.SetTrigger("WaitAfterFirstEjaculation");
				await UniTask.Delay(WaitMillSEcAfterSecond, ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				Animator.SetTrigger("WaitAfterSecondEjaculation");
			}
			catch (OperationCanceledException)
			{
			}
		}

		public void ManagedUpdate()
		{
			if (_moving && _isFellatioMode && Mode == FellatioMode.Auto)
			{
				_insertController.AddManExtacy(3f * Time.deltaTime * (float)((!_moveFast) ? 1 : 2));
				_manager.TemporaryStatus.AddExciteValue(_moveFast ? ExciteIncrementFast : ExciteIncrement);
				_manager.TemporaryStatus.Feelings.AddAtomosphere(_moveFast ? AtomosphereIncrementFast : AtomosphereIncrement);
				EnforceByFellatio();
			}
			_lastFellatioValue = GetFellatioValue();
		}

		private float GetFellatioValue()
		{
			return Model.Parameters[ParameterNumbers.GetTable()[ParameterName.Fellatio]].Value;
		}

		public void ManagedLateUpdate()
		{
			if ((Mode == FellatioMode.Auto && _moving) || _isAnimating)
			{
				_fellatio = _fellatio.Update(parameters[ParameterName.Fellatio].Value);
				InactivateLive2D(ParameterName.Fellatio);
			}
			else
			{
				SetLive2D(ParameterName.Fellatio, _fellatio.Value);
			}
			SetLive2D(ParameterName.Penis, _penis.Value);
			SetLive2D(ParameterName.IramaFlag, _irama.Value);
			SetLive2D(ParameterName.FellatioHandLeft, _manHand.Value);
			SetLive2D(ParameterName.FellatioHandRight, _manHand.Value);
			SetLive2D(ParameterName.BunnyFlag, _bunny.Value);
			SetLive2D(ParameterName.WetSuitUpperFlag, _wetSuit.Value);
			SetLive2D(ParameterName.CatFlag, _cat.Value);
			SetLive2D(ParameterName.SMFlag, _sm.Value);
			SetLive2D(ParameterName.SM_Nipples, _smNipples.Value);
			SetLive2D(ParameterName.SM_Bindfold, _smBindfold.Value);
			if (_manager.TemporaryStatus.Cloth == ClothName.Normal)
			{
				SetLive2D(ParameterName.ShirtFlag, _shirt.Value);
				SetLive2D(ParameterName.SkirtFlag, _skirt.Value);
				SetLive2D(ParameterName.Bra, _bra.Value);
				SetLive2D(ParameterName.PantsFlag, _pants.Value);
			}
			else
			{
				SetLive2D(ParameterName.ShirtFlag, 0f);
				SetLive2D(ParameterName.SkirtFlag, 0f);
				SetLive2D(ParameterName.Bra, 0f);
				SetLive2D(ParameterName.PantsFlag, 0f);
			}
			SetLive2D(ParameterName.RotorBreast, _rotorBreast.Value);
			SetLive2D(ParameterName.RotorKuri, _rotorKuri.Value);
			SetLive2D(ParameterName.RotorVibration, _rotorVibration.Value);
			SetLive2D(ParameterName.SpermInMouth, Math.Min(_spermInMouth.Value, 2));
			foreach (SpermParameterGroup spermGroup in SpermGroups)
			{
				foreach (int item in spermGroup)
				{
					Model.Parameters[item].BlendToValue(CubismParameterBlendMode.Override, 1f);
				}
			}
		}

		private void SetLive2D(ParameterName name, float val)
		{
			_manager.Preserver.SetValue(ParameterNumbers.GetTable()[name], val, CubismParameterBlendMode.Override, 1);
		}

		private void InactivateLive2D(ParameterName name)
		{
			_manager.Preserver.InactivateValue(ParameterNumbers.GetTable()[name], 1);
		}

		public async void StartFellatio(bool active)
		{
			_head.Cancel();
			_onFellatioMoveEnabled.OnNext(active);
			if (_spermInMouth.Value == 0)
			{
				OpenMouth(open: true);
			}
			if (active)
			{
				_masturbate.Cancel();
				_isAnimating = true;
				Animator.SetInteger("Mode", 1);
				IsInFellatio = true;
			}
			else
			{
				IsInFellatio = false;
				if (Mode == FellatioMode.PlayerControl)
				{
					await StartPlayerControlMode(active: false);
				}
				Animator.SetTrigger("TriggerFreeMouth");
				Animator.SetInteger("Mode", 0);
				_moving = false;
				StartFellatioMove(active: false);
			}
			_onEnableManPenis.OnNext(!active);
			_onFellatio.OnNext(active);
		}

		public void StartFellatioMove(bool active)
		{
			_head.Cancel();
			_moving = active;
			Animator.SetBool("FellatioMove", active);
			_onFellatioMove.OnNext(active);
		}

		public void StartSuck(bool active)
		{
			IsSucking = active;
			_head.Cancel();
			if (active)
			{
				_follower.SetSucking(val: true);
				_masturbate.Cancel();
				Animator.SetInteger("Mode", 2);
				_moving = true;
			}
			else
			{
				Animator.SetTrigger("TriggerFreeMouth");
				Animator.SetInteger("Mode", 0);
				_moving = false;
			}
			_onEnableManPenis.OnNext(!active);
			_onSuck.OnNext(active);
		}

		public void SwitchSpeed()
		{
			_moveFast = !_moveFast;
			Animator.SetBool("MoveFast", _moveFast);
			_onMoveFast.OnNext(_moveFast);
		}

		public async UniTask StartPlayerControlMode(bool active)
		{
			_head.Cancel();
			Mode = (active ? FellatioMode.PlayerControl : FellatioMode.Auto);
			if (active)
			{
				StartFellatioMove(active: false);
			}
			else
			{
				_manager.GetOsawariOf<OsawariIrrumatio>().Cancel();
			}
			_onFellatioMoveEnabled.OnNext(!active);
			Animator.SetBool("PlayerControl", active);
			_moving = !active;
			float num = 1f;
			float to = 0f;
			if (active)
			{
				num = 0f;
				to = 1f;
			}
			_onModeChange.OnNext(Mode);
			bool animDone = false;
			DOVirtual.Float(num, to, 0.8f, delegate(float value)
			{
				_manHand = _manHand.Update(value);
				_irama = _irama.Update(value);
			}).OnComplete(delegate
			{
				animDone = true;
			}).Play();
			await UniTask.WaitUntil(() => animDone);
		}

		public void OpenMouth(bool open)
		{
			_head.Cancel();
			Animator.SetBool("MouthOpen", open);
			_onMouthOpen.OnNext(open);
		}

		public void RestrictDrink(bool restrict)
		{
			_head.Cancel();
			Animator.SetBool("DrinkRestricted", restrict);
			_isRestrictDrink = restrict;
			_onRestrictDrink.OnNext(restrict);
		}

		public void EnterPenis(Action onComplete = null)
		{
			DOVirtual.Float(0f, 1f, 0.8f, delegate(float value)
			{
				_penis = _penis.Update(value);
				_follower.SetEnabled(val: true);
			}).OnComplete(delegate
			{
				PenisStatus = ManPenisStatus.Enter;
				onComplete?.Invoke();
			}).Play();
		}

		public void ExitPenis(Action onComplete = null)
		{
			DOVirtual.Float(1f, 0f, 0.8f, delegate(float value)
			{
				_penis = _penis.Update(value);
			}).OnComplete(delegate
			{
				PenisStatus = ManPenisStatus.Unshown;
				_follower.SetEnabled(val: false);
				onComplete?.Invoke();
			}).Play();
		}

		public void SetFellatioValue(float val)
		{
			float value = _fellatio.Value;
			_fellatio = _fellatio.Update(val);
			if (Mathf.Abs(val - value) > 0.05f)
			{
				_insertController.AddManExtacy(3f * Time.deltaTime * (float)((!_moveFast) ? 1 : 2));
			}
			EnforceByFellatio();
		}

		public void Move(bool move)
		{
			_onFellatioMove.OnNext(move);
		}

		public void SetCosplay(float bunny = 0f, float wetSuit = 0f, float sm = 0f, float cat = 0f, float smNipples = 0f, float smBindFold = 0f)
		{
			if (parameters != null)
			{
				_bunny = _bunny.Update(bunny);
				_wetSuit = _wetSuit.Update(wetSuit);
				_sm = _sm.Update(sm);
				_smNipples = _smNipples.Update(smNipples);
				_smBindfold = _smBindfold.Update(smBindFold);
				_cat = _cat.Update(cat);
			}
		}

		public void SetCloth(float shirt = -1f, float skirt = -1f, float bra = -1f, float pants = -1f)
		{
			if (parameters == null)
			{
				return;
			}
			if (shirt >= 0f)
			{
				_shirt = _shirt.Update(shirt);
			}
			if (skirt >= 0f)
			{
				_skirt = _skirt.Update(skirt);
			}
			if (bra >= 0f)
			{
				_bra = _bra.Update(bra);
				if (_manager.ContextManager.Context != OsawariContext.Fellatio)
				{
				}
			}
			else
			{
				_ = _manager.ContextManager.Context;
				_ = 1;
			}
			if (pants >= 0f)
			{
				_pants = _pants.Update(pants);
			}
		}

		public void SwitchCloth(bool shirt = false, bool skirt = false, bool bra = false, bool pants = false)
		{
			if (shirt)
			{
				_shirt = _shirt.Update(1f - _shirt.Value);
				if (_shirt.Value == 1f)
				{
					_bra = _bra.Update(1f);
				}
			}
			if (skirt)
			{
				_skirt = _skirt.Update(1f - _skirt.Value);
			}
			if (bra)
			{
				if (_shirt.Value == 1f && _bra.Value == 1f)
				{
					return;
				}
				_bra = _bra.Update(1f - _bra.Value);
			}
			if (pants)
			{
				_pants = _pants.Update(1f - _pants.Value);
			}
		}

		public void SetRotorOrbit(float val)
		{
			_rotorVibration = _rotorVibration.Update(val);
		}

		public void SetRotor(bool breast, bool kuri)
		{
			_rotorBreast = _rotorBreast.Update(breast);
			_rotorKuri = _rotorKuri.Update(kuri);
		}

		public void SetShirtButton(int buttonCount)
		{
			if (buttonCount == 0)
			{
				_shirt = _shirt.Update(1f);
			}
			else if (buttonCount < 5)
			{
				_shirt = _shirt.Update(2f);
			}
			else
			{
				_shirt = _shirt.Update(3f);
			}
		}

		private void EnforceByFellatio()
		{
			float num = (GetFellatioValue() - _lastFellatioValue) / 30f * 60f / fps / 30f * 10000f;
			PhysicsCalculater.Enforce(num * EnforcementFactor, GetFellatioValue() < _lastFellatioValue, PullFactor);
		}

		public void SwitchContext()
		{
			_onEnableManPenis.OnNext(!IsSucking && !IsInFellatio);
		}

		public void LeaveContext()
		{
		}

		public bool IsWearing()
		{
			bool result = false;
			if (_manager.TemporaryStatus.Cloth == ClothName.Normal)
			{
				if (_shirt.Value > 0f || _skirt.Value > 0f || _bra.Value > 0f || _pants.Value > 0f)
				{
					result = true;
				}
			}
			else if (_bunny.Value > 0f || _wetSuit.Value > 0f || _sm.Value > 0f || _cat.Value > 0f)
			{
				result = true;
			}
			return result;
		}
	}
}
