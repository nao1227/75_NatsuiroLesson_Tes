using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public class SEEvent : OsawariEvent
	{
		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			await UniTask.Yield();
		}
	}
}
