using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class SerialEvent : OsawariEvent
	{
		[SerializeField]
		private List<OsawariEvent> Events;

		protected override void PostInitialize()
		{
			foreach (OsawariEvent @event in Events)
			{
				@event.Initialize(_parent).Forget();
			}
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			foreach (OsawariEvent @event in Events)
			{
				if (@event.IsFullfillCondition(status, conditions))
				{
					if (@event is UtageEvent)
					{
						await (@event as UtageEvent).InvokeEvent(status, conditions);
					}
					else if (@event is MessageEvent)
					{
						await (@event as MessageEvent).InvokeEvent(status, conditions);
					}
					else
					{
						await @event.InvokeEvent(status, conditions);
					}
				}
			}
		}

		public override void Cancel()
		{
			foreach (OsawariEvent item in Events.Where((OsawariEvent evt) => evt.IsInvoked))
			{
				item.Cancel();
			}
			base.Cancel();
		}
	}
}
