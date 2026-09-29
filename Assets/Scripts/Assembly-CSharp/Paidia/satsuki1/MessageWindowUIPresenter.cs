using System;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtageExtensions;

namespace Paidia.satsuki1
{
	public class MessageWindowUIPresenter : MonoBehaviour
	{
		public SceneType SceneType = SceneType.Osawari;

		public CanvasGroup BackgroundCG;

		public CanvasGroup LongMessageWindowCG;

		public CanvasGroup ShortMessageWindowCG;

		public Image Image;

		public Image BackgroundImage;

		public TextMeshProUGUI ShortMessage;

		public TextMeshProUGUI LongMessage;

		public TextMeshProUGUI LongMessage10;

		public bool ShowingMessage
		{
			get
			{
				if (!(null != BackgroundCG))
				{
					return false;
				}
				return BackgroundCG.alpha == 1f;
			}
		}

		private IObservable<PointerEventData> OnClickOutside => from x in BackgroundImage.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		public async UniTask SetShortMessage(string message)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = message.Split('\n');
			foreach (string text in array)
			{
				for (int j = 0; j < text.Length; j += 16)
				{
					stringBuilder.Append(text.Substring(j, Math.Min(16, text.Length - j)));
					stringBuilder.AppendLine();
				}
			}
			await UniTask.WaitUntil(() => !ShowingMessage);
			BackgroundCG.blocksRaycasts = true;
			BackgroundCG.alpha = 1f;
			ShortMessage.text = message;
			ShortMessageWindowCG.alpha = 1f;
			ShortMessageWindowCG.blocksRaycasts = true;
			WaitForClickOutside();
			await UniTask.WaitUntil(() => !ShowingMessage);
		}

		public async UniTask SetLongMessage(string message, Sprite sprite = null)
		{
			await UniTask.WaitUntil(() => !ShowingMessage);
			BackgroundCG.blocksRaycasts = true;
			BackgroundCG.alpha = 1f;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = message.Split('\n');
			foreach (string text in array)
			{
				for (int num2 = 0; num2 < text.Length; num2 += 24)
				{
					stringBuilder.Append(text.Substring(num2, Math.Min(24, text.Length - num2)));
					stringBuilder.AppendLine();
				}
			}
			if (null != sprite)
			{
				Image.sprite = sprite;
				Image.SetAlpha(1f);
				LongMessage10.text = "";
				LongMessage.text = stringBuilder.ToString();
			}
			else
			{
				Image.SetAlpha(0f);
				LongMessage10.text = stringBuilder.ToString();
				LongMessage.text = "";
			}
			LongMessageWindowCG.alpha = 1f;
			LongMessageWindowCG.blocksRaycasts = true;
			WaitForClickOutside();
			try
			{
				await UniTask.WaitUntil(() => !ShowingMessage, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		private void WaitForClickOutside()
		{
			CompositeDisposable disposables = new CompositeDisposable();
			(from _ in Observable.EveryUpdate()
				where Input.GetMouseButtonDown(0)
				select _).First().Subscribe(delegate
			{
				CloseWindow();
				disposables.Dispose();
			}).AddTo(disposables);
		}

		public void CloseWindow()
		{
			ShortMessageWindowCG.alpha = 0f;
			ShortMessageWindowCG.blocksRaycasts = false;
			LongMessageWindowCG.alpha = 0f;
			LongMessageWindowCG.blocksRaycasts = false;
			BackgroundCG.blocksRaycasts = false;
			BackgroundCG.alpha = 0f;
			LongMessage.text = "";
			LongMessage10.text = "";
			ShortMessage.text = "";
		}
	}
}
