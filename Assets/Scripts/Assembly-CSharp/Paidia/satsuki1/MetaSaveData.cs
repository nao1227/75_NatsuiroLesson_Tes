using System;
using Paidia.Utils;

namespace Paidia.satsuki1
{
	[Serializable]
	public class MetaSaveData
	{
		public int Index;

		public string SaveDataName;

		public Relationship Relationship;

		public int PlayTime;

		public string SavedAt;

		public int Days;

		public MetaSaveData(int index, Relationship relationship, int playtime, int days)
		{
			Index = index;
			SaveDataName = Randomize.RandomString(16);
			Relationship = relationship;
			PlayTime = playtime;
			SavedAt = DateTime.Now.ToString("yyyy/MM/dd HH\\:mm");
			Days = days;
		}

		public void Update(Relationship relationship, int playtime, int days)
		{
			Relationship = relationship;
			PlayTime = playtime;
			SavedAt = DateTime.Now.ToString("yyyy/MM/dd HH\\:mm");
			Days = days;
		}
	}
}
