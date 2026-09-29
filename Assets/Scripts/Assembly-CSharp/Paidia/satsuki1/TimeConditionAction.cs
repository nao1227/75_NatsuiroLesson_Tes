using System.Threading;
using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public abstract class TimeConditionAction : OsawariAction
	{
		public int LeastTime;

		protected CancellationTokenSource cts;

		protected bool _timeCondition;

		public override bool GetStatusCondition()
		{
			if (GetStatusConditionCore())
			{
				return _timeCondition;
			}
			return false;
		}

		protected virtual bool GetStatusConditionCore()
		{
			return base.GetStatusCondition();
		}

		public override void Initialize(TemporaryStatus status)
		{
			_timeCondition = false;
			cts = new CancellationTokenSource();
			base.Initialize(status);
		}

		public override void StartAction()
		{
			if (!_timeCondition && GetStatusConditionCore())
			{
				_timeCondition = true;
				CountTime(cts.Token).Forget();
			}
		}

		protected async UniTask CountTime(CancellationToken token)
		{
			await UniTask.Delay(LeastTime, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			if (_timeCondition)
			{
				CountAction();
				_timeCondition = false;
			}
		}

		public virtual void CancelAction()
		{
			if (_timeCondition)
			{
				cts.Cancel();
				cts = new CancellationTokenSource();
				_timeCondition = false;
			}
		}

		private void OnDestroy()
		{
			cts?.Cancel();
		}
	}
}
