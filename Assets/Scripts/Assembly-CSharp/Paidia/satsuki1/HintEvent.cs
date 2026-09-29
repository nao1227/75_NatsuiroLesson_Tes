using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class HintEvent : OsawariEvent
	{
		public HintButtonName HintButton;

		protected override void PostInitialize()
		{
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			Object.FindObjectOfType<OsawariManager>().ShowHint(HintButton);
			await UniTask.Yield();
		}

		public override void Cancel()
		{
			base.Cancel();
		}
	}
}
