using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class FaceAnimationControllerOnEndOfDay : FaceAnimationController, IFaceController
	{
		public bool AfterNade;

		public bool AfterH;

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
				});
			}
			MoveState(_cts.Token).Forget();
		}

		public override void ResetFace(bool reload = true)
		{
			Animator.SetDetailAnimationEnable(enable: true);
			if (AfterH)
			{
				SetFaceList(FaceListName.IdleAfterH, reload);
			}
			else if (AfterNade)
			{
				SetFaceList(FaceListName.IdleAfterNade, reload);
			}
			else
			{
				SetFaceList(FaceListName.Idle, reload);
			}
		}
	}
}
