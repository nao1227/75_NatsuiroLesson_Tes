using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public class VoiceEvent : OsawariEvent
	{
		public VoiceList Voices;

		public VoiceManager Manager;

		public int channel;

		public int PlayTime = 2500;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			Manager.Play(Voices, null, channel);
			Manager.Stop(Voices, PlayTime).Forget();
			await UniTask.Yield();
		}
	}
}
