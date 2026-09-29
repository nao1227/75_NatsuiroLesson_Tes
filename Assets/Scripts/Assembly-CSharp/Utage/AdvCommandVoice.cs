namespace Utage
{
	internal class AdvCommandVoice : AdvCommand
	{
		protected string characterLabel;

		protected AssetFile voiceFile;

		private float volume;

		private bool isLoop;

		public AdvCommandVoice(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			characterLabel = ParseCell<string>(AdvColumnName.Arg1);
			InitVoiceFile(dataManager);
			isLoop = ParseCellOptional(AdvColumnName.Arg2, defaultVal: false);
			volume = ParseCellOptional(AdvColumnName.Arg3, 1f);
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
				engine.SoundManager.PlayVoice(characterLabel, voiceFile, volume, isLoop);
			}
		}

		public override void OnChangeLanguage(AdvEngine engine)
		{
			if (!LanguageManagerBase.Instance.IgnoreLocalizeVoice)
			{
				InitVoiceFile(engine.DataManager.SettingDataManager);
			}
		}

		protected virtual void InitVoiceFile(AdvSettingDataManager dataManager)
		{
			string voiceName = ParseCell<string>(AdvColumnName.Voice);
			voiceFile = ParseVoiceSub(dataManager, voiceName);
		}
	}
}
