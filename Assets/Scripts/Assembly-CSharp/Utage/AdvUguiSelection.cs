using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvUguiSelection")]
	public class AdvUguiSelection : MonoBehaviour
	{
		public Text text;

		protected AdvSelection data;

		public AdvSelection Data => data;

		public virtual void Init(AdvSelection data, Action<AdvUguiSelection> ButtonClickedEvent, int index = 0)
		{
			this.data = data;
			this.data.Index = index;
			text.text = data.Text;
			GetComponent<Button>().onClick.AddListener(delegate
			{
				ButtonClickedEvent(this);
			});
		}

		public virtual void OnInitSelected(Color color)
		{
			text.color = color;
		}
	}
}
