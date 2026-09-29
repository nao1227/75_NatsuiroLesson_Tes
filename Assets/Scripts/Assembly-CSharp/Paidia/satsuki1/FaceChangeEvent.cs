using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	public class FaceChangeEvent : OsawariEvent
	{
		[SerializeField]
		private WeightedStateList StateList;

		public int LayerIndex = 1;

		public int crossFadeTime = 75;

		public Live2DAnimator Animator;

		private Dictionary<FaceStateName, AudioClip> _clip;

		public bool Loop;

		private bool _loopCanceled;

		protected FaceStateName _playing;

		protected override async void PostInitialize()
		{
			_clip = new Dictionary<FaceStateName, AudioClip>();
			foreach (WeightedState state in StateList)
			{
				if (state.Voice.RuntimeKeyIsValid() && !_clip.ContainsKey(state.StateName))
				{
					AudioClip value = await Addressables.LoadAssetAsync<AudioClip>(state.Voice);
					if (!_clip.ContainsKey(state.StateName))
					{
						_clip.Add(state.StateName, value);
					}
				}
			}
			base.PostInitialize();
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			Animator = UnityEngine.Object.FindObjectOfType<OsawariManager>()?.GetAnimator();
			if (null == Animator)
			{
				return;
			}
			_playing = StateList.GetRandomState(Animator.GetCurrentAnimatorStateInfo(LayerIndex).GetStateName<FaceStateName>());
			try
			{
				await Animator.PlayFaceAnimationTemporary(_playing, crossFadeTime, _clip[_playing], Loop, Priority);
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
		}

		public override void Cancel()
		{
			base.Cancel();
			Animator?.CancelTemporaryFaceAnimation(_playing);
			_loopCanceled = true;
		}

		private void OnDestroy()
		{
			foreach (WeightedState state in StateList)
			{
				if (state.Voice.IsValid())
				{
					state.Voice.ReleaseAsset();
				}
			}
		}
	}
}
