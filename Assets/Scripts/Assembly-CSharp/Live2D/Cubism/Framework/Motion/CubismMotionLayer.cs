using System;
using System.Collections.Generic;
using Live2D.Cubism.Framework.MotionFade;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Live2D.Cubism.Framework.Motion
{
	public class CubismMotionLayer : ICubismFadeState
	{
		public Action<int, float> AnimationEndHandler;

		private PlayableGraph _playableGraph;

		private List<CubismFadePlayingMotion> _playingMotions;

		private CubismMotionState _motionState;

		private CubismFadeMotionList _cubismFadeMotionList;

		private int _layerIndex;

		private float _layerWeight;

		private bool _isFinished;

		public AnimationMixerPlayable PlayableOutput { get; private set; }

		public bool IsFinished => _isFinished;

		public List<CubismFadePlayingMotion> GetPlayingMotions()
		{
			return _playingMotions;
		}

		public bool IsDefaultState()
		{
			return false;
		}

		public float GetLayerWeight()
		{
			return _layerWeight;
		}

		public bool GetStateTransitionFinished()
		{
			return true;
		}

		public void SetStateTransitionFinished(bool isFinished)
		{
		}

		public void StopAnimation(int index)
		{
			_playingMotions.RemoveAt(index);
		}

		public void StopAnimationClip()
		{
			if (_motionState != null)
			{
				_playableGraph.Disconnect(_motionState.ClipMixer, 0);
				_motionState = null;
				_isFinished = true;
				StopAllAnimation();
			}
		}

		public static CubismMotionLayer CreateCubismMotionLayer(PlayableGraph playableGraph, CubismFadeMotionList fadeMotionList, int layerIndex, float layerWeight = 1f)
		{
			return new CubismMotionLayer
			{
				_playableGraph = playableGraph,
				_cubismFadeMotionList = fadeMotionList,
				_layerIndex = layerIndex,
				_layerWeight = layerWeight,
				_isFinished = true,
				_motionState = null,
				_playingMotions = new List<CubismFadePlayingMotion>(),
				PlayableOutput = AnimationMixerPlayable.Create(playableGraph, 1)
			};
		}

		private CubismFadePlayingMotion CreateFadePlayingMotion(AnimationClip clip, bool isLooping, float speed = 1f)
		{
			CubismFadePlayingMotion result = default(CubismFadePlayingMotion);
			bool flag = true;
			int num = -1;
			AnimationEvent[] events = clip.events;
			for (int i = 0; i < events.Length; i++)
			{
				if (!(events[i].functionName != "InstanceId"))
				{
					num = events[i].intParameter;
				}
			}
			for (int j = 0; j < _cubismFadeMotionList.MotionInstanceIds.Length; j++)
			{
				if (_cubismFadeMotionList.MotionInstanceIds[j] == num)
				{
					flag = false;
					result.Speed = speed;
					result.StartTime = Time.time;
					result.FadeInStartTime = Time.time;
					result.Motion = _cubismFadeMotionList.CubismFadeMotionObjects[j];
					result.EndTime = ((result.Motion.MotionLength <= 0f) ? (-1f) : (result.StartTime + result.Motion.MotionLength / speed));
					result.IsLooping = isLooping;
					result.Weight = 0f;
					break;
				}
			}
			if (flag)
			{
				Debug.LogError("CubismMotionController : Not found motion from CubismFadeMotionList.");
			}
			return result;
		}

		public void PlayAnimation(AnimationClip clip, bool isLoop = true, float speed = 1f)
		{
			if (_motionState != null)
			{
				_playableGraph.Disconnect(_motionState.ClipMixer, 0);
			}
			_motionState = CubismMotionState.CreateCubismMotionState(_playableGraph, clip, isLoop, speed);
			PlayableOutput.DisconnectInput(0);
			PlayableOutput.ConnectInput(0, _motionState.ClipMixer, 0);
			PlayableOutput.SetInputWeight(0, 1f);
			if (_playingMotions.Count > 0 && _playingMotions[_playingMotions.Count - 1].Motion != null)
			{
				CubismFadePlayingMotion value = _playingMotions[_playingMotions.Count - 1];
				float time = Time.time;
				float num = time + value.Motion.FadeOutTime;
				if (num < 0f || num < value.EndTime)
				{
					value.EndTime = num;
				}
				while (value.IsLooping && !(value.StartTime + value.Motion.MotionLength >= time))
				{
					value.StartTime += value.Motion.MotionLength;
				}
				_playingMotions[_playingMotions.Count - 1] = value;
			}
			CubismFadePlayingMotion item = CreateFadePlayingMotion(clip, isLoop, speed);
			_playingMotions.Add(item);
			_isFinished = false;
		}

		public void StopAllAnimation()
		{
			for (int num = _playingMotions.Count - 1; num >= 0; num--)
			{
				StopAnimation(num);
			}
		}

		public void SetLayerWeight(float weight)
		{
			_layerWeight = weight;
		}

		public void SetStateSpeed(int index, float speed)
		{
			if (index >= 0)
			{
				CubismFadePlayingMotion value = _playingMotions[index];
				value.Speed = speed;
				value.EndTime = (value.EndTime - Time.time) / speed;
				_playingMotions[index] = value;
				_motionState.ClipMixer.SetSpeed(speed);
				_motionState.ClipPlayable.SetDuration(_motionState.Clip.length / speed - 0.0001f);
			}
		}

		public void SetStateIsLoop(int index, bool isLoop)
		{
			if (index >= 0)
			{
				if (isLoop)
				{
					_motionState.ClipPlayable.SetDuration(double.MaxValue);
				}
				else
				{
					_motionState.ClipPlayable.SetDuration(_motionState.Clip.length - 0.0001f);
				}
			}
		}

		public void Update()
		{
			if (AnimationEndHandler == null || _playingMotions.Count != 1 || _isFinished || _motionState.ClipPlayable.GetDuration() == double.MaxValue || Time.time <= _playingMotions[0].EndTime)
			{
				return;
			}
			_isFinished = true;
			int num = -1;
			AnimationEvent[] events = _motionState.Clip.events;
			for (int i = 0; i < events.Length; i++)
			{
				if (!(events[i].functionName != "InstanceId"))
				{
					num = events[i].intParameter;
				}
			}
			AnimationEndHandler(_layerIndex, num);
		}
	}
}
