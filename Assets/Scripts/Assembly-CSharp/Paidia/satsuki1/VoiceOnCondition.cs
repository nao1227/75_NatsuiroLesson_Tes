using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	[Serializable]
	public class VoiceOnCondition
	{
		public AtomosphereName Atomosphere;

		public int ExciteRangeLower;

		public int ExciteRangeUpper;

		public VoiceName Voice;

		public AssetReference VoiceFile;

		public AudioClip Clip { get; private set; }

		public bool IsLoaded { get; private set; }

		public async UniTask LoadAsync()
		{
			Unload();
			Clip = await VoiceFile.LoadAssetAsync<AudioClip>();
			IsLoaded = true;
		}

		public void Unload()
		{
			if (VoiceFile.IsValid())
			{
				Clip = null;
				VoiceFile.ReleaseAsset();
			}
		}
	}
}
