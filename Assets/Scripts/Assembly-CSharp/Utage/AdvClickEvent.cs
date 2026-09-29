using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvClickEvent")]
	internal class AdvClickEvent : MonoBehaviour, IPointerClickHandler, IEventSystemHandler, IAdvClickEvent
	{
		private AdvGraphicBase advGraphic;

		private AdvGraphicBase AdvGraphic => this.GetComponentCache(ref advGraphic);

		private StringGridRow Row { get; set; }

		private UnityAction<BaseEventData> action { get; set; }

		GameObject IAdvClickEvent.gameObject => base.gameObject;

		private void Awake()
		{
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (action != null)
			{
				action(eventData);
			}
		}

		public virtual void AddClickEvent(bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action)
		{
			Row = row;
			this.action = action;
			SetEnableCanvasRaycaster(enable: true);
		}

		public virtual void RemoveClickEvent()
		{
			Row = null;
			action = null;
			SetEnableCanvasRaycaster(enable: false);
		}

		private void SetEnableCanvasRaycaster(bool enable)
		{
			Canvas componentInParent = GetComponentInParent<Canvas>();
			if (!(componentInParent == null))
			{
				componentInParent.GetComponentCreateIfMissing<GraphicRaycaster>().enabled = enable;
			}
		}
	}
}
