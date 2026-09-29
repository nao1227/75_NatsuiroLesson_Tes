using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class BubbleManager : MonoBehaviour
	{
		public Subject<string> OnBubblePublish;

		private void Start()
		{
			OnBubblePublish = new Subject<string>();
		}

		public void CreateBubble(string text)
		{
			OnBubblePublish.OnNext(text);
		}

		private void OnDestroy()
		{
			OnBubblePublish.OnCompleted();
		}
	}
}
