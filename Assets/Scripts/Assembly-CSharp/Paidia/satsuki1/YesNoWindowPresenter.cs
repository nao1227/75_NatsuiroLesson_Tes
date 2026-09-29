using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class YesNoWindowPresenter : MonoBehaviour
	{
		public CanvasGroup CG;

		public TextMeshProUGUI Message;

		public Image YesImage;

		public Image NoImage;

		public Image Background;

		public AssetReference OnMouseSE;

		public AssetReference ClickSE;

		private AudioClip _onMouseSE;

		private AudioClip _clickSE;

		public CanvasGroup NoCG;

		public IObservable<PointerEventData> OnClickYes => from x in YesImage.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		public IObservable<PointerEventData> OnClickNo => from x in NoImage.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		public IObservable<PointerEventData> OnClickOutside => from x in Background.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		private async void Start()
		{
			if (OnMouseSE.RuntimeKeyIsValid())
			{
				AsyncOperationHandle<AudioClip> mouseHandle = Addressables.LoadAssetAsync<AudioClip>(OnMouseSE);
				await mouseHandle.Task;
				if (mouseHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_onMouseSE = mouseHandle.Result;
				}
			}
			if (ClickSE.RuntimeKeyIsValid())
			{
				AsyncOperationHandle<AudioClip> mouseHandle = Addressables.LoadAssetAsync<AudioClip>(ClickSE);
				await mouseHandle.Task;
				if (mouseHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_clickSE = mouseHandle.Result;
				}
			}
			YesImage.OnPointerEnterAsObservable().Subscribe(delegate
			{
				if (null != _onMouseSE)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_onMouseSE);
				}
				YesImage.color = Color.white;
			}).AddTo(this);
			NoImage.OnPointerEnterAsObservable().Subscribe(delegate
			{
				if (null != _onMouseSE)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_onMouseSE);
				}
				NoImage.color = Color.white;
			}).AddTo(this);
			YesImage.OnPointerExitAsObservable().Subscribe(delegate
			{
				YesImage.color = Color.gray;
			}).AddTo(this);
			NoImage.OnPointerExitAsObservable().Subscribe(delegate
			{
				NoImage.color = Color.gray;
			}).AddTo(this);
			(from x in YesImage.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				if (null != _clickSE)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_clickSE);
				}
			}).AddTo(this);
			(from x in NoImage.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				if (null != _clickSE)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_clickSE);
				}
			}).AddTo(this);
		}

		private void SetMessage(string message)
		{
			Message.text = message;
		}

		public void SetActive(bool active)
		{
			CG.alpha = (active ? 1 : 0);
			CG.blocksRaycasts = active;
		}

		public async UniTask<bool> WaitForAnswer(string message, bool defaultAnswer = false)
		{
			YesImage.color = Color.gray;
			NoImage.color = Color.gray;
			SetMessage(message);
			SetActive(active: true);
			bool waiting = true;
			bool answer = defaultAnswer;
			CompositeDisposable disposable = new CompositeDisposable();
			OnClickYes.ThrottleFirst(TimeSpan.FromSeconds(1.0)).Subscribe(delegate
			{
				waiting = false;
				answer = true;
			}).AddTo(disposable);
			OnClickNo.ThrottleFirst(TimeSpan.FromSeconds(1.0)).Subscribe(delegate
			{
				waiting = false;
				answer = false;
			}).AddTo(disposable);
			OnClickOutside.ThrottleFirst(TimeSpan.FromSeconds(1.0)).Subscribe(delegate
			{
				waiting = false;
			}).AddTo(disposable);
			while (waiting)
			{
				await UniTask.Yield();
			}
			disposable.Dispose();
			SetActive(active: false);
			await UniTask.Yield();
			return answer;
		}

		public async UniTask WaitForYes(string message)
		{
			YesImage.color = Color.gray;
			NoCG.alpha = 0f;
			NoCG.blocksRaycasts = false;
			SetMessage(message);
			SetActive(active: true);
			bool waiting = true;
			CompositeDisposable disposable = new CompositeDisposable();
			OnClickYes.ThrottleFirst(TimeSpan.FromSeconds(1.0)).Subscribe(delegate
			{
				waiting = false;
			}).AddTo(disposable);
			OnClickOutside.ThrottleFirst(TimeSpan.FromSeconds(1.0)).Subscribe(delegate
			{
				waiting = false;
			}).AddTo(disposable);
			while (waiting)
			{
				await UniTask.Yield();
			}
			disposable.Dispose();
			SetActive(active: false);
			NoCG.alpha = 1f;
			NoCG.blocksRaycasts = true;
			await UniTask.Yield();
		}
	}
}
