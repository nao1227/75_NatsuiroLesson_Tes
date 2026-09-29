using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class MetaSaveDataList
	{
		[SerializeField]
		private List<MetaSaveData> Metas;

		public MetaSaveDataList()
		{
			Metas = new List<MetaSaveData>();
		}

		public void AddNewMetaData(int index, Relationship relationship, int playtime, DateTime loadedTime, int days)
		{
			Metas.Add(new MetaSaveData(index, relationship, CalcPlayTime(playtime, loadedTime), days));
		}

		public void UpdateMetaData(int index, Relationship relationship, int playtime, DateTime loadedTime, int days)
		{
			Metas.First((MetaSaveData x) => x.Index == index).Update(relationship, CalcPlayTime(playtime, loadedTime), days);
		}

		public MetaSaveData GetAutoSaveMetaData()
		{
			return GetMetaSaveData(0);
		}

		public MetaSaveData GetMetaSaveData(int index)
		{
			return Metas.First((MetaSaveData x) => x.Index == index);
		}

		public bool HasMetaSaveData(int index)
		{
			return Metas.Count((MetaSaveData x) => x.Index == index) > 0;
		}

		private int CalcPlayTime(int playtime, DateTime loadedTime)
		{
			return playtime + Mathf.RoundToInt((float)(DateTime.Now - loadedTime).TotalSeconds);
		}

		public void RemoveMetaData(int index)
		{
			Metas.Remove(Metas.First((MetaSaveData x) => x.Index == index));
		}

		public bool HasAnyMetaSaveData()
		{
			return Metas.Count > 0;
		}

		public MetaSaveData GetNewestSavedData()
		{
			return (from x in Metas
				where x.Index > 0
				orderby DateTime.Parse(x.SavedAt) descending
				select x).First();
		}
	}
}
