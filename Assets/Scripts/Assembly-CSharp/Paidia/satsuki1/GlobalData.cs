using System;
using System.Text;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class GlobalData
	{
		[SerializeField]
		public GameOption GameOption;

		public bool IsCleared;

		public int FullScreen;

		public int ScreenSize;

		public float BGMVolume;

		public float SEVolume;

		public float EnvironmentalSEVolume;

		public float VoiceVolume;

		public GlobalData(int fullScreen = 1, int screenSize = 2, float bgm = 0.5f, float se = 0.5f, float environment = 0.5f, float voice = 0.5f, bool isCleared = false)
		{
			FullScreen = fullScreen;
			ScreenSize = screenSize;
			BGMVolume = bgm;
			SEVolume = se;
			EnvironmentalSEVolume = environment;
			VoiceVolume = voice;
			IsCleared = isCleared;
			GameOption = new GameOption();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("IsCleared:").Append(IsCleared);
			stringBuilder.Append(" FullScreen:").Append(FullScreen);
			stringBuilder.Append(" ScreenSize:").Append(ScreenSize);
			stringBuilder.Append(" BGMVolume:").Append(BGMVolume);
			stringBuilder.Append(" SEVolume:").Append(SEVolume);
			stringBuilder.Append(" EnvironmentalSEVolume:").Append(EnvironmentalSEVolume);
			stringBuilder.Append(" VoiceVolume:").Append(VoiceVolume);
			stringBuilder.Append(" GameOption:").Append(GameOption.ToString());
			return stringBuilder.ToString();
		}

		public float GetMouseSensitivityFactor()
		{
			return Mathf.Pow(20f, GameOption.MouseSensitivity);
		}
	}
}
