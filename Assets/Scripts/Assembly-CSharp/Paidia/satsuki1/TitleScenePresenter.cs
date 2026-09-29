using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UtageExtensions;

namespace Paidia.satsuki1
{
	public class TitleScenePresenter : MonoBehaviour, IOption
	{
		public TextMeshProUGUI FromStart;

		public TextMeshProUGUI FromLoad;

		public TextMeshProUGUI ToBonus;

		public TextMeshProUGUI EndGame;

		public TextMeshProUGUI FreeHMode;

		public TextMeshProUGUI FreeScenarioMode;

		public TextMeshProUGUI Bonus1Text;

		public TextMeshProUGUI Bonus2Text;

		public TextMeshProUGUI BackToMainMenu;

		public CanvasGroup MainMenuCG;

		public CanvasGroup SubMenuCG;

		public SaveLoadUIPresenter Presenter;

		public AssetReference ClickSE;

		public AssetReference BGS;

		public CanvasGroup BlackCG;

		public CanvasGroup NameInputImageCG;

		public TextMeshProUGUI NameInput;

		public CanvasGroup InvalidNameCG;

		public TextMeshProUGUI ConfirmName;

		public Image NameInputConfirmUnderline;

		public YesNoWindowPresenter YesNoWindow;

		private AudioClip _clip;

		private AudioClip _bgs;

		public VideoPlayer VideoPlayer;

		public AssetReference Movie1;

		public AssetReference Movie2;

		public OsawariManager Manager;

		public Texture2D OsawariMouse;

		public Texture2D NormalMouse;

		private InputManager _input;

		public Slider MouseSensitivitySlider;

		private bool _mouseCheckUI;

		public GameObject MouseSensitivityUI;

		public TextMeshProUGUI ClickToNextText;

		public YesNoWindowPresenter YesNoWindowForMouse;

		public CanvasGroup FreeHSelectionCG;

		public TextMeshProUGUI VersionText;

		public Image ToTitle;

		protected IReactiveProperty<BaseScene.MenuPhase> _phase = new ReactiveProperty<BaseScene.MenuPhase>();

		public int MajorVersion;

		public int MinorVersion;

		public string Appendix = "";

		protected bool _loaded;

		private async void Start()
		{
			VersionText.text = $"Version {MajorVersion}.{MinorVersion}{Appendix}";
			MainMenuCG.alpha = 1f;
			MainMenuCG.blocksRaycasts = true;
			SubMenuCG.alpha = 0f;
			SubMenuCG.blocksRaycasts = false;
			SingletonManager<SceneContextManager>.Instance.PlayOP = false;
			SingletonManager<ScreenManager>.Instance.ChangeScreenResolution(SaveLoadManager.GlobalData.ScreenSize);
			SingletonManager<ScreenManager>.Instance.SetFullScreen(SaveLoadManager.GlobalData.FullScreen == 1);
			if (ClickSE.RuntimeKeyIsValid())
			{
				_clip = await Addressables.LoadAssetAsync<AudioClip>(ClickSE);
			}
			if (BGS.RuntimeKeyIsValid())
			{
				_bgs = await Addressables.LoadAssetAsync<AudioClip>(BGS);
				SingletonManager<SoundManager>.Instance.PlayBGS(_bgs);
			}
			Presenter.ManagedStart();
			Presenter.OnLoad.Subscribe(delegate
			{
				SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.FromLoad;
				SceneManager.LoadSceneAsync(1);
			}).AddTo(this);
			(from x in FromStart.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				SaveLoadManager.CreateNewSaveData();
				PlayClickSE();
				BlackCG.blocksRaycasts = true;
				SaveLoadManager.UnsavedData.NeedAutoSave = true;
				SingletonManager<SceneContextManager>.Instance.PlayOP = true;
				DOVirtual.Float(0f, 1f, 1f, delegate(float x)
				{
					BlackCG.alpha = x;
				}).OnComplete(delegate
				{
					NameInputImageCG.blocksRaycasts = true;
					DOVirtual.Float(0f, 1f, 1f, delegate(float x)
					{
						NameInputImageCG.alpha = x;
					}).Play();
				}).Play();
			}).AddTo(this);
			(from x in FromLoad.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				Presenter.SetActive(visible: true).Forget();
				PlayClickSE();
			}).AddTo(this);
			(from x in ToBonus.OnPointerClickAsObservable()
				where SaveLoadManager.GlobalData.IsCleared
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				MainMenuCG.alpha = 0f;
				MainMenuCG.blocksRaycasts = false;
				SubMenuCG.alpha = 1f;
				SubMenuCG.blocksRaycasts = true;
				PlayClickSE();
			}).AddTo(this);
			(from _ in Bonus1Text.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where !SingletonManager<SceneContextManager>.Instance.PlayingMovie
				select _).Subscribe(async delegate
			{
				SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.FreeScenario;
				await SceneManager.LoadSceneAsync(1);
			}).AddTo(this);
			(from _ in Bonus2Text.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where !SingletonManager<SceneContextManager>.Instance.PlayingMovie
				select _).Subscribe(delegate
			{
				FreeHSelectionCG.alpha = 1f;
				FreeHSelectionCG.blocksRaycasts = true;
			}).AddTo(this);
			(from x in EndGame.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				PlayClickSE();
				Application.Quit();
			}).AddTo(this);
			(from x in FreeScenarioMode.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				PlayClickSE();
				SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.FreeScenario;
				SceneManager.LoadSceneAsync(1);
			}).AddTo(this);
			(from x in BackToMainMenu.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				PlayClickSE();
				MainMenuCG.alpha = 1f;
				MainMenuCG.blocksRaycasts = true;
				SubMenuCG.alpha = 0f;
				SubMenuCG.blocksRaycasts = false;
			}).AddTo(this);
			(from x in ConfirmName.OnPointerEnterAsObservable()
				where NameInput.text.Length < 7
				select x).Subscribe(delegate
			{
				NameInputConfirmUnderline.color = Color.white;
			}).AddTo(this);
			ConfirmName.OnPointerExitAsObservable().Subscribe(delegate
			{
				NameInputConfirmUnderline.color = Color.clear;
			}).AddTo(this);
			(from _ in ConfirmName.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where NameInput.text.Length < 7
				select _).Subscribe(async delegate
			{
				bool yesNo = false;
				try
				{
					yesNo = await YesNoWindow.WaitForAnswer("これでよろしいですか？");
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				if (yesNo)
				{
					NameInputImageCG.blocksRaycasts = false;
					string playerName = ((NameInput.text.IsNullOrEmpty() || NameInput.text == "\u200b") ? "主人公" : NameInput.text);
					SaveLoadManager.UnsavedData.PlayerName = playerName;
					DOVirtual.Float(1f, 0f, 1f, delegate(float f)
					{
						NameInputImageCG.alpha = f;
					}).OnComplete(delegate
					{
						_mouseCheckUI = true;
						MouseSensitivityUI.SetActive(value: true);
						MouseSensitivitySlider.value = SaveLoadManager.GlobalData.GameOption.MouseSensitivity;
						MouseSensitivitySlider.OnValueChangedAsObservable().Subscribe(delegate(float x)
						{
							SaveLoadManager.GlobalData.GameOption.MouseSensitivity = x;
						}).AddTo(this);
						Manager.ManagedStart();
						_input = UnityEngine.Object.FindObjectOfType<InputManager>();
					}).Play();
				}
			}).AddTo(this);
			(from _ in ConfirmName.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where NameInput.text.Length >= 7
				select _).Subscribe(async delegate
			{
				await YesNoWindow.WaitForYes("入力できる文字数は５文字以下です。");
			}).AddTo(this);
			(from _ in ClickToNextText.OnPointerEnterAsObservable()
				where !Input.GetMouseButton(0)
				select _).Subscribe(delegate
			{
				ClickToNextText.color = Color.white;
			}).AddTo(this);
			ClickToNextText.OnPointerExitAsObservable().Subscribe(delegate
			{
				Color white = Color.white;
				white.a = 0.5f;
				ClickToNextText.color = white;
			}).AddTo(this);
			(from x in ClickToNextText.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(async delegate
			{
				if (await YesNoWindowForMouse.WaitForAnswer("マウス感度はこれでよいですか？（後でオプションから変更できます）。"))
				{
					_mouseCheckUI = false;
					SaveLoadManager.SaveGlobalData();
					SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.FromStart;
					await SceneManager.LoadSceneAsync(1);
				}
			}).AddTo(this);
			ToTitle.OnPointerClickAsObservable().Subscribe(async delegate
			{
				if (await YesNoWindow.WaitForAnswer("タイトルに戻りますか？"))
				{
					FreeHSelectionCG.alpha = 0f;
					FreeHSelectionCG.blocksRaycasts = false;
				}
			}).AddTo(this);
			ToTitle.OnPointerEnterAsObservable().Subscribe(delegate
			{
				Color uIColor = SaveLoadManager.GlobalData.GameOption.UIColor;
				uIColor.a = 0.5f;
				ToTitle.color = uIColor;
				SingletonManager<SoundManager>.Instance.PlaySE(_clip);
			}).AddTo(this);
			ToTitle.OnPointerExitAsObservable().Subscribe(delegate
			{
				ToTitle.color = SaveLoadManager.GlobalData.GameOption.UIColor;
			}).AddTo(this);
			GameObject gameObject = GameObject.Find("SingletonManagersFromBase");
			if (null != gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			_loaded = true;
		}

		private void PlayClickSE()
		{
			if (null != _clip)
			{
				SingletonManager<SoundManager>.Instance.PlaySE(_clip);
			}
		}

		private void Update()
		{
			InvalidNameCG.alpha = ((NameInput.text.Length >= 7) ? 1 : 0);
			if (_mouseCheckUI && null != _input && _input.MouseOn == MouseOn.Osawari)
			{
				Cursor.SetCursor(OsawariMouse, new Vector2(OsawariMouse.width / 2, OsawariMouse.height / 2), CursorMode.Auto);
			}
			else
			{
				Cursor.SetCursor(NormalMouse, new Vector2(NormalMouse.width / 2, NormalMouse.height / 2), CursorMode.Auto);
			}
			if (Input.GetKeyDown(KeyCode.F1))
			{
				_phase.Value = BaseScene.MenuPhase.Display;
			}
		}

		public IReactiveProperty<BaseScene.MenuPhase> GetMenuPhase()
		{
			return _phase;
		}

		public void SetActiveSceneModalWindowVisible(bool visible)
		{
		}

		public bool IsLoaded()
		{
			return _loaded;
		}
	}
}
