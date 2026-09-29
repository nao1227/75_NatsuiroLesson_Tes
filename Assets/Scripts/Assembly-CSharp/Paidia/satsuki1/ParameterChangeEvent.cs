using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;

namespace Paidia.satsuki1
{
	public class ParameterChangeEvent : OsawariEvent
	{
		public float TargetValue;

		public int ParameterNumber;

		public bool IsCS;

		public CubismModel Model;

		private CubismParameter _param;

		private bool toUpdate;

		protected override void PostInitialize()
		{
			_param = Model.Parameters[ParameterNumber];
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			toUpdate = true;
			await UniTask.Yield();
		}

		private void LateUpdate()
		{
			if (toUpdate)
			{
				_param.Value = TargetValue;
				toUpdate = false;
			}
		}
	}
}
