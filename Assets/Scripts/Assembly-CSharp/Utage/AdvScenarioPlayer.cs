using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvScenarioPlayer")]
	public class AdvScenarioPlayer : MonoBehaviour, IBinaryIO
	{
		[Flags]
		private enum DebugOutPut
		{
			Log = 1,
			Waiting = 2,
			CommandEnd = 4
		}

		[SerializeField]
		private GameObject sendMessageTarget;

		[SerializeField]
		[EnumFlags]
		private DebugOutPut debugOutPut;

		[SerializeField]
		private int maxFilePreload = 20;

		[SerializeField]
		private int preloadDeep = 5;

		[SerializeField]
		public AdvScenarioPlayerEvent onBeginScenario = new AdvScenarioPlayerEvent();

		[SerializeField]
		public AdvScenarioPlayerEvent onEndScenario = new AdvScenarioPlayerEvent();

		[SerializeField]
		public AdvScenarioPlayerEvent onPauseScenario = new AdvScenarioPlayerEvent();

		[SerializeField]
		public AdvScenarioPlayerEvent onEndOrPauseScenario = new AdvScenarioPlayerEvent();

		[SerializeField]
		public AdvCommandEvent onBeginCommand = new AdvCommandEvent();

		[SerializeField]
		public AdvCommandEvent onUpdatePreWaitingCommand = new AdvCommandEvent();

		[SerializeField]
		public AdvCommandEvent onUpdateWaitingCommand = new AdvCommandEvent();

		[SerializeField]
		public AdvCommandEvent onEndCommand = new AdvCommandEvent();

		private AdvEngine engine;

		private AdvScenarioThread mainThread;

		private const int Version0 = 0;

		private const int Version1 = 1;

		private const int Version2 = 2;

		public GameObject SendMessageTarget => sendMessageTarget;

		internal bool DebugOutputLog => (debugOutPut & DebugOutPut.Log) == DebugOutPut.Log;

		internal bool DebugOutputWaiting => (debugOutPut & DebugOutPut.Waiting) == DebugOutPut.Waiting;

		internal bool DebugOutputCommandEnd => (debugOutPut & DebugOutPut.CommandEnd) == DebugOutPut.CommandEnd;

		internal int MaxFilePreload => maxFilePreload;

		internal int PreloadDeep => preloadDeep;

		public AdvScenarioPlayerEvent OnBeginScenario => onBeginScenario;

		public AdvScenarioPlayerEvent OnEndScenario => onEndScenario;

		public AdvScenarioPlayerEvent OnPauseScenario => onPauseScenario;

		public AdvScenarioPlayerEvent OnEndOrPauseScenario => onEndOrPauseScenario;

		public AdvCommandEvent OnBeginCommand => onBeginCommand;

		public AdvCommandEvent OnUpdatePreWaitingCommand => onUpdatePreWaitingCommand;

		public AdvCommandEvent OnUpdateWaitingCommand => onUpdateWaitingCommand;

		public AdvCommandEvent OnEndCommand => onEndCommand;

		public AdvEngine Engine => this.GetComponentCache(ref engine);

		public AdvScenarioThread MainThread
		{
			get
			{
				if (mainThread == null)
				{
					mainThread = base.gameObject.GetComponentCreateIfMissing<AdvScenarioThread>();
					mainThread.Init(this, "MainThread", null);
				}
				return mainThread;
			}
		}

		public bool IsEndScenario { get; set; }

		public bool IsReservedEndScenario { get; set; }

		public bool IsPausing { get; set; }

		public string CurrentGallerySceneLabel { get; set; }

		public bool IsLoading => MainThread.IsLoadingDeep;

		public string SaveKey => "ScenarioPlayer";

		public virtual void StartScenario(string label, int page)
		{
			IsPausing = false;
			IsEndScenario = false;
			IsReservedEndScenario = false;
			CurrentGallerySceneLabel = "";
			MainThread.Clear();
			OnBeginScenario.Invoke(this);
			MainThread.StartScenario(label, page, skipPageHeaer: false);
		}

		internal IEnumerator CoStartSaveData(AdvSaveData saveData)
		{
			IsPausing = false;
			IsEndScenario = false;
			IsReservedEndScenario = false;
			MainThread.Clear();
			OnBeginScenario.Invoke(this);
			saveData.LoadGameData(Engine, Engine.SaveManager.CustomSaveDataIOList, Engine.SaveManager.GetSaveIoListCreateIfMissing(Engine));
			yield return null;
			saveData.Buffer.Overrirde(this);
		}

		public void OnWrite(BinaryWriter writer)
		{
			writer.Write(2);
			MainThread.IfManager.Write(writer);
			MainThread.JumpManager.Write(writer);
			MainThread.Write(writer);
			writer.Write(Engine.Page.ScenarioLabel);
			writer.Write(Engine.Page.PageNo);
			writer.Write(CurrentGallerySceneLabel);
			writer.Write(MainThread.SkipPageHeaerOnSave);
		}

		public void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (0 <= num && num <= 2)
			{
				if (num >= 1)
				{
					MainThread.IfManager.Read(reader);
				}
				else
				{
					MainThread.IfManager.ReadOld();
				}
				MainThread.JumpManager.Read(Engine, reader);
				if (num >= 2)
				{
					MainThread.Read(Engine, reader);
				}
				string label = reader.ReadString();
				int page = reader.ReadInt32();
				string currentGallerySceneLabel = reader.ReadString();
				bool skipPageHeaer = reader.ReadBoolean();
				MainThread.ScenarioPlayer.CurrentGallerySceneLabel = currentGallerySceneLabel;
				MainThread.StartScenario(label, page, skipPageHeaer);
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}

		public virtual void EndScenario()
		{
			OnEndScenario.Invoke(this);
			OnEndOrPauseScenario.Invoke(this);
			Engine.ClearOnEnd();
			MainThread.Clear();
			IsEndScenario = true;
		}

		public void Pause()
		{
			IsPausing = true;
			OnPauseScenario.Invoke(this);
			OnEndOrPauseScenario.Invoke(this);
		}

		public void Resume()
		{
			IsPausing = false;
		}

		public void Clear()
		{
			MainThread.Clear();
			CurrentGallerySceneLabel = "";
		}

		internal void UpdateSceneGallery(string label, AdvEngine engine)
		{
			if (engine.DataManager.SettingDataManager.SceneGallerySetting.Contains(label) && CurrentGallerySceneLabel != label)
			{
				if (!string.IsNullOrEmpty(CurrentGallerySceneLabel))
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.UpdateSceneLabel, CurrentGallerySceneLabel, label));
				}
				CurrentGallerySceneLabel = label;
			}
		}

		public void EndSceneGallery(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(CurrentGallerySceneLabel))
			{
				Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.EndSceneGallery));
				return;
			}
			engine.SystemSaveData.GalleryData.AddSceneLabel(CurrentGallerySceneLabel);
			CurrentGallerySceneLabel = "";
		}
	}
}
