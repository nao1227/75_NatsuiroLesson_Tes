using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class FlashAnimation : MonoBehaviour
	{
		public Color FlashColor;

		[Range(0f, 1f)]
		public float Strength;

		public float Duration;

		private CanvasGroup _cg;

		public AssetReference SE;

		private AudioClip _se;

		private async void Start()
		{
			_cg = GetComponent<CanvasGroup>();
			_cg.alpha = 0f;
			_cg.GetComponentInChildren<Image>().color = FlashColor;
			AsyncOperationHandle<AudioClip> handle = Addressables.LoadAssetAsync<AudioClip>(SE);
			await handle.Task;
			if (handle.Status == AsyncOperationStatus.Succeeded)
			{
				_se = handle.Result;
			}
		}

		public void DoFlash(bool playSe = true)
		{
			if (null != _se && playSe)
			{
				SingletonManager<SoundManager>.Instance.PlaySE(_se);
			}
			DOTween.Sequence().Append(_cg.DOFade(Strength, Duration / 2f)).Append(_cg.DOFade(0f, Duration / 2f))
				.Play();
		}
	}
}
