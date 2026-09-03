using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Stubs;

namespace Paidia.satsuki1
{
	public abstract class OsawariEvent : MonoBehaviour
	{
		protected AbstractOsawari _parent;

		public StatusObject StatusObject;

		public int Priority;

		public int WaitingTime;

		public int WaitingTimeResetTime;

		public int InvokeInterval;

		protected bool _isWaiting;

		public EventWaitPhase _waitPhase;

		public bool IsInvoked;

		protected CancellationTokenSource _ctsForCancel;

		protected CancellationTokenSource _ctsForInvoke;

		public List<FlagEnum> FlagsOnComplete;

		public List<FlagEnum> FlagsOffOnComplete;

		public RandomSE SE;

		public bool SkipIfInPiston;

		public bool IsSubEvent;

		public bool _isInCooldown;

		public bool InvokedWhileWomanMoving;

		public bool InvokeInFreeHMode;

		public bool InvokeOnlyWhenSkippedToDay6;

		public bool IsValidOnFreeHMode;

		public StatusChange StatusChange;

		public List<EventCondition> Conditions;

		public EventFlagCondition FlagCondition;

		public EventScenarioReadCondition ScenarioReadCondition;

		public virtual async UniTask InvokeEvent(TemporaryStatus status, OsawariConditions conditions)
		{
			if (!IsFullfillCondition(status, conditions))
			{
				Cancel();
			}
			else if (_waitPhase != EventWaitPhase.WaitingForInvoke)
			{
				if (_waitPhase == EventWaitPhase.WaitingForCancel)
				{
					_ctsForCancel.Cancel();
					return;
				}
				IsInvoked = true;
				await AwaitInvokeEvent(status, conditions);
			}
		}

		protected virtual async UniTask AwaitInvokeEvent(TemporaryStatus status, OsawariConditions conditions)
		{
			_waitPhase = EventWaitPhase.WaitingForInvoke;
			try
			{
				await UniTask.Delay(WaitingTime, ignoreTimeScale: false, PlayerLoopTiming.Update, _ctsForInvoke.Token);
				await UniTask.WaitUntil(() => !_isInCooldown, PlayerLoopTiming.Update, _ctsForInvoke.Token);
				if (_waitPhase == EventWaitPhase.WaitingForInvoke)
				{
					StatusObject.TemporaryStatus.AddExciteValue(StatusChange.Excite);
					StatusObject.TemporaryStatus.Feelings.AddAtomosphere(StatusChange.Atomosphere);
					StatusObject.TemporaryStatus.Feelings.Stimulus += StatusChange.Stimulus;
					if (SE.HasSE)
					{
						Tuple<AudioClip, int> randomSE = SE.GetRandomSE();
						if (randomSE != null)
						{
							SingletonManager<SoundManager>.Instance.PlaySE(randomSE.Item1, null, randomSE.Item2, Priority);
						}
					}
					await InvokeCore(status, conditions);
					FlagsOnComplete.ForEach(delegate(FlagEnum x)
					{
						SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(x, isOn: true);
					});
					FlagsOffOnComplete.ForEach(delegate(FlagEnum x)
					{
						SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(x, isOn: false);
					});
					if (InvokeInterval > 0)
					{
						_isInCooldown = true;
						StartCooldown().Forget();
					}
				}
				_waitPhase = EventWaitPhase.None;
			}
			catch (OperationCanceledException)
			{
				_waitPhase = EventWaitPhase.None;
			}
			catch (Exception ex2)
			{
				Debug.LogErrorFormat(ex2.StackTrace);
			}
			IsInvoked = false;
		}

		protected async UniTask StartCooldown()
		{
			await UniTask.Delay(TimeSpan.FromSeconds((float)InvokeInterval / 1000f), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			_isInCooldown = false;
		}

		protected abstract UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions);

		public virtual async UniTask InvokeEventAsync()
		{
			await UniTask.Yield();
		}

		public virtual async void Cancel()
		{
			if (_waitPhase != EventWaitPhase.WaitingForInvoke)
			{
				return;
			}
			_ctsForCancel = new CancellationTokenSource();
			_waitPhase = EventWaitPhase.WaitingForCancel;
			try
			{
				await UniTask.Delay(WaitingTimeResetTime, ignoreTimeScale: false, PlayerLoopTiming.Update, _ctsForCancel.Token);
				_waitPhase = EventWaitPhase.None;
				_ctsForInvoke.Cancel();
				_ctsForInvoke = new CancellationTokenSource();
			}
			catch
			{
			}
		}

		public async UniTask Initialize(AbstractOsawari parent)
		{
			_isWaiting = false;
			_ctsForInvoke = new CancellationTokenSource();
			_parent = parent;
			await LoadSE();
			PostInitialize();
			_isInCooldown = false;
		}

		public void Initialize()
		{
			LoadSE().Forget();
			_ctsForInvoke = new CancellationTokenSource();
			PostInitialize();
		}

		private async UniTask LoadSE()
		{
			if (SE.HasSE)
			{
				await SE.LoadSE();
			}
		}

		protected virtual void PostInitialize()
		{
		}

		public virtual bool Trigger(float[] threasholds)
		{
			return true;
		}

		public bool IsFullfillCondition(TemporaryStatus status, OsawariConditions conditions)
		{
			if (!IsValidOnFreeHMode && SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH)
			{
				return false;
			}
			if (Conditions.Count((EventCondition x) => !x.IsFullfillCondition(status, conditions)) == 0 && FlagCondition.IsFullfillCondition() && ScenarioReadCondition.IsFullfillCondition() && (!conditions.IsPistonMoving || !SkipIfInPiston) && (!IsSubEvent || !SaveLoadManager.UnsavedData.SubEventShown))
			{
				if (InvokeOnlyWhenSkippedToDay6)
				{
					return SaveLoadManager.UnsavedData.GlobalFlags.SkippedToDay6;
				}
				return true;
			}
			return false;
		}

		public static OsawariEvent GetEmptyEvent()
		{
			return new EmptyEvent();
		}
	}
}
