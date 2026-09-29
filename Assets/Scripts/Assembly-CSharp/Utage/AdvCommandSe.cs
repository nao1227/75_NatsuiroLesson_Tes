using UnityEngine;

namespace Utage
{
	internal class AdvCommandSe : AdvCommand
	{
		private string label;

		private AssetFile file;

		private float volume;

		private bool isLoop;

		public AdvCommandSe(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			label = ParseCell<string>(AdvColumnName.Arg1);
			if (!dataManager.SoundSetting.Contains(label, SoundType.Se))
			{
				Debug.LogError(ToErrorString(label + " is not contained in file setting"));
			}
			isLoop = ParseCellOptional(AdvColumnName.Arg2, defaultVal: false);
			volume = ParseCellOptional(AdvColumnName.Arg3, 1f);
			file = AddLoadFile(dataManager.SoundSetting.LabelToFilePath(label, SoundType.Se), dataManager.SoundSetting.FindData(label));
		}

		public override void DoCommand(AdvEngine engine)
		{
			bool flag = false;
			if (engine.Page.CheckSkip() && engine.Config.SkipVoiceAndSe)
			{
				flag = ((!isLoop || !engine.Config.DontSkipLoopVoiceAndSe) ? true : false);
			}
			if (!flag)
			{
				engine.SoundManager.PlaySe(file, volume, label, SoundPlayMode.Add, isLoop);
			}
		}
	}
}
