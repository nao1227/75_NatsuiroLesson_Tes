using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class SubIconTray : MonoBehaviour
	{
		public List<SubIconObject> SubIcons;

		public bool IsShowing { get; private set; } = true;

		public bool IsMouseOnAnyIcon => SubIcons.Any((SubIconObject x) => x.IsMouseOn);

		public void ShowSubIcon()
		{
			foreach (SubIconObject subIcon in SubIcons)
			{
				subIcon.ShowIcon(show: true);
			}
			IsShowing = true;
		}

		public void HideSubIcon()
		{
			foreach (SubIconObject subIcon in SubIcons)
			{
				subIcon.ShowIcon(show: false);
			}
			IsShowing = false;
		}

		public bool HasSubIcon(SubIconType iconType)
		{
			return SubIcons.Count((SubIconObject x) => x.SubIconType == iconType) > 0;
		}

		public async UniTask<SubIconObject> GetSubIcon(SubIconType iconType, Func<bool> clickAllowedFunc = null)
		{
			if (SubIcons.Count((SubIconObject x) => x.SubIconType == iconType) > 0)
			{
				SubIconObject bt = SubIcons.First((SubIconObject x) => x.SubIconType == iconType);
				if (!bt.IsLoaded)
				{
					await bt.ManagedStart(clickAllowedFunc);
				}
				return bt;
			}
			return null;
		}

		public IEnumerable<SubIconObject> GetSubIcons(bool excpetWearAll = true, bool exceptUndressAll = true)
		{
			return from x in SubIcons
				where x.SubIconType != SubIconType.WearAll || !excpetWearAll
				where x.SubIconType != SubIconType.TakeOffAll || !exceptUndressAll
				select x;
		}
	}
}
