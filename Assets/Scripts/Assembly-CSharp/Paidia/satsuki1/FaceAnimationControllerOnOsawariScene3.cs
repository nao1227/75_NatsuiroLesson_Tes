using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class FaceAnimationControllerOnOsawariScene3 : FaceAnimationController, IFaceController
	{
		private HScene4Mode _mode;

		private bool _isVibMoving;

		private bool _firstTime = true;

		protected override void SetRx()
		{
			base.SetRx();
			_manager.OnKissStart.Subscribe(delegate
			{
				_isKissing = true;
			}).AddTo(this);
			_manager.OnKissEnd.Subscribe(delegate
			{
				_isKissing = false;
			}).AddTo(this);
			_manager.GetOsawariOf<OsawariVibrator>().OnMove.DistinctUntilChanged().Subscribe(delegate(bool x)
			{
				_isVibMoving = x;
				if (_isVibMoving)
				{
					if (_candidateName != FaceListName.Piston)
					{
						SetFaceList(FaceListName.Piston);
					}
				}
				else
				{
					ResetFace();
				}
			}).AddTo(this);
			HScene3OsawariHelper osawariOf = _manager.GetOsawariOf<HScene3OsawariHelper>();
			osawariOf.OnModeChange.Subscribe(delegate(HScene4Mode x)
			{
				_mode = x;
				ResetFace(_candidateName != FaceListName.Piston && _candidateName != FaceListName.Insert);
			}).AddTo(this);
			_manager.TemporaryStatus.Feelings.ExciteRx.Where((int _) => _isKissing).Subscribe(delegate
			{
			}).AddTo(this);
			_mode = osawariOf.Mode;
			ResetFace();
		}

		protected override void SetUpAnimation()
		{
			StateMachineObservables[] observables = _observables;
			foreach (StateMachineObservables obj in observables)
			{
				obj.OnStateEnterObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
					_state.Value = x.Item2.GetStateName<FaceStateName>().ToString();
				}).AddTo(this);
				obj.OnStateEnterObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Entry).Subscribe(async delegate
				{
					_ = 1;
					try
					{
						await UniTask.Yield();
						await UniTask.WaitUntil(() => !_isUtageAnimating, PlayerLoopTiming.Update, _cts.Token);
						MoveState(_cts.Token).Forget();
					}
					catch (OperationCanceledException)
					{
					}
				}).AddTo(this);
				obj.OnStateEnterObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Insert).Subscribe(delegate
				{
					SetFaceList(FaceListName.Insert);
					ResetFace(reload: false);
				}).AddTo(this);
				obj.OnStateEnterObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Eject).Subscribe(delegate
				{
					SetFaceList(FaceListName.Eject);
					ResetFace(reload: false);
				}).AddTo(this);
				obj.OnStateExitObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.ScenarioEnd).Subscribe(delegate
				{
					_animationStarted = true;
				}).AddTo(this);
			}
			MoveState(_cts.Token).Forget();
		}

		protected override FaceListName GetIdleStateName()
		{
			if (_firstTime)
			{
				_firstTime = false;
				if (SaveLoadManager.UnsavedData.Days == 4)
				{
					return FaceListName.Day4_Default;
				}
				if (SaveLoadManager.UnsavedData.Days == 6)
				{
					return FaceListName.Day6_Default;
				}
			}
			return _mode switch
			{
				HScene4Mode.StudyNormal => FaceListName.IdleStudy, 
				HScene4Mode.H => FaceListName.Idle, 
				HScene4Mode.StudyH => FaceListName.IdleStudyH, 
				_ => throw new NotImplementedException(), 
			};
		}

		public override void ManagedUpdate()
		{
			base.ManagedUpdate();
		}

		public override void ResetFace(bool reload = true)
		{
			FaceListName idleStateName = GetIdleStateName();
			Animator.SetDetailAnimationEnable(enable: true);
			SetFaceList(idleStateName, reload);
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
			case FaceState.Eject:
				SetFaceList(FaceListName.Eject);
				ResetFace(reload: false);
				break;
			case FaceState.Piston:
				break;
			}
		}
	}
}
