using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class FaceAnimationControllerOnOsawariScene2 : FaceAnimationController, IFaceController
	{
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
					await UniTask.Yield();
					MoveState(_cts.Token).Forget();
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
				MoveState(_cts.Token).Forget();
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
			case FaceState.Eject:
				SetFaceList(FaceListName.Eject);
				SetFaceList(FaceListName.Idle, reload: false);
				break;
			}
		}

		public override void ResetFace(bool reload = true)
		{
			Animator.SetDetailAnimationEnable(enable: true);
			if (Animator.GetBool("Move"))
			{
				FaceListName faceListName = (Animator.GetBool("MoveFast") ? FaceListName.MoveFast : FaceListName.MoveSlow);
				SetFaceList(faceListName, reload);
			}
			else if (base._isWomanExtacy && FaceStates.GetFaceList(FaceListName.IdleExtacy, _manager) != null)
			{
				SetFaceList(FaceListName.IdleExtacy, reload);
			}
			else
			{
				SetFaceList(FaceListName.Idle, reload);
			}
		}
	}
}
