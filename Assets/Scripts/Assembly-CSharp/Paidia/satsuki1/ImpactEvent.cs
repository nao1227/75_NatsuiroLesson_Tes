using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class ImpactEvent : SerialEvent
	{
		[SerializeField]
		private int ManExtacy;

		[SerializeField]
		private SESet SESet;

		[SerializeField]
		private float[] ThreasholdLower;

		[SerializeField]
		private float[] ThreasholdUpper;

		private InsertController _insertController;

		private FaceController _faceController;

		protected override void PostInitialize()
		{
			_insertController = _parent.GetComponent<InsertController>();
			_faceController = _parent.GetComponent<FaceController>();
			base.PostInitialize();
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			if (SESet.AudioSetName != AudioSetName.None)
			{
				SingletonManager<SoundManager>.Instance.Play(SESet.AudioSetName, SESet.AudioCategory);
			}
			_insertController.AddWomanExtacy(StatusChange.Excite);
			_insertController.AddManExtacy(ManExtacy);
			await base.InvokeCore(status, conditions);
		}

		public override bool Trigger(float[] threasholds)
		{
			for (int i = 0; i < ThreasholdLower.Length; i++)
			{
				if (ThreasholdLower[i] > threasholds[i] || ThreasholdUpper[i] < threasholds[i])
				{
					return false;
				}
			}
			return true;
		}

		public bool IsInRange(float val)
		{
			for (int i = 0; i < ThreasholdLower.Length; i++)
			{
				if (ThreasholdLower[i] <= val && ThreasholdUpper[i] >= val)
				{
					return true;
				}
			}
			return false;
		}
	}
}
