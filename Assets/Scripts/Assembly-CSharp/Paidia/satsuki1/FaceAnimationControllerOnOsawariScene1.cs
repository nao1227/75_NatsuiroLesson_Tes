using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class FaceAnimationControllerOnOsawariScene1 : FaceAnimationController, IFaceController
	{
		public Live2DAnimator FellaAnimator;

		private StateMachineObservables[] _fellaObservables;

		private OsawariContext _context;

		private bool _isFellatioMode;

		private bool _isPlayingSuckAnimation;

		private bool _isMovingFast;

		private bool _isMouthOpen = true;

		private bool _isWaitingForDrink;

		private bool _isFellatioMove;

		private int _spermInMouth;

		private FaceListName _savedFellatioFaceCandidate;

		private bool _isIrruma => _manager.OsawariFellatio.Mode == FellatioMode.PlayerControl;

		public bool IsRestrictDrink { get; private set; }

		public override async UniTask ManagedStart(OsawariManager manager)
		{
			_fellaObservables = FellaAnimator.GetRx();
			if (_fellaObservables.Count() > 0)
			{
				SetUpFellaAnimation();
			}
			await base.ManagedStart(manager);
		}

		public override Live2DAnimator GetAnimator()
		{
			if (_context == OsawariContext.Osawari)
			{
				return Animator;
			}
			return FellaAnimator;
		}

		protected override void SetRx()
		{
			base.SetRx();
			_manager.OnKissStart.Subscribe(delegate
			{
				_isKissing = true;
				if (_candidateName == FaceListName.Piston)
				{
					SetFaceList(FaceListName.PistonKiss, reload: false);
				}
			}).AddTo(this);
			_manager.OnKissEnd.Subscribe(delegate
			{
				_isKissing = false;
				if (_candidateName == FaceListName.PistonKiss)
				{
					SetFaceList(FaceListName.Piston, reload: false);
				}
			}).AddTo(this);
			_manager.TemporaryStatus.Feelings.ExciteRx.Where((int _) => _context == OsawariContext.Osawari).Subscribe(delegate(int x)
			{
				_womanExtacy = x;
				if (base._isWomanExtacy)
				{
					if (_candidateName == FaceListName.Piston)
					{
						SetFaceList(FaceListName.PistonExtacy);
					}
					else if (_candidateName == FaceListName.PistonKiss)
					{
						SetFaceList(FaceListName.PistonKissExtacy);
					}
					else if (_candidateName == FaceListName.Idle && FaceStates.GetFaceList(FaceListName.IdleExtacy, _manager) != null)
					{
						SetFaceList(FaceListName.IdleExtacy);
					}
				}
				else if (_candidateName == FaceListName.PistonExtacy)
				{
					SetFaceList(FaceListName.Piston);
				}
				else if (_candidateName == FaceListName.PistonKissExtacy)
				{
					SetFaceList(FaceListName.PistonKiss);
				}
				else if (_candidateName == FaceListName.IdleExtacy)
				{
					SetFaceList(FaceListName.Idle);
				}
			}).AddTo(this);
			OsawariFellatio osawariFellatio = _manager.OsawariFellatio;
			osawariFellatio.OnFellatio.DistinctUntilChanged().Subscribe(delegate(bool x)
			{
				if (x)
				{
					bool reload = true;
					if (_isPlayingSuckAnimation)
					{
						SetFaceList(FaceListName.SuckEnd, reload: true, restrictStateChange: true);
						reload = false;
						_isPlayingSuckAnimation = false;
					}
					if (!_isFellatioMode)
					{
						SetFaceList(FaceListName.FellatioStart, reload, restrictStateChange: true);
					}
					SetFellatioFace(reload: false);
					_isFellatioMode = true;
				}
				else
				{
					if (_isFellatioMode)
					{
						if (_isMouthOpen)
						{
							SetFaceList(FaceListName.FellatioEnd, reload: true, restrictStateChange: true);
						}
						else
						{
							SetFaceList(FaceListName.FellatioEndClose, reload: true, restrictStateChange: true);
						}
					}
					if (_isMouthOpen)
					{
						ReserveFace(FaceListName.Idle);
					}
					else
					{
						ReserveFace(FaceListName.CloseIdle);
					}
					_isFellatioMode = false;
				}
			}).AddTo(this);
			osawariFellatio.OnSuck.Subscribe(delegate(bool x)
			{
				if (x)
				{
					bool reload = true;
					if (_isFellatioMode)
					{
						SetFaceList(FaceListName.FellatioEnd, reload: true, restrictStateChange: true);
						reload = false;
						_isFellatioMode = false;
					}
					if (!_isPlayingSuckAnimation)
					{
						SetFaceList(FaceListName.SuckStart, reload, restrictStateChange: true);
					}
					ReserveFace(FaceListName.Suck);
					_isPlayingSuckAnimation = true;
				}
				else
				{
					if (_isPlayingSuckAnimation)
					{
						SetFaceList(FaceListName.SuckEnd, reload: true, restrictStateChange: true);
					}
					ResetFace(reload: false);
					_isPlayingSuckAnimation = false;
				}
			}).AddTo(this);
			osawariFellatio.OnMoveFast.Where((bool _) => _isFellatioMode).DistinctUntilChanged().Subscribe(delegate(bool x)
			{
				_isMovingFast = x;
				if (x)
				{
					SetFaceList(FaceListName.MoveFast);
				}
				else
				{
					SetFaceList(FaceListName.MoveSlow);
				}
			})
				.AddTo(this);
			osawariFellatio.OnMouthOpen.DistinctUntilChanged().Subscribe(delegate(bool x)
			{
				_isMouthOpen = x;
				if (_isWaitingForDrink)
				{
					OpenMouth(x);
				}
			}).AddTo(this);
			osawariFellatio.OnRestrictDrink.DistinctUntilChanged().Subscribe(delegate(bool x)
			{
				IsRestrictDrink = x;
				if (!IsRestrictDrink && _isWaitingForDrink)
				{
					if (_isFellatioMode)
					{
						SetFaceList(FaceListName.Drink, reload: true, restrictStateChange: true);
						SetFellatioFace(reload: false);
					}
					else
					{
						if (_isMouthOpen)
						{
							SetFaceList(FaceListName.Close, reload: true, restrictStateChange: true);
							ReserveFace(FaceListName.DrinkOpen, restrictStateChange: true);
						}
						else
						{
							SetFaceList(FaceListName.DrinkOpen, reload: true, restrictStateChange: true);
						}
						ReserveFace(FaceListName.Idle);
					}
				}
			}).AddTo(this);
			(from _ in osawariFellatio.OnEjaculation
				where _manager.ContextManager.Context == OsawariContext.Fellatio
				where _isFellatioMode
				select _).Subscribe(delegate
			{
				if (IsRestrictDrink)
				{
					_isWaitingForDrink = true;
					if (_isFellatioMove)
					{
						_isFellatioMove = false;
						SetFellatioFace();
						IsStateChangeAllowed = false;
					}
				}
				else
				{
					SetFaceList(FaceListName.Drink, reload: true, restrictStateChange: true);
					ReserveFace(FaceListName.FellatioIdle);
				}
			}).AddTo(this);
			osawariFellatio.OnFellatioMove.DistinctUntilChanged().Subscribe(delegate(bool x)
			{
				_isFellatioMove = x;
				SetFellatioFace();
			}).AddTo(this);
			osawariFellatio.SpermInMouth.Subscribe(delegate(int x)
			{
				_spermInMouth = x;
				if (x == 0)
				{
					_isWaitingForDrink = false;
				}
			}).AddTo(this);
			_manager.ContextManager.OnContextChanged.DistinctUntilChanged().Subscribe(delegate(OsawariContext x)
			{
				_context = x;
			}).AddTo(this);
			_manager.ContextManager.OnContextChanged.Where((OsawariContext x) => x == OsawariContext.Fellatio).Subscribe(delegate
			{
				SetFaceList(_savedFellatioFaceCandidate);
			}).AddTo(this);
			_manager.ContextManager.OnContextChanged.Where((OsawariContext x) => x == OsawariContext.Osawari).Subscribe(delegate
			{
				_savedFellatioFaceCandidate = _candidateName;
				SetFaceList(FaceListName.Idle);
			}).AddTo(this);
			osawariFellatio.OnModeChange.Where((FellatioMode _) => _isFellatioMode).Subscribe(delegate
			{
				SetFellatioFace();
			}).AddTo(this);
		}

		private void SetFellatioFace(bool reload = true)
		{
			if (!_isIrruma)
			{
				if (_isFellatioMove)
				{
					if (_isMovingFast)
					{
						SetFaceList(FaceListName.MoveFast, reload);
					}
					else
					{
						SetFaceList(FaceListName.MoveSlow, reload);
					}
				}
				else
				{
					SetFaceList(FaceListName.FellatioIdle, reload);
				}
			}
			else if (_isFellatioMove)
			{
				SetFaceList(FaceListName.Irruma, reload);
			}
			else
			{
				SetFaceList(FaceListName.IrrumaIdle, reload);
			}
		}

		private void OpenMouth(bool open, bool reload = true)
		{
			if (open)
			{
				SetFaceList(FaceListName.Open, reload, restrictStateChange: true);
				ReserveFace(FaceListName.OpenIdle);
			}
			else
			{
				SetFaceList(FaceListName.Close, reload, restrictStateChange: true);
				ReserveFace(FaceListName.CloseIdle);
			}
		}

		protected override void SetUpAnimation()
		{
			StateMachineObservables[] observables = _observables;
			foreach (StateMachineObservables obj in observables)
			{
				obj.OnStateEnterObservable.Where(((StateID, AnimatorStateInfo) _) => _context == OsawariContext.Osawari).Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
					_state.Value = x.Item2.GetStateName<FaceStateName>().ToString();
				}).AddTo(this);
				(from x in obj.OnStateEnterObservable
					where _context == OsawariContext.Osawari
					where x.Item1 == StateID.Entry
					select x).Subscribe(async delegate
				{
					IsStateChangeAllowed = true;
					await UniTask.Yield();
					MoveState(_cts.Token, FaceStateName.None, 1000, 200).Forget();
				}).AddTo(this);
			}
			MoveState(_cts.Token, FaceStateName.None, 1000, 200).Forget();
		}

		private void SetUpFellaAnimation()
		{
			StateMachineObservables[] fellaObservables = _fellaObservables;
			foreach (StateMachineObservables obj in fellaObservables)
			{
				(from x in obj.OnStateEnterObservable
					where _context == OsawariContext.Fellatio
					where x.Item1 == StateID.Entry
					select x).Subscribe(async delegate
				{
					IsStateChangeAllowed = true;
					await UniTask.Yield();
					MoveState(_cts.Token, FaceStateName.None, 1000, 200).Forget();
				}).AddTo(this);
				(from x in obj.OnStateEnterObservable
					where _context == OsawariContext.Fellatio
					where x.Item1 == StateID.FellatioIdle
					select x).Subscribe(delegate
				{
					IsStateChangeAllowed = true;
				}).AddTo(this);
			}
		}

		protected override async UniTask MoveState(CancellationToken token, FaceStateName forceNext = FaceStateName.None, int crossFadeTime = 1000, int crossFadeOutTime = 200)
		{
			_moveStateTokenSource?.Cancel();
			_moveStateTokenSource = new CancellationTokenSource();
			Live2DAnimator animator = ((_context == OsawariContext.Osawari) ? Animator : FellaAnimator);
			if (null == animator)
			{
				return;
			}
			FaceStateName next = GetNextState();
			try
			{
				await UniTask.WaitUntil(() => _animationStarted, PlayerLoopTiming.Update, _moveStateTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			if (forceNext != FaceStateName.None)
			{
				next = forceNext;
			}
			AudioClip clip = null;
			if (_cache.ContainsKey(next.ToString()))
			{
				clip = _cache[next.ToString()];
			}
			int crossFadeTime2 = FaceStates.GetCrossFadeTime(next);
			try
			{
				await animator.PlayFaceAnimation(next, crossFadeTime2, clip, (_context == OsawariContext.Osawari || _candidateName == FaceListName.SuckStart) ? 150 : 201, skipIfSame: false, _candidateName != FaceListName.Insert, _manager.ContextManager.Context == OsawariContext.Fellatio);
				await UniTask.Yield(_moveStateTokenSource.Token);
				MoveState(_cts.Token, FaceStateName.None, 1000, 200).Forget();
			}
			catch (OperationCanceledException)
			{
			}
			catch (PlayingTemporaryAnimationException)
			{
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			if (_isKissFace)
			{
				_isKissFace = false;
			}
		}

		public override void ManagedUpdate()
		{
			base.ManagedUpdate();
			if (Animator.GetBool("BraTakeOffFace"))
			{
				Animator.SetBool("BraTakeOffFace", on: false);
				RefreshToken();
				MoveState(_cts.Token, FaceStateName.Face_BraOff, 100, 200).Forget();
			}
			else if (_isKissFace)
			{
				_isKissFace = false;
				RefreshToken();
				MoveState(_cts.Token, FaceStateName.None, 1000, 200).Forget();
			}
		}

		protected override void FaceChange(FaceState state)
		{
			RefreshToken();
			switch (state)
			{
			case FaceState.Idle:
				ResetFace();
				break;
			case FaceState.Insert:
				Animator.SetDetailAnimationEnable(enable: false);
				SetFaceList(FaceListName.Insert);
				ResetFace(reload: false);
				break;
			case FaceState.Piston:
				if (_isKissing)
				{
					if (base._isWomanExtacy)
					{
						SetFaceList(FaceListName.PistonKissExtacy);
					}
					else
					{
						SetFaceList(FaceListName.PistonKiss);
					}
				}
				else if (base._isWomanExtacy)
				{
					SetFaceList(FaceListName.PistonExtacy);
				}
				else
				{
					SetFaceList(FaceListName.Piston);
				}
				break;
			}
		}
	}
}
