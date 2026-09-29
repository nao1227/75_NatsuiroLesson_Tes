using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/UI/UguiBackgroundRaycaster ")]
	[RequireComponent(typeof(Camera))]
	public class UguiBackgroundRaycaster : BaseRaycaster
	{
		private Camera cachedCamera;

		[SerializeField]
		private LetterBoxCamera letterBoxCamera;

		[SerializeField]
		private int m_Priority = int.MaxValue;

		[NonSerialized]
		private List<GameObject> targetObjectList = new List<GameObject>();

		public override Camera eventCamera => CachedCamera;

		private Camera CachedCamera => this.GetComponentCache(ref cachedCamera);

		public override int sortOrderPriority => m_Priority;

		public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
		{
			Vector2 vector = ((!(letterBoxCamera == null)) ? ((Vector2)letterBoxCamera.CachedCamera.ScreenToViewportPoint(eventData.position)) : new Vector2(eventData.position.x / (float)Screen.width, eventData.position.y / (float)Screen.height));
			if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
			{
				return;
			}
			int num = 0;
			foreach (GameObject targetObject in targetObjectList)
			{
				resultAppendList.Add(new RaycastResult
				{
					distance = float.MaxValue,
					gameObject = targetObject,
					index = num++,
					module = this
				});
			}
		}

		public void AddTarget(GameObject go)
		{
			if (!targetObjectList.Contains(go))
			{
				targetObjectList.Add(go);
			}
		}

		public void RemoveTarget(GameObject go)
		{
			if (targetObjectList.Contains(go))
			{
				targetObjectList.Remove(go);
			}
		}
	}
}
