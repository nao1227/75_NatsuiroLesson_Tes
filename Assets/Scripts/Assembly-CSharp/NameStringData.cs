using System.Collections.Generic;
using Paidia.satsuki1;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Strings")]
public class NameStringData : ScriptableObject
{
	[SerializeField]
	private AnimationNameDictionary AnimeNames;

	[SerializeField]
	private SENameDictionary SENames;

	[SerializeField]
	private VoiceNameDictionary VoiceNames;

	[SerializeField]
	private BGMNameDictionary BGMNames;

	[SerializeField]
	private ScenarioDictionary ScenarioLabels;

	[SerializeField]
	private List<ScenarioTitle> ScenarioTitles;

	public string GetVoiceNameString(VoiceName name)
	{
		return VoiceNames.GetTable()[name];
	}

	public string GetSENameString(SEName name)
	{
		return SENames.GetTable()[name];
	}

	public string GetBGMNameString(BGMName name)
	{
		return BGMNames.GetTable()[name];
	}

	public string GetAnimeNameString(AnimeName name)
	{
		return AnimeNames.GetTable()[name];
	}

	public string GetScenarioLabelString(ScenarioLabel label)
	{
		try
		{
			return ScenarioLabels.GetTable()[label];
		}
		catch
		{
			Debug.LogError($"{label} is not found in ScenarioLabel.");
			return "";
		}
	}

	public string GetScenarioTitle(ScenarioLabel label)
	{
		try
		{
			return ScenarioTitles.Find((ScenarioTitle x) => x.Label == label).Title;
		}
		catch
		{
			Debug.LogError($"{label} is not found in ScenarioLabel.");
			return "";
		}
	}

	public List<ScenarioLabel> GetScenarioLabelsByCategory(FreeSccenarioCategory category)
	{
		return ScenarioTitles.FindAll((ScenarioTitle x) => x.Category == category && x.UsedInFreeScenario).ConvertAll((ScenarioTitle x) => x.Label);
	}
}
