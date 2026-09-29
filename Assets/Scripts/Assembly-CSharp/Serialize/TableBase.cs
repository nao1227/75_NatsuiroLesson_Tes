using System;
using System.Collections.Generic;
using UnityEngine;

namespace Serialize
{
	[Serializable]
	public class TableBase<TKey, TValue, Type> where Type : KeyAndValue<TKey, TValue>
	{
		[SerializeField]
		private List<Type> list;

		private Dictionary<TKey, TValue> table;

		public Dictionary<TKey, TValue> GetTable()
		{
			if (table == null)
			{
				table = ConvertListToDictionary(list);
			}
			return table;
		}

		public List<Type> GetList()
		{
			return list;
		}

		private static Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> list)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			foreach (Type item in list)
			{
				dictionary.Add(item.Key, item.Value);
			}
			return dictionary;
		}
	}
}
