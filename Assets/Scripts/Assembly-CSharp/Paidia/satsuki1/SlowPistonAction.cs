namespace Paidia.satsuki1
{
	public class SlowPistonAction : TimeConditionAction, IPistonTrigger
	{
		public float Threshold;

		void IPistonTrigger.OnAverageSpeedChanged(float spd)
		{
			if (spd <= Threshold && spd > 0f)
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
