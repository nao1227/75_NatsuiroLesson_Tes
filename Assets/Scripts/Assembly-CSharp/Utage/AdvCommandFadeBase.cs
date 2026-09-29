using UnityEngine;
using UtageExtensions;

namespace Utage
{
	internal abstract class AdvCommandFadeBase : AdvCommandEffectBase, IAdvCommandEffect
	{
		private float time;

		private bool inverse;

		private Color color;

		private string ruleImage;

		private float vague;

		private Timer Timer { get; set; }

		public AdvCommandFadeBase(StringGridRow row, bool inverse)
			: base(row)
		{
			this.inverse = inverse;
		}

		protected override void OnParse()
		{
			color = ParseCellOptional(AdvColumnName.Arg1, Color.white);
			if (IsEmptyCell(AdvColumnName.Arg2))
			{
				targetName = "SpriteCamera";
			}
			else
			{
				targetName = ParseCell<string>(AdvColumnName.Arg2);
			}
			time = ParseCellOptional(AdvColumnName.Arg6, 0.2f);
			ruleImage = ParseCellOptional(AdvColumnName.Arg3, "");
			vague = ParseCellOptional(AdvColumnName.Arg4, 0.2f);
			targetType = AdvEffectManager.TargetType.Camera;
			ParseWait(AdvColumnName.WaitType);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			Camera componentInChildren = target.GetComponentInChildren<Camera>(includeInactive: true);
			ImageEffectBase imageEffect = null;
			IImageEffectStrength effectStrength = null;
			float start;
			float end;
			if (string.IsNullOrEmpty(ruleImage))
			{
				bool flag = componentInChildren.gameObject.GetComponent<RuleFade>();
				if (flag)
				{
					componentInChildren.gameObject.SafeRemoveComponent<RuleFade>();
				}
				ImageEffectUtil.TryGetComonentCreateIfMissing(ImageEffectType.ColorFade.ToString(), out imageEffect, out var alreadyEnabled, componentInChildren.gameObject);
				effectStrength = imageEffect as IImageEffectStrength;
				ColorFade colorFade = imageEffect as ColorFade;
				if (inverse)
				{
					start = (flag ? 1f : colorFade.color.a);
					end = 0f;
				}
				else
				{
					start = (alreadyEnabled ? colorFade.Strength : 0f);
					end = color.a;
				}
				colorFade.enabled = true;
				colorFade.color = color;
			}
			else
			{
				componentInChildren.gameObject.SafeRemoveComponent<ColorFade>();
				ImageEffectUtil.TryGetComonentCreateIfMissing(ImageEffectType.RuleFade.ToString(), out imageEffect, out var alreadyEnabled2, componentInChildren.gameObject);
				effectStrength = imageEffect as IImageEffectStrength;
				RuleFade ruleFade = imageEffect as RuleFade;
				ruleFade.ruleTexture = engine.EffectManager.FindRuleTexture(ruleImage);
				ruleFade.vague = vague;
				if (inverse)
				{
					start = 1f;
					end = 0f;
				}
				else
				{
					start = (alreadyEnabled2 ? ruleFade.Strength : 0f);
					end = 1f;
				}
				ruleFade.enabled = true;
				ruleFade.color = color;
			}
			Timer = componentInChildren.gameObject.AddComponent<Timer>();
			Timer.AutoDestroy = true;
			Timer.StartTimer(engine.Page.ToSkippedTime(time), engine.Time.Unscaled, delegate(Timer x)
			{
				effectStrength.Strength = x.GetCurve(start, end);
			}, delegate
			{
				OnComplete(thread);
				if (inverse)
				{
					imageEffect.enabled = false;
					imageEffect.RemoveComponentMySelf();
				}
			});
		}

		public void OnEffectSkip()
		{
			if (!(Timer == null))
			{
				Timer.SkipToEnd();
			}
		}

		public void OnEffectFinalize()
		{
			Timer = null;
		}
	}
}
