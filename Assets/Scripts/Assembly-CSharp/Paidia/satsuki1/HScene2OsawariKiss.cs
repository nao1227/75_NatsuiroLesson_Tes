namespace Paidia.satsuki1
{
	public class HScene2OsawariKiss : OsawariKiss
	{
		private bool _isHolding;

		protected override void InitializeParams()
		{
			base.InitializeParams();
		}

		public void SetIsHolding(bool isHolding)
		{
			_isHolding = isHolding;
			if (!isHolding)
			{
				IsAuto = false;
				OnMouseUp();
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (base.GetConstraintsCore())
			{
				return _isHolding;
			}
			return false;
		}
	}
}
