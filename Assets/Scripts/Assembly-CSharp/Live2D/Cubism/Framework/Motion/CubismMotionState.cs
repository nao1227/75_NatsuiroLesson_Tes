using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Live2D.Cubism.Framework.Motion
{
	public class CubismMotionState
	{
		public AnimationClip Clip { get; private set; }

		public AnimationMixerPlayable ClipMixer { get; private set; }

		public AnimationClipPlayable ClipPlayable { get; private set; }

		public static CubismMotionState CreateCubismMotionState(PlayableGraph playableGraph, AnimationClip clip, bool isLoop = true, float speed = 1f)
		{
			CubismMotionState cubismMotionState = new CubismMotionState();
			cubismMotionState.Clip = clip;
			cubismMotionState.ClipMixer = AnimationMixerPlayable.Create(playableGraph, 2);
			cubismMotionState.ClipMixer.SetSpeed(speed);
			cubismMotionState.ClipPlayable = AnimationClipPlayable.Create(playableGraph, cubismMotionState.Clip);
			if (!isLoop)
			{
				cubismMotionState.ClipPlayable.SetDuration(clip.length - 0.0001f);
			}
			cubismMotionState.ClipMixer.ConnectInput(0, cubismMotionState.ClipPlayable, 0);
			cubismMotionState.ClipMixer.SetInputWeight(0, 1f);
			return cubismMotionState;
		}

		public void ConnectClipMixer(AnimationMixerPlayable clipMixer)
		{
			int num = ClipMixer.GetInputCount() - 1;
			ClipMixer.DisconnectInput(num);
			ClipMixer.ConnectInput(num, clipMixer, 0);
			ClipMixer.SetInputWeight(num, 1f);
		}
	}
}
