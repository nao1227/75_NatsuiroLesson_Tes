using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class Live2DAnimator : MonoBehaviour
	{
		public Animator Animator;

		[SerializeField]
		private bool _isPlayingTemporaryFaceAnimation;

		private bool _isPlayingTemporaryPoseAnimation;

		[SerializeField]
		private bool _isPlayingTemporaryDetailAnimation;

		private VoiceManager _voiceManager;

		private AudioClip _playingVoiceClip;

		[SerializeField]
		private FaceStateName _playingFaceState;

		private DetailStateName _playingDetailStateName;

		private CancellationTokenSource _temporaryFaceAnimationTokenSource;

		private CancellationTokenSource _temporaryDetailAnimationTokenSource;

		private CancellationTokenSource _pfaTokenSource;

		private FaceAnimationController _faceAnimationController;

		public int FaceAnimationLayer = 1;

		public int DetailAnimationLayer = 2;

		private int _playingTemporaryFacePriority = -1;

		private int _playingDetailAnimationPriority = -1;

		private bool _detailAnimationAllowed = true;

		private float _defaultOutCrossFadeTime = 75f;

		private float _recordedOutCrossFadeTime;

		public void ManagedStart()
		{
			_voiceManager = UnityEngine.Object.FindObjectOfType<VoiceManager>();
			_faceAnimationController = UnityEngine.Object.FindObjectOfType<FaceAnimationController>();
			_playingFaceState = FaceStateName.None;
		}

		private void Update()
		{
			if (_isPlayingTemporaryDetailAnimation)
			{
				Animator.SetLayerWeight(DetailAnimationLayer, 1f);
			}
			else
			{
				Animator.SetLayerWeight(DetailAnimationLayer, Animator.GetLayerWeight(DetailAnimationLayer) * 0.9f);
			}
		}

		public void StopAnimation()
		{
			Animator.SetFloat("Speed", 0f);
		}

		public void RestartAnimation()
		{
			Animator.SetFloat("Speed", 1f);
		}

		public void SetFloat(string name, float val)
		{
			Animator.SetFloat(name, val);
		}

		private void RefreshTemporaryFaceAnimationToken()
		{
			_temporaryFaceAnimationTokenSource?.Cancel();
			_temporaryFaceAnimationTokenSource = new CancellationTokenSource();
		}

		private void RefreshTemporaryDetailAnimationToken()
		{
			_temporaryDetailAnimationTokenSource?.Cancel();
			_temporaryDetailAnimationTokenSource = new CancellationTokenSource();
		}

		public void CancelTemporaryFaceAnimation(FaceStateName toCancel, bool moveToIdle = true)
		{
			if (toCancel == FaceStateName.None || _playingFaceState == toCancel)
			{
				RefreshTemporaryFaceAnimationToken();
				Animator.SetBool("FaceLock", value: false);
				_voiceManager.Stop(_playingVoiceClip).Forget();
				_isPlayingTemporaryFaceAnimation = false;
				_playingFaceState = FaceStateName.None;
				_playingVoiceClip = null;
				_playingTemporaryFacePriority = -1;
				if (moveToIdle)
				{
					_faceAnimationController.ResetFace();
				}
			}
		}

		public void CancelTemporaryDetailAnimation(DetailStateName cancelTarget = DetailStateName.Detail_Entry, bool any = false)
		{
			if (_playingDetailStateName == cancelTarget || any)
			{
				_playingDetailAnimationPriority = -1;
				float num = ((_recordedOutCrossFadeTime == 0f) ? _defaultOutCrossFadeTime : _recordedOutCrossFadeTime);
				RefreshTemporaryDetailAnimationToken();
				_isPlayingTemporaryDetailAnimation = false;
				_playingDetailStateName = DetailStateName.Detail_Entry;
				Animator.CrossFadeInFixedTime(Animator.StringToHash(DetailStateName.Detail_Entry.ToString()), num / 1000f);
			}
		}

		public void SetDetailAnimationEnable(bool enable)
		{
			if (!enable)
			{
				CancelTemporaryDetailAnimation(DetailStateName.Detail_Entry, any: true);
			}
			_detailAnimationAllowed = enable;
		}

		public async UniTask PlayFaceAnimation(FaceStateName stateName, int crossFadeTime, AudioClip clip = null, int shorten = 0, bool skipIfSame = true, bool loop = true, bool noCrossFade = false)
		{
			if (_isPlayingTemporaryFaceAnimation)
			{
				throw new PlayingTemporaryAnimationException();
			}
			if (_playingFaceState == stateName && skipIfSame)
			{
				throw new PlayingSameAnimationException();
			}
			if (_playingFaceState.ToString().Contains("Ferra_End"))
			{
				crossFadeTime = 0;
			}
			_pfaTokenSource?.Cancel();
			_pfaTokenSource = new CancellationTokenSource();
			try
			{
				await UniTask.WaitUntil(() => !_isPlayingTemporaryFaceAnimation, PlayerLoopTiming.Update, _pfaTokenSource.Token);
				if (!(null == Animator))
				{
					_playingFaceState = stateName;
					SetBool("loop", loop);
					if (noCrossFade)
					{
						Animator.Play(stateName.ToString(), FaceAnimationLayer);
					}
					else
					{
						Animator.CrossFadeInFixedTime(Animator.StringToHash(stateName.ToString()), (float)crossFadeTime * 0.001f, FaceAnimationLayer);
					}
					if (null != clip)
					{
						AudioSetting setting = new AudioSetting(1, randomize: false, 1f, loop);
						_voiceManager.Play(clip, setting);
						_playingVoiceClip = clip;
					}
					await UniTask.Delay(crossFadeTime, ignoreTimeScale: false, PlayerLoopTiming.Update, _pfaTokenSource.Token);
					await UniTask.Yield(_pfaTokenSource.Token);
					if (!(null == Animator))
					{
						await UniTask.Delay(Mathf.Max((int)(Animator.GetCurrentAnimatorStateInfo(FaceAnimationLayer).length * 1000f) - crossFadeTime - shorten, 1), ignoreTimeScale: false, PlayerLoopTiming.Update, _pfaTokenSource.Token);
						_playingFaceState = FaceStateName.None;
					}
				}
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public async UniTask PlayFaceAnimationTemporary(FaceStateName stateName, int crossFadeTime, AudioClip clip = null, bool loop = true, int priority = 0, bool restartAutoAnimation = true)
		{
			if (_playingFaceState == stateName || (priority <= _playingTemporaryFacePriority && priority != int.MaxValue))
			{
				throw new OperationCanceledException();
			}
			CancelFaceAnimation();
			CancelTemporaryFaceAnimation(_playingFaceState, moveToIdle: false);
			_playingTemporaryFacePriority = priority;
			_playingFaceState = stateName;
			if (null == Animator)
			{
				return;
			}
			SetBool("loop", loop);
			Animator?.CrossFadeInFixedTime(Animator.StringToHash(stateName.ToString()), (float)crossFadeTime * 0.001f, FaceAnimationLayer);
			Animator?.SetBool("FaceLock", loop);
			await UniTask.Delay(crossFadeTime, ignoreTimeScale: false, PlayerLoopTiming.Update, _temporaryFaceAnimationTokenSource.Token);
			if (null != clip)
			{
				AudioSetting setting = new AudioSetting(1, randomize: false, 1f, loop);
				_voiceManager.Play(clip, setting);
				_playingVoiceClip = clip;
			}
			_isPlayingTemporaryFaceAnimation = true;
			try
			{
				await UniTask.Yield(_temporaryFaceAnimationTokenSource.Token);
				await UniTask.Delay(Mathf.Max((int)Animator.GetCurrentAnimatorStateInfo(FaceAnimationLayer).length * 1000 - crossFadeTime, 1), ignoreTimeScale: false, PlayerLoopTiming.Update, _temporaryFaceAnimationTokenSource.Token);
				if (!loop)
				{
					_isPlayingTemporaryFaceAnimation = false;
					if (restartAutoAnimation && null != Animator)
					{
						_faceAnimationController.RestartPlayingAutoFaceAnimation();
					}
				}
				_playingTemporaryFacePriority = -1;
			}
			catch (OperationCanceledException)
			{
				_isPlayingTemporaryFaceAnimation = false;
				throw new OperationCanceledException();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public async UniTask PlayDetailAnimationTemporary(DetailStateName stateName, int crossFadeTime, AudioClip clip, bool loop = false, int outCrossFadeTime = 0, int priority = 0)
		{
			if (_playingDetailStateName == stateName || !_detailAnimationAllowed || priority <= _playingDetailAnimationPriority)
			{
				throw new OperationCanceledException();
			}
			CancelTemporaryDetailAnimation(DetailStateName.Detail_Entry, any: true);
			_playingDetailAnimationPriority = priority;
			Animator.CrossFadeInFixedTime(Animator.StringToHash(stateName.ToString()), (float)crossFadeTime * 0.001f);
			if (null != clip)
			{
				_voiceManager.Play(clip);
				_playingVoiceClip = clip;
			}
			_playingDetailStateName = stateName;
			_isPlayingTemporaryDetailAnimation = true;
			try
			{
				float outCF = ((outCrossFadeTime == 0) ? _defaultOutCrossFadeTime : ((float)outCrossFadeTime));
				_recordedOutCrossFadeTime = outCrossFadeTime;
				await UniTask.Delay(crossFadeTime);
				await UniTask.Yield(_temporaryDetailAnimationTokenSource.Token);
				await UniTask.Delay(TimeSpan.FromSeconds(Animator.GetCurrentAnimatorStateInfo(DetailAnimationLayer).length - (float)crossFadeTime * 0.001f - outCF * 1.1f * 0.001f), ignoreTimeScale: false, PlayerLoopTiming.Update, _temporaryDetailAnimationTokenSource.Token);
				if (!loop)
				{
					_isPlayingTemporaryDetailAnimation = false;
					_playingDetailStateName = DetailStateName.Detail_Entry;
					_playingDetailAnimationPriority = -1;
					Animator.CrossFadeInFixedTime(Animator.StringToHash(_playingDetailStateName.ToString()), outCF / 1000f);
				}
				else
				{
					_recordedOutCrossFadeTime = outCrossFadeTime;
				}
			}
			catch
			{
				_playingDetailStateName = DetailStateName.Detail_Entry;
				_isPlayingTemporaryDetailAnimation = false;
				_playingDetailAnimationPriority = -1;
			}
		}

		public AnimatorStateInfo GetCurrentAnimatorStateInfo(int layerIndex)
		{
			return Animator.GetCurrentAnimatorStateInfo(layerIndex);
		}

		public StateMachineObservables[] GetRx()
		{
			return Animator.GetBehaviours<StateMachineObservables>();
		}

		public T GetBehaviour<T>() where T : StateMachineBehaviour
		{
			return Animator.GetBehaviour<T>();
		}

		public void SetBool(string name, bool on)
		{
			if (!(null == Animator))
			{
				Animator?.SetBool(name, on);
			}
		}

		public void SetInteger(string name, int val)
		{
			if (!(null == Animator))
			{
				Animator?.SetInteger(name, val);
			}
		}

		public void SetTrigger(string name)
		{
			if (!(null == Animator))
			{
				Animator.SetTrigger(name);
			}
		}

		public bool GetBool(string name)
		{
			return Animator?.GetBool(name) ?? false;
		}

		public int GetInteger(string name)
		{
			return Animator.GetInteger(name);
		}

		private void OnDestroy()
		{
			_pfaTokenSource?.Cancel();
			_temporaryFaceAnimationTokenSource?.Cancel();
			_temporaryDetailAnimationTokenSource?.Cancel();
		}

		public void CancelFaceAnimation()
		{
			_pfaTokenSource?.Cancel();
		}
	}
}
