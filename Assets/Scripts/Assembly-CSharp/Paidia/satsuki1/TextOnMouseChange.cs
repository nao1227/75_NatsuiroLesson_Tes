using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Paidia.satsuki1
{
	public class TextOnMouseChange : MonoBehaviour
	{
		public TextMeshProUGUI Text;

		public Color OnMouseColor = Color.white;

		public Color OnMouseExitColor = new Color(1f, 1f, 1f, 8f / 51f);

		public bool OnlyWhenCleared;

		private void Start()
		{
			Text.overrideColorTags = true;
			(from _ in Text.OnPointerEnterAsObservable()
				where !OnlyWhenCleared || SaveLoadManager.GlobalData.IsCleared
				select _).Subscribe(delegate
			{
				Text.color = OnMouseColor;
			}).AddTo(this);
			(from _ in Text.OnPointerExitAsObservable()
				where !OnlyWhenCleared || SaveLoadManager.GlobalData.IsCleared
				select _).Subscribe(delegate
			{
				Text.color = OnMouseExitColor;
			}).AddTo(this);
		}
	}
}
