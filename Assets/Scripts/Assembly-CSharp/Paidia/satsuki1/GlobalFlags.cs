using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class GlobalFlags
	{
		public List<GameFlag> Flags;

		public List<ReadLabel> ReadLabels;

		public bool SkippedToDay6;

		public bool IsOn(FlagEnum name)
		{
			return Flags.Where((GameFlag x) => x.Name == name).First().IsOn;
		}

		public bool IsRead(ScenarioLabel name)
		{
			return ReadLabels.Where((ReadLabel x) => x.Name == name).First().IsRead;
		}

		public void SetFlag(FlagEnum name, bool isOn)
		{
			Flags.Where((GameFlag x) => x.Name == name).First().IsOn = isOn;
		}

		public void SetRead(ScenarioLabel scenarioLabel)
		{
			ReadLabel item = ReadLabels.Where((ReadLabel x) => x.Name == scenarioLabel).First();
			ReadLabels[ReadLabels.IndexOf(item)].SetRead();
		}

		public override string ToString()
		{
			return string.Join("\n", Flags.Select((GameFlag x) => x.Name.ToString() + ": " + x.IsOn).ToArray().Concat(ReadLabels.Select((ReadLabel x) => x.Name.ToString() + ": " + x.IsRead).ToArray()));
		}

		public GlobalFlags()
		{
			Flags = new List<GameFlag>();
			foreach (FlagEnum value in Enum.GetValues(typeof(FlagEnum)))
			{
				Flags.Add(new GameFlag(value));
			}
			ReadLabels = new List<ReadLabel>();
			foreach (ScenarioLabel value2 in Enum.GetValues(typeof(ScenarioLabel)))
			{
				ReadLabels.Add(new ReadLabel(value2));
			}
		}

		public void SkipToDay6()
		{
			SkippedToDay6 = true;
			SaveLoadManager.UnsavedData.GlobalFlags.SetRead(ScenarioLabel.Day6);
			SaveLoadManager.UnsavedData.Days = 6;
			List<FlagEnum> obj = new List<FlagEnum>
			{
				FlagEnum.DoneTutorial,
				FlagEnum.PenCase_Day1,
				FlagEnum.Face_Day1,
				FlagEnum.Breast_Day1,
				FlagEnum.Note_Day1,
				FlagEnum.Click_All_Day1,
				FlagEnum.Message_Day1_Study,
				FlagEnum.Message_Day5_Study,
				FlagEnum.Pool_Apply_Some_Day4,
				FlagEnum.FirstKiss
			};
			List<ScenarioLabel> list = new List<ScenarioLabel>
			{
				ScenarioLabel.Day1,
				ScenarioLabel.Day2,
				ScenarioLabel.Day3,
				ScenarioLabel.Day4,
				ScenarioLabel.Day5,
				ScenarioLabel.Day6,
				ScenarioLabel.OP,
				ScenarioLabel.Start_Study_Day1,
				ScenarioLabel.Click_PenCase_Day1,
				ScenarioLabel.Click_Face_Day1,
				ScenarioLabel.Click_Breast_Day1,
				ScenarioLabel.Click_Note_Day1,
				ScenarioLabel.Click_LeftHand_Day1,
				ScenarioLabel.Clicked_All_Day1,
				ScenarioLabel.Day1_B,
				ScenarioLabel.Click_PenCase_Day2,
				ScenarioLabel.Click_Face_Day2,
				ScenarioLabel.Click_Breast_Day2,
				ScenarioLabel.Click_Keyholder_Day2,
				ScenarioLabel.Day2_B,
				ScenarioLabel.Click_PenCase_Day3,
				ScenarioLabel.Click_Face_Day3,
				ScenarioLabel.Click_Breast_Day3,
				ScenarioLabel.Day3_B,
				ScenarioLabel.Start_Kiss_Day3,
				ScenarioLabel.Day3_C,
				ScenarioLabel.Start_Study_Day4,
				ScenarioLabel.Click_Breast_Day4,
				ScenarioLabel.Click_LeftHand_Day4,
				ScenarioLabel.Day4_B,
				ScenarioLabel.Start_Pool_Day4,
				ScenarioLabel.Click_Hip_Day4,
				ScenarioLabel.Apply_Some_Day4,
				ScenarioLabel.Click_Body_Day4,
				ScenarioLabel.Day4_C,
				ScenarioLabel.Start_Study_Day5,
				ScenarioLabel.Click_LeftHand_Day5_1,
				ScenarioLabel.Click_LeftHand_Day5_2,
				ScenarioLabel.Start_Pool_Day5,
				ScenarioLabel.Click_Hip_Day5_1,
				ScenarioLabel.Click_Hip_Day5_2,
				ScenarioLabel.Day5_B,
				ScenarioLabel.Start_Kiss_Day5,
				ScenarioLabel.Click_Mouth_Day5,
				ScenarioLabel.Day5_C,
				ScenarioLabel.Command_Talk_Day6,
				ScenarioLabel.Start_Study_Day6
			};
			foreach (FlagEnum item in obj)
			{
				SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(item, isOn: true);
			}
			foreach (ScenarioLabel item2 in list)
			{
				SaveLoadManager.UnsavedData.GlobalFlags.SetRead(item2);
			}
		}
	}
}
