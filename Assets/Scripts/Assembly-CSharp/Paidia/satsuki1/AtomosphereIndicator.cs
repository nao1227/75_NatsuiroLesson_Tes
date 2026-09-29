using System;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class AtomosphereIndicator : MonoBehaviour
	{
		public Sprite NervousImg;

		public Sprite ReliefImg;

		public Sprite ExcitedImg;

		public Sprite RutImage;

		public Sprite NervousName;

		public Sprite ReliefName;

		public Sprite ExcitedName;

		public Sprite RutName;

		public Image ImageHolder;

		public Image NameHolder;

		public Sprite ReliefMax;

		public Sprite ExciteMax;

		public Sprite RutMax;

		public void SetImage(AtomosphereName atomosphere)
		{
			switch (atomosphere)
			{
			case AtomosphereName.Nervous:
				ImageHolder.sprite = NervousImg;
				NameHolder.sprite = NervousName;
				break;
			case AtomosphereName.Relief:
				ImageHolder.sprite = ReliefImg;
				NameHolder.sprite = ReliefName;
				break;
			case AtomosphereName.Excited:
				ImageHolder.sprite = ExcitedImg;
				NameHolder.sprite = ExcitedName;
				break;
			case AtomosphereName.Rut:
				ImageHolder.sprite = RutImage;
				NameHolder.sprite = RutName;
				break;
			default:
				throw new Exception();
			}
		}

		public void SetAtomosphereRate(AtomosphereName atom, float rate)
		{
			ImageHolder.fillAmount = rate;
			if (rate == 1f)
			{
				switch (atom)
				{
				case AtomosphereName.Relief:
					ImageHolder.sprite = ReliefMax;
					break;
				case AtomosphereName.Excited:
					ImageHolder.sprite = ExciteMax;
					break;
				case AtomosphereName.Rut:
					ImageHolder.sprite = RutMax;
					break;
				}
			}
		}
	}
}
