using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class AnimationEvent : OsawariEvent
	{
		[SerializeField]
		private AnimeName AnimeName;

		[SerializeField]
		private bool Switch;

		[SerializeField]
		private bool IsSingle;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			await StartAnimation(StringsManager.GetAnimeName(AnimeName), Switch, IsSingle);
		}

		protected async UniTask StartAnimation(string animationName, bool on, bool singleTime = false)
		{
			Animator animator = _parent.GetComponent<Animator>();
			CancellationToken token = this.GetCancellationTokenOnDestroy();
			while (animator.GetBool(animationName) != on)
			{
				animator.SetBool(animationName, on);
				await UniTask.Yield();
			}
			await UniTask.Yield(token);
			await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			if (singleTime)
			{
				await UniTask.Yield(token);
				animator.SetBool(animationName, !on);
			}
		}
	}
}
