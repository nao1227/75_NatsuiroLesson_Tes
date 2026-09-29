using UnityEngine;

namespace Utage
{
	public class AssetFileCheckerInEditor
	{
		private string path;

		private IAssetFileSettingData settingData;

		public AssetFileCheckerInEditor(string path, IAssetFileSettingData settingData)
		{
			this.path = path;
			this.settingData = settingData;
			if (path.Contains(" "))
			{
				Debug.LogWarning(ToErrorString("[" + path + "] contains white space"));
			}
		}

		internal void CheckError(string rootPath, bool checkExt)
		{
			string arg = FilePathUtil.Combine(rootPath, path);
			if (!Exist(arg, checkExt))
			{
				string msg = $"{arg} is not exit";
				Debug.LogError(ToErrorString(msg));
			}
		}

		internal bool Exist(string path, bool checkExt)
		{
			return true;
		}

		private string ToErrorString(string msg)
		{
			if (settingData != null && settingData.RowData != null)
			{
				return settingData.RowData.ToErrorString(msg);
			}
			return msg;
		}
	}
}
