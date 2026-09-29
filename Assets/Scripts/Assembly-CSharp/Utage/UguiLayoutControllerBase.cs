using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[ExecuteInEditMode]
	public abstract class UguiLayoutControllerBase : MonoBehaviour
	{
		[SerializeField]
		private bool checkTransformChanged = true;

		private RectTransform cachedRectTransform;

		protected DrivenRectTransformTracker tracker;

		public bool CheckTransformChanged
		{
			get
			{
				return checkTransformChanged;
			}
			set
			{
				checkTransformChanged = value;
			}
		}

		public RectTransform CachedRectTransform
		{
			get
			{
				if (cachedRectTransform == null)
				{
					cachedRectTransform = GetComponent<RectTransform>();
				}
				return cachedRectTransform;
			}
		}

		protected virtual void OnEnable()
		{
			SetDirty();
		}

		protected virtual void OnDisable()
		{
			tracker.Clear();
		}

		protected void SetDirty()
		{
			if (base.gameObject.activeInHierarchy)
			{
				LayoutRebuilder.MarkLayoutForRebuild(CachedRectTransform);
			}
		}

		protected virtual void Update()
		{
			if (!CheckTransformChanged || CheckTransform())
			{
				SetDirty();
			}
		}

		private bool CheckTransform()
		{
			bool result = false;
			if (CachedRectTransform.hasChanged)
			{
				CachedRectTransform.hasChanged = false;
				result = true;
			}
			int childCount = base.transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				RectTransform rectTransform = base.transform.GetChild(i) as RectTransform;
				if (!(rectTransform == null) && rectTransform.hasChanged)
				{
					rectTransform.hasChanged = false;
					result = true;
				}
			}
			return result;
		}
	}
}
