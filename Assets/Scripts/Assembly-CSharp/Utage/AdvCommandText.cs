using UnityEngine;

namespace Utage
{
	public class AdvCommandText : AdvCommand, IAdvInitOnCreateEntity
	{
		public bool IsPageEnd { get; private set; }

		public bool IsNextBr { get; private set; }

		public AdvPageControllerType PageCtrlType { get; private set; }

		public AssetFile VoiceFile { get; private set; }

		private AdvScenarioPageData PageData { get; set; }

		private int IndexPageData { get; set; }

		public AdvCommandText(StringGridRow row, AdvSettingDataManager dataManager)
			: base(row)
		{
			InitVoiceFile(dataManager);
			PageCtrlType = ParseCellOptional(AdvColumnName.PageCtrl, AdvPageControllerType.InputBrPage);
			IsNextBr = AdvPageController.IsBrType(PageCtrlType);
			IsPageEnd = AdvPageController.IsPageEndType(PageCtrlType);
			if (AdvCommand.IsEditorErrorCheck)
			{
				TextData textData = new TextData(ParseCellLocalizedText());
				if (!string.IsNullOrEmpty(textData.ErrorMsg))
				{
					Debug.LogError(ToErrorString(textData.ErrorMsg));
				}
			}
		}

		public override void InitFromPageData(AdvScenarioPageData pageData)
		{
			PageData = pageData;
			IndexPageData = PageData.TextDataList.Count;
			PageData.AddTextData(this);
			PageData.InitMessageWindowName(this, ParseCellOptional(AdvColumnName.WindowType, ""));
		}

		public void InitOnCreateEntity(AdvCommand original)
		{
			AdvCommandText advCommandText = original as AdvCommandText;
			PageData = advCommandText.PageData;
			PageData.ChangeTextDataOnCreateEntity(advCommandText.IndexPageData, this);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (IsEmptyCell(AdvColumnName.Arg1))
			{
				engine.Page.CharacterInfo = null;
			}
			if (VoiceFile != null && (!engine.Page.CheckSkip() || !engine.Config.SkipVoiceAndSe))
			{
				engine.SoundManager.PlayVoice(engine.Page.CharacterLabel, VoiceFile);
			}
			engine.Page.UpdatePageTextData(this);
		}

		public override bool Wait(AdvEngine engine)
		{
			return engine.Page.IsWaitTextCommand;
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
			string text = ParseCellOptional(AdvColumnName.Voice, "");
			if (!string.IsNullOrEmpty(text))
			{
				VoiceFile = ParseVoiceSub(dataManager, text);
			}
		}

		public override bool IsTypePage()
		{
			return true;
		}

		public override bool IsTypePageEnd()
		{
			return IsPageEnd;
		}
	}
}
