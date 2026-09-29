namespace Utage
{
	public class AdvSoundSettingData : AdvSettingDictinoayItemBase, IAssetFileSoundSettingData, IAssetFileSettingData, IAssetFileSoundSettingDataKey
	{
		private string fileName;

		public SoundType Type { get; private set; }

		public string Title { get; private set; }

		public string FilePath { get; private set; }

		public float IntroTime { get; private set; }

		public float Volume { get; private set; }

		public override bool InitFromStringGridRow(StringGridRow row)
		{
			if (row.IsEmptyOrCommantOut)
			{
				return false;
			}
			base.RowData = row;
			string value = AdvParser.ParseCell<string>(row, AdvColumnName.Label);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			InitKey(value);
			Type = AdvParser.ParseCell<SoundType>(row, AdvColumnName.Type);
			fileName = AdvParser.ParseCell<string>(row, AdvColumnName.FileName);
			Title = AdvParser.ParseCellOptional(row, AdvColumnName.Title, "");
			IntroTime = AdvParser.ParseCellOptional(row, AdvColumnName.IntroTime, 0f);
			Volume = AdvParser.ParseCellOptional(row, AdvColumnName.Volume, 1f);
			return true;
		}

		public void BootInit(AdvSettingDataManager dataManager)
		{
			FilePath = FileNameToPath(fileName, dataManager.BootSetting);
			AssetFileManager.GetFileCreateIfMissing(FilePath, this);
		}

		private string FileNameToPath(string fileName, AdvBootSetting settingData)
		{
			return Type switch
			{
				SoundType.Se => settingData.SeDirInfo.FileNameToPath(fileName), 
				SoundType.Ambience => settingData.AmbienceDirInfo.FileNameToPath(fileName), 
				_ => settingData.BgmDirInfo.FileNameToPath(fileName), 
			};
		}
	}
}
