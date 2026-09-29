using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public class AnimationStopEvent : OsawariEvent
	{
		public Live2DAnimator Animator;

		private CancellationTokenSource _cts;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			Animator.StopAnimation();
			_cts = new CancellationTokenSource();
			try
			{
				while (true)
				{
					await UniTask.Yield(_cts.Token);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		public override void Cancel()
		{
			base.Cancel();
			_cts.Cancel();
			Animator.RestartAnimation();
		}

		private void OnDestroy()
		{
			_cts?.Cancel();
		}
	}
}
