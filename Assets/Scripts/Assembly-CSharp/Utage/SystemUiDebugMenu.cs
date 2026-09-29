using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/System UI/SystemUiDebugMenu")]
	public class SystemUiDebugMenu : MonoBehaviour
	{
		private enum Mode
		{
			Hide = 0,
			Info = 1,
			Log = 2,
			Memu = 3,
			Max = 4
		}

		[SerializeField]
		private GameObject buttonRoot;

		[SerializeField]
		private GameObject buttonViewRoot;

		[SerializeField]
		private UguiLocalize buttonText;

		[SerializeField]
		private GameObject debugInfo;

		[SerializeField]
		private Text debugInfoText;

		[SerializeField]
		private GameObject debugLog;

		[SerializeField]
		private Text debugLogText;

		[SerializeField]
		private bool autoUpdateLogText = true;

		[SerializeField]
		private GameObject rootDebugMenu;

		[SerializeField]
		private GameObject targetDeleteAllSaveData;

		[SerializeField]
		private bool enabeReleaseBuild;

		private Mode currentMode;

		private bool Ignore
		{
			get
			{
				if (!enabeReleaseBuild)
				{
					return !Debug.isDebugBuild;
				}
				return false;
			}
		}

		private void Start()
		{
			if (Ignore)
			{
				buttonRoot.SetActive(value: false);
			}
			ClearAll();
			ChangeMode(currentMode);
		}

		public void OnClickSwitchButton()
		{
			if (!Ignore)
			{
				ChangeMode(currentMode + 1);
			}
		}

		private void ChangeMode(Mode mode)
		{
			if (currentMode != mode)
			{
				if (mode >= Mode.Max)
				{
					mode = Mode.Hide;
				}
				currentMode = mode;
				ClearAll();
				StopAllCoroutines();
				switch (currentMode)
				{
				case Mode.Info:
					StartCoroutine(CoUpdateInfo());
					break;
				case Mode.Log:
					StartCoroutine(CoUpdateLog());
					break;
				case Mode.Memu:
					StartCoroutine(CoUpdateMenu());
					break;
				case Mode.Hide:
					break;
				}
			}
		}

		private void ClearAll()
		{
			buttonViewRoot.SetActive(value: false);
			debugInfo.SetActive(value: false);
			debugLog.SetActive(value: false);
			rootDebugMenu.SetActive(value: false);
		}

		private IEnumerator CoUpdateInfo()
		{
			buttonViewRoot.SetActive(value: true);
			buttonText.Key = SystemText.DebugInfo.ToString();
			debugInfo.SetActive(value: true);
			while (true)
			{
				debugInfoText.text = DebugPrint.GetDebugString();
				yield return null;
			}
		}

		private IEnumerator CoUpdateLog()
		{
			buttonViewRoot.SetActive(value: true);
			buttonText.Key = SystemText.DebugLog.ToString();
			debugLog.SetActive(value: true);
			if (autoUpdateLogText)
			{
				debugLogText.text += DebugPrint.GetLogString();
			}
			yield break;
		}

		private IEnumerator CoUpdateMenu()
		{
			buttonViewRoot.SetActive(value: true);
			buttonText.Key = SystemText.DebugMenu.ToString();
			rootDebugMenu.SetActive(value: true);
			yield break;
		}

		public void OnClickDeleteAllSaveDataAndQuit()
		{
			targetDeleteAllSaveData.SafeSendMessage("OnDeleteAllSaveDataAndQuit");
			PlayerPrefs.DeleteAll();
			Application.Quit();
		}

		public void OnClickDeleteAllCacheFiles()
		{
			AssetFileManager.GetInstance().AssetBundleInfoManager.DeleteAllCache();
		}

		public void OnClickChangeLanguage()
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (!(instance == null) && instance.Languages.Count >= 1)
			{
				int num = instance.Languages.IndexOf(instance.CurrentLanguage);
				instance.CurrentLanguage = instance.Languages[(num + 1) % instance.Languages.Count];
			}
		}

		public void ChangeLanguage(string language)
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (!(instance == null) && instance.Languages.Count >= 1)
			{
				instance.CurrentLanguage = language;
			}
		}

		public void ChangeVoiceLanguage(string language)
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (!(instance == null))
			{
				instance.VoiceLanguage = language;
			}
		}

		public void ResetVoiceLanguage()
		{
			LanguageManagerBase instance = LanguageManagerBase.Instance;
			if (!(instance == null))
			{
				instance.VoiceLanguage = "";
			}
		}
	}
}
