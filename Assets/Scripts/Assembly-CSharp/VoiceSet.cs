using System;
using UnityEngine;

[Serializable]
public class VoiceSet : AudioSet
{
	[SerializeField]
	private VoiceName[] voiceNames;

	[SerializeField]
	private int channel_Number;

	public VoiceName[] VoiceNames => voiceNames;

	public int Channel_Number => channel_Number;

	public VoiceSet()
		: base(AudioCategory.Voice)
	{
	}
}
