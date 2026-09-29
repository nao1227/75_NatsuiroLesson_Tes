using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Utage
{
	internal interface IAdvClickEvent
	{
		GameObject gameObject { get; }

		void AddClickEvent(bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action);

		void RemoveClickEvent();
	}
}
