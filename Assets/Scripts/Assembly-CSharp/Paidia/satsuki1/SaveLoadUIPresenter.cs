using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class SaveLoadUIPresenter : MonoBehaviour
	{
		public CanvasGroup CG;

		public CanvasGroup Overlay;

		public YesNoWindowPresenter YesNoWindow;

		public SaveLoadMode Mode;

		public int DATA_PER_PAGE = 3;

		public int MAX_PAGE = 33;

		public SaveDataColumnUIPresenter AutoSlot;

		public List<SaveDataColumnUIPresenter> SaveSlots;

		public Image ToRightButton;

		public CanvasGroup ToRightSelected;

		public Image ToLeftButton;

		public CanvasGroup ToLeftSelected;

		public TextMeshProUGUI PageNumber;

		public Image WindowImage;

		public Image HeaderImage;

		public Sprite SaveWindow;

		public Sprite SaveHeader;

		public Sprite LoadWindow;

		public Sprite LoadHeader;

		public Image Background;

		private Subject<Unit> _onLoad = new Subject<Unit>();

		private Subject<Unit> _onClose = new Subject<Unit>();

		private IntReactiveProperty _currentPage = new IntReactiveProperty(0);

		private bool _hold;

		private UtageManager _utage;

		private Action _onComplete;

		private bool _isOpen;

		public IObservable<Unit> OnLoad => _onLoad;

		public IObservable<Unit> OnClose => _onClose;

		public void ManagedStart()
		{
			UpdateImage();
			SetAutoSaveData();
			SetSaveData(0);
			_currentPage.Subscribe(delegate(int x)
			{
				PageNumber.text = (x + 1).ToString();
			}).AddTo(this);
			(from x in ToRightButton.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				MovePage(1);
			}).AddTo(this);
			(from x in ToLeftButton.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				MovePage(-1);
			}).AddTo(this);
			ToRightButton.OnPointerEnterAsObservable().Subscribe(delegate
			{
				ToRightSelected.alpha = 1f;
			}).AddTo(this);
			ToRightButton.OnPointerExitAsObservable().Subscribe(delegate
			{
				ToRightSelected.alpha = 0f;
			}).AddTo(this);
			ToLeftButton.OnPointerEnterAsObservable().Subscribe(delegate
			{
				ToLeftSelected.alpha = 1f;
			}).AddTo(this);
			ToLeftButton.OnPointerExitAsObservable().Subscribe(delegate
			{
				ToLeftSelected.alpha = 0f;
			}).AddTo(this);
			(from _ in AutoSlot.OnClick
				where Mode == SaveLoadMode.Load
				where SaveLoadManager.MetaSaveData.HasMetaSaveData(0)
				select _).Subscribe(async delegate
			{
				if (await YesNoWindow.WaitForAnswer("ロードしますか？"))
				{
					_hold = true;
					SaveDataColumnUIPresenter.SelectedAny = true;
					await UniTask.Yield(this.GetCancellationTokenOnDestroy());
					SaveLoadManager.Load(0);
					_onLoad.OnNext(Unit.Default);
				}
				else
				{
					AutoSlot.Unselect();
				}
			}).AddTo(this);
			for (int num = 0; num < DATA_PER_PAGE; num++)
			{
				SaveDataColumnUIPresenter slot = SaveSlots[num];
				slot.Index = num;
				slot.OnClick.Subscribe(async delegate
				{
					SaveDataColumnUIPresenter.SelectedAny = true;
					int fileNumber = DATA_PER_PAGE * _currentPage.Value + slot.Index + 1;
					if (Mode == SaveLoadMode.Save)
					{
						if (SaveLoadManager.MetaSaveData.HasMetaSaveData(fileNumber) && !(await YesNoWindow.WaitForAnswer("上書きしますか？")))
						{
							slot.Unselect();
						}
						else
						{
							slot.Unselect();
							SaveLoadManager.Save(fileNumber);
							SetSaveData(_currentPage.Value);
						}
					}
					else if (SaveLoadManager.MetaSaveData.HasMetaSaveData(fileNumber))
					{
						if (await YesNoWindow.WaitForAnswer("ロードしますか？"))
						{
							_hold = true;
							Overlay.blocksRaycasts = true;
							SaveLoadManager.Load(fileNumber);
							_onLoad.OnNext(Unit.Default);
						}
						else
						{
							slot.Unselect();
						}
					}
				}).AddTo(this);
			}
			(from x in Background.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left && !_hold
				select x).Subscribe(async delegate
			{
				await SetActive(visible: false);
				_onClose.OnNext(Unit.Default);
				_utage = UnityEngine.Object.FindObjectOfType<UtageManager>();
				_utage?.Resume();
				SingletonManager<SceneContextManager>.Instance.AllowUtage = true;
			}).AddTo(this);
			SaveDataColumnUIPresenter.SelectedAny = false;
			if (SaveLoadManager.UnsavedData.FileNumber == 0 && SaveLoadManager.MetaSaveData.HasAnyMetaSaveData())
			{
				MovePage(SaveLoadManager.MetaSaveData.GetNewestSavedData().Index / DATA_PER_PAGE);
			}
		}

		private void UpdateImage()
		{
			if (Mode == SaveLoadMode.Save)
			{
				WindowImage.sprite = SaveWindow;
				HeaderImage.sprite = SaveHeader;
				AutoSlot.IsLoad = false;
			}
			else
			{
				WindowImage.sprite = LoadWindow;
				HeaderImage.sprite = LoadHeader;
				AutoSlot.IsLoad = true;
			}
		}

		private void SetAutoSaveData()
		{
			if (SaveLoadManager.MetaSaveData.HasMetaSaveData(0))
			{
				AutoSlot.SetData(SaveLoadManager.MetaSaveData.GetAutoSaveMetaData());
				AutoSlot.SetActive(active: true);
			}
			else
			{
				AutoSlot.SetActive(active: false);
			}
		}

		private void SetSaveData(int page)
		{
			for (int i = 1; i <= DATA_PER_PAGE; i++)
			{
				if (SaveLoadManager.MetaSaveData.HasMetaSaveData(DATA_PER_PAGE * page + i))
				{
					SaveSlots[i - 1].SetData(SaveLoadManager.MetaSaveData.GetMetaSaveData(DATA_PER_PAGE * page + i));
					SaveSlots[i - 1].SetActive(active: true);
				}
				else
				{
					SaveSlots[i - 1].SetActive(active: false);
				}
			}
		}

		private void MovePage(int addition)
		{
			if (_currentPage.Value + addition <= MAX_PAGE && _currentPage.Value + addition >= 0)
			{
				_currentPage.Value += addition;
			}
			SingletonManager<SceneContextManager>.Instance.LastOpenedSaveLoadPage = _currentPage.Value;
			SetSaveData(_currentPage.Value);
		}

		public async UniTask SetActive(bool visible, Action onComplete = null)
		{
			if (_isOpen == visible)
			{
				return;
			}
			_isOpen = visible;
			await UniTask.WaitForEndOfFrame();
			SaveLoadManager.ScreenShot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
			if (visible)
			{
				UpdateImage();
			}
			if (visible)
			{
				MovePage(SingletonManager<SceneContextManager>.Instance.LastOpenedSaveLoadPage - _currentPage.Value);
			}
			CG.alpha = (visible ? 1 : 0);
			CG.blocksRaycasts = visible;
			if (visible)
			{
				if (onComplete != null)
				{
					_onComplete = onComplete;
				}
				_utage = UnityEngine.Object.FindObjectOfType<UtageManager>();
				_utage?.Pause();
				Overlay.blocksRaycasts = !visible;
				SetSaveData(_currentPage.Value);
				SetAutoSaveData();
			}
			else if (_onComplete != null)
			{
				_onComplete();
				_onComplete = null;
			}
		}
	}
}
