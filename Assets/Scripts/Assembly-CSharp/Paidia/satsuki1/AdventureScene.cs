using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UtageExtensions;

namespace Paidia.satsuki1
{
	public class AdventureScene : Scene
	{
		public VideoPlayer VideoPlayer;

		public MainSceneFaceAnimation Animation;

		public MainScenarioDict MainScenarioByDay;

		public PartBCScenarioList MainScenarioAfterStudy;

		public PartBCScenarioList MainScenarioPartCScenario;

		public List<EventScenarioPair> EventScenario;

		public List<EventScenarioPair> TalkEvent;

		private UtageManager _utage;

		public Live2DAnimator Animator;

		public SubEventList SubEvents;

		public bool IsWaitingForReload;

		private int _days;

		private Subject<int> _onDayStart;

		private Subject<Unit> _onDayFinished;

		private Subject<Unit> _onLoad;

		private bool _startFromSaveData;

		public bool GoToHSceneDirectly;

		public bool NoFading;

		public bool MenuUISetuped;

		public Sprite Day2Image;

		public bool OnlyDayEnd;

		public IObservable<int> OnDayStart => _onDayStart;

		public IObservable<Unit> OnDayFinsihed => _onDayFinished;

		public IObservable<Unit> OnLoad => _onLoad;

		public override void SetUIVisible(bool visible)
		{
		}

		public void StartFromSaveData()
		{
			_startFromSaveData = true;
		}

		private EventScenarioPair GetEventScenarioPair(ScenarioLabel label)
		{
			return EventScenario.First((EventScenarioPair x) => x.Label == label);
		}

		private bool IsEventScenario(ScenarioLabel label)
		{
			return EventScenario.Any((EventScenarioPair x) => x.Label == label);
		}

		public async UniTask GoToHScene(int num)
		{
			if (!(null == this))
			{
				await SceneManager.LoadSceneAsync(num, LoadSceneMode.Additive);
				SetActive(active: false);
			}
		}

		public TemporaryStatus GetCurrentTemporaryStatus()
		{
			TemporaryStatus temporaryStatus;
			try
			{
				temporaryStatus = UnityEngine.Object.FindObjectOfType<StatusObject>().TemporaryStatus;
			}
			catch
			{
				temporaryStatus = new TemporaryStatus();
			}
			temporaryStatus.Cloth = (ClothName)_utage.GetInt("cloth");
			return temporaryStatus;
		}

		private bool IsEndingScenario(ScenarioLabel label)
		{
			if (label != ScenarioLabel.LastDay_B && label != ScenarioLabel.NormalEnd)
			{
				return label == ScenarioLabel.GoodEnd;
			}
			return true;
		}

		protected override async UniTask OnSetUp()
		{
			await UniTask.Yield();
			_onDayStart = new Subject<int>();
			_onDayFinished = new Subject<Unit>();
			_onLoad = new Subject<Unit>();
			_utage = UnityEngine.Object.FindObjectOfType<UtageManager>();
			(from x in _utage.OnStartPlaying
				where !TalkEvent.Any((EventScenarioPair y) => y.Label == x)
				where x != ScenarioLabel.OP && x != ScenarioLabel.Abstract_Start
				select x).Subscribe(delegate
			{
				EnableHeader(enable: true);
			}).AddTo(this);
			_utage.OnFinishPlaying.Where((ScenarioLabel x) => MainScenarioByDay.Contains(x)).Subscribe(delegate(ScenarioLabel x)
			{
				if (NoFading || GoToHSceneDirectly)
				{
					EnableHeader(enable: false);
				}
				_days = SaveLoadManager.UnsavedData.Days;
				if (MainScenarioByDay.Contains(x))
				{
					Animator.SetBool("NoGreeting", !MainScenarioByDay.HasGreeting(_days));
				}
			}).AddTo(this);
			Animator.ManagedStart();
		}

		public override async void SetActive(bool active)
		{
			if (active && OnlyDayEnd)
			{
				return;
			}
			try
			{
				if (null == this)
				{
					return;
				}
				if (!active)
				{
					base.HeaderEnabled = false;
				}
				base.SetActive(active);
				await UniTask.WaitUntil(() => MenuUISetuped, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				await UniTask.WaitUntil(() => SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.InGame, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				GoToHSceneDirectly = false;
				if (!active || _onDayStart == null)
				{
					_onDayFinished.OnNext(Unit.Default);
					return;
				}
				if (_startFromSaveData)
				{
					base.HeaderEnabled = true;
					_days = SaveLoadManager.UnsavedData.Days;
					ScenarioLabel label = SaveLoadManager.UnsavedData.ScenarioLabel;
					string utageLabel = SaveLoadManager.UnsavedData.ScenarioTitle;
					if (utageLabel.IsNullOrEmpty())
					{
						utageLabel = StringsManager.GetScenarioLabel(label);
					}
					if (MainScenarioAfterStudy.HasScenario(label))
					{
						PartBCScenario partBCScenario = MainScenarioAfterStudy.Get(label);
						GoToHSceneDirectly = partBCScenario.GoToHScene;
						NoFading = partBCScenario.NoFading;
					}
					else if (MainScenarioPartCScenario.HasScenario(label))
					{
						PartBCScenario partBCScenario2 = MainScenarioPartCScenario.Get(label);
						GoToHSceneDirectly = partBCScenario2.GoToHScene;
						NoFading = partBCScenario2.NoFading;
					}
					else if (MainScenarioByDay.HasLabel(label))
					{
						ScenarioLabel label2 = MainScenarioByDay.GetLabel(SaveLoadManager.UnsavedData.Days);
						GoToHSceneDirectly = label2 == ScenarioLabel.Day1;
						NoFading = label2 == ScenarioLabel.Day1;
					}
					else if (SubEvents.Contains(label))
					{
						SingletonManager<SoundManager>.Instance.StopBGS();
						NoFading = true;
						GoToHSceneDirectly = true;
						SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(SubEvents.GetSubEventByLabel(label).UnlockFlag, isOn: true);
						SaveLoadManager.UnsavedData.SubEventShown = true;
					}
					else if (IsEventScenario(label))
					{
						if (label == ScenarioLabel.Bath_Evt_A || label == ScenarioLabel.Relation_3A || label == ScenarioLabel.Relation_4A || label == ScenarioLabel.Relation_4B)
						{
							NoFading = true;
						}
					}
					else if (IsEndingScenario(label))
					{
						NoFading = true;
					}
					else if (SaveLoadManager.UnsavedData.Days == 7 && SaveLoadManager.UnsavedData.GlobalFlags.SkippedToDay6)
					{
						await UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>().SetLongMessage("１日の始めには、行動を選択してください。\n行動の結果によって、\nなつひの好感度が変化します。", Day2Image);
					}
					try
					{
						await _utage.ShowUtageTextFromLine(utageLabel, label, SaveLoadManager.UnsavedData.PageNumber, this.GetCancellationTokenOnDestroy());
					}
					catch (OperationCanceledException)
					{
						return;
					}
					_startFromSaveData = false;
					if (MainScenarioAfterStudy.HasScenario(label))
					{
						if (MainScenarioAfterStudy.Get(label).IsDayEnd)
						{
							IsWaitingForReload = true;
							return;
						}
						if (MainScenarioAfterStudy.Get(label).GoToHScene)
						{
							await GoToHScene((int)MainScenarioAfterStudy.Get(label).MoveScene);
						}
					}
					else if (MainScenarioPartCScenario.HasScenario(label))
					{
						if (MainScenarioPartCScenario.Get(label).IsDayEnd)
						{
							IsWaitingForReload = true;
							return;
						}
						if (MainScenarioPartCScenario.Get(label).GoToHScene)
						{
							await GoToHScene((int)MainScenarioPartCScenario.Get(label).MoveScene);
						}
					}
					else if (MainScenarioByDay.HasLabel(label) && label == ScenarioLabel.Day1)
					{
						await GoToHScene(5);
					}
					else if (SubEvents.Contains(label))
					{
						await GoToHScene((int)SubEvents.GetSubEventByLabel(label).SceneName);
					}
					else if (IsEventScenario(label))
					{
						if (GetEventScenarioPair(label).Timing == Timing.DayStart)
						{
							SaveLoadManager.UnsavedData.DayStartEventShown = true;
						}
						switch (label)
						{
						case ScenarioLabel.Bath_Evt_A:
							await GoToHScene(4);
							NoFading = false;
							break;
						case ScenarioLabel.Relation_3A:
							SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.Evt_BathFellatio, isOn: true);
							await GoToHScene(3);
							NoFading = false;
							break;
						case ScenarioLabel.Relation_3B:
							SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.Evt_BathFellatio, isOn: false);
							break;
						case ScenarioLabel.Relation_4A:
							if (SaveLoadManager.UnsavedData.LastSelectedIndex == 0)
							{
								await GoToHScene(3);
							}
							NoFading = false;
							break;
						case ScenarioLabel.Day3_C:
						case ScenarioLabel.Day5_C:
						case ScenarioLabel.Relation_4B:
							IsWaitingForReload = true;
							break;
						}
					}
					else
					{
						switch (label)
						{
						case ScenarioLabel.LastDay_B:
							if (!SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.GoodEnd))
							{
								await _utage.ShowUtageText(ScenarioLabel.NormalEnd, this.GetCancellationTokenOnDestroy());
							}
							else
							{
								await _utage.ShowUtageText(ScenarioLabel.GoodEnd, this.GetCancellationTokenOnDestroy());
							}
							await MoveToEnding();
							return;
						case ScenarioLabel.GoodEnd:
						case ScenarioLabel.NormalEnd:
							await MoveToEnding();
							return;
						case ScenarioLabel.Bath_Evt_B:
							if (SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_Bath))
							{
								SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.Evt_Bath, isOn: false);
							}
							IsWaitingForReload = true;
							return;
						}
					}
					if (MainScenarioByDay.HasLabel(label))
					{
						Animator.SetBool("NoGreeting", !MainScenarioByDay.HasGreeting(_days));
						_onDayStart.OnNext(_days);
					}
					else
					{
						Animator.SetBool("NoGreeting", on: true);
					}
					Animator.SetTrigger("TriggerStartADVFinished");
					_onLoad.OnNext(Unit.Default);
					return;
				}
				if (_days != SaveLoadManager.UnsavedData.Days)
				{
					if (SaveLoadManager.UnsavedData.GlobalFlags.SkippedToDay6)
					{
						base.HeaderEnabled = false;
					}
					_days = SaveLoadManager.UnsavedData.Days;
					LocalData unsavedData = SaveLoadManager.UnsavedData;
					if (SaveLoadManager.UnsavedData.NeedAutoSave)
					{
						SaveLoadManager.Save(0);
						unsavedData.NeedAutoSave = false;
					}
					if (MainScenarioByDay.HasLabel(_days) && !unsavedData.GlobalFlags.IsRead(MainScenarioByDay.GetLabel(_days)))
					{
						if (MainScenarioByDay.GetLabel(_days) == ScenarioLabel.Day1)
						{
							NoFading = true;
						}
						try
						{
							await _utage.ShowUtageText(MainScenarioByDay.GetLabel(_days), this.GetCancellationTokenOnDestroy());
						}
						catch (OperationCanceledException)
						{
							return;
						}
						Animator?.SetBool("NoGreeting", !MainScenarioByDay.HasGreeting(_days));
						if (MainScenarioByDay.GetLabel(_days) == ScenarioLabel.Day1)
						{
							GoToHScene(5).Forget();
						}
					}
					else
					{
						try
						{
							await CheckEventTrigger();
						}
						catch (OperationCanceledException)
						{
							return;
						}
						if (MainScenarioByDay.HasLabel(SaveLoadManager.UnsavedData.Days))
						{
							Animator.SetBool("NoGreeting", !MainScenarioByDay.HasGreeting(_days));
						}
						else
						{
							Animator.SetBool("NoGreeting", on: false);
						}
					}
					Animator.SetTrigger("TriggerStartADVFinished");
					_onDayStart.OnNext(_days);
					_onLoad.OnNext(Unit.Default);
					if (SaveLoadManager.UnsavedData.Days == 7 && SaveLoadManager.UnsavedData.GlobalFlags.SkippedToDay6)
					{
						await UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>().SetLongMessage("１日の始めには、行動を選択してください。\n行動の結果によって、\nなつひの好感度が変化します。", Day2Image);
					}
					return;
				}
				if (SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_Bath))
				{
					await _utage.ShowUtageText(ScenarioLabel.Bath_Evt_B, this.GetCancellationTokenOnDestroy());
					SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.Evt_Bath, isOn: false);
					IsWaitingForReload = true;
					return;
				}
				if (SaveLoadManager.UnsavedData.HasStudiedToday && MainScenarioAfterStudy.HasScenario(SaveLoadManager.UnsavedData.Days) && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(MainScenarioAfterStudy.Get(SaveLoadManager.UnsavedData.Days).ScenarioLabel))
				{
					PartBCScenario scenarioData = MainScenarioAfterStudy.Get(SaveLoadManager.UnsavedData.Days);
					GoToHSceneDirectly = scenarioData.GoToHScene;
					NoFading = scenarioData.NoFading;
					base.HeaderEnabled = true;
					try
					{
						await _utage.ShowUtageText(scenarioData.ScenarioLabel, this.GetCancellationTokenOnDestroy());
					}
					catch (OperationCanceledException)
					{
						return;
					}
					if (scenarioData.IsDayEnd)
					{
						IsWaitingForReload = true;
						return;
					}
					if (scenarioData.GoToHScene)
					{
						await GoToHScene((int)scenarioData.MoveScene);
					}
					else
					{
						Animator.SetBool("NoGreeting", !MainScenarioByDay.HasGreeting(_days));
						_onDayStart.OnNext(_days);
					}
				}
				else
				{
					if ((SaveLoadManager.UnsavedData.HasHSceneToday || SaveLoadManager.UnsavedData.HasStudiedToday) && MainScenarioPartCScenario.HasScenario(SaveLoadManager.UnsavedData.Days) && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(MainScenarioPartCScenario.Get(SaveLoadManager.UnsavedData.Days).ScenarioLabel))
					{
						PartBCScenario scenarioData = MainScenarioPartCScenario.Get(SaveLoadManager.UnsavedData.Days);
						base.HeaderEnabled = true;
						if (scenarioData.ScenarioLabel == ScenarioLabel.LastDay_A && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Relation_4A))
						{
							try
							{
								NoFading = true;
								await _utage.ShowUtageText(ScenarioLabel.AskForRetry, this.GetCancellationTokenOnDestroy());
							}
							catch (OperationCanceledException)
							{
								return;
							}
							if (await GameObject.Find("YesNoWindow").GetComponent<YesNoWindowPresenter>().WaitForAnswer("15日目からやり直しますか？"))
							{
								SaveLoadManager.UnsavedData.Days = 14;
								IsWaitingForReload = true;
								return;
							}
						}
						NoFading = scenarioData.NoFading;
						try
						{
							await _utage.ShowUtageText(scenarioData.ScenarioLabel, this.GetCancellationTokenOnDestroy());
						}
						catch (OperationCanceledException)
						{
							return;
						}
						if (scenarioData.GoToHScene)
						{
							await GoToHScene((int)scenarioData.MoveScene);
						}
						else
						{
							IsWaitingForReload = scenarioData.IsDayEnd;
						}
						return;
					}
					if (SaveLoadManager.UnsavedData.Days >= 7 && (SaveLoadManager.UnsavedData.HasHSceneToday || SaveLoadManager.UnsavedData.HasStudiedToday))
					{
						try
						{
							await CheckEventTrigger();
						}
						catch (OperationCanceledException)
						{
							return;
						}
						if (!IsWaitingForReload)
						{
							NoFading = true;
							await GoToHScene(7);
							return;
						}
					}
				}
				try
				{
					await CheckEventTrigger();
				}
				catch (OperationCanceledException)
				{
					return;
				}
				_onLoad.OnNext(Unit.Default);
				Animator?.SetBool("NoGreeting", on: true);
				Animator?.SetTrigger("TriggerStartADVFinished");
			}
			catch (OperationCanceledException)
			{
			}
		}

		public bool IsEventTrigger(Timing timing)
		{
			return EventScenario.Any((EventScenarioPair x) => x.Timing == timing && x.Conditions.All((EventCondition y) => y.IsFullfillCondition(GetCurrentTemporaryStatus(), _emptyConditions)) && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(x.Label));
		}

		private async UniTask CheckEventTrigger()
		{
			LocalData unsavedData = SaveLoadManager.UnsavedData;
			if (!unsavedData.HasStudiedToday && !unsavedData.HasTalkedToday && !unsavedData.HasHSceneToday && !unsavedData.SubEventShown && !unsavedData.DayStartEventShown && IsEventTrigger(Timing.DayStart))
			{
				await TriggerEventScenario(Timing.DayStart);
			}
			else if ((unsavedData.HasStudiedToday || unsavedData.HasHSceneToday || SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_BathFellatio)) && IsEventTrigger(Timing.PartC))
			{
				bool isDayEnd = GetTriggeredEventScenarioLabel(Timing.PartC) == ScenarioLabel.Relation_4B;
				if (isDayEnd)
				{
					NoFading = true;
				}
				await TriggerEventScenario(Timing.PartC);
				if (isDayEnd)
				{
					IsWaitingForReload = true;
				}
			}
		}

		private ScenarioLabel GetTriggeredEventScenarioLabel(Timing timing)
		{
			return EventScenario.First((EventScenarioPair x) => x.Timing == timing && x.Conditions.All((EventCondition y) => y.IsFullfillCondition(GetCurrentTemporaryStatus(), _emptyConditions)) && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(x.Label)).Label;
		}

		private async UniTask<ScenarioLabel> TriggerEventScenario(Timing timing)
		{
			ScenarioLabel label = GetTriggeredEventScenarioLabel(timing);
			if (!SaveLoadManager.UnsavedData.GlobalFlags.IsRead(label))
			{
				if (label == ScenarioLabel.Bath_Evt_A || label == ScenarioLabel.Relation_3A || label == ScenarioLabel.Relation_4A)
				{
					NoFading = true;
				}
				try
				{
					base.HeaderEnabled = true;
					await _utage.ShowUtageText(label, this.GetCancellationTokenOnDestroy());
				}
				catch (OperationCanceledException)
				{
					return label;
				}
				if (timing == Timing.DayStart)
				{
					SaveLoadManager.UnsavedData.DayStartEventShown = true;
				}
				switch (label)
				{
				case ScenarioLabel.Bath_Evt_A:
					await GoToHScene(4);
					NoFading = false;
					break;
				case ScenarioLabel.Relation_3A:
					SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.Evt_BathFellatio, isOn: true);
					await GoToHScene(3);
					NoFading = false;
					break;
				case ScenarioLabel.Relation_3B:
					SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(FlagEnum.Evt_BathFellatio, isOn: false);
					break;
				case ScenarioLabel.Relation_4A:
					if (SaveLoadManager.UnsavedData.LastSelectedIndex == 0)
					{
						await GoToHScene(3);
					}
					NoFading = false;
					break;
				}
			}
			return label;
		}

		public async UniTask OnDayEnd()
		{
			await UniTask.WaitUntil(() => Loaded);
			base.HeaderEnabled = true;
			ScenarioLabel scenarioLabel = ScenarioLabel.None;
			if (IsEventTrigger(Timing.DayEnd))
			{
				scenarioLabel = await TriggerEventScenario(Timing.DayEnd);
			}
			else if (SaveLoadManager.UnsavedData.Days == 31)
			{
				base.HeaderEnabled = true;
				await _utage.ShowUtageText(ScenarioLabel.LastDay_B, this.GetCancellationTokenOnDestroy());
				if (!SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.GoodEnd))
				{
					await _utage.ShowUtageText(ScenarioLabel.NormalEnd, this.GetCancellationTokenOnDestroy());
				}
				else
				{
					await _utage.ShowUtageText(ScenarioLabel.GoodEnd, this.GetCancellationTokenOnDestroy());
				}
				await MoveToEnding();
				return;
			}
			if (scenarioLabel != ScenarioLabel.Bath_Evt_A)
			{
				IsWaitingForReload = true;
			}
			OnlyDayEnd = false;
		}

		private async UniTask MoveToEnding()
		{
			base.HeaderEnabled = false;
			SaveLoadManager.GlobalData.IsCleared = true;
			SaveLoadManager.SaveGlobalData();
			await Unload();
			await SceneManager.LoadSceneAsync(9, LoadSceneMode.Additive);
		}

		public async UniTask OnTalk(ScenarioLabel label)
		{
			try
			{
				await _utage.ShowUtageText(label, this.GetCancellationTokenOnDestroy());
				base.HeaderEnabled = true;
			}
			catch (OperationCanceledException)
			{
			}
		}

		public List<ScenarioLabel> GetUnreadScenarioLabels(int count = 3)
		{
			List<ScenarioLabel> list = new List<ScenarioLabel>();
			List<ScenarioLabel> list2 = (from x in TalkEvent
				where !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(x.Label) && x.Conditions.All((EventCondition y) => y.IsFullfillCondition(GetCurrentTemporaryStatus(), _emptyConditions))
				select x.Label).ToList();
			if (list2.Count > count)
			{
				return list2.OrderBy((ScenarioLabel x) => Guid.NewGuid()).Take(count).ToList();
			}
			return list2;
		}

		public void OnBeforeSceneMove(SceneName name)
		{
			SubEvent subEvent = SubEvents.GetSubEvent(name);
			if (subEvent != null)
			{
				SingletonManager<SoundManager>.Instance.StopBGS();
				SaveLoadManager.UnsavedData.GlobalFlags.SetFlag(subEvent.UnlockFlag, isOn: true);
				SaveLoadManager.UnsavedData.SubEventShown = true;
				_utage.ShowUtageText(subEvent.Label, this.GetCancellationTokenOnDestroy()).Forget();
			}
		}

		public bool HasAnyValidEvent(SceneName name)
		{
			if ((name == SceneName.HScene4 && SaveLoadManager.UnsavedData.PersistantStatus.GetSensitivityLevel() >= 3 && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Pool_H) && SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Study_H)) || (name == SceneName.HScene3 && SaveLoadManager.UnsavedData.PersistantStatus.GetSensitivityLevel() >= 1 && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Study_H)))
			{
				return true;
			}
			return SubEvents.List.Any((SubEvent x) => x.SceneName == name && x.Conditions.All((EventCondition y) => y.IsFUllfilSubEventCondition()) && !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(x.Label));
		}
	}
}
