using System;
using System.Text;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class LocalData
	{
		public int FileNumber;

		[SerializeField]
		public PersistantStatus PersistantStatus;

		[SerializeField]
		public GlobalFlags GlobalFlags;

		[SerializeField]
		public int Days;

		public bool IsPlayingUtage;

		public ScenarioLabel ScenarioLabel;

		public string ScenarioTitle;

		public int PageNumber;

		[NonSerialized]
		public DateTime LoadedTime;

		[NonSerialized]
		public int Playtime;

		public string PlayerName;

		public int HSceneCount;

		public int FellaSceneCount;

		public int WomanExtacyCount;

		public int EjaculationInVaginaCount;

		public int FellatioEjaculationCount;

		public int KissCount;

		public int BreastCount;

		public bool HasTalkedToday;

		public bool HasStudiedToday;

		public bool HasHSceneToday;

		public bool SubEventShown;

		public bool DayStartEventShown;

		public bool UtageisMizugi;

		public bool UtageisBra;

		public bool UtageisBunny;

		public bool UtageisMb;

		public bool UtageisNeko;

		public bool UtageisNude;

		public bool UtageisSm;

		public bool UtageisBath;

		public bool ForceLingerieNormal;

		[NonSerialized]
		public bool NeedAutoSave;

		public int LastSelectedIndex { get; private set; } = -1;

		public int PreviousLastSelectedIndex { get; private set; } = -1;

		public LocalData(int fileNum, PersistantStatus status, int days, GlobalFlags flags)
		{
			FileNumber = fileNum;
			PersistantStatus = status;
			GlobalFlags = flags;
			Days = days;
			LoadedTime = DateTime.Now;
			Playtime = 0;
			HSceneCount = 0;
			FellaSceneCount = 0;
			WomanExtacyCount = 0;
			EjaculationInVaginaCount = 0;
			FellatioEjaculationCount = 0;
			KissCount = 0;
			BreastCount = 0;
			PlayerName = "主人公";
			SubEventShown = false;
			UtageisMizugi = false;
			UtageisBra = false;
			UtageisBunny = false;
			UtageisMb = false;
			UtageisNeko = false;
			UtageisNude = false;
			UtageisSm = false;
			UtageisBath = false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("FileNumber:").Append(FileNumber);
			stringBuilder.Append(" Days:").Append(Days);
			stringBuilder.Append(" LoadedAt:").Append(LoadedTime);
			stringBuilder.Append(" Stats:{").Append("HScene:").Append(HSceneCount)
				.Append("FellaScene:")
				.Append(FellaSceneCount)
				.Append("Extacy:")
				.Append(WomanExtacyCount)
				.Append("EjacInVag:")
				.Append(EjaculationInVaginaCount)
				.Append("FellaEjac:")
				.Append(FellatioEjaculationCount)
				.Append("Kiss:")
				.Append(KissCount)
				.Append("Breast:")
				.Append(BreastCount)
				.Append("}")
				.AppendLine();
			stringBuilder.Append(" PersistantStatus: ").Append(PersistantStatus.ToString()).AppendLine();
			stringBuilder.Append("NeedAutoSave:").Append(NeedAutoSave);
			return stringBuilder.ToString();
		}

		public void RefreshDay()
		{
			Days++;
			HasTalkedToday = false;
			HasStudiedToday = false;
			HasHSceneToday = false;
			NeedAutoSave = true;
			SubEventShown = false;
			DayStartEventShown = false;
			ForceLingerieNormal = false;
			SingletonManager<SceneContextManager>.Instance.IsDayRefreshed = true;
			SingletonManager<SceneContextManager>.Instance.EjaculationPlus = false;
		}

		public void ResetSelectIndex()
		{
			LastSelectedIndex = -1;
			PreviousLastSelectedIndex = -1;
		}

		public void SetSelectIndex(int index)
		{
			PreviousLastSelectedIndex = LastSelectedIndex;
			LastSelectedIndex = index;
		}
	}
}
