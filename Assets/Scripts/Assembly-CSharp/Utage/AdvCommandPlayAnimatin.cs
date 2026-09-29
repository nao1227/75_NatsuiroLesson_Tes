using UnityEngine;

namespace Utage
{
	public class AdvCommandPlayAnimatin : AdvCommandEffectBase, IAdvCommandEffect
	{
		private string animationName;

		private AdvAnimationPlayer AnimationPlayer { get; set; }

		private bool EnableSave { get; set; }

		public AdvCommandPlayAnimatin(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			animationName = ParseCell<string>(AdvColumnName.Arg2);
			EnableSave = ParseCellOptional(AdvColumnName.Arg3, defaultVal: true);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			AdvAnimationData advAnimationData = engine.DataManager.SettingDataManager.AnimationSetting.Find(animationName);
			if (advAnimationData == null)
			{
				Debug.LogError(base.RowData.ToErrorString("Animation " + animationName + " is not found"));
				OnComplete(thread);
				return;
			}
			AnimationPlayer = target.AddComponent<AdvAnimationPlayer>();
			AnimationPlayer.AutoDestory = true;
			AnimationPlayer.EnableSave = EnableSave;
			AnimationPlayer.Play(advAnimationData.Clip, engine.Page.SkippedSpeed, delegate
			{
				OnComplete(thread);
			});
		}

		public void OnEffectSkip()
		{
			if (AnimationPlayer != null)
			{
				AnimationPlayer.SkipToEnd();
			}
		}

		public void OnEffectFinalize()
		{
			AnimationPlayer = null;
		}
	}
}
