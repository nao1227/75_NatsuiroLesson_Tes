using System.Collections;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/LipSynchDicing")]
	public class LipSynchDicing : LipSynch2d
	{
		private DicingImage dicing;

		private DicingImage Dicing => base.gameObject.GetComponentCache(ref dicing);

		protected override IEnumerator CoUpdateLipSync()
		{
			while (base.IsPlaying)
			{
				string pattern = Dicing.MainPattern;
				foreach (MiniAnimationData.Data data in base.AnimationData.DataList)
				{
					Dicing.TryChangePatternWithOption(pattern, base.LipTag, data.ComvertNameSimple());
					yield return TimeUtil.WaitForSeconds(base.UnscaledTime, data.Duration);
					while (base.IsPausing)
					{
						yield return null;
					}
				}
				Dicing.TryChangePatternWithOption(pattern, base.LipTag, "");
				if (!base.IsPlaying)
				{
					break;
				}
				yield return TimeUtil.WaitForSeconds(base.UnscaledTime, base.Interval);
				while (base.IsPausing)
				{
					yield return null;
				}
			}
			coLypSync = null;
		}

		protected override void OnStopLipSync()
		{
			base.OnStopLipSync();
			Dicing.TryChangePatternWithOption(Dicing.MainPattern, base.LipTag, "");
		}
	}
}
