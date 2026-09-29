using System;
using System.Collections.Generic;

namespace Serialize
{
	[Serializable]
	public class KeyAndValue<TKey, TValue>
	{
		public TKey Key;

		public TValue Value;

		public KeyAndValue(TKey key, TValue value)
		{
			Key = key;
			Value = value;
		}

		public KeyAndValue(KeyValuePair<TKey, TValue> pair)
		{
			Key = pair.Key;
			Value = pair.Value;
		}
	}
}
