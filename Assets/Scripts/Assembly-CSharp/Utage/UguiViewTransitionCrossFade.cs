using System.Collections;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/UguiViewTransitionCrossFade")]
	[RequireComponent(typeof(UguiView))]
	public class UguiViewTransitionCrossFade : MonoBehaviour, ITransition
	{
		private UguiView uguiView;

		private bool isPlaying;

		public float time = 1f;

		public bool unscaledTime;

		public UguiView UguiView => this.GetComponentCache(ref uguiView);

		public bool IsPlaying => isPlaying;

		public void Open()
		{
			StopCoroutine(CoClose());
			StartCoroutine(CoOpen());
		}

		public void Close()
		{
			StopCoroutine(CoOpen());
			StartCoroutine(CoClose());
		}

		public void CancelClosing()
		{
			StopCoroutine(CoClose());
			EndClose();
			isPlaying = false;
		}

		private IEnumerator CoOpen()
		{
			isPlaying = true;
			CanvasGroup canvasGroup = UguiView.CanvasGroup;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			float currentTime = 0f;
			while (currentTime < time)
			{
				canvasGroup.alpha = currentTime / time;
				currentTime += TimeUtil.GetDeltaTime(unscaledTime);
				yield return null;
			}
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			isPlaying = false;
		}

		private IEnumerator CoClose()
		{
			isPlaying = true;
			CanvasGroup canvasGroup = UguiView.CanvasGroup;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			float currentTime = 0f;
			while (currentTime < time)
			{
				canvasGroup.alpha = 1f - currentTime / time;
				currentTime += TimeUtil.GetDeltaTime(unscaledTime);
				yield return null;
			}
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			EndClose();
		}

		private void EndClose()
		{
			UguiView.CanvasGroup.alpha = 0f;
			isPlaying = false;
		}
	}
}
