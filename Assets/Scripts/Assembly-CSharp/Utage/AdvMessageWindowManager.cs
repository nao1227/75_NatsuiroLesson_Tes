using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvMessageWindowManager")]
	public class AdvMessageWindowManager : MonoBehaviour, IAdvSaveData, IBinaryIO
	{
		[SerializeField]
		[Interface(typeof(IAdvMessageWindowManager))]
		private MonoBehaviour uiMessageWindowManager;

		[SerializeField]
		private MessageWindowEvent onReset = new MessageWindowEvent();

		[SerializeField]
		private MessageWindowEvent onChangeActiveWindows = new MessageWindowEvent();

		[SerializeField]
		private MessageWindowEvent onChangeCurrentWindow = new MessageWindowEvent();

		[SerializeField]
		private MessageWindowEvent onTextChange = new MessageWindowEvent();

		private AdvEngine engine;

		[NonSerialized]
		private bool isInit;

		private Dictionary<string, AdvMessageWindow> allWindows = new Dictionary<string, AdvMessageWindow>();

		private List<string> defaultActiveWindowNameList = new List<string>();

		private Dictionary<string, AdvMessageWindow> activeWindows = new Dictionary<string, AdvMessageWindow>();

		private const int Version = 0;

		public IAdvMessageWindowManager UiMessageWindowManager
		{
			get
			{
				if (uiMessageWindowManager == null)
				{
					uiMessageWindowManager = GetComponentInChildren<IAdvMessageWindowManager>(includeInactive: true) as MonoBehaviour;
				}
				return uiMessageWindowManager as IAdvMessageWindowManager;
			}
		}

		public MessageWindowEvent OnReset => onReset;

		public MessageWindowEvent OnChangeActiveWindows => onChangeActiveWindows;

		public MessageWindowEvent OnChangeCurrentWindow => onChangeCurrentWindow;

		public MessageWindowEvent OnTextChange => onTextChange;

		public AdvEngine Engine => this.GetComponentCache(ref engine);

		public Dictionary<string, AdvMessageWindow> AllWindows
		{
			get
			{
				if (!isInit)
				{
					InitWindows();
				}
				return allWindows;
			}
		}

		private List<string> DefaultActiveWindowNameList
		{
			get
			{
				if (!isInit)
				{
					InitWindows();
				}
				return defaultActiveWindowNameList;
			}
		}

		public Dictionary<string, AdvMessageWindow> ActiveWindows => activeWindows;

		public AdvMessageWindow CurrentWindow { get; private set; }

		public AdvMessageWindow LastWindow { get; private set; }

		public string SaveKey => "MessageWindowManager";

		public bool IsCurrent(string name)
		{
			return CurrentWindow.Name == name;
		}

		public bool IsActiveWindow(string name)
		{
			return ActiveWindows.ContainsKey(name);
		}

		private void InitWindows()
		{
			allWindows.Clear();
			foreach (KeyValuePair<string, IAdvMessageWindow> allWindow in UiMessageWindowManager.AllWindows)
			{
				AddWindowSub(allWindow.Value);
			}
			if (allWindows.Count <= 0)
			{
				Debug.LogError("No windows were found in the scene..");
			}
			foreach (KeyValuePair<string, AdvMessageWindow> allWindow2 in allWindows)
			{
				IAdvMessageWindow messageWindow = allWindow2.Value.MessageWindow;
				if (messageWindow.gameObject.activeSelf)
				{
					defaultActiveWindowNameList.Add(messageWindow.gameObject.name);
				}
			}
			isInit = true;
			foreach (KeyValuePair<string, AdvMessageWindow> allWindow3 in allWindows)
			{
				if (Application.isPlaying)
				{
					allWindow3.Value.MessageWindow.OnInit(this);
				}
			}
		}

		public void AddWindow(IAdvMessageWindow window)
		{
			AddWindowSub(window);
			if (Application.isPlaying)
			{
				window.OnInit(this);
			}
		}

		private void AddWindowSub(IAdvMessageWindow window)
		{
			string text = window.gameObject.name;
			if (allWindows.ContainsKey(text))
			{
				Debug.LogError(text + ". The same name already exists. Please change to a different name.");
			}
			else
			{
				allWindows.Add(text, new AdvMessageWindow(window));
			}
		}

		public void RemoveWindow(IAdvMessageWindow window)
		{
			string key = window.gameObject.name;
			if (AllWindows.ContainsKey(key))
			{
				AllWindows.Remove(window.gameObject.name);
			}
		}

		internal void EmbedWindow(IAdvMessageWindow window)
		{
			string key = window.gameObject.name;
			if (!AllWindows.ContainsKey(key))
			{
				AddWindow(window);
			}
			AllWindows[key].MessageWindow = window;
		}

		internal void ChangeActiveWindows(List<string> names)
		{
			ActiveWindows.Clear();
			foreach (string name in names)
			{
				if (!AllWindows.TryGetValue(name, out var value))
				{
					Debug.LogError(name + " is not found in message windows");
				}
				else if (!ActiveWindows.ContainsKey(name))
				{
					ActiveWindows.Add(name, value);
				}
			}
			CalllEventActiveWindows();
		}

		private void CalllEventActiveWindows()
		{
			foreach (AdvMessageWindow value in AllWindows.Values)
			{
				value.ChangeActive(IsActiveWindow(value.Name));
			}
			OnChangeActiveWindows.Invoke(this);
		}

		internal void ChangeCurrentWindow(string name)
		{
			if (string.IsNullOrEmpty(name) || (CurrentWindow != null && CurrentWindow.Name == name))
			{
				return;
			}
			if (!ActiveWindows.TryGetValue(name, out var value))
			{
				if (!AllWindows.TryGetValue(name, out value))
				{
					Debug.LogWarning(name + "is not found in window manager");
					name = DefaultActiveWindowNameList[0];
					value = AllWindows[name];
				}
				if (CurrentWindow != null)
				{
					ActiveWindows.Remove(CurrentWindow.Name);
				}
				ActiveWindows.Add(name, value);
				CalllEventActiveWindows();
			}
			LastWindow = CurrentWindow;
			CurrentWindow = value;
			if (LastWindow != null)
			{
				LastWindow.ChangeCurrent(isCurrent: false);
			}
			CurrentWindow.ChangeCurrent(isCurrent: true);
			OnChangeCurrentWindow.Invoke(this);
		}

		internal AdvMessageWindow FindWindow(string name)
		{
			AdvMessageWindow value = CurrentWindow;
			if (!string.IsNullOrEmpty(name) && !AllWindows.TryGetValue(name, out value))
			{
				Debug.LogError(name + "is not found in all message windows");
			}
			return value;
		}

		internal void OnPageTextChange(AdvPage page)
		{
			CurrentWindow.PageTextChange(page);
			OnTextChange.Invoke(this);
		}

		public virtual void OnClear()
		{
			if (DefaultActiveWindowNameList.Count <= 0)
			{
				Debug.LogWarning("defaultWindowNameList is zero");
				return;
			}
			ChangeActiveWindows(DefaultActiveWindowNameList);
			ChangeCurrentWindow(DefaultActiveWindowNameList[0]);
			foreach (AdvMessageWindow value in AllWindows.Values)
			{
				value.Reset();
			}
			OnReset.Invoke(this);
		}

		public virtual void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(ActiveWindows.Count);
			foreach (KeyValuePair<string, AdvMessageWindow> activeWindow in ActiveWindows)
			{
				writer.Write(activeWindow.Key);
				writer.WriteBuffer(activeWindow.Value.WritePageData);
			}
			string value = ((CurrentWindow == null) ? "" : CurrentWindow.Name);
			writer.Write(value);
		}

		public virtual void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				List<string> list = new List<string>();
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					string item = reader.ReadString();
					byte[] bytes = reader.ReadBytes(reader.ReadInt32());
					list.Add(item);
					BinaryUtil.BinaryRead(bytes, FindWindow(item).ReadPageData);
				}
				string text = reader.ReadString();
				ChangeActiveWindows(list);
				ChangeCurrentWindow(text);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
