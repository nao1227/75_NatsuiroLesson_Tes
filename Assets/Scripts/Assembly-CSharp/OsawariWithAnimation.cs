using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Paidia.satsuki1;
using UnityEngine;

public abstract class OsawariWithAnimation : AbstractOsawari
{
	protected Live2DAnimator animator;

	protected bool CancelAnimation;

	public override bool IsAnimating { get; protected set; }

	public override void ManagedStart(OsawariManager osawariManager, CancellationToken token)
	{
		animator = GetComponent<Live2DAnimator>();
		IsAnimating = false;
		base.ManagedStart(osawariManager, token);
	}

	public virtual async UniTask StartAnimation(AnimeName nameId, bool on, bool singleTime = false)
	{
		await StartAnimation(StringsManager.GetAnimeName(nameId), on, singleTime);
	}

	protected virtual async UniTask StartAnimation(string animationName, bool on, bool singleTime = false)
	{
		_ = 3;
		try
		{
			while (animator.GetBool(animationName) != on)
			{
				animator.SetBool(animationName, on);
				await UniTask.Yield(_token);
			}
			IsAnimating = true;
			await UniTask.Yield(_token);
			await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length), ignoreTimeScale: false, PlayerLoopTiming.Update, _token);
			if (singleTime)
			{
				await UniTask.Yield(_token);
				animator.SetBool(animationName, !on);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.StackTrace);
		}
		IsAnimating = false;
	}

	protected virtual async UniTask SetAnimationStatusOff(AnimationNameDictionary dict)
	{
		foreach (KeyValuePair<AnimeName, string> item in dict.GetTable())
		{
			animator.SetBool(StringsManager.GetAnimeName(item.Key), on: false);
		}
		await UniTask.Yield(_token);
		IsAnimating = true;
		await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length), ignoreTimeScale: false, PlayerLoopTiming.Update, _token);
		IsAnimating = false;
	}

	protected virtual async UniTask SetAnimationStatusOff(string[] strs)
	{
		for (int i = 0; i < strs.Length; i++)
		{
			animator.SetBool(strs[i], on: false);
		}
		await UniTask.Yield(_token);
		IsAnimating = true;
		await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length), ignoreTimeScale: false, PlayerLoopTiming.Update, _token);
		IsAnimating = false;
	}

	protected virtual void SetAnimationStatusOffNoYield(AnimeName nameID)
	{
		animator.SetBool(StringsManager.GetAnimeName(nameID), on: false);
	}

	protected virtual async UniTask SetAnimationStatusOff(AnimeName nameId)
	{
		string[] animationStatusOff = new string[1] { StringsManager.GetAnimeName(nameId) };
		await SetAnimationStatusOff(animationStatusOff);
	}

	protected virtual async UniTask StartAnimation(AnimeName nameId, int add)
	{
		string animeName = StringsManager.GetAnimeName(nameId);
		animator.SetInteger(animeName, animator.GetInteger(animeName) + 1);
		IsAnimating = true;
		await UniTask.Yield(_token);
		await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length), ignoreTimeScale: false, PlayerLoopTiming.Update, _token);
		IsAnimating = false;
	}
}
