using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class MainSceneFaceAnimation : MonoBehaviour
	{
		public AdventureScene _adv;

		private Live2DAnimator _animator;

		public Subject<StateID> OnFaceAnimationEnd;

		public int WaitForSelectedMotion;

		public VoiceManager VoiceManager;

		private bool _playingAnimation;

		private bool _activated;

		private CancellationTokenSource _cts = new CancellationTokenSource();

		public bool IsLoad;

		private CompositeDisposable _disp;

		private bool _onEnableCalled;

		private async void OnEnable()
		{
			if (_onEnableCalled)
			{
				return;
			}
			_onEnableCalled = true;
			_animator = GetComponent<Live2DAnimator>();
			_disp?.Dispose();
			_cts?.Cancel();
			_cts = new CancellationTokenSource();
			_disp = new CompositeDisposable();
			OnFaceAnimationEnd = new Subject<StateID>();
			try
			{
				await UniTask.WaitUntil(() => VoiceManager.IsLoaded, PlayerLoopTiming.Update, _cts.Token);
			}
			catch
			{
				return;
			}
			StateMachineObservables[] rx = _animator.GetRx();
			foreach (StateMachineObservables obj2 in rx)
			{
				obj2.OnStateExitObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
					OnFaceAnimationEnd.OnNext(x.Item1);
				}).AddTo(_disp);
				obj2.OnStateEnterObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
				{
					switch (x.Item1)
					{
					case StateID.Entry:
						VoiceManager.Play(VoiceName.MainEntry, 0);
						break;
					case StateID.MainWait:
						VoiceManager.Play(VoiceName.MainWait, 0);
						break;
					case StateID.MainStudy:
						if (SaveLoadManager.UnsavedData.Days == 2)
						{
							VoiceManager.Play(VoiceName.MainStudy_Day2, 0);
						}
						else if (SaveLoadManager.UnsavedData.Days == 6)
						{
							VoiceManager.Play(VoiceName.MainStudy_Day6, 0);
						}
						else
						{
							VoiceManager.Play(VoiceName.MainStudy, 0);
						}
						break;
					case StateID.MainSex:
					case StateID.MainBath:
						VoiceManager.Play(VoiceName.MainBath, 0);
						break;
					case StateID.MainSwim:
						VoiceManager.Play(VoiceName.MainSwim, 0);
						break;
					case StateID.MainEnd:
						VoiceManager.Play(VoiceName.MainEnd, 0);
						break;
					case StateID.Zettyou:
					case StateID.Vibrator:
						break;
					}
				}).AddTo(_disp);
			}
			_activated = true;
			_playingAnimation = false;
			_animator.SetInteger("Day", SaveLoadManager.UnsavedData.Days);
			_animator.SetBool("IsLoad", on: true);
			Object.FindObjectOfType<UtageManager>().OnFinishPlaying.Where((ScenarioLabel x) => _adv.MainScenarioByDay.Contains(x)).Subscribe(delegate
			{
				_animator.SetTrigger("TriggerStartADVFinished");
			}).AddTo(_disp);
			_adv.OnDayStart.First().Subscribe(delegate
			{
				MoveToSelect().Forget();
			}).AddTo(_cts.Token);
			await UniTask.Yield();
			IsLoad = true;
		}

		private async UniTask MoveToSelect()
		{
			try
			{
				UtageManager utage = Object.FindObjectOfType<UtageManager>();
				bool waiting = true;
				float timeLeft = (float)WaitForSelectedMotion / 1000f;
				while (waiting)
				{
					if (!utage.IsPlaying)
					{
						timeLeft -= Time.deltaTime;
						if (timeLeft < 0f)
						{
							waiting = false;
						}
					}
					await UniTask.Yield(_cts.Token);
				}
			}
			catch
			{
				return;
			}
			if (!_playingAnimation)
			{
				_animator.SetTrigger("TriggerSelect");
			}
		}

		public void PlayStudy()
		{
			_playingAnimation = true;
			_animator.SetTrigger("TriggerStudy");
		}

		public void PlaySex()
		{
			_playingAnimation = true;
			_animator.SetTrigger("TriggerSex");
		}

		public void PlayBath()
		{
			_playingAnimation = true;
			_animator.SetTrigger("TriggerBath");
		}

		public void PlaySwim()
		{
			_playingAnimation = true;
			_animator.SetTrigger("TriggerSwim");
		}

		public void PlayEnd()
		{
			_playingAnimation = true;
			_animator.SetTrigger("TriggerEnd");
		}

		private void OnDisable()
		{
			_cts.Cancel();
			_disp?.Dispose();
			_onEnableCalled = false;
		}
	}
}
