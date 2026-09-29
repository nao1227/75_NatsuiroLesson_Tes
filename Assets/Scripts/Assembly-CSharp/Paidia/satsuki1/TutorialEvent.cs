using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class TutorialEvent : OsawariEvent
	{
		public TutorialName SceneName;

		private TutorialPresenter _tutorialPresenter;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			if (!_tutorialPresenter)
			{
				_tutorialPresenter = Object.FindObjectOfType<TutorialPresenter>();
			}
			_tutorialPresenter.Show(SceneName);
			await UniTask.WaitUntil(() => _tutorialPresenter.CG.alpha == 0f, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
		}
	}
}
