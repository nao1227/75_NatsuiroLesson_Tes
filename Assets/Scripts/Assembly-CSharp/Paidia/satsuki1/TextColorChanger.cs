using TMPro;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class TextColorChanger : MonoBehaviour
	{
		private TextMeshProUGUI Text;

		private void Start()
		{
			Text = GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			Text.color = SaveLoadManager.GlobalData.GameOption.UIColor;
		}
	}
}
