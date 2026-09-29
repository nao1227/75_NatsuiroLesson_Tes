using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class SceneEndEvent : OsawariEvent
	{
		public bool IsDayEnd = true;

		protected override void PostInitialize()
		{
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			HScene scene = Object.FindObjectOfType<OsawariManager>().Scene;
			await UniTask.Yield();
			if (scene.Name == SceneName.HScene3)
			{
				SaveLoadManager.UnsavedData.HasStudiedToday = true;
			}
			if (IsDayEnd)
			{
				SaveLoadManager.UnsavedData.RefreshDay();
			}
			scene.SetActive(active: false);
		}

		public override void Cancel()
		{
			base.Cancel();
		}
	}
}
