using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvScenarioThread")]
	public class AdvScenarioThread : MonoBehaviour
	{
		private class SubTreadSaveData
		{
			public string threadName;
		}

		[SerializeField]
		[NotEditable]
		private string threadName;

		private AdvIfManager ifManager = new AdvIfManager();

		private AdvJumpManager jumpManager = new AdvJumpManager();

		private AdvWaitManager waitManager = new AdvWaitManager();

		private List<AdvScenarioThread> subThreadList = new List<AdvScenarioThread>();

		private HashSet<AssetFile> preloadFileSet = new HashSet<AssetFile>();

		private AdvCommand currentCommand;

		private List<SubTreadSaveData> loadedSaveData = new List<SubTreadSaveData>();

		private const int Version = 0;

		public string ThreadName => threadName;

		public bool IsMainThread { get; private set; }

		public bool IsLoading { get; private set; }

		public bool IsLoadingDeep
		{
			get
			{
				if (IsLoading)
				{
					return true;
				}
				foreach (AdvScenarioThread subThread in SubThreadList)
				{
					if (subThread.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsPlaying { get; set; }

		internal AdvIfManager IfManager => ifManager;

		internal AdvJumpManager JumpManager => jumpManager;

		internal AdvWaitManager WaitManager => waitManager;

		internal AdvScenarioThread ParenetThread { get; private set; }

		private List<AdvScenarioThread> SubThreadList => subThreadList;

		public AdvCommand CurrentCommand => currentCommand;

		internal bool SkipPageHeaerOnSave { get; private set; }

		internal AdvScenarioPlayer ScenarioPlayer { get; private set; }

		internal AdvEngine Engine => ScenarioPlayer.Engine;

		private string WaitingThreadName { get; set; }

		private bool IsBreakCommand
		{
			get
			{
				if (IsPlaying && !JumpManager.IsReserved)
				{
					if (IsMainThread)
					{
						return ScenarioPlayer.IsReservedEndScenario;
					}
					return false;
				}
				return true;
			}
		}

		public bool IsCurrentCommand(AdvCommand command)
		{
			if (command != null)
			{
				return currentCommand == command;
			}
			return false;
		}

		internal bool IsWaitingSubTread(string subThreadName)
		{
			return WaitingThreadName == subThreadName;
		}

		internal void SetWaitingSubTread(string subThreadName, bool waiting)
		{
			WaitingThreadName = (waiting ? subThreadName : "");
		}

		internal void Init(AdvScenarioPlayer scenarioPlayer, string name, AdvScenarioThread parent)
		{
			ScenarioPlayer = scenarioPlayer;
			threadName = name;
			ParenetThread = parent;
			IsMainThread = parent == null;
		}

		private void OnDestroy()
		{
			ClearPreload();
			CleaSubTreadList();
			if ((bool)ParenetThread)
			{
				ParenetThread.SubThreadList.Remove(this);
			}
		}

		internal void Clear()
		{
			IsPlaying = false;
			WaitingThreadName = "";
			loadedSaveData.Clear();
			CleaSubTreadList();
			ResetOnJump();
			WaitManager.Clear();
			jumpManager.Clear();
			StopAllCoroutines();
		}

		internal void Cancel()
		{
			Clear();
			Object.Destroy(this);
		}

		private void ResetOnJump()
		{
			IsLoading = false;
			jumpManager.ClearOnJump();
			ifManager.ResetOnJump();
			ClearPreload();
		}

		internal void StartScenario(string label, int page, bool skipPageHeaer)
		{
			StartCoroutine(CoStartScenario(label, page, null, skipPageHeaer));
		}

		private IEnumerator CoStartScenario(string label, int page, AdvCommand returnToCommand, bool skipPageHeaer)
		{
			IsPlaying = true;
			SkipPageHeaerOnSave = false;
			if (ScenarioPlayer.DebugOutputLog)
			{
				Debug.Log("Jump : " + label + " :" + page);
			}
			while (Engine.IsLoading)
			{
				yield return null;
			}
			IsLoading = true;
			while (!Engine.DataManager.IsLoadEndScenarioLabel(label))
			{
				yield return null;
			}
			IsLoading = false;
			ResetOnJump();
			if (page < 0)
			{
				page = 0;
			}
			LoadSubThreadSaveData();
			AdvScenarioLabelData currentLabelData = Engine.DataManager.FindScenarioLabelData(label);
			while (currentLabelData != null)
			{
				ScenarioPlayer.UpdateSceneGallery(currentLabelData.ScenarioLabel, Engine);
				AdvScenarioPageData pageData = currentLabelData.GetPageData(page);
				while (pageData != null)
				{
					UpdatePreLoadFiles(currentLabelData.ScenarioLabel, page);
					if (IsMainThread)
					{
						Engine.Page.BeginPage(pageData);
					}
					Coroutine coroutine = StartCoroutine(CoStartPage(currentLabelData, pageData, returnToCommand, skipPageHeaer));
					if (coroutine != null)
					{
						yield return coroutine;
					}
					currentCommand = null;
					returnToCommand = null;
					skipPageHeaer = false;
					if (IsMainThread)
					{
						Engine.Page.EndPage();
					}
					if (IsBreakCommand)
					{
						if (IsMainThread && ScenarioPlayer.IsReservedEndScenario)
						{
							ScenarioPlayer.EndScenario();
						}
						else if (JumpManager.IsReserved)
						{
							JumpToReserved();
						}
						else
						{
							OnEndThread();
						}
						yield break;
					}
					AdvScenarioLabelData advScenarioLabelData = currentLabelData;
					int num = page + 1;
					page = num;
					pageData = advScenarioLabelData.GetPageData(num);
				}
				IfManager.OldSaveDataStart = false;
				currentLabelData = Engine.DataManager.NextScenarioLabelData(currentLabelData.ScenarioLabel);
				page = 0;
			}
			OnEndThread();
		}

		private void OnEndThread()
		{
			IsPlaying = false;
			if (IsMainThread)
			{
				ScenarioPlayer.EndScenario();
			}
			else
			{
				Object.Destroy(this);
			}
		}

		private IEnumerator CoStartPage(AdvScenarioLabelData labelData, AdvScenarioPageData pageData, AdvCommand returnToCommand, bool skipPageHeaer)
		{
			if (pageData.CheckSkipByLocalize())
			{
				yield break;
			}
			int index = (skipPageHeaer ? pageData.IndexTextTopCommand : 0);
			AdvCommand command = pageData.GetCommand(index);
			if (returnToCommand != null)
			{
				while (command != returnToCommand)
				{
					int num = index + 1;
					index = num;
					command = pageData.GetCommand(num);
				}
			}
			if (IfManager.OldSaveDataStart)
			{
				index = pageData.GetIfSkipCommandIndex(index);
				command = pageData.GetCommand(index);
			}
			if (EnableSaveOnPageTop() && pageData.EnableSave)
			{
				SkipPageHeaerOnSave = false;
				Engine.SaveManager.UpdateAutoSaveData(Engine);
			}
			CheckSystemDataWriteIfChanged();
			while (command != null)
			{
				if (command.IsEntityType)
				{
					command = AdvEntityData.CreateEntityCommand(command, Engine, pageData);
				}
				int num;
				if (IfManager.CheckSkip(command))
				{
					if (ScenarioPlayer.DebugOutputLog)
					{
						Debug.Log("Command If Skip: " + command.GetType()?.ToString() + " " + labelData.ScenarioLabel + ":" + pageData.PageNo);
					}
					num = index + 1;
					index = num;
					command = pageData.GetCommand(num);
					continue;
				}
				currentCommand = command;
				command.Load();
				if (EnableSaveTextTop() && pageData.EnableSaveTextTop(command))
				{
					SkipPageHeaerOnSave = true;
					Engine.SaveManager.UpdateAutoSaveData(Engine);
					CheckSystemDataWriteIfChanged();
				}
				while (!command.IsLoadEnd())
				{
					IsLoading = true;
					yield return null;
				}
				IsLoading = false;
				command.CurrentTread = this;
				if (ScenarioPlayer.DebugOutputLog)
				{
					Debug.Log("Command : " + command.GetType()?.ToString() + " " + labelData.ScenarioLabel + ":" + pageData.PageNo);
				}
				ScenarioPlayer.OnBeginCommand.Invoke(command);
				command.DoCommand(Engine);
				command.Unload();
				command.CurrentTread = null;
				while (ScenarioPlayer.IsPausing)
				{
					yield return null;
				}
				while (true)
				{
					command.CurrentTread = this;
					ScenarioPlayer.OnUpdatePreWaitingCommand.Invoke(command);
					if (!command.Wait(Engine))
					{
						break;
					}
					if (ScenarioPlayer.DebugOutputWaiting)
					{
						Debug.Log("Wait..." + command.GetType());
					}
					ScenarioPlayer.OnUpdateWaitingCommand.Invoke(command);
					command.CurrentTread = null;
					yield return null;
				}
				command.CurrentTread = this;
				if (ScenarioPlayer.DebugOutputCommandEnd)
				{
					Debug.Log("End :" + command.GetType()?.ToString() + " " + labelData.ScenarioLabel + ":" + pageData.PageNo);
				}
				ScenarioPlayer.OnEndCommand.Invoke(command);
				command.CurrentTread = null;
				Engine.UiManager.IsInputTrig = false;
				Engine.UiManager.IsInputTrigCustom = false;
				if (IsBreakCommand)
				{
					break;
				}
				num = index + 1;
				index = num;
				command = pageData.GetCommand(num);
			}
		}

		private void CheckSystemDataWriteIfChanged()
		{
			if (Engine.Param.HasChangedSystemParam)
			{
				Engine.Param.HasChangedSystemParam = false;
				Engine.SystemSaveData.Write();
			}
		}

		internal bool EnableSaveOnPageTop()
		{
			if (!IsMainThread)
			{
				return false;
			}
			if (Engine.IsSceneGallery)
			{
				return false;
			}
			switch (Engine.SaveManager.Type)
			{
			case AdvSaveManager.SaveType.Default:
				return true;
			case AdvSaveManager.SaveType.SavePoint:
				if (Engine.Page.PageNo == 0)
				{
					return Engine.Page.CurrentData.ScenarioLabelData.IsSavePoint;
				}
				return false;
			default:
				return false;
			}
		}

		internal bool EnableSaveTextTop()
		{
			if (!IsMainThread)
			{
				return false;
			}
			if (Engine.IsSceneGallery)
			{
				return false;
			}
			if (WaitManager.IsWaiting)
			{
				return false;
			}
			_ = SubThreadList.Count;
			_ = 0;
			return false;
		}

		private void JumpToReserved()
		{
			StopAllCoroutines();
			if (JumpManager.SubRoutineReturnInfo != null)
			{
				SubRoutineInfo subRoutineReturnInfo = JumpManager.SubRoutineReturnInfo;
				StartCoroutine(CoStartScenario(subRoutineReturnInfo.ReturnLabel, subRoutineReturnInfo.ReturnPageNo, subRoutineReturnInfo.ReturnCommand, skipPageHeaer: false));
			}
			else
			{
				StartCoroutine(CoStartScenario(JumpManager.Label, 0, null, skipPageHeaer: false));
			}
		}

		internal void StartSubThread(string label)
		{
			AdvScenarioThread advScenarioThread = base.gameObject.AddComponent<AdvScenarioThread>();
			advScenarioThread.Init(ScenarioPlayer, label, this);
			SubThreadList.Add(advScenarioThread);
			advScenarioThread.StartScenario(label, 0, skipPageHeaer: false);
		}

		internal bool IsPlayingSubThread(string name)
		{
			foreach (AdvScenarioThread subThread in SubThreadList)
			{
				if ((bool)subThread && subThread.ThreadName == name)
				{
					return subThread.IsPlaying;
				}
			}
			return false;
		}

		internal void CleaSubTreadList()
		{
			foreach (AdvScenarioThread subThread in SubThreadList)
			{
				Object.Destroy(subThread);
			}
			SubThreadList.Clear();
		}

		internal void CancelSubThread(string name)
		{
			foreach (AdvScenarioThread subThread in SubThreadList)
			{
				if ((bool)subThread && subThread.ThreadName == name)
				{
					subThread.Cancel();
				}
			}
		}

		private void ClearPreload()
		{
			foreach (AssetFile item in preloadFileSet)
			{
				item.Unuse(this);
			}
			preloadFileSet.Clear();
		}

		private void UpdatePreLoadFiles(string scenarioLabel, int page)
		{
			HashSet<AssetFile> hashSet = preloadFileSet;
			preloadFileSet = Engine.DataManager.MakePreloadFileList(scenarioLabel, page, ScenarioPlayer.MaxFilePreload, ScenarioPlayer.PreloadDeep);
			if (preloadFileSet == null)
			{
				preloadFileSet = new HashSet<AssetFile>();
			}
			foreach (AssetFile item in preloadFileSet)
			{
				AssetFileManager.Preload(item, this);
			}
			foreach (AssetFile item2 in hashSet)
			{
				if (!preloadFileSet.Contains(item2))
				{
					item2.Unuse(this);
				}
			}
		}

		private void LoadSubThreadSaveData()
		{
			if (!IsMainThread || loadedSaveData.Count <= 0)
			{
				return;
			}
			if (!Engine.SaveManager.RestartSubThread)
			{
				loadedSaveData.Clear();
				return;
			}
			foreach (SubTreadSaveData loadedSaveDatum in loadedSaveData)
			{
				StartSubThread(loadedSaveDatum.threadName);
			}
			loadedSaveData.Clear();
		}

		internal void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(subThreadList.Count);
			foreach (AdvScenarioThread subThread in subThreadList)
			{
				writer.Write(subThread.ThreadName);
			}
		}

		internal void Read(AdvEngine engine, BinaryReader reader)
		{
			loadedSaveData.Clear();
			int num = reader.ReadInt32();
			if (num == 0)
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					string text = reader.ReadString();
					loadedSaveData.Add(new SubTreadSaveData
					{
						threadName = text
					});
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}

		private void OnEnable()
		{
		}

		private void OnDisable()
		{
		}
	}
}
