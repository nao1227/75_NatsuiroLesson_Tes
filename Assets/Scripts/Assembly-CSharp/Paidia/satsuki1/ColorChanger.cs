using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class ColorChanger : MonoBehaviour
	{
		private Image Image;

		private void Start()
		{
			Image = GetComponent<Image>();
		}

		private void Update()
		{
			Image.color = SaveLoadManager.GlobalData.GameOption.UIColor;
		}
	}
}
