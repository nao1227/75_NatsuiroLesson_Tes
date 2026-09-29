using System;
using UnityEngine;

public class SoundController : MonoBehaviour
{
	[Serializable]
	private class SoundSet
	{
		[SerializeField]
		private AudioClip[] Sound;

		[SerializeField]
		private string SoundSetName;

		[SerializeField]
		private int Priority = 1;

		[SerializeField]
		private bool RandomizePitch;

		[SerializeField]
		private float PitchRange = 0.1f;

		[SerializeField]
		private bool Loop;

		[SerializeField]
		private bool OneShot;

		public AudioClip[] GetSound()
		{
			return Sound;
		}

		public string GetSoundSetName()
		{
			return SoundSetName;
		}

		public int GetPriority()
		{
			return Priority;
		}

		public bool GetRandamizePitch()
		{
			return RandomizePitch;
		}

		public float GetPitchRange()
		{
			return PitchRange;
		}

		public bool GetLoop()
		{
			return Loop;
		}

		public bool GetOneShot()
		{
			return OneShot;
		}
	}

	[SerializeField]
	private SoundSet[] soundSet;

	private int _pastPriority;

	private bool _pastOneShot;

	private bool _pastLoop;

	private bool _playFlag;

	protected AudioSource source;

	public bool PlaySound(string SoundName)
	{
		UserDataEventListener(SoundName);
		return _playFlag;
	}

	public bool IsPlaying()
	{
		return source.isPlaying;
	}

	private void Awake()
	{
		source = GetComponents<AudioSource>()[0];
		_pastPriority = 0;
		_playFlag = false;
	}

	private void Update()
	{
	}

	public void UserDataEventListener(string eventname)
	{
		_ = eventname == "voice_extacy";
		for (int i = 0; i < soundSet.Length; i++)
		{
			if (!(soundSet[i].GetSoundSetName() == eventname))
			{
				continue;
			}
			if (_pastPriority <= soundSet[i].GetPriority() || !source.isPlaying || _pastOneShot)
			{
				if (!_pastLoop || _pastPriority != soundSet[i].GetPriority())
				{
					if (soundSet[i].GetRandamizePitch())
					{
						source.pitch = 1f + UnityEngine.Random.Range(0f - soundSet[i].GetPitchRange(), soundSet[i].GetPitchRange());
					}
					source.clip = soundSet[i].GetSound()[UnityEngine.Random.Range(0, soundSet[i].GetSound().Length)];
					source.loop = soundSet[i].GetLoop();
					if (soundSet[i].GetOneShot())
					{
						source.PlayOneShot(soundSet[i].GetSound()[UnityEngine.Random.Range(0, soundSet[i].GetSound().Length)]);
					}
					else
					{
						_ = soundSet[i].GetSoundSetName() == "voice_extacy";
						source.Play();
						_playFlag = true;
					}
					_pastPriority = soundSet[i].GetPriority();
					_pastOneShot = soundSet[i].GetOneShot();
					_pastLoop = soundSet[i].GetLoop();
				}
				else if (!_pastLoop || _pastPriority != soundSet[i].GetPriority())
				{
					if (soundSet[i].GetRandamizePitch())
					{
						source.pitch = 1f + UnityEngine.Random.Range(0f - soundSet[i].GetPitchRange(), soundSet[i].GetPitchRange());
					}
					source.clip = soundSet[i].GetSound()[UnityEngine.Random.Range(0, soundSet[i].GetSound().Length)];
					source.loop = soundSet[i].GetLoop();
					source.Play();
					_pastPriority = soundSet[i].GetPriority();
					_pastOneShot = soundSet[i].GetOneShot();
					_pastLoop = soundSet[i].GetLoop();
				}
			}
			else
			{
				_playFlag = false;
			}
		}
	}
}
