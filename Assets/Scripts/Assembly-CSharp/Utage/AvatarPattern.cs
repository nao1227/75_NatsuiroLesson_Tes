using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[Serializable]
	public class AvatarPattern
	{
		[Serializable]
		public class PartternData
		{
			public string tag;

			public string patternName;
		}

		[SerializeField]
		private List<PartternData> avatarPatternDataList = new List<PartternData>();

		[SerializeField]
		private List<string> optionPatternNameList = new List<string>();

		public List<PartternData> DataList => avatarPatternDataList;

		public List<string> OptionPatternNameList => optionPatternNameList;

		public void SetPatternName(string tag, string patternName)
		{
			PartternData partternData = DataList.Find((PartternData x) => x.tag == tag);
			if (partternData == null)
			{
				Debug.LogError($"Unknown Pattern [{patternName}], tag[{tag}] ");
			}
			else
			{
				partternData.patternName = patternName;
			}
		}

		public string GetPatternName(string tag)
		{
			PartternData partternData = DataList.Find((PartternData x) => x.tag == tag);
			if (partternData != null)
			{
				return partternData.patternName;
			}
			return "";
		}

		internal void SetPattern(StringGridRow rowData)
		{
			foreach (KeyValuePair<string, int> keyValue in rowData.Grid.ColumnIndexTbl)
			{
				PartternData partternData = DataList.Find((PartternData x) => x.tag == keyValue.Key);
				if (partternData != null)
				{
					if (keyValue.Value < rowData.Strings.Length)
					{
						partternData.patternName = rowData.Strings[keyValue.Value];
					}
					else
					{
						partternData.patternName = "";
					}
				}
			}
		}

		public void SetOptionEnable(string optionName, bool enable)
		{
			if (enable)
			{
				EnableOption(optionName);
			}
			else
			{
				DisableOption(optionName);
			}
		}

		public void EnableOption(string optionName)
		{
			if (!OptionPatternNameList.Contains(optionName))
			{
				OptionPatternNameList.Add(optionName);
			}
		}

		public void DisableOption(string optionName)
		{
			if (OptionPatternNameList.Contains(optionName))
			{
				OptionPatternNameList.Remove(optionName);
			}
		}

		internal bool Rebuild(AvatarData data)
		{
			if (data == null)
			{
				return false;
			}
			bool result = false;
			foreach (AvatarData.Category category in data.categories)
			{
				PartternData partternData = DataList.Find((PartternData x) => x.tag == category.Tag);
				if (partternData == null)
				{
					partternData = new PartternData();
					partternData.tag = category.Tag;
					DataList.Add(partternData);
					result = true;
				}
			}
			return result;
		}
	}
}
