using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class VoiceManager : MonoBehaviour
	{
		public StatusObject Status;

		private bool _isIdleVoicePlaying;

		public VoiceList Voices;

		private VoiceList _playingVoicecList;

		private AudioClip _playingClip;

		private CancellationTokenSource _cts;

		public bool IsIdle { get; private set; }

		public bool IsLoaded { get; private set; }

		private async void Start()
		{
			await UniTask.WaitUntil(() => null != SingletonManager<SoundManager>.Instance);
			IsIdle = true;
			_cts = new CancellationTokenSource();
			await Voices.Start();
			IsLoaded = true;
		}

		public void Play(VoiceList voices, AudioSetting setting = null, int channel = 0)
		{
			_playingVoicecList = voices;
			SingletonManager<SoundManager>.Instance.Play(_playingVoicecList.GetVoice(Status.TemporaryStatus.Feelings), setting, channel);
			IsIdle = false;
			_isIdleVoicePlaying = false;
		}

		public void PlayRandom(VoiceName name, AudioSetting setting = null, int channel = 0)
		{
			VoiceOnCondition randomVoiceOnCondition = Voices.GetRandomVoiceOnCondition(name, Status.TemporaryStatus.GetCurrentAtomosphere());
			if (randomVoiceOnCondition != null)
			{
				SingletonManager<SoundManager>.Instance.PlayVoice(randomVoiceOnCondition.Clip, setting, channel);
			}
		}

		public void Play(VoiceName name, int index, AudioSetting setting = null, int channel = 0)
		{
			VoiceOnCondition voiceOnCondition = Voices.GetVoiceOnCondition(name, Status.TemporaryStatus.GetCurrentAtomosphere(), index);
			if (voiceOnCondition != null)
			{
				SingletonManager<SoundManager>.Instance.PlayVoice(voiceOnCondition.Clip, setting, channel);
			}
		}

		public void Play(AudioClip clip, AudioSetting setting = null, int channel = 0)
		{
			_playingClip = clip;
			SingletonManager<SoundManager>.Instance.PlayVoice(clip, setting);
		}

		public async UniTask Stop(VoiceList voices, int millsec = 0)
		{
			_cts.Cancel();
			_cts = new CancellationTokenSource();
			await UniTask.Delay(millsec, ignoreTimeScale: false, PlayerLoopTiming.Update, _cts.Token);
			if (_playingVoicecList == voices)
			{
				SingletonManager<SoundManager>.Instance.StopVoice();
				IsIdle = true;
				_isIdleVoicePlaying = false;
			}
		}

		public async UniTask Stop(AudioClip clip, int millsec = 0)
		{
			_cts.Cancel();
			_cts = new CancellationTokenSource();
			await UniTask.Delay(millsec, ignoreTimeScale: false, PlayerLoopTiming.Update, _cts.Token);
			if (_playingClip == clip)
			{
				SingletonManager<SoundManager>.Instance.StopVoice();
				IsIdle = true;
				_isIdleVoicePlaying = false;
			}
		}

		private void Update()
		{
			if (IsIdle)
			{
				_ = _isIdleVoicePlaying;
			}
		}

		private void OnDestroy()
		{
			Voices.OnDestroy();
		}
	}
}
