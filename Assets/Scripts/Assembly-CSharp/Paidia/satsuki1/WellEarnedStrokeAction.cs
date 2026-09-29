using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public class WellEarnedStrokeAction : TimeConditionAction, IAfterWomanExtacy, IStrokeTrigger
	{
		public int AfterExtacyTimeLimit;

		private bool _isInAfterExtacy;

		protected override bool GetStatusConditionCore()
		{
			return _isInAfterExtacy;
		}

		public void StartExtacy()
		{
			_isInAfterExtacy = true;
			CountAfterExtacyTime().Forget();
		}

		private async UniTask CountAfterExtacyTime()
		{
			await UniTask.Delay(AfterExtacyTimeLimit, ignoreTimeScale: false, PlayerLoopTiming.Update, cts.Token);
			_isInAfterExtacy = false;
		}
	}
}
