using UnityEngine;

namespace Utage
{
	internal abstract class AdvCommandRuleFadeBase : AdvCommandEffectBase, IAdvCommandEffect
	{
		protected IAdvFadeSkippable Fade { get; set; }

		protected AdvTransitionArgs TransitionArgs { get; set; }

		protected AdvCommandRuleFadeBase(StringGridRow row)
			: base(row)
		{
			string textureName = ParseCell<string>(AdvColumnName.Arg2);
			float vague = ParseCellOptional(AdvColumnName.Arg3, 0.2f);
			float time = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
			TransitionArgs = new AdvTransitionArgs(textureName, vague, time);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			Fade = target.GetComponentInChildren<IAdvFadeSkippable>(includeInactive: true);
			if (Fade == null)
			{
				Debug.LogError("Can't find [ " + base.TargetName + " ]");
				OnComplete(thread);
			}
			else
			{
				OnStartFade(target, engine, thread);
			}
		}

		protected abstract void OnStartFade(GameObject target, AdvEngine engine, AdvScenarioThread thread);

		public void OnEffectSkip()
		{
			if (Fade != null)
			{
				OnSkipFade();
			}
		}

		protected virtual void OnSkipFade()
		{
			Fade.SkipRuleFade();
		}

		public void OnEffectFinalize()
		{
			Fade = null;
		}
	}
}
