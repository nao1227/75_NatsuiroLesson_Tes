using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/AdvSelectionTimeLimitText")]
	public class AdvSelectionTimeLimitText : MonoBehaviour
	{
		[SerializeField]
		protected GameObject targetRoot;

		[SerializeField]
		protected Text text;

		protected AdvSelectionTimeLimit timeLimit;

		[SerializeField]
		protected AdvEngine engine;

		public GameObject TargetRoot
		{
			get
			{
				if (targetRoot == null)
				{
					targetRoot = base.gameObject;
				}
				return targetRoot;
			}
		}

		public Text Target => this.GetComponentCacheInChildren(ref text);

		public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

		private void Awake()
		{
			Engine.SelectionManager.OnBeginWaitInput.AddListener(OnBeginWaitInput);
			Engine.SelectionManager.OnUpdateWaitInput.AddListener(OnUpdateWaitInput);
			Engine.SelectionManager.OnSelected.AddListener(OnSelected);
			Engine.SelectionManager.OnClear.AddListener(OnClear);
			TargetRoot.SetActive(value: false);
		}

		private void OnBeginWaitInput(AdvSelectionManager selection)
		{
			timeLimit = Object.FindObjectOfType<AdvSelectionTimeLimit>();
			if (timeLimit != null)
			{
				TargetRoot.SetActive(value: true);
			}
		}

		private void OnUpdateWaitInput(AdvSelectionManager selection)
		{
			if (TargetRoot.activeSelf && timeLimit != null)
			{
				Target.text = Mathf.CeilToInt(timeLimit.limitTime - timeLimit.TimeCount).ToString() ?? "";
			}
		}

		private void OnSelected(AdvSelectionManager selection)
		{
			TargetRoot.SetActive(value: false);
		}

		private void OnClear(AdvSelectionManager selection)
		{
			TargetRoot.SetActive(value: false);
		}
	}
}
