using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utage;

namespace Paidia.satsuki1
{
	public class UtageManager : MonoBehaviour
	{
		[SerializeField]
		private AdvEngine _advEngine;

		public bool IsPlaying;

		public Subject<ScenarioLabel> OnStartPlaying;

		public Subject<ScenarioLabel> OnFinishPlaying;

		public Subject<Unit> OnPausePlaying;

		private ScenarioLabel _playingScenarioLabel;

		public YesNoWindowPresenter YesNo;

		private float defaultSpeed = -1f;

		public Sprite Day2Image;

		public AdvEngine AdvEngine => _advEngine;

		public void StopAuto()
		{
			_advEngine.Config.IsAutoBrPage = false;
			_advEngine.Config.IsSkip = false;
		}

		private void Start()
		{
			OnStartPlaying = new Subject<ScenarioLabel>();
			OnFinishPlaying = new Subject<ScenarioLabel>();
			OnPausePlaying = new Subject<Unit>();
		}

		private void Update()
		{
			if (!(null == _advEngine))
			{
				_advEngine.Config.BgmVolume = SaveLoadManager.GlobalData.BGMVolume;
				_advEngine.Config.VoiceVolume = SaveLoadManager.GlobalData.VoiceVolume;
				_advEngine.Config.SeVolume = SaveLoadManager.GlobalData.SEVolume;
				_advEngine.Config.AmbienceVolume = SaveLoadManager.GlobalData.EnvironmentalSEVolume;
				_advEngine.Config.IsSkipUnread = SaveLoadManager.GlobalData.GameOption.SkipUnread == 1;
				_advEngine.Config.IsStopSkipInSelection = SaveLoadManager.GlobalData.GameOption.StopSkipOnChoice == 1;
				_advEngine.Config.MessageSpeed = SaveLoadManager.GlobalData.GameOption.TextSpeed;
				_advEngine.Config.AutoBrPageSpeed = SaveLoadManager.GlobalData.GameOption.TextAutoSpeed;
				if (_advEngine.Param.TryGetParameter("player_name", out var _))
				{
					_advEngine.Param.SetParameterString("player_name", SaveLoadManager.UnsavedData.PlayerName);
				}
			}
		}

		public async UniTask ShowUtageText(ScenarioLabel label, CancellationToken token)
		{
			await ShowUtageTextFromLine(label, 0, token);
		}

		public async UniTask ShowUtageTextWithoutReadFlag(ScenarioLabel label, CancellationToken token)
		{
			await ShowUtageTextFromLine(label, 0, token, setFlag: false);
		}

		public async UniTask ShowUtageTextFromLine(string utageLabel, ScenarioLabel label, int line, CancellationToken token, bool setFlag = true)
		{
			_advEngine.Param.SetParameterString("player_name", SaveLoadManager.UnsavedData.PlayerName);
			try
			{
				await UniTask.WaitUntil(() => !_advEngine.IsLoading, PlayerLoopTiming.Update, token);
				OnScenarioStartPlaying(label);
				await ShowText(utageLabel, line, token);
				await OnScenarioFinishPlaying(label);
				if (setFlag)
				{
					SaveLoadManager.UnsavedData.GlobalFlags.SetRead(label);
				}
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (Exception ex2)
			{
				Debug.LogError(ex2.StackTrace);
			}
		}

		public async UniTask ShowUtageTextFromLine(ScenarioLabel label, int line, CancellationToken token, bool setFlag = true)
		{
			_advEngine.Param.SetParameterString("player_name", SaveLoadManager.UnsavedData.PlayerName);
			try
			{
				await UniTask.WaitUntil(() => !_advEngine.IsLoading, PlayerLoopTiming.Update, token);
				OnScenarioStartPlaying(label);
				await ShowText(label, line, token);
				await OnScenarioFinishPlaying(label);
				if (setFlag)
				{
					SaveLoadManager.UnsavedData.GlobalFlags.SetRead(label);
				}
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (Exception ex2)
			{
				Debug.LogError(ex2.StackTrace);
			}
		}

		private async UniTask ShowText(string label, int line, CancellationToken token)
		{
			_advEngine.JumpScenario(label, line);
			do
			{
				try
				{
					await UniTask.Yield(token);
				}
				catch (OperationCanceledException ex)
				{
					throw ex;
				}
				if (null == _advEngine)
				{
					throw new OperationCanceledException();
				}
			}
			while (!_advEngine.IsEndScenario);
		}

		private async UniTask ShowText(ScenarioLabel label, int line, CancellationToken token)
		{
			_advEngine.JumpScenario(StringsManager.GetScenarioLabel(label), line);
			do
			{
				try
				{
					await UniTask.Yield(token);
				}
				catch (OperationCanceledException ex)
				{
					throw ex;
				}
				if (null == _advEngine)
				{
					throw new OperationCanceledException();
				}
			}
			while (!_advEngine.IsEndScenario);
		}

		public void ForceEndText()
		{
			if (IsPlaying)
			{
				_advEngine.EndScenario();
				OnScenarioFinishPlaying(_playingScenarioLabel).Forget();
			}
		}

		public int GetInt(string paramName)
		{
			if (_advEngine.Param.TryGetParameter(paramName, out var parameter))
			{
				return (int)parameter;
			}
			return 0;
		}

		public void SetInt(string paramName, int val)
		{
			_advEngine.Param.SetParameter(paramName, val);
		}

		public int GetCurrentPageNumber()
		{
			return _advEngine.Page.PageNo;
		}

		public string GetScenarioTitle()
		{
			return _advEngine.Page.ScenarioLabel;
		}

		public ScenarioLabel GetScenarioLabel()
		{
			return _playingScenarioLabel;
		}

		private void OnScenarioStartPlaying(ScenarioLabel label)
		{
			_playingScenarioLabel = label;
			IsPlaying = true;
			SaveLoadManager.UnsavedData.ResetSelectIndex();
			SaveLoadManager.UnsavedData.IsPlayingUtage = true;
			SynchroDataWithUtage();
			OnStartPlaying.OnNext(label);
		}

		private async UniTask OnScenarioFinishPlaying(ScenarioLabel label)
		{
			_advEngine.WriteSystemData();
			SaveLoadManager.UnsavedData.IsPlayingUtage = false;
			IsPlaying = false;
			OnFinishPlaying.OnNext(_playingScenarioLabel);
			_playingScenarioLabel = ScenarioLabel.None;
			UnityEngine.Object.FindObjectOfType<FaceAnimationController>()?.SetUtageAnimating(value: false);
			if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeScenario)
			{
				return;
			}
			switch (label)
			{
			case ScenarioLabel.Sub_Underwear:
				SaveLoadManager.UnsavedData.ForceLingerieNormal = true;
				break;
			case ScenarioLabel.Day6_C:
			{
				await UpdateRelationship(Relationship.Secret);
				MessageWindowUIPresenter messageUI = UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>();
				await messageUI.SetLongMessage("行動内容によって、好感度と開発度が上昇します。\n一定の値を超えると、なつひとの関係が変化し、\n新しい服装やプレイがアンロックされます。");
				await messageUI.SetLongMessage("ここからは自由に日々の行動を選択できます。\n夏休みが終わる３１日までに、なつひとの関係を深めましょう。");
				break;
			}
			case ScenarioLabel.Relation_2A:
				await UpdateRelationship(Relationship.Special);
				break;
			case ScenarioLabel.Relation_3B:
				await UpdateRelationship(Relationship.Lovers);
				break;
			case ScenarioLabel.Relation_4A:
				SaveLoadManager.UnsavedData.ForceLingerieNormal = true;
				if (SaveLoadManager.UnsavedData.LastSelectedIndex == 0)
				{
					SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.GoodEnd, isOn: true);
				}
				break;
			case ScenarioLabel.Relation_4B:
				if (SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.GoodEnd))
				{
					await UpdateRelationship(Relationship.LoveyDovey);
				}
				break;
			case ScenarioLabel.Relation_3A:
				SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.Evt_BathFellatio, isOn: true);
				break;
			case ScenarioLabel.Day2:
				await UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>().SetLongMessage("１日の始めには、行動を選択してください。\n行動の結果によって、\nなつひの好感度が変化します。", Day2Image);
				break;
			}
		}

		private async UniTask UpdateRelationship(Relationship relation)
		{
			string name = relation switch
			{
				Relationship.None => "生徒と先生", 
				Relationship.Secret => "ヒミツの関係", 
				Relationship.Special => "求め合う関係", 
				Relationship.Lovers => "恋人？", 
				Relationship.LoveyDovey => "ラブラブな二人", 
				_ => "生徒と先生", 
			};
			AudioClip se = await Addressables.LoadAssetAsync<AudioClip>("RelationshipUp");
			SingletonManager<SoundManager>.Instance.PlaySE(se);
			SaveLoadManager.UnsavedData.PersistantStatus.SetRelationship(relation);
			MessageWindowUIPresenter msg = UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>();
			await msg.SetShortMessage("なつひとの関係性が" + name + "になりました。");
			switch (relation)
			{
			case Relationship.Lovers:
				await msg.SetShortMessage("雰囲気度の上限が\n「興奮」に上がりました。");
				break;
			case Relationship.LoveyDovey:
				await msg.SetShortMessage("雰囲気度の上限が\n「発情」に上がりました。");
				break;
			}
			SingletonManager<SceneContextManager>.Instance.AllowUtage = false;
			UnityEngine.Object.FindObjectOfType<HScene>()?.SetModalWindowVisible(visible: true);
			StatusUIPresenter statusUI = UnityEngine.Object.FindObjectOfType<StatusUIPresenter>();
			statusUI.DrawData();
			await UniTask.WaitUntil(() => statusUI.CG.alpha == 0f);
		}

		public void SetAdvEngine(AdvEngine engine)
		{
			_advEngine = engine;
		}

		private Live2DAnimator GetAnimator()
		{
			return UnityEngine.Object.FindObjectOfType<FaceAnimationController>().GetAnimator();
		}

		private async void OnDoCommand(AdvCommandSendMessage command)
		{
			if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeScenario)
			{
				await UniTask.Yield();
				_advEngine.UiManager.IsInputTrigCustom = true;
				return;
			}
			switch (command.Name)
			{
			case "FaceAnimation":
			{
				Live2DAnimator animator = GetAnimator();
				OsawariHelper helper = GameObject.FindGameObjectWithTag("OsawariMain").GetComponent<OsawariHelper>();
				UnityEngine.Object.FindObjectOfType<FaceAnimationController>().SetUtageAnimating(value: true);
				try
				{
					helper?.OnUtageAnimation();
					animator.SetFloat("ScenarioAnimationSpeed", 1f);
					await animator.PlayFaceAnimationTemporary((FaceStateName)Enum.Parse(typeof(FaceStateName), command.Arg2.ToString()), 75, null, loop: false, int.MaxValue, command.Arg3.ToString() == "");
					if (command.Arg3.ToString() != "")
					{
						if (command.Arg3.ToString() != "KeepLookingForward")
						{
							AudioClip clip = null;
							if (command.Arg4.ToString() != "")
							{
								clip = await Addressables.LoadAssetAsync<AudioClip>(command.Arg4.ToString());
							}
							await animator.PlayFaceAnimationTemporary((FaceStateName)Enum.Parse(typeof(FaceStateName), command.Arg3.ToString()), 300, clip);
						}
					}
					else
					{
						helper?.OnUtageAnimationFinished();
					}
					break;
				}
				catch (OperationCanceledException)
				{
					break;
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					break;
				}
			}
			case "ResetFaceAnimation":
			{
				Live2DAnimator animator = GetAnimator();
				OsawariHelper helper = GameObject.FindGameObjectWithTag("OsawariMain").GetComponent<OsawariHelper>();
				animator.CancelTemporaryFaceAnimation(FaceStateName.None);
				helper?.OnUtageAnimationFinished();
				UnityEngine.Object.FindObjectOfType<FaceAnimationController>()?.SetAnimationEnable();
				break;
			}
			case "SetMicroBikini":
				_advEngine.Param.SetParameter("cloth", 2);
				break;
			case "SetBunny":
				_advEngine.Param.SetParameter("cloth", 1);
				break;
			case "SetCat":
				_advEngine.Param.SetParameter("cloth", 3);
				break;
			case "SetSM":
				_advEngine.Param.SetParameter("cloth", 4);
				break;
			case "SetWetSuit":
				_advEngine.Param.SetParameter("cloth", 2);
				break;
			case "ShortMessage":
				await UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>().SetShortMessage(command.Text);
				_advEngine.UiManager.IsInputTrigCustom = true;
				break;
			case "LongMessage":
				await UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>().SetLongMessage(command.Text);
				_advEngine.UiManager.IsInputTrigCustom = true;
				break;
			case "SetHMode":
				await UnityEngine.Object.FindObjectOfType<HScene3OsawariHelper>().MoveToStudyHMode(wait: false);
				break;
			case "AskForSave":
				if (await YesNo.WaitForAnswer("セーブしますか？"))
				{
					SaveLoadUIPresenter saveUi = UnityEngine.Object.FindObjectOfType<SaveLoadUIPresenter>();
					saveUi.SetActive(visible: true).Forget();
					await UniTask.WaitUntil(() => saveUi.CG.alpha == 0f);
				}
				_advEngine.UiManager.IsInputTrigCustom = true;
				break;
			case "WriteData":
			{
				_advEngine.Param.TryGetParameter("isMizugi", out var parameter);
				SaveLoadManager.UnsavedData.UtageisMizugi = (bool)parameter;
				_advEngine.Param.TryGetParameter("isBra", out parameter);
				SaveLoadManager.UnsavedData.UtageisBra = (bool)parameter;
				_advEngine.Param.TryGetParameter("isBunny", out parameter);
				SaveLoadManager.UnsavedData.UtageisBunny = (bool)parameter;
				_advEngine.Param.TryGetParameter("isMb", out parameter);
				SaveLoadManager.UnsavedData.UtageisMb = (bool)parameter;
				_advEngine.Param.TryGetParameter("isNeko", out parameter);
				SaveLoadManager.UnsavedData.UtageisNeko = (bool)parameter;
				_advEngine.Param.TryGetParameter("isNude", out parameter);
				SaveLoadManager.UnsavedData.UtageisNude = (bool)parameter;
				_advEngine.Param.TryGetParameter("isSm", out parameter);
				SaveLoadManager.UnsavedData.UtageisSm = (bool)parameter;
				_advEngine.Param.TryGetParameter("isBath", out parameter);
				SaveLoadManager.UnsavedData.UtageisBath = (bool)parameter;
				break;
			}
			default:
				Debug.LogWarning("No registered command: " + command.Name);
				break;
			}
		}

		private void SynchroDataWithUtage()
		{
			_advEngine.Param.SetParameter("isMizugi", SaveLoadManager.UnsavedData.UtageisMizugi);
			_advEngine.Param.SetParameter("isBra", SaveLoadManager.UnsavedData.UtageisBra);
			_advEngine.Param.SetParameter("isBunny", SaveLoadManager.UnsavedData.UtageisBunny);
			_advEngine.Param.SetParameter("isMb", SaveLoadManager.UnsavedData.UtageisMb);
			_advEngine.Param.SetParameter("isNeko", SaveLoadManager.UnsavedData.UtageisNeko);
			_advEngine.Param.SetParameter("isNude", SaveLoadManager.UnsavedData.UtageisNude);
			_advEngine.Param.SetParameter("isSm", SaveLoadManager.UnsavedData.UtageisSm);
			_advEngine.Param.SetParameter("isBath", SaveLoadManager.UnsavedData.UtageisBath);
		}

		private void OnWait(AdvCommandSendMessage command)
		{
			command.IsWait = false;
		}

		public void ShowUtageWindow(bool show)
		{
			if (show)
			{
				AdvEngine.UiManager.Status = AdvUiManager.UiStatus.Default;
			}
			else
			{
				AdvEngine.UiManager.Status = AdvUiManager.UiStatus.HideMessageWindow;
			}
		}

		public void Pause()
		{
			AdvEngine.PauseScenario();
		}

		public void Resume()
		{
			AdvEngine.ResumeScenario();
		}
	}
}
