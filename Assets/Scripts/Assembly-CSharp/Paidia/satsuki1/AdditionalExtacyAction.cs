namespace Paidia.satsuki1
{
	public class AdditionalExtacyAction : OsawariAction, IOnExtraExtacyAdded
	{
		public int Threshold;

		private int _extraExtacy;

		public override void Initialize(TemporaryStatus status)
		{
			base.Initialize(status);
			_extraExtacy = 0;
		}

		public override bool GetStatusCondition()
		{
			return _status.IsWomanExtacy;
		}

		public override bool GetConditionOnFinish()
		{
			return _extraExtacy >= Threshold;
		}

		public override void FinishAction()
		{
			base.FinishAction();
			_extraExtacy = 0;
		}

		public void OnExtraExtacyAdded(int val)
		{
			StartAction();
			_extraExtacy += val;
		}
	}
}
