namespace Paidia.satsuki1
{
	public class HardPistonAction : OsawariAction, IImpactTrigger
	{
		public float ImpactThreshold;

		public void OnImpact(float impact)
		{
			if (impact >= ImpactThreshold)
			{
				CountAction();
			}
		}

		public override bool GetStatusCondition()
		{
			AtomosphereName currentAtomosphere = _status.GetCurrentAtomosphere();
			if (currentAtomosphere != AtomosphereName.Excited)
			{
				return currentAtomosphere == AtomosphereName.Rut;
			}
			return true;
		}
	}
}
