using System;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class StateMachineObservables : StateMachineBehaviour
	{
		public StateID ID;

		private Subject<(StateID, AnimatorStateInfo)> onStateEnterSubject = new Subject<(StateID, AnimatorStateInfo)>();

		private Subject<(StateID, AnimatorStateInfo)> onStateExitSubject = new Subject<(StateID, AnimatorStateInfo)>();

		private Subject<int> onStateMachineEnterSubject = new Subject<int>();

		private Subject<int> onStateMachineExitrSubject = new Subject<int>();

		private Subject<AnimatorStateInfo> onStateMoveSubject = new Subject<AnimatorStateInfo>();

		private Subject<AnimatorStateInfo> onStateUpdateSubject = new Subject<AnimatorStateInfo>();

		private Subject<AnimatorStateInfo> onStateIKSubject = new Subject<AnimatorStateInfo>();

		public IObservable<(StateID, AnimatorStateInfo)> OnStateEnterObservable => onStateEnterSubject.AsObservable();

		public IObservable<(StateID, AnimatorStateInfo)> OnStateExitObservable => onStateExitSubject.AsObservable();

		public IObservable<int> OnStateMachineEnterObservable => onStateMachineEnterSubject.AsObservable();

		public IObservable<int> OnStateMachineExitObservable => onStateMachineExitrSubject.AsObservable();

		public IObservable<AnimatorStateInfo> OnStateMoveObservable => onStateMoveSubject.AsObservable();

		public IObservable<AnimatorStateInfo> OnStateUpdateObservable => onStateUpdateSubject.AsObservable();

		public IObservable<AnimatorStateInfo> OnStateIKObservable => onStateIKSubject.AsObservable();

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			onStateEnterSubject.OnNext((ID, stateInfo));
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			onStateExitSubject.OnNext((ID, stateInfo));
		}

		public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
		{
			onStateMachineEnterSubject.OnNext(stateMachinePathHash);
		}

		public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
		{
			onStateMachineExitrSubject.OnNext(stateMachinePathHash);
		}

		public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			onStateMoveSubject.OnNext(stateInfo);
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			onStateUpdateSubject.OnNext(stateInfo);
		}

		public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			onStateIKSubject.OnNext(stateInfo);
		}
	}
}
