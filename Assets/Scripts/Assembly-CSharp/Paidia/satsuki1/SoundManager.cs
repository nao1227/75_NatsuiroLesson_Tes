using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class SoundManager : SingletonManager<SoundManager>
	{
		public string PATH = "Sounds";

		public int SE_CHANNEL_COUNT = 5;

		public int VOICE_CHANNEL_COUNT = 1;

		public int BGS_CHANNEL_COUNT = 1;

		[SerializeField]
		private List<BGMSet> BGMSet;

		[SerializeField]
		private List<SESet> SESet;

		[SerializeField]
		private List<VoiceSet> VoiceSet;

		private Dictionary<string, AudioClip> _cache = new Dictionary<string, AudioClip>();

		private GameObject _object;

		private AudioSource _bgmSource;

		private AudioSource[] _voiceSources;

		private AudioSource[] _seSources;

		private AudioSource[] _bgsSources;

		private int _sePriorityInPlay;

		private List<int> _cutVolumeFrames;

		public float CutVolumeThreshold = 1f;

		public int CutVolumeFrameThreshold = 2;

		public bool IsPlayingBGM => _bgmSource.isPlaying;

		public bool IsPlayingVoice
		{
			get
			{
				for (int i = 0; i < VOICE_CHANNEL_COUNT; i++)
				{
					if (_voiceSources[i].isPlaying)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool Isvalid
		{
			get
			{
				if (null != _bgmSource && _voiceSources != null)
				{
					return _seSources != null;
				}
				return false;
			}
		}

		public bool IsPlayingSE
		{
			get
			{
				for (int i = 0; i < SE_CHANNEL_COUNT; i++)
				{
					if (_seSources[i].isPlaying)
					{
						return true;
					}
				}
				return false;
			}
		}

		private void Start()
		{
			_cutVolumeFrames = new List<int>();
			for (int i = 0; i < VOICE_CHANNEL_COUNT; i++)
			{
				_cutVolumeFrames.Add(0);
			}
			MakeAudioSource();
		}

		public void Play(SEName se, AudioSetting setting = null, int channel = 0)
		{
			if (channel > SE_CHANNEL_COUNT || channel < 0)
			{
				channel = 0;
			}
			for (int i = 0; i < SE_CHANNEL_COUNT; i++)
			{
				if (!_seSources[channel].isPlaying)
				{
					channel = i;
					break;
				}
			}
			if (_seSources[channel].isPlaying)
			{
				if (_seSources[channel].priority >= SESet.First((SESet x) => x.SENames[0] == se).Priority)
				{
					return;
				}
				_seSources[channel].Stop();
			}
			if (setting == null)
			{
				setting = AudioSetting.GetDefault();
			}
			PlayAudioClipAsync(StringsManager.GetSEName(se), _seSources[channel], setting, SaveLoadManager.GlobalData.SEVolume);
		}

		public void Play(VoiceName voice, AudioSetting setting = null, int channel = 0)
		{
			if (channel > VOICE_CHANNEL_COUNT || channel < 0)
			{
				channel = 0;
			}
			if (_voiceSources[channel].isPlaying)
			{
				if (_voiceSources[channel].priority >= VoiceSet.First((VoiceSet x) => x.VoiceNames[0] == voice).Priority)
				{
					return;
				}
				_voiceSources[channel].Stop();
			}
			if (setting == null)
			{
				setting = AudioSetting.GetDefault();
			}
			PlayAudioClipAsync(StringsManager.GetVoiceName(voice), _voiceSources[channel], setting, SaveLoadManager.GlobalData.VoiceVolume);
		}

		public void PlayVoice(VoiceName name, AudioClip voice, AudioSetting setting = null, int channel = 0)
		{
			if (channel > VOICE_CHANNEL_COUNT || channel < 0)
			{
				channel = 0;
			}
			if (_voiceSources[channel].isPlaying)
			{
				_voiceSources[channel].Stop();
			}
			if (setting == null)
			{
				setting = AudioSetting.GetDefault();
			}
			PlayAudioClip(voice, _voiceSources[channel], setting, SaveLoadManager.GlobalData.VoiceVolume);
		}

		public void PlayVoice(AudioClip voice, AudioSetting setting = null, int channel = 0)
		{
			if (!(null == voice))
			{
				if (channel > VOICE_CHANNEL_COUNT || channel < 0)
				{
					channel = 0;
				}
				if (_voiceSources[channel].isPlaying)
				{
					_voiceSources[channel].Stop();
				}
				if (setting == null)
				{
					setting = AudioSetting.GetDefault();
				}
				PlayAudioClip(voice, _voiceSources[channel], setting, SaveLoadManager.GlobalData.VoiceVolume);
			}
		}

		public void PlaySE(AudioClip se, AudioSetting setting = null, int channel = 0, int priority = 0)
		{
			if (!_seSources[channel].isPlaying || _sePriorityInPlay <= priority)
			{
				_sePriorityInPlay = priority;
				if (_seSources[channel].isPlaying)
				{
					_seSources[channel].Stop();
				}
				if (setting == null)
				{
					setting = AudioSetting.GetDefault();
				}
				PlayAudioClip(se, _seSources[channel], setting, SaveLoadManager.GlobalData.SEVolume);
			}
		}

		public void PlayBGM(AudioClip bgm, AudioSetting setting = null)
		{
			if (IsPlayingBGM)
			{
				_bgmSource.Stop();
			}
			if (setting == null)
			{
				setting = AudioSetting.GetDefault();
			}
			PlayAudioClip(bgm, _bgmSource, setting, SaveLoadManager.GlobalData.BGMVolume);
		}

		public void PlayBGS(AudioClip bgs, AudioSetting setting = null, int channel = 0)
		{
			if (_bgsSources[channel].isPlaying)
			{
				_bgsSources[channel].Stop();
			}
			if (setting == null)
			{
				setting = AudioSetting.GetLoopedDefault();
			}
			PlayAudioClip(bgs, _bgsSources[channel], setting, SaveLoadManager.GlobalData.EnvironmentalSEVolume);
		}

		public void Play(BGMName bgm, AudioSetting setting = null)
		{
			if (IsPlayingBGM)
			{
				if (_bgmSource.priority >= BGMSet.First((BGMSet x) => x.BGMNames[0] == bgm).Priority)
				{
					return;
				}
				_bgmSource.Stop();
			}
			if (setting == null)
			{
				setting = AudioSetting.GetDefault();
			}
			PlayAudioClip(StringsManager.GetBGMName(bgm), _bgmSource, setting, SaveLoadManager.GlobalData.BGMVolume);
		}

		public void Play(AudioSetName audio, AudioCategory category)
		{
			switch (category)
			{
			case AudioCategory.SE:
			{
				SESet sESet = SESet.Where((SESet x) => x.AudioSetName == audio).First();
				Play(sESet.SENames[0], sESet.Setting);
				break;
			}
			case AudioCategory.Voice:
			{
				VoiceSet voiceSet = VoiceSet.Where((VoiceSet x) => x.AudioSetName == audio).First();
				Play(voiceSet.VoiceNames[0], voiceSet.Setting);
				break;
			}
			case AudioCategory.BGM:
			{
				BGMSet bGMSet = BGMSet.Where((BGMSet x) => x.AudioSetName == audio).First();
				Play(bGMSet.BGMNames[0], bGMSet.Setting);
				break;
			}
			}
		}

		private void PlayAudioClipAsync(string fileName, AudioSource source, AudioSetting setting, float volume)
		{
			PlayAudioClip(fileName, source, setting, volume);
		}

		private void PlayAudioClip(string fileName, AudioSource source, AudioSetting setting, float volume)
		{
			if (!_cache.ContainsKey(fileName))
			{
				_cache.Add(fileName, Resources.Load(GetPath(fileName)) as AudioClip);
			}
			source.loop = setting.Loop;
			source.pitch = setting.GetPitch();
			source.priority = setting.Priority;
			source.clip = _cache[fileName];
			source.volume = volume;
			source.Play();
		}

		private void PlayAudioClip(AudioClip clip, AudioSource source, AudioSetting setting, float volume)
		{
			source.loop = setting.Loop;
			source.pitch = setting.GetPitch();
			source.priority = setting.Priority;
			source.clip = clip;
			source.volume = volume;
			source.Play();
		}

		private string GetPath(string fileName)
		{
			return PATH + "/" + fileName;
		}

		private void MakeAudioSource()
		{
			if (_object == null)
			{
				_object = new GameObject("Sound");
				UnityEngine.Object.DontDestroyOnLoad(_object);
				_bgmSource = _object.AddComponent<AudioSource>();
				_bgmSource.playOnAwake = false;
				_bgmSource.loop = true;
				_voiceSources = new AudioSource[VOICE_CHANNEL_COUNT];
				for (int i = 0; i < VOICE_CHANNEL_COUNT; i++)
				{
					_voiceSources[i] = _object.AddComponent<AudioSource>();
					_voiceSources[i].playOnAwake = false;
				}
				_seSources = new AudioSource[SE_CHANNEL_COUNT];
				for (int j = 0; j < SE_CHANNEL_COUNT; j++)
				{
					_seSources[j] = _object.AddComponent<AudioSource>();
					_seSources[j].playOnAwake = false;
				}
				_bgsSources = new AudioSource[BGS_CHANNEL_COUNT];
				for (int k = 0; k < BGS_CHANNEL_COUNT; k++)
				{
					_bgsSources[k] = _object.AddComponent<AudioSource>();
					_bgsSources[k].playOnAwake = false;
				}
			}
		}

		public void StopAll()
		{
			_bgmSource.Stop();
			AudioSource[] voiceSources = _voiceSources;
			foreach (AudioSource obj in voiceSources)
			{
				obj.Stop();
				obj.clip = null;
			}
			voiceSources = _seSources;
			foreach (AudioSource obj2 in voiceSources)
			{
				obj2.Stop();
				obj2.clip = null;
			}
			voiceSources = _bgsSources;
			foreach (AudioSource obj3 in voiceSources)
			{
				obj3.Stop();
				obj3.clip = null;
			}
		}

		public void StopVoice(int channel = 0)
		{
			_voiceSources[channel].Stop();
			_voiceSources[channel].clip = null;
		}

		public void StopBGS(int channel = 0)
		{
			_bgsSources[channel].Stop();
			_bgsSources[channel].clip = null;
		}

		public void StopSE(int channel = 0)
		{
			_seSources[channel].Stop();
			_seSources[channel].clip = null;
		}

		public void ChangeVolume(AudioCategory category, float volume)
		{
			switch (category)
			{
			case AudioCategory.SE:
			{
				AudioSource[] voiceSources = _seSources;
				for (int i = 0; i < voiceSources.Length; i++)
				{
					voiceSources[i].volume = volume;
				}
				break;
			}
			case AudioCategory.Environment:
			{
				AudioSource[] voiceSources = _bgsSources;
				for (int i = 0; i < voiceSources.Length; i++)
				{
					voiceSources[i].volume = volume;
				}
				break;
			}
			case AudioCategory.Voice:
			{
				AudioSource[] voiceSources = _voiceSources;
				for (int i = 0; i < voiceSources.Length; i++)
				{
					voiceSources[i].volume = volume;
				}
				break;
			}
			case AudioCategory.BGM:
				_bgmSource.volume = volume;
				break;
			}
		}

		public float GetVolume(AudioCategory category)
		{
			return category switch
			{
				AudioCategory.BGM => _bgmSource.volume, 
				AudioCategory.SE => _seSources[0].volume, 
				AudioCategory.Environment => _seSources[1].volume, 
				AudioCategory.Voice => _voiceSources[0].volume, 
				_ => throw new NotImplementedException(), 
			};
		}

		private void Update()
		{
			int num = 0;
			AudioSource[] voiceSources = _voiceSources;
			foreach (AudioSource obj in voiceSources)
			{
				float[] array = new float[1024];
				obj.GetOutputData(array, 1);
				if (1000f * array.Select((float x) => x * x).Sum() / (float)array.Length < CutVolumeThreshold)
				{
					_cutVolumeFrames[num]++;
				}
				else
				{
					_cutVolumeFrames[num] = 0;
				}
				_ = _cutVolumeFrames[num];
				_ = CutVolumeFrameThreshold;
				num++;
			}
		}

		private bool IsChannelSilent(int channel = 0)
		{
			return _cutVolumeFrames[channel] > CutVolumeFrameThreshold;
		}
	}
}
