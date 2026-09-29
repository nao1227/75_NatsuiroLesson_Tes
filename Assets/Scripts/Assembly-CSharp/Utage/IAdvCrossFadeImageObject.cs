using System;

namespace Utage
{
	public interface IAdvCrossFadeImageObject
	{
		bool IsCrossFading { get; }

		void RestartCrossFade(float fadeTime, Action onComplete);

		void SkipCrossFade();
	}
}
