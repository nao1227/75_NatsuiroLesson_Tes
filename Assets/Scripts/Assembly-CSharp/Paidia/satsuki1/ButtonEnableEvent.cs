using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class ButtonEnableEvent : OsawariEvent
	{
		public ButtonName ButtonName;

		public bool Enable;

		private OsawariUIPresenter _presenter;

		protected override void PostInitialize()
		{
			_presenter = Object.FindObjectOfType<OsawariUIPresenter>();
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			_presenter.SetButtonEnable(ButtonName, Enable);
		}
	}
}
