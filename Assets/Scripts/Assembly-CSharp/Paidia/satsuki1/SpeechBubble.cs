using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class SpeechBubble : MonoBehaviour
	{
		public int Duration = 5000;

		public Subject<int> OnClick;

		public Subject<int> OnDismiss;

		private IObservable<PointerEventData> OnClickImg;

		[NonSerialized]
		public int Index;

		[SerializeField]
		private TextMeshProUGUI text;

		[SerializeField]
		private CanvasGroup cg;

		private bool _isLoaded;

		private bool _isDismissed;

		public bool IsLoaded => _isLoaded;

		private void Start()
		{
			OnClickImg = from x in GetComponent<Image>().OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x;
			OnClick = new Subject<int>();
			OnDismiss = new Subject<int>();
			_isLoaded = true;
		}

		public async UniTask Initialize(string speech)
		{
			await UniTask.WaitUntil(() => _isLoaded);
			text.text = speech;
			OnClickImg.Subscribe(delegate
			{
				OnClick.OnNext(Index);
			}).AddTo(this);
			StartDismissCount().Forget();
		}

		private async UniTask StartDismissCount()
		{
			await UniTask.Delay(UnityEngine.Random.Range(2500, Duration), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			Dismiss();
		}

		public void Dismiss()
		{
			if (!_isDismissed)
			{
				_isDismissed = true;
				DOTween.Sequence().Append(cg.DOFade(0f, 1f)).SetEase(Ease.OutQuad)
					.OnComplete(delegate
					{
						OnDismiss.OnNext(Index);
						UnityEngine.Object.Destroy(base.gameObject);
					})
					.Play();
			}
		}
	}
}
