using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class FreeHSelectionImage : MonoBehaviour
	{
		public Image CoverImage;

		public Image Image;

		public TextMeshProUGUI Title;

		public IObservable<PointerEventData> OnClick => from x in CoverImage.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		private void Start()
		{
			CoverImage.OnPointerEnterAsObservable().Subscribe(delegate
			{
				Image.color = Color.white;
				Title.color = Color.white;
			}).AddTo(this);
			CoverImage.OnPointerExitAsObservable().Subscribe(delegate
			{
				Image.color = Color.gray;
				Title.color = Color.gray;
			}).AddTo(this);
		}
	}
}
