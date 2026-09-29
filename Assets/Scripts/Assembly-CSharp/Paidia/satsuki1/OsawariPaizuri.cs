using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Live2D.Cubism.Core;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariPaizuri : AbstractOsawariAnotherModel
	{
		private ParameterValue _paizuri;

		private ParameterValue _paizuriFlag;

		private ParameterValue _piston;

		private OsawariHeadPaizuri _head;

		private Tweener _enterAnim;

		private Tweener _exitAnim;

		private List<OsawariBreastPaizuri> _breasts;

		private List<OsawariNipplePaizuri> _nipples;

		private OsawariBreastWhilePaizuri _breastWhilePaizuri;

		private OsawariAibuPaizuri _aibu;

		private bool _isMovingFromAnother;

		private bool _special;

		public PhysicsCalculater PhysicsCalculater;

		public InsertController InsertController;

		public List<SpermParameterGroup> SpermGroups;

		private bool _isAnimating;

		public float PullFactor;

		public float EnforcementFactor;

		private float _lastPistonValue;

		private float _deltaT = 30f;

		public float EXTACY_MAN = 0.1f;

		public Live2DAnimator Animator;

		public int WaitMillSecAfterFirst = 1000;

		public int WaitMillSecAfterSecond = 1000;

		private bool _isAutoAnimationReturning;

		public float AutoSpeed = 1f;

		public PenisFollowerHScene4Under UnderPenisFollower;

		public PenisFollowerHScene4Upper UpperPenisFollower;

		private Subject<float> _onPaizuriChanged = new Subject<float>();

		protected bool _isPistonAutoReturning;

		public float AutoPistonPeriod = 3f;

		public bool IsInPaizuriMode => _paizuriFlag.Value == 1f;

		public IObservable<float> OnPaizuriChanged => _onPaizuriChanged;

		public override bool IsAnimating
		{
			get
			{
				return _isAnimating;
			}
			protected set
			{
				_isAnimating = value;
			}
		}

		protected override void AutoAnimation()
		{
			if (InsertController.IsManExtacy)
			{
				IsAuto = false;
			}
			if (IsAuto && !IsAnimating)
			{
				float easedValue = GetEasedValue(_lastTimeAuto);
				UpdateAutoTimeCount();
				float y = (GetEasedValue(_autoTime) - easedValue) * SensitivityY;
				Vector3 move = new Vector3(0f, y, 0f) * ((!_isPistonAutoReturning) ? 1 : (-1));
				UpdateParamsCore(move);
				if (_piston.IsMax() || _piston.IsMin() || _autoTime == AutoPistonPeriod)
				{
					ResetAutoTimeCount();
					_isPistonAutoReturning = !_isPistonAutoReturning;
				}
			}
		}

		private void SetAnimation(Action action = null)
		{
			_enterAnim = DOVirtual.Float(0f, 1f, 0.5f, delegate(float x)
			{
				_paizuri = _paizuri.Update(x);
				_paizuriFlag = _paizuriFlag.Update(x);
				_onPaizuriChanged.OnNext(x);
			}).OnComplete(delegate
			{
				action?.Invoke();
				IsAnimating = false;
			});
		}

		private void SetExitAnimation(Action action = null)
		{
			_exitAnim = DOVirtual.Float(1f, 0f, 0.5f, delegate(float x)
			{
				_paizuri = _paizuri.Update(x);
				_paizuriFlag = _paizuriFlag.Update(x);
				_onPaizuriChanged.OnNext(x);
			}).OnComplete(delegate
			{
				action?.Invoke();
				IsAnimating = false;
				UnderPenisFollower.SetEnabled(enabled: false);
				UpperPenisFollower.SetEnabled(enabled: false);
			});
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return 0;
		}

		protected override void InitializeParams()
		{
			Animator.ManagedStart();
			_paizuri = new ParameterValue(parameters[ParameterName.ManEnter]);
			_paizuriFlag = new ParameterValue(parameters[ParameterName.PaizuriFlag]);
			_piston = new ParameterValue(parameters[ParameterName.Piston]);
			_lastPistonValue = _piston.Value;
			_head = _manager.GetOsawariOf<OsawariHeadPaizuri>();
			_breasts = _manager.GetEveryOsawariOf<OsawariBreastPaizuri>();
			_nipples = _manager.GetEveryOsawariOf<OsawariNipplePaizuri>();
			_breastWhilePaizuri = _manager.GetOsawariOf<OsawariBreastWhilePaizuri>();
			_aibu = _manager.GetOsawariOf<OsawariAibuPaizuri>();
			SetAnimation();
			SetExitAnimation();
			InsertController.OnEjaculate.Where((Unit _) => _manager.ContextManager.Context == OsawariContext.Paizuri).Subscribe(delegate
			{
				Animator.SetInteger("SpermGroup1", SpermGroups[0].GetRandomSpermparameter());
				Animator.SetInteger("SpermGroup2", SpermGroups[1].GetRandomSpermparameter());
				Animator.SetInteger("SpermGroup3", SpermGroups[2].GetRandomSpermparameter());
				Animator.SetTrigger("TriggerEjaculate");
				TriggerWaitBetweenEjaculation().Forget();
				_isAnimating = true;
			}).AddTo(this);
			foreach (SpermParameterGroup spermGroup in SpermGroups)
			{
				spermGroup.Initialize();
			}
			StateMachineObservables[] rx = Animator.GetRx();
			for (int num = 0; num < rx.Length; num++)
			{
				rx[num].OnStateExitObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
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
				}).AddTo(this);
			}
		}

		private async UniTask TriggerWaitBetweenEjaculation()
		{
			_ = 1;
			try
			{
				await UniTask.Delay(WaitMillSecAfterFirst, ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				Animator.SetTrigger("WaitAfterFirstEjaculation");
				await UniTask.Delay(WaitMillSecAfterSecond, ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				Animator.SetTrigger("WaitAfterSecondEjaculation");
			}
			catch (OperationCanceledException)
			{
			}
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.ManEnter, _paizuri);
			SetLive2D(ParameterName.PaizuriFlag, _paizuriFlag);
			SetLive2D(ParameterName.Piston, _piston);
			foreach (SpermParameterGroup spermGroup in SpermGroups)
			{
				foreach (int item in spermGroup)
				{
					SetLive2D(item, 1f);
				}
			}
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (_special)
			{
				_breastWhilePaizuri.SynchroMove(move);
				_special = false;
			}
			_piston += move.y / SensitivityY * (float)((!_isMovingFromAnother) ? 1 : (-1));
			_manHand.Lock();
			Enforce(_piston, move);
			_lastPistonValue = _piston.Value;
			InsertController.AddManExtacy(EXTACY_MAN * move.magnitude);
		}

		protected override void UpdateWhileNotClicked()
		{
			if (_isMovingFromAnother)
			{
				_isMovingFromAnother = false;
				return;
			}
			_special = false;
			_manHand.Unlock();
		}

		public void EnterPenis(Action action = null)
		{
			if (!_enterAnim.IsPlaying())
			{
				UnderPenisFollower.SetEnabled(enabled: true);
				UpperPenisFollower.SetEnabled(enabled: true);
				_head.Cancel();
				_nipples.ForEach(delegate(OsawariNipplePaizuri n)
				{
					n.Cancel();
				});
				_breasts.ForEach(delegate(OsawariBreastPaizuri b)
				{
					b.Cancel();
				});
				_aibu.Cancel();
				IsAnimating = true;
				SetAnimation(action);
				_enterAnim.Play();
			}
		}

		public void ExitPenis(Action action = null)
		{
			if (!_exitAnim.IsPlaying())
			{
				Cancel();
				IsAnimating = true;
				SetExitAnimation(action);
				_exitAnim.Play();
			}
		}

		public override void OnClick(CubismDrawable targetMesh, bool isFirst)
		{
			_head.Cancel();
			if (isFirst)
			{
				if (IsAuto)
				{
					IsAuto = false;
				}
				if (GetConstraintsCore())
				{
					OnFirstClick();
					_manager.IsAction = true;
				}
			}
			if (GetConstraintsCore() && !IsAnimating)
			{
				UpdateParams(ConvertMovementVec3ForParams());
			}
		}

		public void SynchroMove(Vector3 move)
		{
			_special = false;
			_isMovingFromAnother = true;
			UpdateParamsCore(move);
		}

		public override void OnSpecial()
		{
			_special = true;
		}

		protected void Enforce(ParameterValue piston, Vector3 move)
		{
			_manager.GetCurrentMousePosition();
			_ = _mousePositionOnLastFrame;
			float num = (piston.Value - _lastPistonValue) / _deltaT * 60f / base.fps / _deltaT * 10000f;
			PhysicsCalculater.Enforce(num * EnforcementFactor, move.y < 0f, PullFactor);
		}

		public override void SwitchContext()
		{
			base.SwitchContext();
			UnderPenisFollower.SetContext(_manager.ContextManager.Context);
			UpperPenisFollower.SetContext(_manager.ContextManager.Context);
		}

		public override void SetAuto()
		{
			base.SetAuto();
			ResetAutoTimeCount();
		}

		protected override bool GetConstraintsCore()
		{
			return IsInPaizuriMode;
		}
	}
}
