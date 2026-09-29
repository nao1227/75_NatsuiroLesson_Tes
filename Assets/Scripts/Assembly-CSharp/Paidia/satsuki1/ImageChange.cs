using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class ImageChange : MonoBehaviour
	{
		public List<Sprite> Sprites;

		public Image Image;

		private int _index;

		public void ChangeImage()
		{
			if (_index >= Sprites.Count)
			{
				_index = 0;
			}
			Image.DOColor(Color.clear, 1.5f).OnComplete(delegate
			{
				Image.sprite = Sprites[_index];
				Image.sprite = Sprites[_index];
				Image.DOColor(Color.white, 1.5f).Play();
				_index++;
			}).Play();
		}
	}
}
