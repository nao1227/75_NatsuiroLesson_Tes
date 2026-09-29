using UnityEngine;

namespace Utage
{
	internal class AdvCommandImageEffectBase : AdvCommandEffectBase, IAdvCommandEffect
	{
		private string animationName;

		private float time;

		private bool inverse;

		private string imageEffectType { get; set; }

		private Timer Timer { get; set; }

		private AdvAnimationPlayer AnimationPlayer { get; set; }

		public AdvCommandImageEffectBase(StringGridRow row, AdvSettingDataManager dataManager, bool inverse)
			: base(row)
		{
			this.inverse = inverse;
			targetType = AdvEffectManager.TargetType.Camera;
			imageEffectType = base.RowData.ParseCell<string>(AdvColumnName.Arg2.ToString());
			animationName = ParseCellOptional(AdvColumnName.Arg3, "");
			time = ParseCellOptional(AdvColumnName.Arg6, 0f);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			if (imageEffectType == "All")
			{
				OnStartAll(target, engine, thread);
				return;
			}
			Camera componentInChildren = target.GetComponentInChildren<Camera>(includeInactive: true);
			if (!ImageEffectUtil.TryGetComonentCreateIfMissing(imageEffectType, out var imageEffect, out var _, componentInChildren.gameObject))
			{
				Complete(imageEffect, thread);
				return;
			}
			if (!inverse)
			{
				imageEffect.enabled = true;
			}
			bool enableAnimation = !string.IsNullOrEmpty(animationName);
			bool flag = imageEffect is IImageEffectStrength;
			if (!flag && !enableAnimation)
			{
				Complete(imageEffect, thread);
				return;
			}
			if (flag)
			{
				IImageEffectStrength fade = imageEffect as IImageEffectStrength;
				float start = (inverse ? fade.Strength : 0f);
				float end = ((!inverse) ? 1 : 0);
				Timer = componentInChildren.gameObject.AddComponent<Timer>();
				Timer.AutoDestroy = true;
				Timer.StartTimer(engine.Page.ToSkippedTime(time), engine.Time.Unscaled, delegate(Timer x)
				{
					fade.Strength = x.GetCurve(start, end);
				}, delegate
				{
					if (!enableAnimation)
					{
						Complete(imageEffect, thread);
					}
				});
			}
			if (!enableAnimation)
			{
				return;
			}
			AdvAnimationData advAnimationData = engine.DataManager.SettingDataManager.AnimationSetting.Find(animationName);
			if (advAnimationData == null)
			{
				Debug.LogError(base.RowData.ToErrorString("Animation " + animationName + " is not found"));
				Complete(imageEffect, thread);
				return;
			}
			AnimationPlayer = componentInChildren.gameObject.AddComponent<AdvAnimationPlayer>();
			AnimationPlayer.AutoDestory = true;
			AnimationPlayer.EnableSave = true;
			AnimationPlayer.Play(advAnimationData.Clip, engine.Page.SkippedSpeed, delegate
			{
				Complete(imageEffect, thread);
			});
		}

		private void OnStartAll(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			ImageEffectBase[] components = target.GetComponentInChildren<Camera>(includeInactive: true).gameObject.GetComponents<ImageEffectBase>();
			if (components.Length == 0)
			{
				OnComplete(thread);
				return;
			}
			ImageEffectBase[] array = components;
			foreach (ImageEffectBase imageEffectBase in array)
			{
				if (!(imageEffectBase is ColorFade))
				{
					Object.DestroyImmediate(imageEffectBase);
				}
			}
			OnComplete(thread);
		}

		private void Complete(ImageEffectBase imageEffect, AdvScenarioThread thread)
		{
			if (inverse)
			{
				Object.DestroyImmediate(imageEffect);
			}
			OnComplete(thread);
		}

		public void OnEffectSkip()
		{
			if (Timer != null)
			{
				Timer.SkipToEnd();
			}
			if (AnimationPlayer != null)
			{
				AnimationPlayer.SkipToEnd();
			}
		}

		public void OnEffectFinalize()
		{
			Timer = null;
			AnimationPlayer = null;
		}
	}
}
