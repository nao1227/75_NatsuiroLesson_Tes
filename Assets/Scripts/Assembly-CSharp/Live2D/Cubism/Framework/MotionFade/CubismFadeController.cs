using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Motion;
using UnityEngine;

namespace Live2D.Cubism.Framework.MotionFade
{
	[RequireComponent(typeof(Animator))]
	public class CubismFadeController : MonoBehaviour, ICubismUpdatable
	{
		[SerializeField]
		public CubismFadeMotionList CubismFadeMotionList;

		private CubismMotionController _motionController;

		private ICubismFadeState[] _fadeStates;

		private Animator _animator;

		private CubismParameterStore _parameterStore;

		private bool[] _isFading;

		private CubismParameter[] DestinationParameters { get; set; }

		private CubismPart[] DestinationParts { get; set; }

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismFadeController;

		public bool NeedsUpdateOnEditing => false;

		public void Refresh()
		{
			_animator = GetComponent<Animator>();
			if (!(_animator == null))
			{
				DestinationParameters = this.FindCubismModel().Parameters;
				DestinationParts = this.FindCubismModel().Parts;
				_motionController = GetComponent<CubismMotionController>();
				_parameterStore = GetComponent<CubismParameterStore>();
				HasUpdateController = GetComponent<CubismUpdateController>() != null;
				ICubismFadeState[] behaviours = _animator.GetBehaviours<CubismFadeStateObserver>();
				_fadeStates = behaviours;
				if ((_fadeStates == null || _fadeStates.Length == 0) && _motionController != null)
				{
					_fadeStates = _motionController.GetFadeStates();
				}
				if (_fadeStates != null)
				{
					_isFading = new bool[_fadeStates.Length];
				}
			}
		}

		public void OnLateUpdate()
		{
			if (!base.enabled || _fadeStates == null || _parameterStore == null || DestinationParameters == null || DestinationParts == null)
			{
				return;
			}
			float time = Time.time;
			for (int i = 0; i < _fadeStates.Length; i++)
			{
				_isFading[i] = false;
				List<CubismFadePlayingMotion> playingMotions = _fadeStates[i].GetPlayingMotions();
				if (playingMotions == null || playingMotions.Count <= 1)
				{
					continue;
				}
				CubismFadePlayingMotion cubismFadePlayingMotion = playingMotions[playingMotions.Count - 1];
				CubismFadeMotionData motion = cubismFadePlayingMotion.Motion;
				float num = time - cubismFadePlayingMotion.StartTime;
				for (int j = 0; j < motion.ParameterFadeInTimes.Length; j++)
				{
					if (num <= motion.FadeInTime || (0f <= motion.ParameterFadeInTimes[j] && num <= motion.ParameterFadeInTimes[j]))
					{
						_isFading[i] = true;
						break;
					}
				}
			}
			bool flag = true;
			for (int k = 0; k < _fadeStates.Length; k++)
			{
				List<CubismFadePlayingMotion> playingMotions2 = _fadeStates[k].GetPlayingMotions();
				int num2 = playingMotions2.Count - 1;
				if (_isFading[k])
				{
					flag = false;
					continue;
				}
				int num3 = num2;
				while (num3 >= 0 && playingMotions2.Count > 1)
				{
					if (!(time <= playingMotions2[num3].EndTime))
					{
						_fadeStates[k].StopAnimation(num3);
					}
					num3--;
				}
			}
			if (flag)
			{
				return;
			}
			_parameterStore.RestoreParameters();
			for (int l = 0; l < _fadeStates.Length; l++)
			{
				if (_isFading[l])
				{
					UpdateFade(_fadeStates[l]);
				}
			}
		}

		private void UpdateFade(ICubismFadeState fadeState)
		{
			List<CubismFadePlayingMotion> playingMotions = fadeState.GetPlayingMotions();
			if (playingMotions == null)
			{
				return;
			}
			float layerWeight = fadeState.GetLayerWeight();
			float time = Time.time;
			if (playingMotions.Count > 0 && playingMotions[playingMotions.Count - 1].Motion != null && playingMotions[playingMotions.Count - 1].IsLooping)
			{
				CubismFadePlayingMotion value = playingMotions[playingMotions.Count - 1];
				float endTime = time + value.Motion.FadeOutTime;
				value.EndTime = endTime;
				while (!(value.StartTime + value.Motion.MotionLength >= time))
				{
					value.StartTime += value.Motion.MotionLength;
				}
				playingMotions[playingMotions.Count - 1] = value;
			}
			for (int i = 0; i < playingMotions.Count; i++)
			{
				CubismFadePlayingMotion value2 = playingMotions[i];
				CubismFadeMotionData motion = value2.Motion;
				if (motion == null)
				{
					continue;
				}
				float num = time - value2.StartTime;
				float endTime2 = value2.EndTime - num;
				float fadeInTime = motion.FadeInTime;
				float fadeOutTime = motion.FadeOutTime;
				float num2 = ((fadeInTime <= 0f) ? 1f : CubismFadeMath.GetEasingSine(num / fadeInTime));
				float num3 = ((fadeOutTime <= 0f) ? 1f : CubismFadeMath.GetEasingSine((value2.EndTime - Time.time) / fadeOutTime));
				playingMotions[i] = value2;
				float motionWeight = ((i == 0) ? (num2 * num3) : (num2 * num3 * layerWeight));
				for (int j = 0; j < DestinationParameters.Length; j++)
				{
					int num4 = -1;
					for (int k = 0; k < motion.ParameterIds.Length; k++)
					{
						if (!(motion.ParameterIds[k] != DestinationParameters[j].Id))
						{
							num4 = k;
							break;
						}
					}
					if (num4 >= 0)
					{
						DestinationParameters[j].Value = Evaluate(motion.ParameterCurves[num4], num, endTime2, num2, num3, motion.ParameterFadeInTimes[num4], motion.ParameterFadeOutTimes[num4], motionWeight, DestinationParameters[j].Value);
					}
				}
				for (int l = 0; l < DestinationParts.Length; l++)
				{
					int num5 = -1;
					for (int m = 0; m < motion.ParameterIds.Length; m++)
					{
						if (!(motion.ParameterIds[m] != DestinationParts[l].Id))
						{
							num5 = m;
							break;
						}
					}
					if (num5 >= 0)
					{
						DestinationParts[l].Opacity = Evaluate(motion.ParameterCurves[num5], num, endTime2, num2, num3, motion.ParameterFadeInTimes[num5], motion.ParameterFadeOutTimes[num5], motionWeight, DestinationParts[l].Opacity);
					}
				}
			}
		}

		public float Evaluate(AnimationCurve curve, float elapsedTime, float endTime, float fadeInTime, float fadeOutTime, float parameterFadeInTime, float parameterFadeOutTime, float motionWeight, float currentValue)
		{
			if (curve.length <= 0)
			{
				return currentValue;
			}
			if (parameterFadeInTime < 0f && parameterFadeOutTime < 0f)
			{
				return currentValue + (curve.Evaluate(elapsedTime) - currentValue) * motionWeight;
			}
			float num = ((!(parameterFadeInTime < 0f)) ? ((parameterFadeInTime < float.Epsilon) ? 1f : CubismFadeMath.GetEasingSine(elapsedTime / parameterFadeInTime)) : fadeInTime);
			float num2 = ((!(parameterFadeOutTime < 0f)) ? ((parameterFadeOutTime < float.Epsilon) ? 1f : CubismFadeMath.GetEasingSine(endTime / parameterFadeOutTime)) : fadeOutTime);
			float num3 = num * num2;
			return currentValue + (curve.Evaluate(elapsedTime) - currentValue) * num3;
		}

		private void OnEnable()
		{
			Refresh();
		}

		private void LateUpdate()
		{
			if (!HasUpdateController)
			{
				OnLateUpdate();
			}
		}
	}
}
