namespace Utage
{
	public class AdvParser
	{
		public static string Localize(AdvColumnName name)
		{
			return name.QuickToString();
		}

		public static T ParseCell<T>(StringGridRow row, AdvColumnName name)
		{
			return row.ParseCell<T>(Localize(name));
		}

		public static T ParseCellOptional<T>(StringGridRow row, AdvColumnName name, T defaultVal)
		{
			return row.ParseCellOptional(Localize(name), defaultVal);
		}

		public static bool TryParseCell<T>(StringGridRow row, AdvColumnName name, out T val)
		{
			return row.TryParseCell<T>(Localize(name), out val);
		}

		public static bool IsEmptyCell(StringGridRow row, AdvColumnName name)
		{
			return row.IsEmptyCell(Localize(name));
		}

		public static bool IsEmptyTextCommand(StringGridRow row)
		{
			if (!IsEmptyCell(row, AdvColumnName.PageCtrl) || !IsEmptyCell(row, AdvColumnName.Text))
			{
				return false;
			}
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (instance == null)
			{
				return true;
			}
			return instance.IsEmptyTextCommand(row);
		}

		public static string ParseCellLocalizedText(StringGridRow row, AdvColumnName defaultColumnName)
		{
			return ParseCellLocalizedText(row, defaultColumnName.QuickToString());
		}

		public static string ParseCellLocalizedText(StringGridRow row, string defaultColumnName)
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (instance == null)
			{
				return row.ParseCellOptional(defaultColumnName, "");
			}
			return instance.ParseCellLocalizedText(row, defaultColumnName);
		}
	}
}
