namespace Paidia.satsuki1
{
	public class EjaculateWhileWomanExtacyAction : OsawariAction, IEjaculateTrigger
	{
		public override bool GetStatusCondition()
		{
			if (base.GetStatusCondition())
			{
				return _status.IsWomanExtacy;
			}
			return false;
		}
	}
}
