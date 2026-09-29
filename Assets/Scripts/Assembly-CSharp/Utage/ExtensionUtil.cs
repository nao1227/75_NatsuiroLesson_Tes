using UnityEngine;

namespace Utage
{
	public static class ExtensionUtil
	{
		public const string Ogg = ".ogg";

		public const string Mp3 = ".mp3";

		public const string Wav = ".wav";

		public const string Txt = ".txt";

		public const string CSV = ".csv";

		public const string TSV = ".tsv";

		public const string AssetBundle = ".unity3d";

		public const string UtageFile = ".utage";

		public const string ConvertFileList = ".list.bytes";

		public const string ConvertFileListLog = ".list.log";

		public const string Log = ".log";

		public static AudioType GetAudioType(string path)
		{
			return FilePathUtil.GetExtension(path).ToLower() switch
			{
				".mp3" => AudioType.MPEG, 
				".ogg" => AudioType.OGGVORBIS, 
				_ => AudioType.WAV, 
			};
		}

		public static string ChangeSoundExt(string path)
		{
			switch (FilePathUtil.GetExtension(path).ToLower())
			{
			case ".ogg":
				if (!IsSupportOggPlatform())
				{
					return FilePathUtil.ChangeExtension(path, ".mp3");
				}
				break;
			case ".mp3":
				if (IsSupportOggPlatform())
				{
					return FilePathUtil.ChangeExtension(path, ".ogg");
				}
				break;
			}
			return path;
		}

		public static bool IsSupportOggPlatform()
		{
			return true;
		}
	}
}
