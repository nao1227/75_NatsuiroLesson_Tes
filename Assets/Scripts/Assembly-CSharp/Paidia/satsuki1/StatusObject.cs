using System;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class StatusObject : MonoBehaviour
	{
		public int ExciteExtinctionSpeed;

		public int AtomosphereExtinctionSpeed;

		public int StimulusExtinctionSpeed;

		[NonSerialized]
		public PersistantStatus PersistantStatus;

		public Text DebugLog;

		public TemporaryStatus TemporaryStatus { get; protected set; } = new TemporaryStatus();

		private void Start()
		{
			PersistantStatus = SaveLoadManager.UnsavedData.PersistantStatus;
			if (TemporaryStatus == null)
			{
				TemporaryStatus = new TemporaryStatus();
			}
		}

		private void Update()
		{
			TemporaryStatus.Update(ExciteExtinctionSpeed, AtomosphereExtinctionSpeed, StimulusExtinctionSpeed);
			if (null != DebugLog)
			{
				DebugLog.text = $"Excite: {TemporaryStatus.Feelings.Excite}, Atomosphere:{TemporaryStatus.Feelings.Atomosphere}, Stimulus: {TemporaryStatus.Feelings.Stimulus}, Cloth: {TemporaryStatus.Cloth}";
			}
		}
	}
}
