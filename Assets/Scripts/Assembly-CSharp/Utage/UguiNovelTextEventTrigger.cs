using UnityEngine;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/UguiNovelTextEventTrigger")]
	[RequireComponent(typeof(UguiNovelText))]
	public class UguiNovelTextEventTrigger : MonoBehaviour, ICanvasRaycastFilter, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
	{
		private UguiNovelTextGenerator generator;

		private UguiNovelText novelText;

		private RectTransform cachedRectTransform;

		public OnClickLinkEvent OnClick = new OnClickLinkEvent();

		public OnPointerEnterEvent OnEnter = new OnPointerEnterEvent();

		public OnPointerExitEvent OnExit = new OnPointerExitEvent();

		public Color hoverColor = ColorUtil.Red;

		private UguiNovelTextHitArea currentTarget;

		private bool isEntered;

		public UguiNovelTextGenerator Generator => this.GetComponentCache(ref generator);

		public UguiNovelText NovelText => this.GetComponentCache(ref novelText);

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

		public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
		{
			UguiNovelTextHitArea uguiNovelTextHitArea = HitTest(sp, eventCamera);
			if (isEntered)
			{
				ChangeCurrentTarget(uguiNovelTextHitArea);
			}
			return uguiNovelTextHitArea != null;
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			UguiNovelTextHitArea uguiNovelTextHitArea = HitTest(eventData);
			if (uguiNovelTextHitArea != null)
			{
				OnClick.Invoke(uguiNovelTextHitArea);
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			Debug.Log("TEST Pointer: Enter");
			isEntered = true;
			UguiNovelTextHitArea uguiNovelTextHitArea = HitTest(eventData);
			ChangeCurrentTarget(uguiNovelTextHitArea);
			if (uguiNovelTextHitArea != null)
			{
				OnEnter.Invoke(uguiNovelTextHitArea);
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			Debug.Log("TEST Pointer: Exit");
			isEntered = false;
			ChangeCurrentTarget(null);
			OnExit.Invoke(null);
		}

		private UguiNovelTextHitArea HitTest(PointerEventData eventData)
		{
			return HitTest(eventData.position, eventData.pressEventCamera);
		}

		private UguiNovelTextHitArea HitTest(Vector2 screenPoint, Camera cam)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, screenPoint, cam, out var localPoint);
			foreach (UguiNovelTextHitArea hitGroupList in Generator.HitGroupLists)
			{
				if (hitGroupList.HitTest(localPoint))
				{
					return hitGroupList;
				}
			}
			return null;
		}

		private void ChangeCurrentTarget(UguiNovelTextHitArea target)
		{
			if (currentTarget != target)
			{
				if (currentTarget != null)
				{
					currentTarget.ResetEffectColor();
				}
				currentTarget = target;
				if (currentTarget != null)
				{
					currentTarget.ChangeEffectColor(hoverColor);
				}
			}
		}

		private void OnDrawGizmos()
		{
			foreach (UguiNovelTextHitArea hitGroupList in Generator.HitGroupLists)
			{
				foreach (Rect hitArea in hitGroupList.HitAreaList)
				{
					Gizmos.color = Color.yellow;
					Vector3 position = hitArea.center;
					Vector3 vector = hitArea.size;
					position = CachedRectTransform.TransformPoint(position);
					vector = CachedRectTransform.TransformVector(vector);
					Gizmos.DrawWireCube(position, vector);
				}
			}
		}
	}
}
