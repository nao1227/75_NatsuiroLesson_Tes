using System.Collections.Generic;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvUguiMessageWindowManager")]
	public class AdvUguiMessageWindowManager : MonoBehaviour, IAdvMessageWindowManager
	{
		[SerializeField]
		protected List<GameObject> messageWindowList;

		protected Dictionary<string, IAdvMessageWindow> allWindows;

		public virtual Dictionary<string, IAdvMessageWindow> AllWindows
		{
			get
			{
				if (allWindows == null)
				{
					InitWindows();
				}
				return allWindows;
			}
		}

		GameObject IAdvMessageWindowManager.gameObject => base.gameObject;

		protected virtual void InitWindows()
		{
			allWindows = new Dictionary<string, IAdvMessageWindow>();
			foreach (GameObject messageWindow2 in messageWindowList)
			{
				IAdvMessageWindow component = messageWindow2.GetComponent<IAdvMessageWindow>();
				if (component == null)
				{
					Debug.LogErrorFormat("{0} is not MessageWindow");
				}
				else
				{
					AddWindow(component);
				}
			}
			IAdvMessageWindow[] componentsInChildren = GetComponentsInChildren<IAdvMessageWindow>(includeInactive: true);
			foreach (IAdvMessageWindow messageWindow in componentsInChildren)
			{
				AddWindow(messageWindow);
			}
		}

		protected virtual void AddWindow(IAdvMessageWindow messageWindow)
		{
			string key = messageWindow.gameObject.name;
			if (allWindows.ContainsKey(key))
			{
				if (!allWindows.ContainsValue(messageWindow))
				{
					Debug.LogErrorFormat("{0} is already exists in windows");
				}
			}
			else
			{
				allWindows.Add(key, messageWindow);
			}
		}

		internal virtual void Close()
		{
			base.gameObject.SetActive(value: false);
		}

		internal virtual void Open()
		{
			base.gameObject.SetActive(value: true);
		}
	}
}
