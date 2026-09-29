using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/AdvLoadScene")]
	public class AdvLoadScene : MonoBehaviour
	{
		private void LoadScene(AdvCommandSendMessageByName command)
		{
			SceneManager.LoadScene(command.ParseCell<string>(AdvColumnName.Arg3));
		}
	}
}
