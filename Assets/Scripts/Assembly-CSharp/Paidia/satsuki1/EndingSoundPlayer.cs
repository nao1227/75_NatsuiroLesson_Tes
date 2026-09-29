using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	public class EndingSoundPlayer : MonoBehaviour
	{
		public AssetReference GoodEndBGM;

		public AssetReference NormalEndBGM;

		private AudioClip bgm;

		private void Start()
		{
			LoadBGM(delegate
			{
				Play();
			}).Forget();
		}

		private async UniTask LoadBGM(Action onComplete)
		{
			if (SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.GoodEnd))
			{
				bgm = await Addressables.LoadAssetAsync<AudioClip>(GoodEndBGM);
			}
			else
			{
				bgm = await Addressables.LoadAssetAsync<AudioClip>(NormalEndBGM);
			}
			onComplete();
		}

		private void Play()
		{
			SingletonManager<SoundManager>.Instance.PlayBGM(bgm);
		}
	}
}
