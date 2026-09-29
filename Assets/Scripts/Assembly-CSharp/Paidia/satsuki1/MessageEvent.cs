using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class MessageEvent : OsawariEvent
	{
		[Multiline]
		public string Message;

		public MessageType MessageType;

		public Sprite Sprite;

		protected override void PostInitialize()
		{
		}

		public override async UniTask InvokeEvent(TemporaryStatus status, OsawariConditions conditions)
		{
			if (IsFullfillCondition(status, conditions) && _waitPhase != EventWaitPhase.WaitingForInvoke)
			{
				if (_waitPhase == EventWaitPhase.WaitingForCancel)
				{
					_ctsForCancel.Cancel();
					return;
				}
				IsInvoked = true;
				await AwaitInvokeEvent(status, conditions);
			}
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			MessageWindowUIPresenter messageWindowUIPresenter = Object.FindObjectOfType<MessageWindowUIPresenter>();
			if (!(null == messageWindowUIPresenter))
			{
				switch (MessageType)
				{
				case MessageType.Short:
					await messageWindowUIPresenter.SetShortMessage(Message);
					break;
				case MessageType.Long:
					await messageWindowUIPresenter.SetLongMessage(Message);
					break;
				case MessageType.LongWithImage:
					await messageWindowUIPresenter.SetLongMessage(Message, Sprite);
					break;
				}
			}
		}

		public override void Cancel()
		{
			base.Cancel();
		}
	}
}
