using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paidia.satsuki1
{
	public class HeaderUIPresenter : MonoBehaviour
	{
		public IconObject InvisibleButton;

		public IconObject StatusButton;

		public IconObject SettingsButton;

		public IconObject PowerOfButton;

		public IconObject TutorialButton;

		public IconObject SaveButton;

		public IconObject LoadButton;

		public UtageManager UtageManager;

		public BaseScene Scene;

		public TutorialPresenter TutorialPresenter;

		public StatusUIPresenter StatusUIPresenter;

		public TextMeshProUGUI DescriptionText;

		public YesNoWindowPresenter YesNoWindow;

		private SceneName _currentScene = SceneName.Base;

		private bool _showUI;

		public CanvasGroup CG;

		private CompositeDisposable _loadDisposable;

		private Camera _vfxCamera;

		private async void OnEnable()
		{
			await UniTask.WaitUntil(() => Scene.Loaded);
			await InvisibleButton.ManagedStart(() => true);
			await StatusButton.ManagedStart(() => true);
			await SettingsButton.ManagedStart(() => true);
			await PowerOfButton.ManagedStart(() => true);
			await TutorialButton.ManagedStart(() => true);
			await SaveButton.ManagedStart(CanSaveOrLoad);
			await LoadButton.ManagedStart(CanSaveOrLoad);
			InvisibleButton.OnClick.Subscribe(delegate
			{
				bool flag = !Scene.UIShown.Value;
				InvisibleButton.ChangeIcon((!flag) ? 1 : 0);
				UtageManager.ShowUtageWindow(flag);
				Scene.SetUIVisible(flag);
				SetDescriptionText(InvisibleButton.GetDescription());
			}).AddTo(this);
			Scene.UIShown.Subscribe(delegate(bool x)
			{
				_showUI = x;
				StatusButton.gameObject.SetActive(x);
				SettingsButton.gameObject.SetActive(x);
				PowerOfButton.gameObject.SetActive(x);
				TutorialButton.gameObject.SetActive(x);
				SaveButton.gameObject.SetActive(x && _currentScene == SceneName.Adventure);
				LoadButton.gameObject.SetActive(x && _currentScene == SceneName.Adventure);
			}).AddTo(this);
			StatusButton.OnClick.Subscribe(delegate
			{
				SingletonManager<SceneContextManager>.Instance.AllowUtage = false;
				UtageManager.StopAuto();
				Object.FindObjectOfType<HScene>()?.SetModalWindowVisible(visible: true);
				SetVFXCameraEnable(enable: false);
				StatusUIPresenter.DrawData(delegate
				{
					SetVFXCameraEnable(enable: true);
				});
			}).AddTo(this);
			SettingsButton.OnClick.Subscribe(delegate
			{
				UtageManager.StopAuto();
				SetVFXCameraEnable(enable: false);
				if (Scene.Phase.Value == BaseScene.MenuPhase.None)
				{
					SingletonManager<SceneContextManager>.Instance.AllowUtage = false;
					Scene.Phase.Value = BaseScene.MenuPhase.Display;
				}
				else
				{
					Scene.Phase.Value = BaseScene.MenuPhase.None;
				}
			}).AddTo(this);
			PowerOfButton.OnClick.Subscribe(async delegate
			{
				UtageManager.StopAuto();
				SingletonManager<SceneContextManager>.Instance.AllowUtage = false;
				Object.FindObjectOfType<HScene>()?.SetModalWindowVisible(visible: true);
				if (await YesNoWindow.WaitForAnswer("タイトルに戻りますか？"))
				{
					SingletonManager<SoundManager>.Instance.StopAll();
					await SceneManager.LoadSceneAsync(0);
				}
				Object.FindObjectOfType<HScene>()?.SetModalWindowVisible(visible: false);
				SingletonManager<SceneContextManager>.Instance.AllowUtage = true;
			}).AddTo(this);
			TutorialButton.OnClick.Subscribe(delegate
			{
				UtageManager.StopAuto();
				SetVFXCameraEnable(enable: false);
				SingletonManager<SceneContextManager>.Instance.AllowUtage = false;
				Object.FindObjectOfType<HScene>()?.SetModalWindowVisible(visible: true);
				OsawariManager osawariManager = Object.FindObjectOfType<OsawariManager>();
				TutorialName tutorialName = Scene.GetActiveScene().Name switch
				{
					SceneName.HScene1 => (osawariManager.ContextManager.Context == OsawariContext.Osawari) ? TutorialName.Tutorial_H : TutorialName.Tutorial_Fellatio, 
					SceneName.HScene2 => TutorialName.Tutorial_Bath, 
					SceneName.HScene3 => TutorialName.Tutorial_Study, 
					SceneName.HScene4 => (osawariManager.ContextManager.Context == OsawariContext.Osawari) ? TutorialName.Tutorial_Pool : TutorialName.Tutorial_Paizuri, 
					SceneName.EndOfDay => TutorialName.Tutorial_Kiss, 
					_ => TutorialName.Tutorial_Main, 
				};
				TutorialPresenter.Show(tutorialName, delegate
				{
					SetVFXCameraEnable(enable: true);
				});
			}).AddTo(this);
			SaveButton.OnClick.Subscribe(delegate
			{
				UtageManager.StopAuto();
				SetVFXCameraEnable(enable: false);
				SingletonManager<SceneContextManager>.Instance.AllowUtage = false;
				Scene.SaveLoadUIPresenter.Mode = SaveLoadMode.Save;
				Scene.SaveLoadUIPresenter.SetActive(visible: true, delegate
				{
					SetVFXCameraEnable(enable: true);
				}).Forget();
			}).AddTo(this);
			LoadButton.OnClick.Subscribe(delegate
			{
				_loadDisposable?.Dispose();
				_loadDisposable = new CompositeDisposable();
				UtageManager.StopAuto();
				SetVFXCameraEnable(enable: false);
				SingletonManager<SceneContextManager>.Instance.AllowUtage = false;
				Scene.SaveLoadUIPresenter.Mode = SaveLoadMode.Load;
				Scene.SaveLoadUIPresenter.OnLoad.Subscribe(async delegate
				{
					SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.FromLoad;
					GameObject singletons = GameObject.Find("SingletonManagersFromBase");
					await Scene.UnloadEveryScene();
					Object.Destroy(singletons);
					await SceneManager.LoadSceneAsync(1);
				}).AddTo(_loadDisposable);
				Scene.SaveLoadUIPresenter.SetActive(visible: true, delegate
				{
					SetVFXCameraEnable(enable: true);
				}).Forget();
			}).AddTo(this);
			InvisibleButton.OnMouse.Subscribe(delegate(string x)
			{
				SetDescriptionText(x);
			}).AddTo(this);
			StatusButton.OnMouse.Subscribe(delegate(string x)
			{
				SetDescriptionText(x);
			}).AddTo(this);
			SettingsButton.OnMouse.Subscribe(delegate(string x)
			{
				SetDescriptionText(x);
			}).AddTo(this);
			PowerOfButton.OnMouse.Subscribe(delegate(string x)
			{
				SetDescriptionText(x);
			}).AddTo(this);
			TutorialButton.OnMouse.Subscribe(delegate(string x)
			{
				SetDescriptionText(x);
			}).AddTo(this);
			SaveButton.OnMouse.Subscribe(delegate(string x)
			{
				SetDescriptionText(x);
			}).AddTo(this);
			LoadButton.OnMouse.Subscribe(delegate(string x)
			{
				SetDescriptionText(x);
			}).AddTo(this);
			InvisibleButton.OnMouseExit.Subscribe(delegate
			{
				SetDescriptionText("");
			}).AddTo(this);
			StatusButton.OnMouseExit.Subscribe(delegate
			{
				SetDescriptionText("");
			}).AddTo(this);
			SettingsButton.OnMouseExit.Subscribe(delegate
			{
				SetDescriptionText("");
			}).AddTo(this);
			PowerOfButton.OnMouseExit.Subscribe(delegate
			{
				SetDescriptionText("");
			}).AddTo(this);
			TutorialButton.OnMouseExit.Subscribe(delegate
			{
				SetDescriptionText("");
			}).AddTo(this);
			SaveButton.OnMouseExit.Subscribe(delegate
			{
				SetDescriptionText("");
			}).AddTo(this);
			LoadButton.OnMouseExit.Subscribe(delegate
			{
				SetDescriptionText("");
			}).AddTo(this);
			Scene.OnSceneChanged.Subscribe(delegate(SceneName x)
			{
				_currentScene = x;
				if (x == SceneName.Adventure)
				{
					SaveButton.ShowIcon(show: true);
					LoadButton.ShowIcon(show: true);
				}
				else
				{
					SaveButton.ShowIcon(show: false);
					LoadButton.ShowIcon(show: false);
				}
			}).AddTo(this);
		}

		private bool CanSaveOrLoad()
		{
			return _currentScene == SceneName.Adventure;
		}

		public void SetDescriptionText(string text)
		{
			DescriptionText.text = text;
		}

		private void Update()
		{
			if (SaveLoadManager.UnsavedData != null)
			{
				DescriptionText.color = SaveLoadManager.GlobalData.GameOption.UIColor;
			}
			if (SingletonManager<SceneContextManager>.Instance.PlayingMovie || !Scene.HeaderEnabled)
			{
				CG.alpha = 0f;
				CG.blocksRaycasts = false;
				Scene.SaveLoadUIPresenter.SetActive(visible: false).Forget();
			}
			else
			{
				CG.alpha = 1f;
				CG.blocksRaycasts = true;
			}
			if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH || SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeScenario)
			{
				SaveButton.SetEnable(isAble: false);
				LoadButton.SetEnable(isAble: false);
				StatusButton.SetEnable(isAble: false);
			}
		}

		private void SetVFXCameraEnable(bool enable)
		{
			if (_vfxCamera == null)
			{
				_vfxCamera = GameObject.Find("VFXCamera")?.GetComponent<Camera>();
			}
			if (_vfxCamera != null)
			{
				_vfxCamera.enabled = enable;
			}
		}
	}
}
