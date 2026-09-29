using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MotionFade;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Live2D.Cubism.Framework.Motion
{
	[RequireComponent(typeof(CubismFadeController))]
	public class CubismMotionController : MonoBehaviour
	{
		[SerializeField]
		public Action<float> AnimationEndHandler;

		public int LayerCount = 1;

		private CubismFadeMotionList _cubismFadeMotionList;

		private bool _isActive;

		private PlayableGraph _playableGrap;

		private AnimationPlayableOutput _playableOutput;

		private AnimationLayerMixerPlayable _layerMixer;

		private CubismMotionLayer[] _motionLayers;

		private int[] _motionPriorities;

		private void OnAnimationEnd(int layerIndex, float instanceId)
		{
			_motionPriorities[layerIndex] = 0;
			if (AnimationEndHandler != null)
			{
				AnimationEndHandler(instanceId);
			}
		}

		public void PlayAnimation(AnimationClip clip, int layerIndex = 0, int priority = 2, bool isLoop = true, float speed = 1f)
		{
			if (!base.enabled || !_isActive || _cubismFadeMotionList == null || clip == null || layerIndex < 0 || layerIndex >= LayerCount || (_motionPriorities[layerIndex] >= priority && priority != 3))
			{
				Debug.Log("can't start motion.");
				return;
			}
			_motionPriorities[layerIndex] = priority;
			_motionLayers[layerIndex].PlayAnimation(clip, isLoop, speed);
			if (!_playableGrap.IsPlaying())
			{
				_playableGrap.Play();
			}
		}

		public void StopAnimation(int animationIndex, int layerIndex = 0)
		{
			if (layerIndex >= 0 && layerIndex < LayerCount)
			{
				_motionLayers[layerIndex].StopAnimationClip();
			}
		}

		public void StopAllAnimation()
		{
			for (int i = 0; i < LayerCount; i++)
			{
				_motionLayers[i].StopAnimationClip();
			}
		}

		public bool IsPlayingAnimation(int layerIndex = 0)
		{
			if (layerIndex < 0 || layerIndex >= LayerCount)
			{
				return false;
			}
			return !_motionLayers[layerIndex].IsFinished;
		}

		public void SetLayerWeight(int layerIndex, float weight)
		{
			if (layerIndex > 0 && layerIndex < LayerCount)
			{
				_motionLayers[layerIndex].SetLayerWeight(weight);
				_layerMixer.SetInputWeight(layerIndex, weight);
			}
		}

		public void SetLayerAdditive(int layerIndex, bool isAdditive)
		{
			if (layerIndex > 0 && layerIndex < LayerCount)
			{
				_layerMixer.SetLayerAdditive((uint)layerIndex, isAdditive);
			}
		}

		public void SetAnimationSpeed(int layerIndex, int index, float speed)
		{
			if (layerIndex >= 0 && layerIndex < LayerCount)
			{
				_motionLayers[layerIndex].SetStateSpeed(index, speed);
			}
		}

		public void SetAnimationIsLoop(int layerIndex, int index, bool isLoop)
		{
			if (layerIndex >= 0 && layerIndex < LayerCount)
			{
				_motionLayers[layerIndex].SetStateIsLoop(index, isLoop);
			}
		}

		public ICubismFadeState[] GetFadeStates()
		{
			if (_motionLayers == null)
			{
				LayerCount = ((LayerCount < 1) ? 1 : LayerCount);
				_motionLayers = new CubismMotionLayer[LayerCount];
				_motionPriorities = new int[LayerCount];
			}
			return _motionLayers;
		}

		private void OnEnable()
		{
			_cubismFadeMotionList = GetComponent<CubismFadeController>().CubismFadeMotionList;
			if (_cubismFadeMotionList == null)
			{
				Debug.LogError("CubismMotionController : CubismFadeMotionList doesn't set in CubismFadeController.");
				return;
			}
			Animator component = GetComponent<Animator>();
			if (component.runtimeAnimatorController != null)
			{
				Debug.LogWarning("Animator Controller was set in Animator component.");
				return;
			}
			_isActive = true;
			PlayableGraph playableGraph = component.playableGraph;
			if (playableGraph.IsValid())
			{
				playableGraph.GetOutput(0).SetWeight(0f);
			}
			_playableGrap = PlayableGraph.Create("Playable Graph : " + this.FindCubismModel().name);
			_playableGrap.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
			_playableOutput = AnimationPlayableOutput.Create(_playableGrap, "Animation", component);
			_playableOutput.SetWeight(1f);
			_layerMixer = AnimationLayerMixerPlayable.Create(_playableGrap, LayerCount);
			if (_motionLayers == null)
			{
				LayerCount = ((LayerCount < 1) ? 1 : LayerCount);
				_motionLayers = new CubismMotionLayer[LayerCount];
				_motionPriorities = new int[LayerCount];
			}
			for (int i = 0; i < LayerCount; i++)
			{
				_motionLayers[i] = CubismMotionLayer.CreateCubismMotionLayer(_playableGrap, _cubismFadeMotionList, i);
				CubismMotionLayer obj = _motionLayers[i];
				obj.AnimationEndHandler = (Action<int, float>)Delegate.Combine(obj.AnimationEndHandler, new Action<int, float>(OnAnimationEnd));
				_layerMixer.ConnectInput(i, _motionLayers[i].PlayableOutput, 0);
				_layerMixer.SetInputWeight(i, 1f);
			}
			_playableOutput.SetSourcePlayable(_layerMixer);
		}

		private void OnDisable()
		{
			if (_playableGrap.IsValid())
			{
				_playableGrap.Destroy();
			}
		}

		private void Update()
		{
			if (!_isActive)
			{
				return;
			}
			for (int i = 0; i < _motionLayers.Length; i++)
			{
				_motionLayers[i].Update();
				if (_motionLayers[i].IsFinished)
				{
					_motionPriorities[i] = 0;
				}
			}
		}
	}
}
