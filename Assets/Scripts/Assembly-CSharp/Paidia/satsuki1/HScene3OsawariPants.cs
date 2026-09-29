using UnityEngine;

namespace Paidia.satsuki1
{
	public class HScene3OsawariPants : OsawariPants, IPants
	{
		private OsawariLeg _leg;

		private HScene3OsawariHelper _helper;

		private HScene3OsawariGoods _goods;

		protected override void InitializeParams()
		{
			base.InitializeParams();
			_leg = _manager.GetOsawariOf<OsawariLeg>();
			_helper = _manager.GetOsawariOf<HScene3OsawariHelper>();
			_goods = Object.FindObjectOfType<HScene3OsawariGoods>();
			if (_helper.ClothStatus == ClothStatus.Naked || _helper.ClothStatus == ClothStatus.SwimSuit)
			{
				InactivatePants();
			}
			OnPants();
		}

		protected override bool GetConstraintsCore()
		{
			if (_leg.IsOpen && _helper.ClothStatus != ClothStatus.Naked)
			{
				return !_goods.IsVibratorAppeared;
			}
			return false;
		}

		public override bool IsAbleToInsert()
		{
			if (!base.IsAbleToInsert())
			{
				return _helper.ClothStatus == ClothStatus.Naked;
			}
			return true;
		}
	}
}
