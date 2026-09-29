using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	public class PettingAction : TimeConditionAction, IPettingTrigger
	{
		public List<AtomosphereSpeed> Speeds;

		public void OnAverageSpeedChanged(float spd)
		{
			AtomosphereSpeed atomosphereSpeed = Speeds.Where((AtomosphereSpeed x) => x.Atomosphere == _status.GetCurrentAtomosphere()).First();
			if (spd <= atomosphereSpeed.Maximum && spd > atomosphereSpeed.Minimum)
			{
				StartAction();
			}
			else
			{
				CancelAction();
			}
		}
	}
}
