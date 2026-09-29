using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public class HScene3ModeChangeEvent : OsawariEvent
	{
		public HScene3OsawariHelper Helper;

		public HScene4Mode Mode = HScene4Mode.H;

		public bool StudyHAfterH;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			if (Mode == HScene4Mode.StudyH)
			{
				Helper.MoveToStudyHMode().Forget();
			}
			else
			{
				Helper.ChangeMode(Mode);
				if (StudyHAfterH)
				{
					Helper.MoveToStudyHMode().Forget();
				}
			}
			await UniTask.Yield();
		}
	}
}
