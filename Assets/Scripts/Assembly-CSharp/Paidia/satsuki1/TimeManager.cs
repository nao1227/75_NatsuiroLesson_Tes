using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class TimeManager : SingletonManager<TimeManager>
	{
		public float PassedTime { get; private set; }

		public int PassedFrame { get; private set; }

		public float ModifiedPassedTime => PassedTime * 4f;

		private void Start()
		{
			PassedTime = 0f;
			PassedFrame = 0;
			Observable.EveryUpdate().Subscribe(delegate
			{
				PassedFrame++;
				PassedTime += Time.deltaTime;
			}).AddTo(this);
		}
	}
}
