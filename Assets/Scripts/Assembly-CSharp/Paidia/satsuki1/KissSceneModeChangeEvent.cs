using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public class KissSceneModeChangeEvent : OsawariEvent
	{
		public bool IsH;

		public bool IsNade;

		public FaceAnimationControllerOnEndOfDay FaceAnimation;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			FaceAnimation.AfterH = IsH;
			FaceAnimation.AfterNade = IsNade;
			FaceAnimation.ResetFace();
			await UniTask.Yield(this.GetCancellationTokenOnDestroy());
		}
	}
}
