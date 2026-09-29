using UnityEngine;
using UnityEngine.UI;

public class ScreenSize : MonoBehaviour
{
	public Text SizeText;

	private void Update()
	{
		SizeText.text = $"{Screen.width} x {Screen.height} ";
	}
}
