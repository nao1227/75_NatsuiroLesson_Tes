using System;
using Serialize;

namespace Paidia.satsuki1
{
	[Serializable]
	public class VoiceNameStrPair : KeyAndValue<VoiceName, string>
	{
		public VoiceNameStrPair(VoiceName key, string value)
			: base(key, value)
		{
		}
	}
}
