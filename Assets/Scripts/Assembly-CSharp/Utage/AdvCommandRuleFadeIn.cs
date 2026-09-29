using UnityEngine;

namespace Utage
{
	internal class AdvCommandRuleFadeIn : AdvCommandRuleFadeBase
	{
		public AdvCommandRuleFadeIn(StringGridRow row)
			: base(row)
		{
		}

		protected override void OnStartFade(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			base.Fade.RuleFadeIn(engine, base.TransitionArgs, delegate
			{
				OnComplete(thread);
			});
		}
	}
}
