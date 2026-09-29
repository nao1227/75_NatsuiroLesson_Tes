public class AudioSetting
{
	public readonly int Priority;

	public readonly bool RandomizePitch;

	public readonly float PitchRange;

	public readonly bool Loop;

	public readonly bool OneShot;

	public AudioSetting(int priority = 1, bool randomize = false, float pitchRange = 0f, bool loop = false, bool oneShot = false)
	{
		Priority = priority;
		RandomizePitch = randomize;
		PitchRange = pitchRange;
		Loop = loop;
		OneShot = oneShot;
	}

	public static AudioSetting GetDefault()
	{
		return new AudioSetting();
	}

	public static AudioSetting GetLoopedDefault()
	{
		return new AudioSetting(1, randomize: false, 0f, loop: true);
	}

	public float GetPitch()
	{
		return 1f;
	}
}
