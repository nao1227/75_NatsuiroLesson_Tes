using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Live2D.Cubism.Framework.MotionFade
{
	public class CubismFadeStateObserver : StateMachineBehaviour, ICubismFadeState
	{
		private CubismFadeMotionList _cubismFadeMotionList;

		private List<CubismFadePlayingMotion> _playingMotions;

		private bool _isDefaulState;

		private int _layerIndex;

		private float _layerWeight;

		private bool _isStateTransitionFinished;

		public List<CubismFadePlayingMotion> GetPlayingMotions()
		{
			return _playingMotions;
		}

		public bool IsDefaultState()
		{
			return _isDefaulState;
		}

		public float GetLayerWeight()
		{
			return _layerWeight;
		}

		public bool GetStateTransitionFinished()
		{
			return _isStateTransitionFinished;
		}

		public void SetStateTransitionFinished(bool isFinished)
		{
			_isStateTransitionFinished = isFinished;
		}

		public void StopAnimation(int index)
		{
			_playingMotions.RemoveAt(index);
		}

		private void OnEnable()
		{
			_isStateTransitionFinished = false;
			if (_playingMotions == null)
			{
				_playingMotions = new List<CubismFadePlayingMotion>();
			}
		}

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			CubismFadeController component = animator.gameObject.GetComponent<CubismFadeController>();
			if (component == null)
			{
				return;
			}
			_cubismFadeMotionList = component.CubismFadeMotionList;
			_layerIndex = layerIndex;
			_layerWeight = ((_layerIndex == 0) ? 1f : animator.GetLayerWeight(_layerIndex));
			AnimatorClipInfo[] array = controller.GetNextAnimatorClipInfo(layerIndex);
			_isDefaulState = array.Length == 0;
			if (_isDefaulState)
			{
				array = controller.GetCurrentAnimatorClipInfo(layerIndex);
			}
			if (_playingMotions.Count > 0 && _playingMotions[_playingMotions.Count - 1].Motion != null)
			{
				CubismFadePlayingMotion value = _playingMotions[_playingMotions.Count - 1];
				float time = Time.time;
				float endTime = time + value.Motion.FadeOutTime;
				value.EndTime = endTime;
				while (value.IsLooping && !(value.StartTime + value.Motion.MotionLength >= time))
				{
					value.StartTime += value.Motion.MotionLength;
				}
				_playingMotions[_playingMotions.Count - 1] = value;
			}
			CubismFadePlayingMotion item = default(CubismFadePlayingMotion);
			for (int i = 0; i < array.Length; i++)
			{
				int num = -1;
				AnimationEvent[] events = array[i].clip.events;
				for (int j = 0; j < events.Length; j++)
				{
					if (!(events[j].functionName != "InstanceId"))
					{
						num = events[j].intParameter;
						break;
					}
				}
				int num2 = -1;
				for (int k = 0; k < _cubismFadeMotionList.MotionInstanceIds.Length; k++)
				{
					if (_cubismFadeMotionList.MotionInstanceIds[k] == num)
					{
						num2 = k;
						break;
					}
				}
				item.Motion = ((num2 == -1) ? null : _cubismFadeMotionList.CubismFadeMotionObjects[num2]);
				item.Speed = 1f;
				item.StartTime = Time.time;
				item.FadeInStartTime = Time.time;
				item.EndTime = ((item.Motion.MotionLength <= 0f) ? (-1f) : (item.StartTime + item.Motion.MotionLength));
				item.IsLooping = array[i].clip.isLooping;
				item.Weight = 0f;
				_playingMotions.Add(item);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_isStateTransitionFinished = true;
		}
	}
}
