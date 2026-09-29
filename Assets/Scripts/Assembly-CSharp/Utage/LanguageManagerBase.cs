using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utage
{
	public abstract class LanguageManagerBase : ScriptableObject
	{
		private static LanguageManagerBase instance;

		private const string Auto = "Auto";

		[SerializeField]
		protected string language = "Auto";

		[SerializeField]
		protected string defaultLanguage = "Japanese";

		[SerializeField]
		protected string dataLanguage = "";

		[SerializeField]
		private List<TextAsset> languageData = new List<TextAsset>();

		[SerializeField]
		private bool ignoreLocalizeUiText;

		[SerializeField]
		private bool ignoreLocalizeVoice = true;

		[SerializeField]
		private List<string> voiceLanguages = new List<string>();

		[SerializeField]
		private LanguageBlankTextType blankTextType;

		[FormerlySerializedAs("textColumnErrorCheckLanguages")]
		[SerializeField]
		private List<string> textColumnLanguages = new List<string>();

		private string currentLanguage;

		private string voiceLanguage = "";

		public static LanguageManagerBase Instance
		{
			get
			{
				if (instance == null)
				{
					if ((bool)CustomProjectSetting.Instance)
					{
						instance = CustomProjectSetting.Instance.Language;
					}
					if (instance != null)
					{
						instance.Init();
					}
				}
				return instance;
			}
		}

		public string Language => language;

		public string DefaultLanguage => defaultLanguage;

		public string DataLanguage => dataLanguage;

		public bool IgnoreLocalizeUiText => ignoreLocalizeUiText;

		public bool IgnoreLocalizeVoice => ignoreLocalizeVoice;

		public List<string> VoiceLanguages => voiceLanguages;

		public LanguageBlankTextType BlankTextType => blankTextType;

		public List<string> TextColumnLanguages => textColumnLanguages;

		public Action OnChangeLanugage { get; set; }

		public string CurrentLanguage
		{
			get
			{
				return currentLanguage;
			}
			set
			{
				if (currentLanguage != value)
				{
					currentLanguage = value;
					RefreshCurrentLanguage();
				}
			}
		}

		public string VoiceLanguage
		{
			get
			{
				return voiceLanguage;
			}
			set
			{
				if (voiceLanguage != value)
				{
					voiceLanguage = value;
					RefreshCurrentLanguage();
				}
			}
		}

		public string CurrentVoiceLanguage
		{
			get
			{
				if (!string.IsNullOrEmpty(VoiceLanguage))
				{
					return VoiceLanguage;
				}
				return CurrentLanguage;
			}
		}

		private LanguageData Data { get; set; }

		public List<string> Languages => Data.Languages;

		private void OnEnable()
		{
			Init();
		}

		private void Init()
		{
			Data = new LanguageData();
			foreach (TextAsset languageDatum in languageData)
			{
				if (!(languageDatum == null))
				{
					Data.OverwriteData(languageDatum);
				}
			}
			currentLanguage = ((string.IsNullOrEmpty(language) || language == "Auto") ? Application.systemLanguage.ToString() : language);
			voiceLanguage = "";
			RefreshCurrentLanguage();
		}

		protected void RefreshCurrentLanguage()
		{
			if (!(Instance != this))
			{
				if (OnChangeLanugage != null)
				{
					OnChangeLanugage();
				}
				OnRefreshCurrentLanguage();
			}
		}

		protected abstract void OnRefreshCurrentLanguage();

		public string LocalizeText(string dataName, string key)
		{
			if (Data.ContainsKey(key) && Data.TryLocalizeText(out var text, CurrentLanguage, DefaultLanguage, key, dataName))
			{
				return text;
			}
			Debug.LogError(key + " is not found in " + dataName);
			return key;
		}

		public string LocalizeText(string key)
		{
			string text = key;
			TryLocalizeText(key, out text);
			return text;
		}

		public bool TryLocalizeText(string key, out string text)
		{
			text = key;
			if (Data.ContainsKey(key) && Data.TryLocalizeText(out text, CurrentLanguage, DefaultLanguage, key))
			{
				return true;
			}
			return false;
		}

		public string DefaultLanuguageText(string key)
		{
			if (Data.ContainsKey(key) && Data.TryLocalizeText(out var text, DefaultLanguage, DefaultLanguage, key))
			{
				return text;
			}
			Debug.LogError(key + " is not found in language key");
			return "";
		}

		internal void OverwriteData(StringGrid grid)
		{
			Data.OverwriteData(grid);
			RefreshCurrentLanguage();
		}

		public bool IsEmptyTextCommand(StringGridRow row)
		{
			LanguageBlankTextType languageBlankTextType = BlankTextType;
			if ((uint)(languageBlankTextType - 1) <= 1u)
			{
				foreach (string textColumnLanguage in TextColumnLanguages)
				{
					if (!row.IsEmptyCell(textColumnLanguage))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}

		public string ParseCellLocalizedText(StringGridRow row, string defaultColumnName)
		{
			switch (BlankTextType)
			{
			case LanguageBlankTextType.SwapDefaultLanguage:
				return ParseCellLocalizedTextBySwapDefaultLanguage(row, defaultColumnName);
			case LanguageBlankTextType.NoBlankText:
				return ParseCellLocalizedTextByNoSwap(row, defaultColumnName);
			case LanguageBlankTextType.AllowBlankText:
				return ParseCellLocalizedTextByNoSwap(row, defaultColumnName);
			default:
				Debug.LogError(row.ToErrorString(BlankTextType.ToString() + " is Unknown Type"));
				return "";
			}
		}

		private string ParseCellLocalizedTextBySwapDefaultLanguage(StringGridRow row, string defaultColumnName)
		{
			string columnName = defaultColumnName;
			if (row.Grid.ContainsColumn(CurrentLanguage))
			{
				columnName = currentLanguage;
			}
			else if (DataLanguage == CurrentLanguage)
			{
				columnName = defaultColumnName;
			}
			else if (!string.IsNullOrEmpty(DefaultLanguage))
			{
				columnName = DefaultLanguage;
			}
			else if (!string.IsNullOrEmpty(DataLanguage))
			{
				columnName = ((!(CurrentLanguage == DataLanguage)) ? DefaultLanguage : defaultColumnName);
			}
			if (row.IsEmptyCell(columnName))
			{
				return row.ParseCellOptional(defaultColumnName, "");
			}
			return row.ParseCellOptional(columnName, "");
		}

		private string ParseCellLocalizedTextByNoSwap(StringGridRow row, string defaultColumnName)
		{
			string localizedColumnName = GetLocalizedColumnName(defaultColumnName);
			if (!row.Grid.ContainsColumn(localizedColumnName))
			{
				Debug.LogError(row.ToErrorString(localizedColumnName + " is empty column. Set localize text column"));
				return "";
			}
			if (BlankTextType == LanguageBlankTextType.NoBlankText && row.IsEmptyCell(localizedColumnName) && row.IsEmptyCell(AdvColumnName.PageCtrl.QuickToString()))
			{
				Debug.LogError(row.ToErrorString(localizedColumnName + " is empty cell. Set localize text"));
				return "";
			}
			return row.ParseCellOptional(localizedColumnName, "");
		}

		private string GetLocalizedColumnName(string defaultColumnName)
		{
			if (TextColumnLanguages.Contains(CurrentLanguage))
			{
				return CurrentLanguage;
			}
			if (string.IsNullOrEmpty(DataLanguage))
			{
				return defaultColumnName;
			}
			if (DataLanguage != CurrentLanguage && !string.IsNullOrEmpty(DefaultLanguage))
			{
				return DefaultLanguage;
			}
			return defaultColumnName;
		}

		public bool CheckSkipPage(StringGridRow row, string defaultColumnName)
		{
			if (!ContainsLocalizeText(row, defaultColumnName))
			{
				return false;
			}
			return ParseCellLocalizedTextByNoSwap(row, defaultColumnName) == "<skip_page>";
		}

		public bool CheckSkipByLocalize(StringGridRow row, string defaultColumnName)
		{
			if (!ContainsLocalizeText(row, defaultColumnName))
			{
				return false;
			}
			return ParseCellLocalizedTextByNoSwap(row, defaultColumnName).Length == 0;
		}

		private bool ContainsLocalizeText(StringGridRow row, string defaultColumnName)
		{
			if (!row.IsEmptyCell(defaultColumnName))
			{
				return true;
			}
			foreach (string textColumnLanguage in TextColumnLanguages)
			{
				if (!row.IsEmptyCell(textColumnLanguage))
				{
					return true;
				}
			}
			return false;
		}
	}
}
