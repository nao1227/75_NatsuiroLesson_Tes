using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class UtageEvent : OsawariEvent
	{
		[SerializeField]
		private ScenarioLabel Label;

		public bool SkipIfRead = true;

		public bool IsDayEndScenario;

		private UtageManager _manager;

		private CancellationToken _token;

		private bool _loaded;

		private OsawariConditions _conditions;

		private void Start()
		{
			_token = this.GetCancellationTokenOnDestroy();
			Load().Forget();
		}

		private async UniTask Load()
		{
			while (null == _manager)
			{
				_manager = Object.FindObjectOfType<UtageManager>();
				await UniTask.Yield(_token);
			}
			_loaded = true;
		}

		public new async UniTask InvokeEvent(TemporaryStatus status, OsawariConditions conditions)
		{
			if (IsFullfillCondition(status, conditions) && (!SkipIfRead || !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(Label)) && _waitPhase != EventWaitPhase.WaitingForInvoke)
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
			_conditions = conditions;
			await InvokeEventAsync();
		}

		public override async UniTask InvokeEventAsync()
		{
			TemporaryStatus temporaryStatus = Object.FindObjectOfType<StatusObject>().TemporaryStatus;
			if ((!SkipIfRead || !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(Label)) && !Conditions.Any((EventCondition x) => !x.IsFullfillCondition(temporaryStatus, _conditions)) && ScenarioReadCondition.IsFullfillCondition() && FlagCondition.IsFullfillCondition())
			{
				IsInvoked = true;
				await UniTask.WaitUntil(() => !_manager.IsPlaying && _loaded, PlayerLoopTiming.Update, _token);
				await _manager.ShowUtageText(Label, _token);
				if (IsDayEndScenario)
				{
					SaveLoadManager.UnsavedData.RefreshDay();
				}
			}
		}
	}
}
