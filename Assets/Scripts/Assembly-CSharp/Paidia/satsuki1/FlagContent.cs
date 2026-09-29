using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class FlagContent : MonoBehaviour
	{
		public TextMeshProUGUI Title;

		public Image On;

		public Image Off;

		public Color SelectedColor;

		public Color UnselectedColor;

		private void Start()
		{
			On.color = new Color(1f, 1f, 1f, 0f);
		}

		public void SetName(string name)
		{
			Title.text = name;
		}

		public void SetState(bool state)
		{
			On.color = (state ? SelectedColor : UnselectedColor);
			Off.color = (state ? UnselectedColor : SelectedColor);
		}
	}
}
