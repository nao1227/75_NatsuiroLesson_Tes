using System;
using System.Collections.Generic;
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
	public class ChoiceWindowPresenter : MonoBehaviour
	{
		public CanvasGroup CG;

		public List<Image> Choices;

		public List<Image> FiveChoices;

		public bool EnableFinishOnOutsideClick;

		public Image Background;

		public AssetReference OnMouseSE;

		public AssetReference ClickSE;

		private AudioClip _onMouseSE;

		private AudioClip _clickSE;

		public IObservable<PointerEventData> OnClickOutside => from x in Background.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		private async void Start()
		{
			if (OnMouseSE != null)
			{
				AsyncOperationHandle<AudioClip> mouseHandle = Addressables.LoadAssetAsync<AudioClip>(OnMouseSE);
				await mouseHandle.Task;
				if (mouseHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_onMouseSE = mouseHandle.Result;
				}
			}
			if (ClickSE != null)
			{
				AsyncOperationHandle<AudioClip> mouseHandle = Addressables.LoadAssetAsync<AudioClip>(ClickSE);
				await mouseHandle.Task;
				if (mouseHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_clickSE = mouseHandle.Result;
				}
			}
			foreach (Image fiveChoice in FiveChoices)
			{
				fiveChoice.OnPointerEnterAsObservable().Subscribe(delegate
				{
					if (null != _onMouseSE)
					{
						SingletonManager<SoundManager>.Instance.PlaySE(_onMouseSE);
					}
				}).AddTo(this);
				(from x in fiveChoice.OnPointerClickAsObservable()
					where x.button == PointerEventData.InputButton.Left
					select x).Subscribe(delegate
				{
					if (null != _clickSE)
					{
						SingletonManager<SoundManager>.Instance.PlaySE(_clickSE);
					}
				}).AddTo(this);
			}
		}

		public void SetActive(bool active)
		{
			CG.alpha = (active ? 1 : 0);
			CG.blocksRaycasts = active;
		}

		public async UniTask<int> WaitForAnswer(List<string> messages)
		{
			SetActive(active: true);
			int answer = 0;
			bool waiting = true;
			CompositeDisposable disposable = new CompositeDisposable();
			List<Image> list = ((messages.Count <= 3) ? Choices : FiveChoices);
			if (list == Choices)
			{
				FiveChoices.ForEach(delegate(Image x)
				{
					x.gameObject.SetActive(value: false);
				});
			}
			for (int num = 0; num < list.Count; num++)
			{
				if (messages.Count > num)
				{
					int index = num;
					Image image = list[num];
					image.gameObject.SetActive(value: true);
					image.GetComponentInChildren<TextMeshProUGUI>().text = messages[index];
					(from x in image.OnPointerClickAsObservable()
						where x.button == PointerEventData.InputButton.Left
						select x).Take(1).Subscribe(delegate
					{
						waiting = false;
						answer = index;
					}).AddTo(disposable);
				}
				else
				{
					list[num].gameObject.SetActive(value: false);
				}
			}
			if (EnableFinishOnOutsideClick)
			{
				OnClickOutside.Take(1).Subscribe(delegate
				{
					waiting = false;
				}).AddTo(disposable);
			}
			while (waiting)
			{
				await UniTask.Yield();
				if (Input.GetMouseButtonDown(1))
				{
					disposable.Dispose();
					SetActive(active: false);
					throw new NoChoiceException();
				}
			}
			disposable.Dispose();
			SetActive(active: false);
			return answer;
		}
	}
}
