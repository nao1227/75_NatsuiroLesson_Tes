namespace Paidia.satsuki1
{
	public class ExtacyKissAction : OsawariAction, IKissTrigger
	{
		public override bool GetStatusCondition()
		{
			return _status.IsWomanExtacy;
		}
	}
}
