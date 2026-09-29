using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvEngine")]
	[RequireComponent(typeof(DontDestoryOnLoad))]
	[RequireComponent(typeof(AdvDataManager))]
	[RequireComponent(typeof(AdvScenarioPlayer))]
	[RequireComponent(typeof(AdvPage))]
	[RequireComponent(typeof(AdvMessageWindowManager))]
	[RequireComponent(typeof(AdvSelectionManager))]
	[RequireComponent(typeof(AdvBacklogManager))]
	[RequireComponent(typeof(AdvConfig))]
	[RequireComponent(typeof(AdvSystemSaveData))]
	[RequireComponent(typeof(AdvSaveManager))]
	public class AdvEngine : MonoBehaviour
	{
		private string startScenarioLabel = "Start";

		private AdvDataManager dataManager;

		private AdvScenarioPlayer scenarioPlayer;

		private AdvPage page;

		private AdvSelectionManager selectionManager;

		private AdvMessageWindowManager messageWindowManager;

		private AdvBacklogManager backlogManager;

		private AdvConfig config;

		private AdvSystemSaveData systemSaveData;

		private AdvSaveManager saveManager;

		[SerializeField]
		private AdvGraphicManager graphicManager;

		[SerializeField]
		private AdvEffectManager effectManager;

		[SerializeField]
		private AdvUiManager uiManager;

		[SerializeField]
		[FormerlySerializedAs("soundManger")]
		private SoundManager soundManager;

		[SerializeField]
		private CameraManager cameraManager;

		[SerializeField]
		private AdvTime time;

		private AdvParamManager param = new AdvParamManager();

		[SerializeField]
		private bool bootAsync;

		[SerializeField]
		private bool isStopSoundOnStart = true;

		[SerializeField]
		private bool isStopSoundOnEnd = true;

		[SerializeField]
		private bool isStopVoiceOnSoundStop = true;

		[SerializeField]
		private string languageKeyOfParam = "";

		[SerializeField]
		private string voiceLanguageKeyOfParam = "";

		private List<AdvCustomCommandManager> customCommandManagerList;

		public UnityEvent onPreInit;

		[SerializeField]
		private OpenDialogEvent onOpenDialog;

		[SerializeField]
		private AdvEvent onPageTextChange = new AdvEvent();

		public AdvEvent OnClear;

		[SerializeField]
		public AdvEvent onChangeLanguage = new AdvEvent();

		private bool isWaitBootLoading = true;

		private bool isStarted;

		private bool isSceneGallery;

		public string StartScenarioLabel
		{
			get
			{
				return startScenarioLabel;
			}
			set
			{
				startScenarioLabel = value;
			}
		}

		public AdvDataManager DataManager => this.GetComponentCache(ref dataManager);

		public AdvScenarioPlayer ScenarioPlayer => this.GetComponentCache(ref scenarioPlayer);

		public AdvPage Page => this.GetComponentCache(ref page);

		public AdvSelectionManager SelectionManager => this.GetComponentCache(ref selectionManager);

		public AdvMessageWindowManager MessageWindowManager => this.GetComponentCacheCreateIfMissing(ref messageWindowManager);

		public AdvBacklogManager BacklogManager => this.GetComponentCache(ref backlogManager);

		public AdvConfig Config => this.GetComponentCache(ref config);

		public AdvSystemSaveData SystemSaveData => this.GetComponentCache(ref systemSaveData);

		public AdvSaveManager SaveManager => this.GetComponentCache(ref saveManager);

		public AdvGraphicManager GraphicManager
		{
			get
			{
				if (graphicManager == null)
				{
					graphicManager = base.transform.GetCompoentInChildrenCreateIfMissing<AdvGraphicManager>();
					graphicManager.transform.localPosition = new Vector3(0f, 0f, 20f);
				}
				return graphicManager;
			}
		}

		public AdvEffectManager EffectManager
		{
			get
			{
				if (effectManager == null)
				{
					effectManager = base.transform.GetCompoentInChildrenCreateIfMissing<AdvEffectManager>();
				}
				return effectManager;
			}
		}

		public AdvUiManager UiManager => this.GetComponentCacheFindIfMissing(ref uiManager);

		public SoundManager SoundManager => this.GetComponentCacheFindIfMissing(ref soundManager);

		public CameraManager CameraManager => this.GetComponentCacheFindIfMissing(ref cameraManager);

		public AdvTime Time => this.GetComponentCacheCreateIfMissing(ref time);

		public AdvParamManager Param => param;

		public string LanguageKeyOfParam => languageKeyOfParam;

		public string VoiceLanguageKeyOfParam => voiceLanguageKeyOfParam;

		public List<AdvCustomCommandManager> CustomCommandManagerList
		{
			get
			{
				if (customCommandManagerList == null)
				{
					customCommandManagerList = new List<AdvCustomCommandManager>();
					GetComponentsInChildren(includeInactive: true, customCommandManagerList);
				}
				return customCommandManagerList;
			}
		}

		public OpenDialogEvent OnOpenDialog
		{
			get
			{
				if (onOpenDialog.GetPersistentEventCount() == 0 && SystemUi.GetInstance() != null)
				{
					onOpenDialog.AddListener(SystemUi.GetInstance().OpenDialog);
				}
				return onOpenDialog;
			}
			set
			{
				onOpenDialog = value;
			}
		}

		public AdvEvent OnPageTextChange => onPageTextChange;

		public AdvEvent OnChangeLanguage => onChangeLanguage;

		public bool IsWaitBootLoading => isWaitBootLoading;

		public bool IsStarted => isStarted;

		public bool IsSceneGallery => isSceneGallery;

		public bool IsLoading
		{
			get
			{
				if (IsWaitBootLoading)
				{
					return true;
				}
				if (GraphicManager.IsLoading)
				{
					return true;
				}
				return ScenarioPlayer.IsLoading;
			}
		}

		public bool IsEndScenario
		{
			get
			{
				if (ScenarioPlayer == null)
				{
					return false;
				}
				if (IsLoading)
				{
					return false;
				}
				return ScenarioPlayer.IsEndScenario;
			}
		}

		public bool IsPausingScenario => ScenarioPlayer.IsPausing;

		public bool IsEndOrPauseScenario
		{
			get
			{
				if (!IsEndScenario)
				{
					return IsPausingScenario;
				}
				return true;
			}
		}

		private bool InitCallback { get; set; }

		private void OnDestroy()
		{
			if (InitCallback)
			{
				AdvGraphicInfo.CallbackExpression = null;
				TextParser.CallbackCalcExpression = (Func<string, object>)Delegate.Remove(TextParser.CallbackCalcExpression, new Func<string, object>(Param.CalcExpressionNotSetParam));
				iTweenData.CallbackGetValue = (Func<string, object>)Delegate.Remove(iTweenData.CallbackGetValue, new Func<string, object>(Param.GetParameter));
				LanguageManagerBase.Instance.OnChangeLanugage = null;
			}
		}

		public void BootFromExportData(AdvImportScenarios scenarios, string resourceDir)
		{
			base.gameObject.SetActive(value: true);
			StopAllCoroutines();
			StartCoroutine(CoBootFromExportData(scenarios, resourceDir));
		}

		private IEnumerator CoBootFromExportData(AdvImportScenarios scenarios, string resourceDir)
		{
			ClearSub(isStopSound: false);
			isStarted = true;
			isWaitBootLoading = true;
			onPreInit.Invoke();
			while (!AssetFileManager.IsInitialized())
			{
				yield return null;
			}
			yield return null;
			DataManager.SettingDataManager.ImportedScenarios = scenarios;
			yield return CoBootInit(resourceDir);
			isWaitBootLoading = false;
		}

		public bool ExitsChapter(string url)
		{
			string chapterAssetName = FilePathUtil.GetFileNameWithoutExtension(url);
			return DataManager.SettingDataManager.ImportedScenarios.Chapters.Exists((AdvChapterData x) => x.name == chapterAssetName);
		}

		public IEnumerator LoadChapterAsync(string url)
		{
			AssetFile file = AssetFileManager.Load(url, this);
			while (!file.IsLoadEnd)
			{
				yield return null;
			}
			AdvChapterData advChapterData = file.UnityObject as AdvChapterData;
			if (advChapterData == null)
			{
				Debug.LogError(url + " is  not scenario file");
				yield break;
			}
			if (DataManager.SettingDataManager.ImportedScenarios == null)
			{
				DataManager.SettingDataManager.ImportedScenarios = new AdvImportScenarios();
			}
			if (DataManager.SettingDataManager.ImportedScenarios.TryAddChapter(advChapterData))
			{
				DataManager.BootInitChapter(advChapterData);
			}
		}

		private void AutoChangeLanguageOnBoot()
		{
			string text = (string.IsNullOrEmpty(LanguageKeyOfParam) ? "" : Param.GetParameterString(LanguageKeyOfParam));
			string text2 = (string.IsNullOrEmpty(VoiceLanguageKeyOfParam) ? "" : Param.GetParameterString(VoiceLanguageKeyOfParam));
			if (!string.IsNullOrEmpty(text))
			{
				LanguageManagerBase.Instance.CurrentLanguage = text;
			}
			else if (!string.IsNullOrEmpty(LanguageKeyOfParam))
			{
				Param.SetParameterString(LanguageKeyOfParam, LanguageManagerBase.Instance.CurrentLanguage);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				LanguageManagerBase.Instance.VoiceLanguage = text2;
			}
		}

		private void ChangeLanguage()
		{
			if (!string.IsNullOrEmpty(LanguageKeyOfParam))
			{
				Param.SetParameterString(LanguageKeyOfParam, LanguageManagerBase.Instance.CurrentLanguage);
			}
			if (!string.IsNullOrEmpty(VoiceLanguageKeyOfParam))
			{
				Param.SetParameterString(VoiceLanguageKeyOfParam, LanguageManagerBase.Instance.VoiceLanguage);
			}
			Page.OnChangeLanguage();
			OnChangeLanguage.Invoke(this);
			ForEachCommand(delegate(AdvCommand x)
			{
				x.OnChangeLanguage(this);
			});
		}

		private void ForEachCommand(Action<AdvCommand> action)
		{
			foreach (KeyValuePair<string, AdvScenarioData> item in DataManager.ScenarioDataTbl)
			{
				foreach (KeyValuePair<string, AdvScenarioLabelData> scenarioLabel in item.Value.ScenarioLabels)
				{
					foreach (AdvScenarioPageData pageData in scenarioLabel.Value.PageDataList)
					{
						foreach (AdvCommand command in pageData.CommandList)
						{
							action(command);
						}
					}
				}
			}
		}

		public void ClearOnStart()
		{
			ClearSub(isStopSoundOnStart);
		}

		public void ClearOnEnd()
		{
			ClearSub(isStopSoundOnEnd);
		}

		private void ClearOnLaod()
		{
			ClearSub(isStopSound: true);
		}

		private void ClearSub(bool isStopSound)
		{
			Page.Clear();
			SelectionManager.Clear();
			BacklogManager.Clear();
			GraphicManager.Clear();
			GraphicManager.gameObject.SetActive(value: true);
			if (UiManager != null)
			{
				UiManager.Close();
			}
			ClearCustomCommand();
			ScenarioPlayer.Clear();
			if (isStopSound && SoundManager != null)
			{
				SoundManager.StopBgm();
				SoundManager.StopAmbience();
				SoundManager.StopAllLoop();
				if (isStopVoiceOnSoundStop)
				{
					SoundManager.StopVoice();
				}
			}
			if (MessageWindowManager == null)
			{
				Debug.LogError("MessageWindowManager is Missing");
			}
			CameraManager.OnClear();
			SaveManager.GetSaveIoListCreateIfMissing(this).ForEach(delegate(IBinaryIO x)
			{
				((IAdvSaveData)x).OnClear();
			});
			SaveManager.CustomSaveDataIOList.ForEach(delegate(IBinaryIO x)
			{
				((IAdvSaveData)x).OnClear();
			});
			OnClear.Invoke(this);
		}

		public void EndScenario()
		{
			ScenarioPlayer.EndScenario();
		}

		private IEnumerator CoBootInit(string rootDirResource)
		{
			BootInitCustomCommand();
			DataManager.BootInit(rootDirResource);
			GraphicManager.BootInit(this, DataManager.SettingDataManager.LayerSetting);
			Param.InitDefaultAll(DataManager.SettingDataManager.DefaultParam);
			InitCallback = true;
			AdvGraphicInfo.CallbackExpression = Param.CalcExpressionBoolean;
			TextParser.CallbackCalcExpression = (Func<string, object>)Delegate.Combine(TextParser.CallbackCalcExpression, new Func<string, object>(Param.CalcExpressionNotSetParam));
			iTweenData.CallbackGetValue = (Func<string, object>)Delegate.Combine(iTweenData.CallbackGetValue, new Func<string, object>(Param.GetParameter));
			LanguageManagerBase.Instance.OnChangeLanugage = ChangeLanguage;
			SystemSaveData.Init(this);
			SaveManager.Init();
			AutoChangeLanguageOnBoot();
			if (bootAsync)
			{
				yield return StartCoroutine(DataManager.CoBootInitScenariodData());
				yield break;
			}
			DataManager.BootInitScenariodData();
			DataManager.StartBackGroundDownloadResource();
		}

		public void BootInitCustomCommand()
		{
			AdvCommandParser.OnCreateCustomCommandFromID = null;
			foreach (AdvCustomCommandManager customCommandManager in CustomCommandManagerList)
			{
				customCommandManager.OnBootInit();
			}
		}

		public void ClearCustomCommand()
		{
			foreach (AdvCustomCommandManager customCommandManager in CustomCommandManagerList)
			{
				customCommandManager.OnClear();
			}
		}

		public void WriteSystemData()
		{
			systemSaveData.Write();
		}

		public void WriteSaveData(AdvSaveData saveData)
		{
			SaveManager.WriteSaveData(this, saveData);
		}

		private void LoadSaveData(AdvSaveData saveData)
		{
			ClearOnLaod();
			StartCoroutine(CoStartSaveData(saveData));
		}

		public void QuickSave()
		{
			WriteSaveData(SaveManager.QuickSaveData);
		}

		public bool QuickLoad()
		{
			if (SaveManager.ReadQuickSaveData())
			{
				LoadSaveData(SaveManager.QuickSaveData);
				return true;
			}
			return false;
		}

		public void StartGame()
		{
			StartGame(StartScenarioLabel);
		}

		public void StartGame(string scenarioLabel)
		{
			isSceneGallery = false;
			StartGameSub(scenarioLabel);
		}

		private void StartGameSub(string scenarioLabel)
		{
			StartCoroutine(CoStartGameSub(scenarioLabel));
		}

		private IEnumerator CoStartGameSub(string scenarioLabel)
		{
			while (IsWaitBootLoading)
			{
				yield return null;
			}
			Param.InitDefaultNormal(DataManager.SettingDataManager.DefaultParam);
			ClearOnStart();
			StartScenario(scenarioLabel, 0);
		}

		public void OpenLoadGame(AdvSaveData saveData)
		{
			isSceneGallery = false;
			LoadSaveData(saveData);
		}

		public void StartSceneGallery(string label)
		{
			isSceneGallery = true;
			StartGameSub(label);
		}

		public bool ResumeScenario()
		{
			if (!ScenarioPlayer.IsPausing)
			{
				return false;
			}
			ScenarioPlayer.Resume();
			return true;
		}

		public void PauseScenario()
		{
			if (!ScenarioPlayer.IsPausing)
			{
				ScenarioPlayer.Pause();
			}
		}

		public void JumpScenario(string label, int page = 0)
		{
			if (ScenarioPlayer.MainThread.IsPlaying)
			{
				if (ScenarioPlayer.IsPausing)
				{
					ScenarioPlayer.Resume();
				}
				ScenarioPlayer.MainThread.JumpManager.RegistoreLabel(label);
			}
			else
			{
				StartScenario(label, page);
			}
		}

		private void StartScenario(string label, int page)
		{
			StartCoroutine(CoStartScenario(label, page));
		}

		private IEnumerator CoStartScenario(string label, int page)
		{
			while (IsWaitBootLoading)
			{
				yield return null;
			}
			while (GraphicManager.IsLoading)
			{
				yield return null;
			}
			while (SoundManager.IsLoading)
			{
				yield return null;
			}
			if (UiManager != null)
			{
				UiManager.Open();
			}
			if (label.Length > 1 && label[0] == '*')
			{
				label = label.Substring(1);
			}
			ScenarioPlayer.StartScenario(label, page);
		}

		private IEnumerator CoStartSaveData(AdvSaveData saveData)
		{
			while (IsWaitBootLoading)
			{
				yield return null;
			}
			while (GraphicManager.IsLoading)
			{
				yield return null;
			}
			while (SoundManager.IsLoading)
			{
				yield return null;
			}
			if (UiManager != null)
			{
				UiManager.Open();
			}
			yield return ScenarioPlayer.CoStartSaveData(saveData);
		}

		public HashSet<AssetFile> GetAllFileSet()
		{
			return DataManager.GetAllFileSet();
		}
	}
}
