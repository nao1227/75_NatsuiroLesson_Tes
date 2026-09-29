using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class HScene : Scene
	{
		public InsertController InsertController;

		public FaceController FaceController;

		public FaceController AnotherModelFaceController;

		public OsawariManager Manager;

		public ActionManager ActionManager;

		public FaceAnimationController FaceAnimator;

		public OsawariGoods OsawariGoods;

		public List<OsawariEvent> OnSceneEnterEvents;

		public List<Live2DAnimator> Animators;

		protected bool _allManagerLoaded;

		protected override async UniTask OnSetUp()
		{
			foreach (Live2DAnimator animator in Animators)
			{
				animator.ManagedStart();
			}
			InsertController.ManagedStart();
			OsawariGoods.ManagedStart();
			Manager.ManagedStart();
			ActionManager.InsertController = InsertController;
			ActionManager.osawariManager = Manager;
			FaceController.ManagedStart();
			AnotherModelFaceController?.ManagedStart();
			await FaceAnimator.ManagedStart(Manager);
			ActionManager.ManagedStart();
			_allManagerLoaded = true;
		}

		protected override void SetUpButtonEvents()
		{
			foreach (OsawariEvent item in OnSceneEnterEvents.Where((OsawariEvent x) => x is ButtonEnableEvent))
			{
				item.Initialize();
				item.InvokeEvent(Manager.TemporaryStatus, _emptyConditions);
			}
		}

		protected override async UniTask SetUpEvent()
		{
			OsawariUIPresenter _presenter = null;
			while (null == _presenter)
			{
				_presenter = Object.FindObjectOfType<OsawariUIPresenter>();
				await UniTask.Yield();
			}
			await UniTask.WaitUntil(() => _presenter.IsLoaded, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			base.HeaderEnabled = true;
			foreach (OsawariEvent item in OnSceneEnterEvents.Where((OsawariEvent x) => x is ButtonEnableEvent))
			{
				item.Initialize();
				item.InvokeEvent(Manager.TemporaryStatus, _emptyConditions);
			}
			if ((SaveLoadManager.UnsavedData.Days != 4 && SaveLoadManager.UnsavedData.Days != 6) || Name != SceneName.HScene3)
			{
				Object.FindObjectOfType<FaceAnimationController>().SetAnimationEnable();
				_presenter.SetFadeInEnable();
			}
			foreach (OsawariEvent item2 in OnSceneEnterEvents.Where((OsawariEvent x) => !(x is ButtonEnableEvent)))
			{
				item2.Initialize();
				await item2.InvokeEvent(Manager.TemporaryStatus, _emptyConditions);
			}
		}

		public override void ManagedUpdate()
		{
			if (_allManagerLoaded)
			{
				base.ManagedUpdate();
				FaceAnimator.ManagedUpdate();
				FaceController.ManagedUpdate();
				AnotherModelFaceController?.ManagedUpdate();
				InsertController.ManagedUpdate();
				ActionManager.ManagedUpdate();
			}
		}
	}
}
