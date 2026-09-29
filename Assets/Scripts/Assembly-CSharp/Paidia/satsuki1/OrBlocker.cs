using System.Collections.Generic;

namespace Paidia.satsuki1
{
	public class OrBlocker : OsawariBlocker
	{
		public List<OsawariBlocker> Blockers;

		public override bool IsBlocked()
		{
			foreach (OsawariBlocker blocker in Blockers)
			{
				if (!blocker.IsBlocked())
				{
					return false;
				}
			}
			return true;
		}
	}
}
