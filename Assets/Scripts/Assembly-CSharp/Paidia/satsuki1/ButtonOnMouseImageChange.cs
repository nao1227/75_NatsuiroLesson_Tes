using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class ButtonOnMouseImageChange : MonoBehaviour
	{
		public Image Image;

		public Sprite OriginalSprite;

		public Sprite OnMouseSprite;

		private void Start()
		{
			Image.OnPointerEnterAsObservable().Subscribe(delegate
			{
				Image.sprite = OnMouseSprite;
			}).AddTo(this);
			Image.OnPointerExitAsObservable().Subscribe(delegate
			{
				Image.sprite = OriginalSprite;
			}).AddTo(this);
		}
	}
}
