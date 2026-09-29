using System.Collections.Generic;

namespace Live2D.Cubism.Framework.MotionFade
{
	public interface ICubismFadeState
	{
		List<CubismFadePlayingMotion> GetPlayingMotions();

		bool IsDefaultState();

		float GetLayerWeight();

		bool GetStateTransitionFinished();

		void SetStateTransitionFinished(bool isFinished);

		void StopAnimation(int index);
	}
}
