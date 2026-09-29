namespace Paidia.satsuki1
{
	public class Hscene2WomanBody : OsawariHelper
	{
		private ParameterValue _body;

		private ParameterValue _hold;

		private bool _started;

		private bool _clickAllowd;

		public PenisFollowerHScene2 _follower;

		public bool IsHolding => parameters[ParameterName.Hold].Value > 0f;

		public override bool ClickAllowed()
		{
			return _clickAllowd;
		}

		protected override void InitializeParams()
		{
			_body = new ParameterValue(parameters[ParameterName.WomanBodyY]);
			_hold = new ParameterValue(parameters[ParameterName.Hold]);
			_clickAllowd = true;
			_started = true;
		}

		protected override void OnLateUpdate()
		{
			if (_started)
			{
				_follower.SetEnable((double)parameters[ParameterName.Hold].Value <= 0.13);
			}
		}

		public float GetCSValue()
		{
			return parameters[ParameterName.WomanBodyY].Value;
		}
	}
}
