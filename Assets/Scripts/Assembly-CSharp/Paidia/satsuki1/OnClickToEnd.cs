using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class OnClickToEnd : MonoBehaviour
	{
		public Image Image;

		public PlayableDirector Director;

		public Canvas Canvas;

		public MessageWindowUIPresenter MessageWindow;

		public TextMeshProUGUI ThanksText;

		private void Start()
		{
			(from x in Image.OnPointerClickAsObservable()
				where Director.time == Director.duration
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(async delegate
			{
				if (SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.GoodEnd))
				{
					ThanksText.alpha = 0f;
					Canvas.enabled = false;
					await UnityEngine.Object.FindObjectOfType<UtageManager>().ShowUtageText(ScenarioLabel.Epilogue, this.GetCancellationTokenOnDestroy());
					Canvas.enabled = true;
				}
				SingletonManager<SoundManager>.Instance.StopAll();
				await SceneManager.LoadSceneAsync(0);
			}).AddTo(this);
			EnableSkip().Forget();
		}

		private async UniTask EnableSkip()
		{
			await UniTask.Delay(TimeSpan.FromSeconds(5.0));
			(from x in Image.OnPointerClickAsObservable()
				where Director.time != Director.duration
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				Director.time = Director.duration - 0.005;
			}).AddTo(this);
		}
	}
}
