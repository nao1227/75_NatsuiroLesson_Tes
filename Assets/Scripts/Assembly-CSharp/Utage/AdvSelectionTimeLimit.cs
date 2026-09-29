using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/AdvSelectionTimeLimit")]
	public class AdvSelectionTimeLimit : MonoBehaviour
	{
		[SerializeField]
		private bool disable;

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		protected AdvUguiSelection selection;

		public float limitTime = 10f;

		public int timeLimitIndex = -1;

		private float time;

		public bool Disable
		{
			get
			{
				return disable;
			}
			set
			{
				disable = value;
			}
		}

		public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

		public AdvUguiSelection Selection => this.GetComponentCache(ref selection);

		public float TimeCount => time;

		private void Awake()
		{
			Engine.SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			Engine.SelectionManager.OnUpdateWaitInput.AddListener(OnUpdateWaitInput);
		}

		private void OnBeginWaitInput(AdvSelectionManager selection)
		{
			time = 0f - Engine.Time.DeltaTime;
		}

		private void OnUpdateWaitInput(AdvSelectionManager selection)
		{
			time += Engine.Time.DeltaTime;
			if (!(time >= limitTime) || !Engine.SelectionManager.IsWaitInput)
			{
				return;
			}
			if (timeLimitIndex < 0)
			{
				if (Selection != null)
				{
					selection.Select(Selection.Data);
				}
			}
			else
			{
				selection.Select(timeLimitIndex);
			}
		}
	}
}
