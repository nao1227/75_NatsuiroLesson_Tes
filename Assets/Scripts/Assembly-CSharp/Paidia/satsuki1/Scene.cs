using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paidia.satsuki1
{
	public abstract class Scene : MonoBehaviour
	{
		public GameObject RootObject;

		public SceneName Name;

		protected BoolReactiveProperty _uiShown;

		public bool Loaded;

		private BoolReactiveProperty _isModalWindowOpen;

		private BoolReactiveProperty _isResultWindowOpen;

		protected OsawariConditions _emptyConditions = OsawariConditions.Empty;

		public bool IsActive { get; protected set; } = true;

		public IReadOnlyReactiveProperty<bool> UIShown => _uiShown;

		public bool HeaderEnabled { get; protected set; }

		public IReadOnlyReactiveProperty<bool> IsModalWindowOpen => _isModalWindowOpen;

		public IReadOnlyReactiveProperty<bool> IsResultWindowOpen => _isResultWindowOpen;

		private void Start()
		{
			Loaded = false;
			_isModalWindowOpen = new BoolReactiveProperty(initialValue: false);
			_isResultWindowOpen = new BoolReactiveProperty(initialValue: false);
			SetUp().Forget();
		}

		public void EnableHeader(bool enable)
		{
			HeaderEnabled = enable;
		}

		protected virtual async UniTask SetUp()
		{
			if (!SceneManager.GetSceneByBuildIndex(1).isLoaded && !SceneManager.GetSceneByBuildIndex(0).isLoaded)
			{
				await SceneManager.LoadSceneAsync(0);
				return;
			}
			_uiShown = new BoolReactiveProperty(initialValue: true);
			await OnSetUp();
			Object.FindObjectOfType<UtageManager>().ForceEndText();
			SetUpEvent().Forget();
			Loaded = true;
		}

		protected virtual async UniTask OnSetUp()
		{
			await UniTask.Yield();
		}

		protected virtual void SetUpButtonEvents()
		{
		}

		protected virtual async UniTask SetUpEvent()
		{
			await UniTask.Yield();
		}

		public virtual void ManagedUpdate()
		{
		}

		public virtual void SetActive(bool active)
		{
			if (!active)
			{
				SingletonManager<SoundManager>.Instance.StopAll();
				Object.FindObjectOfType<FaceAnimationController>()?.CancelToken();
			}
			IsActive = active;
			if (null != RootObject)
			{
				RootObject.SetActive(active);
			}
		}

		public async UniTask Unload()
		{
			SingletonManager<SoundManager>.Instance.StopAll();
			await SceneManager.UnloadSceneAsync((int)Name);
		}

		public virtual void SetUIVisible(bool visible)
		{
			_uiShown.Value = visible;
		}

		public virtual void SetModalWindowVisible(bool visible)
		{
			_isModalWindowOpen.Value = visible;
		}

		public virtual void SetResultWindowVisible(bool visible)
		{
			_isResultWindowOpen.Value = visible;
		}

		public void SetTemporaryStatus(TemporaryStatus status)
		{
			OsawariManager osawariManager = Object.FindObjectOfType<OsawariManager>();
			if (null != osawariManager)
			{
				osawariManager.SetTemporaryStatus(status);
			}
			OsawariUIPresenter osawariUIPresenter = Object.FindObjectOfType<OsawariUIPresenter>();
			if (null != osawariUIPresenter)
			{
				osawariUIPresenter.SetUp().Forget();
			}
		}
	}
}
