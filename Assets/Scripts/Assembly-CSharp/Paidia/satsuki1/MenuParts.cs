using UnityEngine;

namespace Paidia.satsuki1
{
	public abstract class MenuParts : MonoBehaviour
	{
		public GameObject Root;

		public void SetActive(bool active)
		{
			Root.SetActive(active);
		}

		public abstract void SetUp(MainMenuPresenter presenter);
	}
}
