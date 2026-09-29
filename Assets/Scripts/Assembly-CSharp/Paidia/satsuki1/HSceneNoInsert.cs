using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class HSceneNoInsert : HScene
	{
		protected override async UniTask OnSetUp()
		{
			try
			{
				OsawariGoods.ManagedStart();
				Manager.ManagedStart();
				ActionManager.osawariManager = Manager;
				FaceController.ManagedStart();
				await FaceAnimator.ManagedStart(Manager);
				ActionManager.ManagedStart();
				_allManagerLoaded = true;
			}
			catch (OperationCanceledException)
			{
			}
		}

		protected override async UniTask SetUpEvent()
		{
			OsawariUIPresenter _presenter = null;
			while (null == _presenter)
			{
				_presenter = UnityEngine.Object.FindObjectOfType<OsawariUIPresenter>();
				await UniTask.Yield();
			}
			await UniTask.WaitUntil(() => _presenter.IsLoaded);
			foreach (OsawariEvent onSceneEnterEvent in OnSceneEnterEvents)
			{
				onSceneEnterEvent.Initialize();
				onSceneEnterEvent.InvokeEvent(Manager.TemporaryStatus, _emptyConditions);
			}
		}

		public override void ManagedUpdate()
		{
			if (_allManagerLoaded)
			{
				FaceAnimator.ManagedUpdate();
				FaceController.ManagedUpdate();
				ActionManager.ManagedUpdate();
			}
		}
	}
}
