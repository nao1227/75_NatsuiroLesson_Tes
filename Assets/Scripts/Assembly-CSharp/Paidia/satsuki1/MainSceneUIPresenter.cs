using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Paidia.satsuki1
{
	public class MainSceneUIPresenter : MonoBehaviour
	{
		public List<MainSceneButton> Buttons;

		public CanvasGroup StudyAttentionCG;

		public CanvasGroup SwimAttentionCG;

		public CanvasGroup SexAttentionCG;

		public CanvasGroup BathAttentionCG;

		public AdventureScene AdventureScene;

		public MainSceneFaceAnimation MainSceneFaceAnimation;

		public TextMeshProUGUI Days;

		public CanvasGroup CG;

		public CanvasGroup ButtonCG;

		public CanvasGroup BlackScreenCG;

		public CanvasGroup OnMouseCG;

		public CanvasGroup SkipCG;

		public TextMeshProUGUI CommandText;

		public TextMeshProUGUI OnMouseMainText;

		public List<CommandText> CommandTexts;

		public ChoiceWindowPresenter ChoiceWindow;

		public AssetReference ClickSE;

		public AssetReference OnMouseSE;

		public AssetReference BGS;

		public AssetReference DayRefreshSE;

		public CanvasGroup DayStartImageCG;

		public TextMeshProUGUI DayStartText;

		public int FavExpOnConversation = 5;

		public int SensitivityExpOnConversation = 5;

		private AudioClip _clickSe;

		private AudioClip _onMouseSe;

		private AudioClip _bgs;

		private AudioClip _dayRefreshSE;

		public MainScenePhase _phase;

		private UtageManager _utage;

		private Tweener _fadeIn;

		private List<ScenarioLabel> _talkList;

		[SerializeField]
		private bool _chosenIcon;

		private async void OnEnable()
		{
			ChoiceWindow.SetActive(active: false);
			_chosenIcon = false;
			_talkList = new List<ScenarioLabel>();
			_phase = MainScenePhase.Preparing;
			OnMouseCG.alpha = 0f;
			if (null == SingletonManager<SceneContextManager>.Instance)
			{
				return;
			}
			BlackScreenCG.alpha = 1f;
			BlackScreenCG.blocksRaycasts = true;
			if (SaveLoadManager.UnsavedData.Days == 31 || !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Relation_4A))
			{
				SkipCG.alpha = 0f;
				SkipCG.blocksRaycasts = false;
			}
			_fadeIn = DOVirtual.Float(1f, 0f, 1f, delegate(float x)
			{
				BlackScreenCG.alpha = x;
			}).OnComplete(delegate
			{
				BlackScreenCG.blocksRaycasts = false;
				if (null != _bgs)
				{
					SingletonManager<SoundManager>.Instance.PlayBGS(_bgs);
				}
			}).SetAutoKill(autoKillOnCompletion: false);
			try
			{
				AsyncOperationHandle<AudioClip> clickHandle = Addressables.LoadAssetAsync<AudioClip>(ClickSE);
				AsyncOperationHandle<AudioClip> bgsHandle = Addressables.LoadAssetAsync<AudioClip>(BGS);
				AsyncOperationHandle<AudioClip> onMouseHandle = Addressables.LoadAssetAsync<AudioClip>(OnMouseSE);
				AsyncOperationHandle<AudioClip> dayRefreshHandle = Addressables.LoadAssetAsync<AudioClip>(DayRefreshSE);
				await clickHandle.Task;
				await bgsHandle.Task;
				await onMouseHandle.Task;
				await dayRefreshHandle.Task;
				if (clickHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_clickSe = clickHandle.Result;
				}
				if (bgsHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_bgs = bgsHandle.Result;
				}
				if (onMouseHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_onMouseSe = onMouseHandle.Result;
				}
				if (dayRefreshHandle.Status == AsyncOperationStatus.Succeeded)
				{
					_dayRefreshSE = dayRefreshHandle.Result;
				}
				await UniTask.WaitUntil(() => MainSceneFaceAnimation.OnFaceAnimationEnd != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch
			{
				return;
			}
			Days.text = new StringBuilder().Append(SaveLoadManager.UnsavedData.Days).ToString();
			SetMenuButtonEnable();
			_utage = UnityEngine.Object.FindObjectOfType<UtageManager>();
			try
			{
				await UniTask.WaitUntil(() => null != _utage, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch
			{
				return;
			}
			_utage.OnStartPlaying.Subscribe(delegate
			{
				if (_fadeIn.IsPlaying())
				{
					_fadeIn.Complete();
				}
				BlackScreenCG.alpha = 1f;
				BlackScreenCG.blocksRaycasts = true;
			}).AddTo(this);
			_utage.OnFinishPlaying.Where((ScenarioLabel _) => !AdventureScene.GoToHSceneDirectly && AdventureScene.IsActive && !AdventureScene.NoFading && !_chosenIcon).Subscribe(async delegate
			{
				SetMenuButtonEnable();
				if (!_fadeIn.IsPlaying())
				{
					await UniTask.WaitUntil(() => MainSceneFaceAnimation.IsLoad);
					_fadeIn.Restart();
				}
			}).AddTo(this);
			try
			{
				await UniTask.WaitUntil(() => AdventureScene.OnDayStart != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch
			{
				return;
			}
			AdventureScene.OnLoad.Where((Unit _) => !AdventureScene.GoToHSceneDirectly && !AdventureScene.NoFading).Subscribe(async delegate
			{
				SetMenuButtonEnable();
				await UniTask.WaitUntil(() => MainSceneFaceAnimation.IsLoad);
				AdventureScene.EnableHeader(enable: true);
				_fadeIn.Restart();
				_phase = MainScenePhase.WaitingForCommand;
			}).AddTo(this);
			AdventureScene.OnDayFinsihed.Subscribe(delegate
			{
				BlackScreenCG.alpha = 1f;
				BlackScreenCG.blocksRaycasts = true;
			}).AddTo(this);
			MainSceneFaceAnimation.OnFaceAnimationEnd.Subscribe(delegate(StateID x)
			{
				if (x == StateID.Entry || x == StateID.MainWait)
				{
					_phase = MainScenePhase.WaitingForCommand;
				}
				else
				{
					_phase = MainScenePhase.MoveToNextScene;
				}
			}).AddTo(this);
			foreach (MainSceneButton button in Buttons)
			{
				button.ManagedStart();
				button.OnClick.Where((MainSceneButtonID _) => _phase == MainScenePhase.WaitingForCommand && !_utage.IsPlaying && !_chosenIcon).ThrottleFirst(TimeSpan.FromSeconds(1.0)).Subscribe(async delegate(MainSceneButtonID x)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_clickSe);
					try
					{
						List<string> cosplayList = GetCosplayList(x);
						switch (x)
						{
						case MainSceneButtonID.Talk:
							AdventureScene.EnableHeader(enable: false);
							ButtonCG.alpha = 0f;
							ButtonCG.blocksRaycasts = false;
							if (SaveLoadManager.UnsavedData.Days == 6)
							{
								await AdventureScene.OnTalk(ScenarioLabel.Command_Talk_Day6);
							}
							else
							{
								List<string> list = new List<string>();
								foreach (ScenarioLabel talk in _talkList)
								{
									list.Add(StringsManager.GetScenarioTitle(talk));
								}
								ChoiceWindow.SetActive(active: true);
								try
								{
									int index = await ChoiceWindow.WaitForAnswer(list);
									SingletonManager<SoundManager>.Instance.StopBGS();
									await AdventureScene.OnTalk(_talkList[index]);
									if (SaveLoadManager.UnsavedData.Days >= 7)
									{
										MessageWindowUIPresenter message = UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>();
										switch (SaveLoadManager.UnsavedData.LastSelectedIndex)
										{
										case 0:
										{
											int favorabilityLevel = SaveLoadManager.UnsavedData.PersistantStatus.GetFavorabilityLevel();
											SaveLoadManager.UnsavedData.PersistantStatus.AddFavourability(FavExpOnConversation);
											int newLv = SaveLoadManager.UnsavedData.PersistantStatus.GetFavorabilityLevel();
											if (favorabilityLevel != newLv)
											{
												AudioClip se = await Addressables.LoadAssetAsync<AudioClip>("StatusLvUp");
												SingletonManager<SoundManager>.Instance.PlaySE(se);
												await message.SetShortMessage($"なつひの好感度が{newLv}になりました");
											}
											else
											{
												AudioClip se = await Addressables.LoadAssetAsync<AudioClip>("ExpUp");
												SingletonManager<SoundManager>.Instance.PlaySE(se);
												await message.SetShortMessage($"なつひの好感度expが+{FavExpOnConversation}されました");
											}
											break;
										}
										case 1:
										{
											int sensitivityLevel = SaveLoadManager.UnsavedData.PersistantStatus.GetSensitivityLevel();
											SaveLoadManager.UnsavedData.PersistantStatus.AddSensitivity(SensitivityExpOnConversation);
											int newLv = SaveLoadManager.UnsavedData.PersistantStatus.GetSensitivityLevel();
											if (sensitivityLevel != newLv)
											{
												AudioClip se = await Addressables.LoadAssetAsync<AudioClip>("StatusLvUp");
												SingletonManager<SoundManager>.Instance.PlaySE(se);
												await message.SetShortMessage($"なつひの開発度が{newLv}になりました");
											}
											else
											{
												AudioClip se = await Addressables.LoadAssetAsync<AudioClip>("ExpUp");
												SingletonManager<SoundManager>.Instance.PlaySE(se);
												await message.SetShortMessage($"なつひの開発度expが+{SensitivityExpOnConversation}されました");
											}
											break;
										}
										case 2:
											SingletonManager<SceneContextManager>.Instance.EjaculationPlus = true;
											break;
										}
									}
								}
								catch (NoChoiceException)
								{
									AdventureScene.EnableHeader(enable: true);
									SetMenuButtonEnable();
									ButtonCG.alpha = 1f;
									ButtonCG.blocksRaycasts = true;
									break;
								}
							}
							SaveLoadManager.UnsavedData.HasTalkedToday = true;
							SetMenuButtonEnable();
							ButtonCG.alpha = 1f;
							ButtonCG.blocksRaycasts = true;
							break;
						case MainSceneButtonID.Study:
							try
							{
								AdventureScene.EnableHeader(enable: false);
								if (cosplayList.Count > 1 && !AdventureScene.HasAnyValidEvent(SceneName.HScene3))
								{
									ButtonCG.alpha = 0f;
									ButtonCG.blocksRaycasts = false;
									try
									{
										int num3 = await ChoiceWindow.WaitForAnswer(cosplayList);
										ButtonCG.alpha = 1f;
										ButtonCG.blocksRaycasts = true;
										if (num3 > 1)
										{
											num3++;
										}
										_utage.SetInt("study_cloth", num3);
									}
									catch (NoChoiceException)
									{
										AdventureScene.EnableHeader(enable: true);
										ButtonCG.alpha = 1f;
										ButtonCG.blocksRaycasts = true;
										break;
									}
								}
								else
								{
									_utage.SetInt("study_cloth", 0);
								}
								_chosenIcon = true;
								_phase = MainScenePhase.FaceAnimating;
								MainSceneFaceAnimation.PlayStudy();
								await UniTask.WaitUntil(() => _phase == MainScenePhase.MoveToNextScene, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
								AdventureScene.OnBeforeSceneMove(SceneName.HScene3);
								await UniTask.WaitUntil(() => !_utage.IsPlaying, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
								AdventureScene.GoToHScene(5).Forget();
								break;
							}
							catch (OperationCanceledException)
							{
								break;
							}
						case MainSceneButtonID.Swim:
							try
							{
								AdventureScene.EnableHeader(enable: false);
								if (cosplayList.Count > 1 && !AdventureScene.HasAnyValidEvent(SceneName.HScene4))
								{
									ButtonCG.alpha = 0f;
									ButtonCG.blocksRaycasts = false;
									try
									{
										int num2 = await ChoiceWindow.WaitForAnswer(cosplayList);
										ButtonCG.alpha = 1f;
										ButtonCG.blocksRaycasts = true;
										if (num2 == 1)
										{
											_utage.SetInt("cloth", 2);
										}
										else
										{
											_utage.SetInt("cloth", num2);
										}
									}
									catch (NoChoiceException)
									{
										AdventureScene.EnableHeader(enable: true);
										ButtonCG.alpha = 1f;
										ButtonCG.blocksRaycasts = true;
										break;
									}
								}
								else
								{
									_utage.SetInt("cloth", 0);
								}
								_chosenIcon = true;
								_phase = MainScenePhase.FaceAnimating;
								MainSceneFaceAnimation.PlaySwim();
								await UniTask.WaitUntil(() => _phase == MainScenePhase.MoveToNextScene, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
								AdventureScene.OnBeforeSceneMove(SceneName.HScene4);
								await UniTask.WaitUntil(() => !_utage.IsPlaying, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
								AdventureScene.GoToHScene(6).Forget();
								break;
							}
							catch (OperationCanceledException)
							{
								break;
							}
						case MainSceneButtonID.Sex:
							try
							{
								AdventureScene.EnableHeader(enable: false);
								if (cosplayList.Count > 1 && !AdventureScene.HasAnyValidEvent(SceneName.HScene1))
								{
									ButtonCG.alpha = 0f;
									ButtonCG.blocksRaycasts = false;
									try
									{
										int num = await ChoiceWindow.WaitForAnswer(cosplayList);
										ButtonCG.alpha = 1f;
										ButtonCG.blocksRaycasts = true;
										switch (num)
										{
										case 1:
											_utage.SetInt("cloth", 2);
											break;
										case 2:
											_utage.SetInt("cloth", 1);
											break;
										default:
											_utage.SetInt("cloth", num);
											break;
										}
									}
									catch (NoChoiceException)
									{
										AdventureScene.EnableHeader(enable: true);
										ButtonCG.alpha = 1f;
										ButtonCG.blocksRaycasts = true;
										break;
									}
								}
								else
								{
									_utage.SetInt("cloth", 0);
								}
								SaveLoadManager.UnsavedData.HSceneCount++;
								_chosenIcon = true;
								_phase = MainScenePhase.FaceAnimating;
								MainSceneFaceAnimation.PlaySex();
								await UniTask.WaitUntil(() => _phase == MainScenePhase.MoveToNextScene, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
								AdventureScene.OnBeforeSceneMove(SceneName.HScene1);
								await UniTask.WaitUntil(() => !_utage.IsPlaying, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
								AdventureScene.GoToHScene(3).Forget();
								break;
							}
							catch (OperationCanceledException)
							{
								break;
							}
						case MainSceneButtonID.Bath:
							AdventureScene.EnableHeader(enable: false);
							SaveLoadManager.UnsavedData.HSceneCount++;
							_chosenIcon = true;
							_phase = MainScenePhase.FaceAnimating;
							MainSceneFaceAnimation.PlayBath();
							await UniTask.WaitUntil(() => _phase == MainScenePhase.MoveToNextScene, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
							AdventureScene.OnBeforeSceneMove(SceneName.HScene2);
							await UniTask.WaitUntil(() => !_utage.IsPlaying, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
							AdventureScene.GoToHScene(4).Forget();
							break;
						case MainSceneButtonID.DoNothing:
							AdventureScene.EnableHeader(enable: false);
							_phase = MainScenePhase.FaceAnimating;
							_chosenIcon = true;
							MainSceneFaceAnimation.PlayEnd();
							await UniTask.WaitUntil(() => _phase == MainScenePhase.MoveToNextScene, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
							AdventureScene.GoToHScene(7).Forget();
							break;
						case MainSceneButtonID.Skip:
							if (await GameObject.Find("YesNoWindow").GetComponent<YesNoWindowPresenter>().WaitForAnswer("夏休み最終日の３１日目までスキップしますか？（セーブ推奨）"))
							{
								SaveLoadManager.UnsavedData.Days = 30;
								UnityEngine.Object.FindObjectOfType<AdventureScene>().IsWaitingForReload = true;
							}
							break;
						}
					}
					catch
					{
					}
				})
					.AddTo(this);
				button.OnMouseEnter.Where((MainSceneButtonID _) => _phase == MainScenePhase.WaitingForCommand && !_utage.IsPlaying).Subscribe(delegate(MainSceneButtonID x)
				{
					if (null != _onMouseSe)
					{
						SingletonManager<SoundManager>.Instance.PlaySE(_onMouseSe);
					}
					CommandText commandText = CommandTexts.First((CommandText y) => y.ID == x);
					OnMouseCG.alpha = 1f;
					CommandText.text = commandText.CommandName;
					OnMouseMainText.text = commandText.CommandHelpText;
				}).AddTo(this);
				button.OnMouseExit.Subscribe(delegate
				{
					OnMouseCG.alpha = 0f;
				}).AddTo(this);
			}
			try
			{
				await UniTask.WaitUntil(() => !SingletonManager<SceneContextManager>.Instance.PlayOP);
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (Exception ex2)
			{
				Debug.LogError(ex2.StackTrace);
			}
			if (SaveLoadManager.UnsavedData.Days == 2)
			{
				_talkList = new List<ScenarioLabel> { ScenarioLabel.Talk_Day2 };
			}
			else
			{
				_talkList = AdventureScene.GetUnreadScenarioLabels();
			}
			if (SingletonManager<SceneContextManager>.Instance.IsDayRefreshed)
			{
				try
				{
					await UniTask.WaitUntil(() => !SingletonManager<SceneContextManager>.Instance.PlayOP, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				}
				catch (OperationCanceledException)
				{
					return;
				}
				SingletonManager<SoundManager>.Instance.PlaySE(_dayRefreshSE);
				SingletonManager<SceneContextManager>.Instance.IsDayRefreshed = false;
				DayStartImageCG.blocksRaycasts = true;
				DayStartText.text = $"Day {SaveLoadManager.UnsavedData.Days}";
				DOVirtual.Float(0f, 1f, 1f, delegate(float x)
				{
					DayStartImageCG.alpha = x;
				}).Play();
				bool waitingForClick = true;
				await UniTask.Delay(2000);
				while (waitingForClick)
				{
					await UniTask.Yield();
					waitingForClick = !Input.GetMouseButton(0);
				}
				DOVirtual.Float(1f, 0f, 1f, delegate(float x)
				{
					DayStartImageCG.alpha = x;
				}).OnComplete(delegate
				{
					DayStartImageCG.blocksRaycasts = false;
					AdventureScene.MenuUISetuped = true;
					_phase = MainScenePhase.WaitingForCommand;
					SingletonManager<SceneContextManager>.Instance.CurrentSceneContext = SceneContext.InGame;
				}).Play();
			}
			else
			{
				AdventureScene.MenuUISetuped = true;
				_phase = MainScenePhase.WaitingForCommand;
			}
		}

		public void Reset()
		{
			_phase = MainScenePhase.WaitingForCommand;
			SetMenuButtonEnable();
		}

		private void Update()
		{
			if (!(null == _utage))
			{
				CG.blocksRaycasts = !_utage.IsPlaying;
				_ = SaveLoadManager.UnsavedData;
				SexAttentionCG.alpha = ((AdventureScene.HasAnyValidEvent(SceneName.HScene1) && !IsActionDone()) ? 1 : 0);
				BathAttentionCG.alpha = ((AdventureScene.HasAnyValidEvent(SceneName.HScene2) && !IsActionDone()) ? 1 : 0);
				StudyAttentionCG.alpha = ((AdventureScene.HasAnyValidEvent(SceneName.HScene3) && !IsActionDone()) ? 1 : 0);
				SwimAttentionCG.alpha = ((AdventureScene.HasAnyValidEvent(SceneName.HScene4) && !IsActionDone()) ? 1 : 0);
			}
		}

		private void OnDestroy()
		{
			if (ClickSE.IsValid())
			{
				ClickSE.ReleaseAsset();
			}
		}

		private void OnDisable()
		{
			DisableAllButton();
		}

		private void SetMenuButtonEnable()
		{
			LocalData unsavedData = SaveLoadManager.UnsavedData;
			if (unsavedData.Days == 1)
			{
				foreach (MainSceneButton button in Buttons)
				{
					if (button.ButtonID == MainSceneButtonID.Study)
					{
						button.SetEnabled(enabled: true);
					}
					else
					{
						button.SetEnabled(enabled: false);
					}
				}
				return;
			}
			if (unsavedData.Days == 2)
			{
				foreach (MainSceneButton button2 in Buttons)
				{
					if ((button2.ButtonID == MainSceneButtonID.Talk && !unsavedData.HasTalkedToday) || button2.ButtonID == MainSceneButtonID.Study)
					{
						button2.SetEnabled(enabled: true);
					}
					else
					{
						button2.SetEnabled(enabled: false);
					}
				}
				return;
			}
			if (unsavedData.Days == 3)
			{
				foreach (MainSceneButton button3 in Buttons)
				{
					if ((button3.ButtonID == MainSceneButtonID.Talk && !unsavedData.HasTalkedToday) || (button3.ButtonID == MainSceneButtonID.Study && !unsavedData.HasStudiedToday) || (button3.ButtonID == MainSceneButtonID.DoNothing && unsavedData.HasStudiedToday))
					{
						button3.SetEnabled(enabled: true);
					}
					else
					{
						button3.SetEnabled(enabled: false);
					}
				}
				return;
			}
			if (unsavedData.Days == 4)
			{
				foreach (MainSceneButton button4 in Buttons)
				{
					if ((button4.ButtonID == MainSceneButtonID.Talk && !unsavedData.HasTalkedToday) || (button4.ButtonID == MainSceneButtonID.Study && !unsavedData.HasStudiedToday) || (button4.ButtonID == MainSceneButtonID.Swim && unsavedData.HasStudiedToday))
					{
						button4.SetEnabled(enabled: true);
					}
					else
					{
						button4.SetEnabled(enabled: false);
					}
				}
				return;
			}
			if (unsavedData.Days == 5)
			{
				foreach (MainSceneButton button5 in Buttons)
				{
					if ((button5.ButtonID == MainSceneButtonID.Talk && !unsavedData.HasTalkedToday) || (button5.ButtonID == MainSceneButtonID.Study && !unsavedData.HasStudiedToday) || button5.ButtonID == MainSceneButtonID.Swim)
					{
						button5.SetEnabled(enabled: true);
					}
					else
					{
						button5.SetEnabled(enabled: false);
					}
				}
				return;
			}
			if (unsavedData.Days == 6)
			{
				foreach (MainSceneButton button6 in Buttons)
				{
					if ((button6.ButtonID == MainSceneButtonID.Talk && !unsavedData.HasTalkedToday) || (button6.ButtonID == MainSceneButtonID.Study && !unsavedData.HasStudiedToday))
					{
						button6.SetEnabled(enabled: true);
					}
					else
					{
						button6.SetEnabled(enabled: false);
					}
				}
				return;
			}
			foreach (MainSceneButton button7 in Buttons)
			{
				if ((button7.ButtonID == MainSceneButtonID.Talk && !unsavedData.HasTalkedToday) || (button7.ButtonID == MainSceneButtonID.Study && !IsActionDone()) || (button7.ButtonID == MainSceneButtonID.Swim && !IsActionDone()) || (button7.ButtonID == MainSceneButtonID.Sex && !IsActionDone()) || (button7.ButtonID == MainSceneButtonID.Bath && !IsActionDone() && unsavedData.GlobalFlags.IsRead(ScenarioLabel.Relation_3A)) || (button7.ButtonID == MainSceneButtonID.DoNothing && unsavedData.Days != 31) || (button7.ButtonID == MainSceneButtonID.Skip && SaveLoadManager.UnsavedData.Days != 31))
				{
					button7.SetEnabled(enabled: true);
				}
				else
				{
					button7.SetEnabled(enabled: false);
				}
			}
		}

		private bool IsActionDone()
		{
			LocalData unsavedData = SaveLoadManager.UnsavedData;
			if (!unsavedData.HasStudiedToday)
			{
				return unsavedData.HasHSceneToday;
			}
			return true;
		}

		private void DisableAllButton()
		{
			foreach (MainSceneButton button in Buttons)
			{
				button.SetEnabled(enabled: false);
			}
		}

		private List<string> GetCosplayList(MainSceneButtonID id)
		{
			List<string> list = new List<string> { "いつもの服装" };
			GlobalFlags globalFlags = SaveLoadManager.UnsavedData.GlobalFlags;
			switch (id)
			{
			case MainSceneButtonID.Sex:
				if (globalFlags.IsRead(ScenarioLabel.Sub_MicroBikini_H))
				{
					list.Add("水着で");
				}
				if (globalFlags.IsRead(ScenarioLabel.Sub_Bunny_H))
				{
					list.Add("バニーで");
				}
				if (globalFlags.IsRead(ScenarioLabel.Sub_Cat_H))
				{
					list.Add("猫耳で");
				}
				if (globalFlags.IsRead(ScenarioLabel.Sub_SM_H))
				{
					list.Add("SM服で");
				}
				break;
			case MainSceneButtonID.Study:
				if (globalFlags.IsRead(ScenarioLabel.Sub_Underwear))
				{
					list.Add("下着を見せて");
				}
				if (globalFlags.IsRead(ScenarioLabel.Sub_MicroBikini_Study))
				{
					list.Add("マイクロビキニで");
				}
				if (globalFlags.IsRead(ScenarioLabel.Sub_Naked_Study))
				{
					list.Add("全裸で");
				}
				break;
			case MainSceneButtonID.Swim:
				if (globalFlags.IsRead(ScenarioLabel.Sub_MicroBikini))
				{
					list.Add("マイクロビキニで");
				}
				break;
			}
			return list;
		}
	}
}
