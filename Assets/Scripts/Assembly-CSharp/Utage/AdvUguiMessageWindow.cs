using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvUguiMessageWindow")]
	public class AdvUguiMessageWindow : MonoBehaviour, IAdvMessageWindow, IAdvMessageWindowCaracterCountChecker
	{
		protected enum ReadColorMode
		{
			None = 0,
			Change = 1
		}

		[SerializeField]
		protected AdvEngine engine;

		[SerializeField]
		protected ReadColorMode readColorMode;

		[SerializeField]
		protected Color readColor = new Color(0.8f, 0.8f, 0.8f);

		protected Color defaultTextColor = Color.white;

		protected Color defaultNameTextColor = Color.white;

		[SerializeField]
		protected UguiNovelText text;

		[SerializeField]
		protected Text nameText;

		[SerializeField]
		protected GameObject rootChildren;

		[SerializeField]
		[FormerlySerializedAs("transrateMessageWindowRoot")]
		protected CanvasGroup translateMessageWindowRoot;

		[SerializeField]
		protected GameObject iconWaitInput;

		[SerializeField]
		protected GameObject iconBrPage;

		[SerializeField]
		protected bool isLinkPositionIconBrPage = true;

		public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

		public UguiNovelText Text => text;

		public bool IsCurrent { get; protected set; }

		GameObject IAdvMessageWindow.gameObject => base.gameObject;

		GameObject IAdvMessageWindowCaracterCountChecker.gameObject => base.gameObject;

		public virtual void OnInit(AdvMessageWindowManager windowManager)
		{
			defaultTextColor = text.color;
			if ((bool)nameText)
			{
				defaultNameTextColor = nameText.color;
			}
			Clear();
		}

		protected virtual void Clear()
		{
			text.text = "";
			text.LengthOfView = 0;
			if ((bool)nameText)
			{
				nameText.text = "";
			}
			if ((bool)iconWaitInput)
			{
				iconWaitInput.SetActive(value: false);
			}
			if ((bool)iconBrPage)
			{
				iconBrPage.SetActive(value: false);
			}
			rootChildren.SetActive(value: false);
		}

		public virtual void OnReset()
		{
			Clear();
		}

		public virtual void OnChangeCurrent(bool isCurrent)
		{
			IsCurrent = isCurrent;
		}

		public virtual void OnChangeActive(bool isActive)
		{
			base.gameObject.SetActive(isActive);
			if (!isActive)
			{
				Clear();
			}
			else
			{
				rootChildren.SetActive(value: true);
			}
		}

		public virtual void OnTextChanged(AdvMessageWindow window)
		{
			if ((bool)text)
			{
				text.text = "";
				text.text = window.Text.OriginalText;
				text.LengthOfView = window.TextLength;
			}
			if ((bool)nameText)
			{
				nameText.text = "";
				nameText.text = window.NameText;
			}
			ReadColorMode readColorMode = this.readColorMode;
			if (readColorMode != ReadColorMode.None && readColorMode == ReadColorMode.Change)
			{
				text.color = (Engine.Page.CheckReadPage() ? readColor : defaultTextColor);
				if ((bool)nameText)
				{
					nameText.color = (Engine.Page.CheckReadPage() ? readColor : defaultNameTextColor);
				}
			}
			LinkIcon();
		}

		protected virtual void Awake()
		{
			if (!rootChildren.activeSelf)
			{
				rootChildren.SetActive(value: true);
				rootChildren.SetActive(value: false);
			}
		}

		protected virtual void LateUpdate()
		{
			if (Engine.UiManager.Status == AdvUiManager.UiStatus.Default)
			{
				rootChildren.SetActive(Engine.UiManager.IsShowingMessageWindow);
				if (Engine.UiManager.IsShowingMessageWindow)
				{
					translateMessageWindowRoot.alpha = Engine.Config.MessageWindowAlpha;
				}
			}
			UpdateCurrent();
		}

		protected virtual void UpdateCurrent()
		{
			if (IsCurrent && Engine.UiManager.Status == AdvUiManager.UiStatus.Default)
			{
				if (Engine.UiManager.IsShowingMessageWindow)
				{
					text.LengthOfView = Engine.Page.CurrentTextLength;
				}
				LinkIcon();
			}
		}

		protected virtual void LinkIcon()
		{
			if (iconWaitInput == null)
			{
				LinkIconSub(iconBrPage, Engine.Page.IsWaitInputInPage || Engine.Page.IsWaitBrPage);
				return;
			}
			LinkIconSub(iconWaitInput, Engine.Page.IsWaitInputInPage);
			LinkIconSub(iconBrPage, Engine.Page.IsWaitBrPage);
		}

		protected virtual void LinkIconSub(GameObject icon, bool isActive)
		{
			if (icon == null)
			{
				return;
			}
			if (!Engine.UiManager.IsShowingMessageWindow)
			{
				icon.SetActive(value: false);
				return;
			}
			icon.SetActive(isActive);
			if (isLinkPositionIconBrPage)
			{
				icon.transform.localPosition = text.CurrentEndPosition;
			}
		}

		public virtual void OnTapCloseWindow()
		{
			Engine.UiManager.Status = AdvUiManager.UiStatus.HideMessageWindow;
		}

		public virtual void OnTapBackLog()
		{
			Engine.UiManager.Status = AdvUiManager.UiStatus.Backlog;
		}

		public virtual string StartCheckCaracterCount()
		{
			if (text == null)
			{
				return "";
			}
			return text.text;
		}

		public virtual bool TryCheckCaracterCount(string text, out int count, out string errorString)
		{
			return this.text.TextGenerator.EditorCheckRect(text, out count, out errorString);
		}

		public virtual void EndCheckCaracterCount(string text)
		{
			if (!(this.text == null))
			{
				this.text.text = text;
			}
		}
	}
}
