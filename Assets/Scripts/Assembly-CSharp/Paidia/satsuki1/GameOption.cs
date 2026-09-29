using System;
using System.Text;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class GameOption
	{
		public int ImageQuality;

		public int FrameRate;

		public int MouseButtonDecision;

		public int MouseButtonAuto;

		public int MouseButtonSpecial;

		public Color UIColor = Color.white;

		public float TextSpeed;

		public float TextAutoSpeed;

		public int SkipUnread;

		public int StopSkipOnChoice;

		public float MouseSensitivity;

		public float HeartGaugeCorrection = 1f;

		public float AtomosphereGaugeCorrection = 1f;

		public float EjaculationGaugeCorrection = 1f;

		public bool CountEjaculationWithCondom = true;

		public GameOption(int quality = 0, int frameRate = 1, int decision = 0, int auto = 2, int special = 1, float textSpeed = 0.5f, float textAuto = 0.5f, int skip = 0, int stopSkip = 1, int mouseSensitivity = 10)
		{
			ImageQuality = quality;
			FrameRate = frameRate;
			MouseButtonDecision = decision;
			MouseButtonAuto = auto;
			MouseButtonSpecial = special;
			UIColor = Color.white;
			TextSpeed = textSpeed;
			TextAutoSpeed = textAuto;
			SkipUnread = skip;
			StopSkipOnChoice = stopSkip;
			MouseSensitivity = mouseSensitivity;
		}

		public void SetColor(Color color)
		{
			UIColor = color;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("ImageQuality:").Append(ImageQuality);
			stringBuilder.Append(" FrameRate:").Append(FrameRate);
			stringBuilder.Append(" MouseButtonDecision:").Append(MouseButtonDecision);
			stringBuilder.Append(" MouseButtonAuto:").Append(MouseButtonAuto);
			stringBuilder.Append(" MouseButtonSpecial:").Append(MouseButtonSpecial);
			stringBuilder.Append(" UIColor:").Append(UIColor);
			stringBuilder.Append(" TextSpeed:").Append(TextSpeed);
			stringBuilder.Append(" TextAutoSpeed:").Append(TextAutoSpeed);
			stringBuilder.Append(" SkipUnread:").Append(SkipUnread);
			stringBuilder.Append(" StopSkipOnChoice:").Append(StopSkipOnChoice);
			stringBuilder.Append(" MouseSensitivity:").Append(MouseSensitivity);
			return stringBuilder.ToString();
		}
	}
}
