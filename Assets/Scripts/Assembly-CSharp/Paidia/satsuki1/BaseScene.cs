using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utage;

namespace Paidia.satsuki1
{
	public class BaseScene : Scene, IOption
	{
		public enum MenuPhase
		{
			None = 0,
			Display = 1,
			Sound = 2,
			Game = 3,
			Text = 4,
			Save = 5,
			Mouse = 6
		}

		private Scene _activeScene;

		private AdventureScene _adventureScene;

		private FreeScenarioScene _freeScenarioScene;

		public ReactiveProperty<MenuPhase> Phase;

		private CancellationTokenSource _cts;

		private bool _unloadingScene;

		private bool _moveToFreeScenario;

		private bool _moveToFreeH;

		public SaveLoadUIPresenter SaveLoadUIPresenter;

		private Subject<SceneName> _onSceneChanged;

		private Scene _endingScene;

		public IObservable<SceneName> OnSceneChanged => _onSceneChanged;

		public IReactiveProperty<MenuPhase> GetMenuPhase()
		{
			return Phase;
		}

		public bool IsLoaded()
		{
			return Loaded;
		}

		public bool IsNotHScene(SceneName sceneName)
		{
			if (sceneName != SceneName.Adventure && sceneName != SceneName.Base && sceneName != SceneName.Title)
			{
				return sceneName == SceneName.FreeScenario;
			}
			return true;
		}

		protected override async UniTask SetUp()
		{
			_onSceneChanged = new Subject<SceneName>();
			_cts = new CancellationTokenSource();
			UtageManager utageManager = UnityEngine.Object.FindObjectOfType<UtageManager>();
			if (null == utageManager.AdvEngine)
			{
				utageManager.SetAdvEngine(UnityEngine.Object.FindObjectOfType<AdvEngine>());
			}
			_adventureScene = await LoadSceneAsync<AdventureScene>(SceneName.Adventure, _cts.Token);
			_adventureScene.SetActive(active: false);
			_moveToFreeScenario = SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeScenario;
			_moveToFreeH = SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH;
			foreach (SceneName value in Enum.GetValues(typeof(SceneName)))
			{
				if (IsNotHScene(value))
				{
					continue;
				}
				UnityEngine.SceneManagement.Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex((int)value);
				if (sceneByBuildIndex.isLoaded)
				{
					_activeScene = sceneByBuildIndex.GetRootGameObjects().First((GameObject x) => null != x.GetComponent<HScene>()).GetComponent<HScene>();
					break;
				}
			}
			await UniTask.WaitUntil(() => _activeScene?.Loaded ?? true);
			_activeScene?.SetActive(active: false);
			Phase = new ReactiveProperty<MenuPhase>(MenuPhase.None);
			await base.SetUp();
			SaveLoadUIPresenter.ManagedStart();
		}

		private async UniTask<T> LoadSceneAsync<T>(SceneName name, CancellationToken token) where T : Scene
		{
			await SceneManager.LoadSceneAsync((int)name, LoadSceneMode.Additive);
			if (token.IsCancellationRequested)
			{
				throw new OperationCanceledException();
			}
			return (from x in SceneManager.GetSceneByBuildIndex((int)name).GetRootGameObjects()
				where null != x.GetComponent<T>()
				select x).ToList()[0].GetComponent<T>();
		}

		private async void Update()
		{
			if (!Loaded || _unloadingScene || null == _adventureScene)
			{
				return;
			}
			base.HeaderEnabled = false;
			Scene activeScene = _activeScene;
			if ((object)activeScene == null || !activeScene.IsActive)
			{
				foreach (SceneName value in Enum.GetValues(typeof(SceneName)))
				{
					if (IsNotHScene(value))
					{
						continue;
					}
					UnityEngine.SceneManagement.Scene sceneByBuildIndex = SceneManager.GetSceneByBuildIndex((int)value);
					if (sceneByBuildIndex.isLoaded)
					{
						SingletonManager<SceneContextManager>.Instance.IsSaveEnabled = false;
						_onSceneChanged.OnNext(value);
						_activeScene = sceneByBuildIndex.GetRootGameObjects().First((GameObject x) => null != x.GetComponent<HScene>()).GetComponent<HScene>();
						_activeScene.SetTemporaryStatus(_adventureScene.GetCurrentTemporaryStatus());
						break;
					}
				}
			}
			if (_adventureScene.IsActive)
			{
				if (_adventureScene.IsWaitingForReload)
				{
					SaveLoadManager.UnsavedData.RefreshDay();
					await SceneManager.UnloadSceneAsync(2);
					_adventureScene = await LoadSceneAsync<AdventureScene>(SceneName.Adventure, _cts.Token);
					await UniTask.WaitUntil(() => _adventureScene.Loaded);
					_adventureScene.SetActive(active: true);
				}
				_adventureScene.ManagedUpdate();
				base.HeaderEnabled = _adventureScene.HeaderEnabled;
			}
			else if (null != _freeScenarioScene && _freeScenarioScene.IsActive)
			{
				base.HeaderEnabled = true;
				_freeScenarioScene.ManagedUpdate();
			}
			else if (_activeScene?.IsActive ?? false)
			{
				_activeScene.ManagedUpdate();
				base.HeaderEnabled = _activeScene.HeaderEnabled;
			}
			else
			{
				if (_unloadingScene)
				{
					return;
				}
				if (null == _activeScene)
				{
					base.HeaderEnabled = _adventureScene?.HeaderEnabled ?? false;
				}
				else
				{
					_unloadingScene = true;
					if (_activeScene.Name == SceneName.EndOfDay)
					{
						try
						{
							_adventureScene.OnlyDayEnd = true;
							await _activeScene.Unload();
							_unloadingScene = false;
							await _adventureScene.OnDayEnd();
						}
						catch (OperationCanceledException)
						{
							return;
						}
					}
					if (null == _activeScene)
					{
						return;
					}
					await _activeScene.Unload();
					_unloadingScene = false;
				}
				if (_moveToFreeScenario)
				{
					_moveToFreeScenario = false;
					_freeScenarioScene = await LoadSceneAsync<FreeScenarioScene>(SceneName.FreeScenario, _cts.Token);
					_freeScenarioScene.SetActive(active: true);
					await SceneManager.UnloadSceneAsync(2);
					return;
				}
				if (_moveToFreeH)
				{
					_moveToFreeH = false;
					if (SingletonManager<SceneContextManager>.Instance.FreeHTargetScene == SceneName.HScene3)
					{
						UnityEngine.Object.FindObjectOfType<UtageManager>().SetInt("study_cloth", SingletonManager<SceneContextManager>.Instance.FreeHCloth);
					}
					else
					{
						UnityEngine.Object.FindObjectOfType<UtageManager>().SetInt("cloth", SingletonManager<SceneContextManager>.Instance.FreeHCloth);
					}
					await _adventureScene.GoToHScene((int)SingletonManager<SceneContextManager>.Instance.FreeHTargetScene);
					return;
				}
				_onSceneChanged.OnNext(SceneName.Adventure);
				OnMoveToAdventure();
				_adventureScene.SetActive(active: true);
				if (SceneManager.GetSceneByBuildIndex(8).isLoaded)
				{
					await SceneManager.UnloadSceneAsync(8);
				}
			}
		}

		private async void OnMoveToAdventure()
		{
			SingletonManager<SceneContextManager>.Instance.IsSaveEnabled = true;
			if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FromStart)
			{
				_adventureScene.NoFading = true;
				SingletonManager<SceneContextManager>.Instance.IsSaveEnabled = false;
				UtageManager utage = UnityEngine.Object.FindObjectOfType<UtageManager>();
				CompositeDisposable disp = new CompositeDisposable();
				await utage.ShowUtageText(ScenarioLabel.Abstract_Start, this.GetCancellationTokenOnDestroy());
				switch (SaveLoadManager.UnsavedData.LastSelectedIndex)
				{
				case 0:
					SingletonManager<SceneContextManager>.Instance.IsDayRefreshed = true;
					utage.OnFinishPlaying.Subscribe(delegate
					{
						SingletonManager<SceneContextManager>.Instance.IsSaveEnabled = true;
						SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.InGame;
						SingletonManager<SceneContextManager>.Instance.PlayOP = false;
						disp.Dispose();
					}).AddTo(disp);
					utage.ShowUtageText(ScenarioLabel.OP, this.GetCancellationTokenOnDestroy()).Forget();
					break;
				case 1:
					_adventureScene.NoFading = false;
					SingletonManager<SceneContextManager>.Instance.IsDayRefreshed = true;
					SingletonManager<SceneContextManager>.Instance.IsSaveEnabled = true;
					SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.InGame;
					SingletonManager<SceneContextManager>.Instance.PlayOP = false;
					break;
				case 2:
					SaveLoadManager.UnsavedData.GlobalFlags.SkipToDay6();
					SaveLoadManager.UnsavedData.HasHSceneToday = true;
					SaveLoadManager.UnsavedData.NeedAutoSave = false;
					_adventureScene.GoToHScene(3).Forget();
					SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.InGame;
					SingletonManager<SceneContextManager>.Instance.PlayOP = false;
					break;
				}
			}
			else if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FromLoad)
			{
				SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.InGame;
				if (SaveLoadManager.UnsavedData.IsPlayingUtage)
				{
					_adventureScene.StartFromSaveData();
				}
			}
		}

		public override void SetUIVisible(bool visible)
		{
			_uiShown.Value = visible;
			_adventureScene?.SetUIVisible(visible);
			_activeScene?.SetUIVisible(visible);
		}

		public void SetActiveSceneModalWindowVisible(bool visible)
		{
			_activeScene?.SetModalWindowVisible(visible);
		}

		public Scene GetActiveScene()
		{
			Scene activeScene = _activeScene;
			if ((object)activeScene != null && activeScene.IsActive)
			{
				return _activeScene;
			}
			return _adventureScene;
		}

		private void OnDestroy()
		{
			_cts.Cancel();
		}

		private async void OnApplicationFocus(bool focus)
		{
			await UniTask.WaitUntil(() => !focus || !Input.GetMouseButtonDown(0));
			SingletonManager<SceneContextManager>.Instance.IsFocusOn = focus;
		}

		public async UniTask UnloadEveryScene()
		{
			_unloadingScene = true;
			if (null != _activeScene)
			{
				await _activeScene.Unload();
			}
			await _adventureScene.Unload();
		}
	}
}
