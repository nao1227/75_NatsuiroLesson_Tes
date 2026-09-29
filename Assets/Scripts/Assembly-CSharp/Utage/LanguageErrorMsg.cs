using UnityEngine;

namespace Utage
{
	public static class LanguageErrorMsg
	{
		private const string LanguageDataName = "ErrorMsg";

		public static string LocalizeText(ErrorMsg type)
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

		public static string LocalizeTextFormat(ErrorMsg type, params object[] args)
		{
			return string.Format(LocalizeText(type), args);
		}
	}
}
