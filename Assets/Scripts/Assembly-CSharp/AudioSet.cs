using System;
using UnityEngine;

[Serializable]
public abstract class AudioSet
{
	[SerializeField]
	protected AudioSetName audioSetName;

	public int Priority = 1;

	[SerializeField]
	private bool randomizePitch;

	[SerializeField]
	private float pitchRange = 0.1f;

	[SerializeField]
	private bool loop;

	[SerializeField]
	private bool oneShot;

	private AudioSetting _setting;

	public readonly AudioCategory AudioCategory;

	public AudioSetName AudioSetName => audioSetName;

	public AudioSetting Setting
	{
		get
		{
			if (_setting == null)
			{
				_setting = new AudioSetting(Priority, randomizePitch, pitchRange, loop, oneShot);
			}
			return _setting;
		}
	}

	public AudioSet(AudioCategory category)
	{
		AudioCategory = category;
	}
}
