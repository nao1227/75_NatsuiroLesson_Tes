using UnityEngine;

namespace Utage
{
	internal class AdvCommandRuleFadeOut : AdvCommandRuleFadeBase
	{
		public AdvCommandRuleFadeOut(StringGridRow row)
			: base(row)
		{
		}

		protected override void OnStartFade(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			base.Fade.RuleFadeOut(engine, base.TransitionArgs, delegate
			{
				OnComplete(thread);
			});
		}
	}
}
