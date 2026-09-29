using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class SubMenuText : MonoBehaviour
	{
		private const float UNSELECTED_ALPHA = 0.25f;

		public TextMeshProUGUI TMP;

		public Image Touchable;

		public IObservable<PointerEventData> OnClick => from x in Touchable.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		public void SetText(string value)
		{
			TMP.text = value;
		}

		public void Select(bool select)
		{
			if (select)
			{
				TMP.color = new Color(TMP.color.r, TMP.color.g, TMP.color.b, 1f);
			}
			else
			{
				TMP.color = new Color(TMP.color.r, TMP.color.g, TMP.color.b, 0.25f);
			}
		}

		public void SetActive(bool active)
		{
			Touchable.gameObject.SetActive(active);
		}
	}
}
