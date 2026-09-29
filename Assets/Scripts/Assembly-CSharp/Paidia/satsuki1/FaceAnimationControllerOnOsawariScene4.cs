using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class FaceAnimationControllerOnOsawariScene4 : FaceAnimationController, IFaceController
	{
		public Live2DAnimator PaizuriAnimator;

		private OsawariContext _context;

		private StateMachineObservables[] _paizuriObservables;

		public override async UniTask ManagedStart(OsawariManager manager)
		{
			_paizuriObservables = PaizuriAnimator.GetRx();
			await base.ManagedStart(manager);
		}

		protected override void SetRx()
		{
			base.SetRx();
			_manager.TemporaryStatus.Feelings.ExciteRx.Subscribe(delegate(int x)
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
			_manager.ContextManager.OnContextChanged.DistinctUntilChanged().Subscribe(delegate(OsawariContext x)
			{
				_context = x;
				SetFaceList(FaceListName.Idle);
			}).AddTo(this);
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
					await UniTask.Yield();
					MoveState(_cts.Token, FaceStateName.None, 1000, 200).Forget();
				}).AddTo(this);
			}
			observables = _paizuriObservables;
			foreach (StateMachineObservables obj2 in observables)
			{
				obj2.OnStateEnterObservable.Where(((StateID, AnimatorStateInfo) _) => _context == OsawariContext.Paizuri).Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
					_state.Value = x.Item2.GetStateName<FaceStateName>().ToString();
				}).AddTo(this);
				(from x in obj2.OnStateEnterObservable
					where _context == OsawariContext.Paizuri
					where x.Item1 == StateID.Entry
					select x).Subscribe(async delegate
				{
					await UniTask.Yield();
					MoveState(_cts.Token, FaceStateName.None, 1000, 200).Forget();
				}).AddTo(this);
			}
		}

		public override void ManagedUpdate()
		{
			base.ManagedUpdate();
			if (_isKissFace)
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
				if (base._isWomanExtacy)
				{
					SetFaceList(FaceListName.PistonExtacy);
				}
				else
				{
					SetFaceList(FaceListName.Piston);
				}
				break;
			case FaceState.Eject:
				SetFaceList(FaceListName.Eject);
				SetFaceList(FaceListName.Idle, reload: false);
				break;
			}
		}

		public override void ResetFace(bool reload = true)
		{
			Animator.SetDetailAnimationEnable(enable: true);
			if (base._isWomanExtacy && FaceStates.GetFaceList(FaceListName.IdleExtacy, _manager) != null)
			{
				SetFaceList(FaceListName.IdleExtacy, reload);
			}
			else
			{
				SetFaceList(FaceListName.Idle, reload);
			}
		}

		protected override async UniTask MoveState(CancellationToken token, FaceStateName forceNext = FaceStateName.None, int crossFadeTime = 1000, int crossFadeOutTime = 200)
		{
			_moveStateTokenSource?.Cancel();
			_moveStateTokenSource = new CancellationTokenSource();
			Live2DAnimator animator = ((_context == OsawariContext.Osawari) ? Animator : PaizuriAnimator);
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
			AudioClip clip = null;
			if (_cache.ContainsKey(next.ToString()))
			{
				clip = _cache[next.ToString()];
			}
			int crossFadeTime2 = FaceStates.GetCrossFadeTime(next);
			try
			{
				await animator.PlayFaceAnimation(next, crossFadeTime2, clip, 0, skipIfSame: false, _candidateName != FaceListName.Insert);
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
	}
}
