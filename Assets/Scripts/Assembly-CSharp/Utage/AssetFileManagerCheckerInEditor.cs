using System.Collections.Generic;

namespace Utage
{
	public class AssetFileManagerCheckerInEditor
	{
		private Dictionary<string, AssetFileCheckerInEditor> alliFiles = new Dictionary<string, AssetFileCheckerInEditor>();

		private Dictionary<string, AssetFileCheckerInEditor> AlliFiles => alliFiles;

		public void Clear()
		{
			AlliFiles.Clear();
		}

		public void AddFile(string path, IAssetFileSettingData settingData)
		{
			if (!AlliFiles.ContainsKey(path))
			{
				AlliFiles.Add(path, new AssetFileCheckerInEditor(path, settingData));
			}
		}

		public void CheckAll(string rootPath, bool checkExt)
		{
			foreach (KeyValuePair<string, AssetFileCheckerInEditor> alliFile in AlliFiles)
			{
				alliFile.Value.CheckError(rootPath, checkExt);
			}
		}
	}
}
