using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AdvBootSetting
	{
		[Serializable]
		public class DefaultDirInfo
		{
			public string defaultDir;

			public string defaultExt;

			public bool legacyAutoChangeSoundExt;

			public string FileNameToPath(string fileName)
			{
				return FileNameToPath(fileName, "");
			}

			public string FileNameToPath(string fileName, string LocalizeDir)
			{
				if (string.IsNullOrEmpty(fileName))
				{
					return fileName;
				}
				string text;
				try
				{
					if (string.IsNullOrEmpty(FilePathUtil.GetExtension(fileName)))
					{
						fileName += defaultExt;
					}
					text = defaultDir + LocalizeDir + "/" + fileName;
				}
				catch (Exception ex)
				{
					Debug.LogError(fileName + "  " + ex.ToString());
					text = defaultDir + LocalizeDir + "/" + fileName;
				}
				if (legacyAutoChangeSoundExt)
				{
					return ExtensionUtil.ChangeSoundExt(text);
				}
				return text;
			}
		}

		private DefaultDirInfo characterDirInfo;

		private DefaultDirInfo bgDirInfo;

		private DefaultDirInfo eventDirInfo;

		private DefaultDirInfo spriteDirInfo;

		private DefaultDirInfo thumbnailDirInfo;

		private DefaultDirInfo bgmDirInfo;

		private DefaultDirInfo seDirInfo;

		private DefaultDirInfo ambienceDirInfo;

		private DefaultDirInfo voiceDirInfo;

		private DefaultDirInfo particleDirInfo;

		private DefaultDirInfo videoDirInfo;

		public string ResourceDir { get; set; }

		public DefaultDirInfo CharacterDirInfo => characterDirInfo;

		public DefaultDirInfo BgDirInfo => bgDirInfo;

		public DefaultDirInfo EventDirInfo => eventDirInfo;

		public DefaultDirInfo SpriteDirInfo => spriteDirInfo;

		public DefaultDirInfo ThumbnailDirInfo => thumbnailDirInfo;

		public DefaultDirInfo BgmDirInfo => bgmDirInfo;

		public DefaultDirInfo SeDirInfo => seDirInfo;

		public DefaultDirInfo AmbienceDirInfo => ambienceDirInfo;

		public DefaultDirInfo VoiceDirInfo => voiceDirInfo;

		public DefaultDirInfo ParticleDirInfo => particleDirInfo;

		public DefaultDirInfo VideoDirInfo => videoDirInfo;

		public void BootInit(string resourceDir, AdvDataManager dataManager = null)
		{
			ResourceDir = resourceDir;
			bool legacyAutoChangeSoundExt = false;
			if (dataManager != null)
			{
				legacyAutoChangeSoundExt = dataManager.LegacyAutoChangeSoundExt;
			}
			characterDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Character",
				defaultExt = ".png"
			};
			bgDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/BG",
				defaultExt = ".jpg"
			};
			eventDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Event",
				defaultExt = ".jpg"
			};
			spriteDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Sprite",
				defaultExt = ".png"
			};
			thumbnailDirInfo = new DefaultDirInfo
			{
				defaultDir = "Texture/Thumbnail",
				defaultExt = ".jpg"
			};
			bgmDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/BGM",
				defaultExt = ".wav",
				legacyAutoChangeSoundExt = legacyAutoChangeSoundExt
			};
			seDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/SE",
				defaultExt = ".wav",
				legacyAutoChangeSoundExt = legacyAutoChangeSoundExt
			};
			ambienceDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/Ambience",
				defaultExt = ".wav",
				legacyAutoChangeSoundExt = legacyAutoChangeSoundExt
			};
			voiceDirInfo = new DefaultDirInfo
			{
				defaultDir = "Sound/Voice",
				defaultExt = ".wav",
				legacyAutoChangeSoundExt = legacyAutoChangeSoundExt
			};
			particleDirInfo = new DefaultDirInfo
			{
				defaultDir = "Particle",
				defaultExt = ".prefab"
			};
			videoDirInfo = new DefaultDirInfo
			{
				defaultDir = "Video",
				defaultExt = ".mp4"
			};
			InitDefaultDirInfo(ResourceDir, characterDirInfo);
			InitDefaultDirInfo(ResourceDir, bgDirInfo);
			InitDefaultDirInfo(ResourceDir, eventDirInfo);
			InitDefaultDirInfo(ResourceDir, spriteDirInfo);
			InitDefaultDirInfo(ResourceDir, thumbnailDirInfo);
			InitDefaultDirInfo(ResourceDir, bgmDirInfo);
			InitDefaultDirInfo(ResourceDir, seDirInfo);
			InitDefaultDirInfo(ResourceDir, ambienceDirInfo);
			InitDefaultDirInfo(ResourceDir, voiceDirInfo);
			InitDefaultDirInfo(ResourceDir, particleDirInfo);
			InitDefaultDirInfo(ResourceDir, videoDirInfo);
		}

		private void InitDefaultDirInfo(string root, DefaultDirInfo info)
		{
			info.defaultDir = FilePathUtil.Combine(root, info.defaultDir);
		}

		public string GetLocalizeVoiceFilePath(string file)
		{
			if (LanguageManagerBase.Instance.IgnoreLocalizeVoice)
			{
				return VoiceDirInfo.FileNameToPath(file);
			}
			string currentVoiceLanguage = LanguageManagerBase.Instance.CurrentVoiceLanguage;
			if (LanguageManagerBase.Instance.VoiceLanguages.Contains(currentVoiceLanguage))
			{
				return VoiceDirInfo.FileNameToPath(file, currentVoiceLanguage);
			}
			return VoiceDirInfo.FileNameToPath(file);
		}

		public List<string> GetAllLocalizeVoiceFilePathList(string file)
		{
			List<string> list = new List<string>();
			list.Add(VoiceDirInfo.FileNameToPath(file));
			if ((bool)LanguageManagerBase.Instance && !LanguageManagerBase.Instance.IgnoreLocalizeVoice)
			{
				foreach (string voiceLanguage in LanguageManagerBase.Instance.VoiceLanguages)
				{
					list.Add(VoiceDirInfo.FileNameToPath(file, voiceLanguage));
				}
			}
			return list;
		}
	}
}
