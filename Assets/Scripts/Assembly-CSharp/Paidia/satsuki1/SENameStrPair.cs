using System;
using Serialize;

namespace Paidia.satsuki1
{
	[Serializable]
	public class SENameStrPair : KeyAndValue<SEName, string>
	{
		public SENameStrPair(SEName key, string value)
			: base(key, value)
		{
		}
	}
}
