namespace Paidia.satsuki1
{
	public class PistonWhileBreastGrabAction : OsawariAction, IPistonTrigger, IWhileBreastGrab
	{
		private bool _breastGrab;

		private bool _alreadyCount;

		void IPistonTrigger.OnAverageSpeedChanged(float spd)
		{
			if (spd > 0f && _breastGrab && !_alreadyCount)
			{
				_alreadyCount = true;
				CountAction();
			}
			if (spd == 0f)
			{
				_alreadyCount = false;
			}
		}

		void IWhileBreastGrab.GrabBreast(bool grab)
		{
			_breastGrab = grab;
		}
	}
}
