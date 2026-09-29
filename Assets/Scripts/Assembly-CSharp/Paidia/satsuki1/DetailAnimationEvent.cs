using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class DetailAnimationEvent : OsawariEvent
	{
		[SerializeField]
		private WeightedDetailStateList StateList;

		public int LayerIndex = 2;

		public int crossFadeTime = 75;

		public int OutCrossFadeTime;

		public Live2DAnimator Animator;

		private Dictionary<DetailStateName, AudioClip> _clip;

		public bool Loop;

		private DetailStateName _playingDetailStateName;

		protected override async void PostInitialize()
		{
			_clip = new Dictionary<DetailStateName, AudioClip>();
			foreach (WeightedDetailState state in StateList)
			{
				AudioClip value = ((state.Voice.IsValid() || !(state.Voice.RuntimeKey.ToString() != "")) ? (state.Voice.Asset as AudioClip) : (await state.Voice.LoadAssetAsync<AudioClip>()));
				if (!_clip.ContainsKey(state.StateName))
				{
					_clip.Add(state.StateName, value);
				}
			}
			base.PostInitialize();
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			if (null == Animator)
			{
				return;
			}
			_playingDetailStateName = StateList.GetRandomState(Animator.GetCurrentAnimatorStateInfo(LayerIndex).GetStateName<DetailStateName>());
			try
			{
				await Animator.PlayDetailAnimationTemporary(_playingDetailStateName, crossFadeTime, _clip[_playingDetailStateName], Loop, OutCrossFadeTime, Priority);
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
		}

		public override void Cancel()
		{
			if (_playingDetailStateName != DetailStateName.Detail_Entry)
			{
				base.Cancel();
				Animator.CancelTemporaryDetailAnimation(_playingDetailStateName);
			}
		}

		private void OnDestroy()
		{
			foreach (WeightedDetailState state in StateList)
			{
				if (state.Voice.IsValid())
				{
					state.Voice.ReleaseAsset();
				}
			}
		}
	}
}
