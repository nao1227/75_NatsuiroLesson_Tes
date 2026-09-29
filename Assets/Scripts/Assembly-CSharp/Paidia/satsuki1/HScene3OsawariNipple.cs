using System.Collections.Generic;
using UniRx;

namespace Paidia.satsuki1
{
	public class HScene3OsawariNipple : OsawariNipple
	{
		private HScene3OsawariHelper _helper;

		protected override void InitializeParams()
		{
			_goods = GetComponent<OsawariGoods>();
			_goods.OnRotorAppear.Where(((RotorPlace, bool) x) => x.Item1 == RotorPlace.Breast && x.Item2).Subscribe(delegate
			{
				Cancel();
			}).AddTo(this);
			_nipple = new ParameterValue(parameters[ParameterName.Nipple]);
			_helper = _manager.GetOsawariOf<HScene3OsawariHelper>();
			List<OsawariNipple> everyOsawariOf = _manager.GetEveryOsawariOf<OsawariNipple>();
			if (everyOsawariOf[0] == this)
			{
				_another = everyOsawariOf[1];
			}
			else
			{
				_another = everyOsawariOf[0];
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (!_goods.IsRotorAppear(RotorPlace.Breast))
			{
				return _helper.ClothStatus == ClothStatus.Naked;
			}
			return false;
		}
	}
}
