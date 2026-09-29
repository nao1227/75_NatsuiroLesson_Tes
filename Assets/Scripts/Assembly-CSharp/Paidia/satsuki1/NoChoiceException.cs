using System;

namespace Paidia.satsuki1
{
	public class NoChoiceException : Exception
	{
		public NoChoiceException()
			: base("No Choice chosen")
		{
		}
	}
}
