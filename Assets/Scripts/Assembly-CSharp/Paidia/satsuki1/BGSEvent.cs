using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	public class BGSEvent : OsawariEvent
	{
		public AssetReference BGS;

		private AudioClip _audioClip;

		protected override async void PostInitialize()
		{
			base.PostInitialize();
			if (BGS.RuntimeKeyIsValid())
			{
				_audioClip = await Addressables.LoadAssetAsync<AudioClip>(BGS);
			}
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			await UniTask.WaitUntil(() => _audioClip != null);
			SingletonManager<SoundManager>.Instance.PlayBGS(_audioClip);
		}
	}
}
