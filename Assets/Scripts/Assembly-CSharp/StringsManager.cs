using UnityEngine;

public class StringsManager : SingletonManager<StringsManager>
{
	[SerializeField]
	private NameStringData data;

	public static string GetAnimeName(AnimeName enm)
	{
		return SingletonManager<StringsManager>.Instance.data.GetAnimeNameString(enm);
	}

	public static string GetSEName(SEName enm)
	{
		return SingletonManager<StringsManager>.Instance.data.GetSENameString(enm);
	}

	public static string GetVoiceName(VoiceName enm)
	{
		return SingletonManager<StringsManager>.Instance.data.GetVoiceNameString(enm);
	}

	public static string GetBGMName(BGMName enm)
	{
		return SingletonManager<StringsManager>.Instance.data.GetBGMNameString(enm);
	}

	public static string GetScenarioLabel(ScenarioLabel label)
	{
		return SingletonManager<StringsManager>.Instance.data.GetScenarioLabelString(label);
	}

	public static string GetScenarioTitle(ScenarioLabel label)
	{
		return SingletonManager<StringsManager>.Instance.data.GetScenarioTitle(label);
	}

	public static string GetDialogue(DialogueEnum enm)
	{
		return enm.ToString();
	}
}
