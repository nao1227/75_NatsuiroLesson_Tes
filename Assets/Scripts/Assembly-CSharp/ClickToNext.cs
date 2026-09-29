using Cysharp.Threading.Tasks;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToNext : MonoBehaviour
{
	public SaveLoadUIPresenter Presenter;

	private void Start()
	{
		Presenter.ManagedStart();
		Presenter.OnLoad.Subscribe(delegate
		{
			SceneManager.LoadSceneAsync(1);
		}).AddTo(this);
		PlayerPrefs.SetInt("MoveToFreeScenario", 0);
	}

	public void GoToBaseScene(bool playOPMovie)
	{
		PlayerPrefs.SetInt("PlayOPMovie", playOPMovie ? 1 : 0);
		SceneManager.LoadSceneAsync(1);
	}

	public void GoToFreeScenarioScene()
	{
		PlayerPrefs.SetInt("MoveToFreeScenario", 1);
		SceneManager.LoadSceneAsync(1);
	}

	public void LoadData()
	{
		Presenter.SetActive(visible: true).Forget();
		PlayerPrefs.SetInt("PlayOPMovie", 0);
	}
}
