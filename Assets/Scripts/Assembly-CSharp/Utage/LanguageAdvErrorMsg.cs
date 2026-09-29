using UnityEngine;

namespace Utage
{
	public static class LanguageAdvErrorMsg
	{
		private const string LanguageDataName = "AdvErrorMsg";

		public static string LocalizeText(AdvErrorMsg type)
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (instance == null)
			{
				Debug.LogWarning("LanguageManager is NULL");
				return type.ToString();
			}
			if (instance.TryLocalizeText(type.ToString(), out var text))
			{
				return text;
			}
			return instance.DefaultLanuguageText(type.ToString());
		}

		public static string LocalizeTextFormat(AdvErrorMsg type, params object[] args)
		{
			return string.Format(LocalizeText(type), args);
		}
	}
}
