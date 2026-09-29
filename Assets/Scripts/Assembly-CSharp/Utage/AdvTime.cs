using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvTime")]
	public class AdvTime : MonoBehaviour
	{
		[SerializeField]
		private bool unscaled;

		public bool Unscaled
		{
			get
			{
				return unscaled;
			}
			set
			{
				unscaled = value;
			}
		}

		public float Time
		{
			get
			{
				if (!Unscaled)
				{
					return UnityEngine.Time.time;
				}
				return UnityEngine.Time.unscaledTime;
			}
		}

		public float DeltaTime => TimeUtil.GetDeltaTime(unscaled);

		public IEnumerator WaitForSeconds(float time)
		{
			yield return TimeUtil.WaitForSeconds(unscaled, time);
		}
	}
}
