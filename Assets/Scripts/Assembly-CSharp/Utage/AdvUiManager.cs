using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UtageExtensions;

namespace Utage
{
	public abstract class AdvUiManager : MonoBehaviour, IAdvSaveData, IBinaryIO
	{
		public enum UiStatus
		{
			Default = 0,
			Backlog = 1,
			HideMessageWindow = 2
		}

		[SerializeField]
		protected AdvEngine engine;

		private AdvGuiManager guiManager;

		protected UiStatus status;

		private const int Version = 0;

		public AdvEngine Engine => this.GetComponentCache(ref engine);

		[SerializeField]
		public AdvGuiManager GuiManager => base.gameObject.GetComponentCacheCreateIfMissing(ref guiManager);

		public virtual UiStatus Status
		{
			get
			{
				return status;
			}
			set
			{
				if (status != value)
				{
					ChangeStatus(value);
				}
			}
		}

		public PointerEventData CurrentPointerData { get; private set; }

		public virtual bool IsPointerDowned => CurrentPointerData != null;

		public virtual bool IsInputTrig { get; set; }

		public virtual bool IsInputTrigCustom { get; set; }

		public virtual bool IsShowingMessageWindow { get; private set; }

		public bool IsShowingMenuButton
		{
			get
			{
				if (!IsHideMenuButton)
				{
					if (!IsShowingMessageWindow)
					{
						return Engine.SelectionManager.IsShowing;
					}
					return true;
				}
				return false;
			}
		}

		public bool IsHideMenuButton { get; private set; }

		[Obsolete]
		public bool IsHide => !IsShowingMessageWindow;

		[Obsolete]
		public bool IsShowingUI
		{
			get
			{
				if (!IsShowingMessageWindow)
				{
					return Engine.SelectionManager.IsShowing;
				}
				return true;
			}
		}

		public string SaveKey => "UiManager";

		public abstract void Open();

		public abstract void Close();

		protected abstract void ChangeStatus(UiStatus newStatus);

		public virtual void OnPointerDown(PointerEventData data)
		{
			CurrentPointerData = data;
			IsInputTrig = true;
		}

		protected virtual void LateUpdate()
		{
			ClearPointerDown();
			IsInputTrigCustom = false;
		}

		public virtual void ClearPointerDown()
		{
			CurrentPointerData = null;
			IsInputTrig = false;
		}

		public virtual void HideMessageWindow()
		{
			IsShowingMessageWindow = false;
		}

		public virtual void ShowMessageWindow()
		{
			IsShowingMessageWindow = true;
		}

		internal virtual void ShowMenuButton()
		{
			IsHideMenuButton = false;
		}

		internal virtual void HideMenuButton()
		{
			IsHideMenuButton = true;
		}

		public virtual void OnBeginPage()
		{
			IsShowingMessageWindow = false;
		}

		public virtual void OnEndPage()
		{
			IsShowingMessageWindow = false;
		}

		public virtual void OnClear()
		{
			IsHideMenuButton = false;
			IsShowingMessageWindow = false;
		}

		public virtual void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(IsHideMenuButton);
			writer.Write(IsShowingMessageWindow);
		}

		public virtual void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num == 0)
			{
				IsHideMenuButton = reader.ReadBoolean();
				IsShowingMessageWindow = reader.ReadBoolean();
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}
	}
}
