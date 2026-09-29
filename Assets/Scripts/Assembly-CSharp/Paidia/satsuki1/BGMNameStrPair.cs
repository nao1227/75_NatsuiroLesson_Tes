using System;
using Serialize;

namespace Paidia.satsuki1
{
	[Serializable]
	public class BGMNameStrPair : KeyAndValue<BGMName, string>
	{
		public BGMNameStrPair(BGMName key, string value)
			: base(key, value)
		{
		}
	}
}
