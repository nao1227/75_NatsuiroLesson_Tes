using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Stubs;

namespace Paidia.satsuki1
{
	public class ScriptExecuteEvent : OsawariEvent
	{
		[SerializeField]
		private UnityEvent OnExecuteEvent;

		[SerializeField]
		private UnityEvent OnCancelledEvent;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			OnExecuteEvent?.Invoke();
			await UniTask.Yield();
		}

		public override void Cancel()
		{
			base.Cancel();
			OnCancelledEvent?.Invoke();
		}
	}
}
