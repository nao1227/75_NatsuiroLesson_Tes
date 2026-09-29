using UnityEngine;

namespace Utage
{
	public class AdvCommandTween : AdvCommandEffectBase, IAdvCommandEffect
	{
		protected iTweenData tweenData;

		private AdvITweenPlayer Player { get; set; }

		public AdvCommandTween(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			InitTweenData();
			if (tweenData.Type == iTweenType.Stop)
			{
				base.WaitType = AdvCommandWaitType.Add;
			}
			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(ToErrorString(tweenData.ErrorMsg));
			}
		}

		protected override void OnParse()
		{
			ParseEffectTarget(AdvColumnName.Arg1);
			if (!IsEmptyCell(AdvColumnName.WaitType))
			{
				ParseWait(AdvColumnName.WaitType);
			}
			else if (!IsEmptyCell(AdvColumnName.Arg6))
			{
				ParseWait(AdvColumnName.Arg6);
			}
			else
			{
				ParseWait(AdvColumnName.WaitType);
			}
		}

		protected virtual void InitTweenData()
		{
			string type = ParseCell<string>(AdvColumnName.Arg2);
			string arg = ParseCellOptional(AdvColumnName.Arg3, "");
			string easeType = ParseCellOptional(AdvColumnName.Arg4, "");
			string loopType = ParseCellOptional(AdvColumnName.Arg5, "");
			tweenData = new iTweenData(type, arg, easeType, loopType);
		}

		protected override void OnStartEffect(GameObject target, AdvEngine engine, AdvScenarioThread thread)
		{
			if (!string.IsNullOrEmpty(tweenData.ErrorMsg))
			{
				Debug.LogError(tweenData.ErrorMsg);
				OnComplete(thread);
				return;
			}
			Player = target.AddComponent<AdvITweenPlayer>();
			float skipSpeed = (engine.Page.CheckSkip() ? engine.Config.SkipSpped : 0f);
			Player.Init(tweenData, IsUnder2DSpace(target), engine.GraphicManager.PixelsToUnits, skipSpeed, engine.Time.Unscaled, delegate
			{
				Player = null;
				OnComplete(thread);
			});
			Player.Play();
			_ = Player.IsEndlessLoop;
		}

		private bool IsUnder2DSpace(GameObject target)
		{
			return targetType switch
			{
				AdvEffectManager.TargetType.MessageWindow => true, 
				AdvEffectManager.TargetType.Default => target.GetComponent<AdvGraphicObject>() != null, 
				_ => false, 
			};
		}

		public void OnEffectSkip()
		{
			if (Player == null)
			{
				Debug.LogError(" cant skip tween effect");
			}
			Player.SkipToEnd();
		}

		public void OnEffectFinalize()
		{
			Player = null;
		}
	}
}
