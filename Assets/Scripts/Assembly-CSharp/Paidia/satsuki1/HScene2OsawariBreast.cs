namespace Paidia.satsuki1
{
	public class HScene2OsawariBreast : OsawariBrest
	{
		private Hscene2WomanBody _body;

		protected override void InitializeParams()
		{
			base.InitializeParams();
			_body = _manager.GetOsawariOf<Hscene2WomanBody>();
		}

		protected override bool GetConstraintsCore()
		{
			if (base.GetConstraintsCore())
			{
				return !_body.IsHolding;
			}
			return false;
		}
	}
}
