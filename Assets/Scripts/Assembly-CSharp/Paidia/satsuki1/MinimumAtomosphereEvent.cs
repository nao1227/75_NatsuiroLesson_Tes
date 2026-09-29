using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class MinimumAtomosphereEvent : OsawariEvent
	{
		public int Value;

		protected override void PostInitialize()
		{
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			Object.FindObjectOfType<OsawariManager>().SetMinimumAtomosphere(Value);
			await UniTask.Yield();
		}

		public override void Cancel()
		{
			base.Cancel();
		}
	}
}
