using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class BubbleHolder : MonoBehaviour
	{
		private List<SpeechBubble> Bubbles = new List<SpeechBubble>();

		public int MAX = 5;

		private const int HEIGHT = 134;

		private const int MARGIN = 10;

		public void AddBubble(SpeechBubble bubble)
		{
			bubble.OnClick.Subscribe(delegate(int x)
			{
				RemoveBubble(x);
				SetBubblesPosition();
			}).AddTo(bubble);
			bubble.OnDismiss.Subscribe(delegate(int x)
			{
				Bubbles.RemoveAt(x);
				ResetIndex();
				SetBubblesPosition();
			}).AddTo(bubble);
			Bubbles.Add(bubble);
			if (Bubbles.Count > MAX)
			{
				RemoveBubble(0);
			}
			ResetIndex();
			SetBubblesPosition();
		}

		private void RemoveBubble(int index)
		{
			Bubbles[index].Dismiss();
		}

		private void ResetIndex()
		{
			for (int i = 0; i < Bubbles.Count; i++)
			{
				Bubbles[i].Index = i;
			}
		}

		private void SetBubblesPosition()
		{
			foreach (SpeechBubble bubble in Bubbles)
			{
				bubble.transform.localPosition = new Vector2(0f, bubble.Index * 144);
			}
		}
	}
}
