using System.Collections;
using UnityEngine;

namespace Utage
{
	public static class TimeUtil
	{
		public static float GetTime(bool unscaled)
		{
			if (!unscaled)
			{
				return Time.time;
			}
			return Time.unscaledTime;
		}

		public static float GetDeltaTime(bool unscaled)
		{
			if (!unscaled)
			{
				return Time.deltaTime;
			}
			return Time.unscaledDeltaTime;
		}

		public static IEnumerator WaitForSeconds(bool unscaled, float time)
		{
			if (unscaled)
			{
				yield return new WaitForSecondsRealtime(time);
			}
			else
			{
				yield return new WaitForSeconds(time);
			}
		}
	}
}
