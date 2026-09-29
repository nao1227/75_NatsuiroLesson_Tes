using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class DialogueEvent : OsawariEvent
	{
		[SerializeField]
		private DialogueEnum Dialogue;

		[SerializeField]
		private int Percentage;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			await UniTask.Yield();
		}
	}
}
