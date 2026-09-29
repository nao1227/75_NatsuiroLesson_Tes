using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Paidia.satsuki1
{
	public class OsawariUIPresenter : MonoBehaviour
	{
		[SerializeField]
		private IconObject ClothButton;

		[SerializeField]
		private IconObject GoodsButton;

		[SerializeField]
		private IconObject VaginaButton;

		[SerializeField]
		private IconObject PenisButton;

		[SerializeField]
		private IconObject ContextButton;

		[SerializeField]
		private IconObject ExitButton;

		[SerializeField]
		private IconObject HitAreaButton;

		[SerializeField]
		private IconObject RequestButton;

		[SerializeField]
		private GaugeObject HeartGauge;

		[SerializeField]
		private GaugeObject ShaseiGauge;

		[SerializeField]
		private GameObject SubIconTrayParent;

		[SerializeField]
		private GameObject GoodsSubIconParent;

		private SubIconTray ClothIconTray;

		[SerializeField]
		private List<ClothSubIconSet> SubIcons;

		[SerializeField]
		private SubIconTray GoodsSubIcon;

		private SubIconTray _goodsSubIcon;

		[SerializeField]
		private SubIconTray RequestSubIcon;

		private SubIconTray _requestSubIcon;

		[SerializeField]
		private TextMeshProUGUI ShaseiCount;

		[SerializeField]
		private HScene _scene;

		[SerializeField]
		private GameObject UIHolder;

		[SerializeField]
		private VariableSizeObject CSVariableSize;

		[SerializeField]
		private OsawariManager _manager;

		[SerializeField]
		private OsawariPiston _piston;

		[SerializeField]
		private InputManager InputManager;

		[SerializeField]
		private AtomosphereIndicator AtomosphereIndicator;

		[SerializeField]
		private BubbleHolder BubbleHolder;

		[SerializeField]
		private GameObject Bubble;

		[SerializeField]
		private BubbleManager BubbleManager;

		[SerializeField]
		private OsawariGoods OsawariGoods;

		[SerializeField]
		private ResultUIAnimsTween Results;

		[SerializeField]
		private CanvasGroup LowerUICG;

		[SerializeField]
		private CanvasGroup BlackScreenCG;

		[SerializeField]
		private CanvasGroup ShaseiUICG;

		[SerializeField]
		private bool UseResult = true;

		[SerializeField]
		private OsawariContext ContextToChange = OsawariContext.Fellatio;

		[SerializeField]
		private MessageWindowUIPresenter MessageWindowUIPresenter;

		public FlashAnimation EjaculateFlash;

		public YesNoWindowPresenter YesNoWindow;

		public Camera VFXCamera;

		private UtageManager _utage;

		private ContextManager _context;

		public Texture2D OsawariMouse;

		public Texture2D ResizeMouse;

		public Texture2D NormalMouse;

		public TextMeshProUGUI DescriptionText;

		private List<IconObject> buttons;

		private InsertController _insertController;

		private Hscene2WomanBody _furoBody;

		private bool _isShowingCrossSection;

		[NonSerialized]
		public bool IsLoaded;

		private Sequence _uiShowSeq;

		private Sequence _uiCollapseSeq;

		private FaceAnimationController _faceAnimation;

		private bool _fadeInEnabled;

		public bool IsMouseOnAnyIcon
		{
			get
			{
				if (!ClothButton.IsMouseOn && !VaginaButton.IsMouseOn && !PenisButton.IsMouseOn && !ExitButton.IsMouseOn && !GoodsButton.IsMouseOn && !RequestButton.IsMouseOn && !ClothIconTray.IsMouseOnAnyIcon && !_goodsSubIcon.IsMouseOnAnyIcon)
				{
					return _requestSubIcon.IsMouseOnAnyIcon;
				}
				return true;
			}
		}

		public bool IsMouseOnAnyVariableSizeObjects
		{
			get
			{
				if (_isShowingCrossSection)
				{
					return CSVariableSize.IsMouseOn;
				}
				return false;
			}
		}

		public bool IsMouseOnEdgeOfVariableSizeObject
		{
			get
			{
				if (IsMouseOnAnyVariableSizeObjects)
				{
					return CSVariableSize.IsMouseOnEdge;
				}
				return false;
			}
		}

		private bool ClickAllowed()
		{
			if ((null == _piston || !_piston.IsAnimating) && (null == _furoBody || _furoBody.ClickAllowed()) && !OsawariGoods.IsAnimating() && _faceAnimation.IsStateChangeAllowed && (null == _manager.OsawariFellatio || !_manager.OsawariFellatio.IsRestrictButton) && (null == _manager.OsawariPaizuri || !_manager.OsawariPaizuri.IsAnimating) && !MessageWindowUIPresenter.ShowingMessage && !_utage.IsPlaying)
			{
				return !_insertController.IsWomanExtacy;
			}
			return false;
		}

		public void SetFadeInEnable()
		{
			_fadeInEnabled = true;
		}

		private void Start()
		{
			_isShowingCrossSection = false;
			BlackScreenCG.alpha = 1f;
		}

		public async UniTask SetUp()
		{
			_ = 53;
			try
			{
				await UniTask.WaitUntil(() => _scene.Loaded, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				_faceAnimation = UnityEngine.Object.FindObjectOfType<FaceAnimationController>();
				_utage = UnityEngine.Object.FindObjectOfType<UtageManager>();
				_context = UnityEngine.Object.FindObjectOfType<ContextManager>();
				_insertController = _manager.GetComponent<InsertController>();
				if (SubIcons.Count((ClothSubIconSet x) => x.ClothName == _manager.TemporaryStatus.Cloth) > 0)
				{
					ClothIconTray = UnityEngine.Object.Instantiate(SubIcons.First((ClothSubIconSet x) => x.ClothName == _manager.TemporaryStatus.Cloth).SubIcon, SubIconTrayParent.transform);
				}
				else
				{
					ClothIconTray = UnityEngine.Object.Instantiate(SubIcons.First().SubIcon, SubIconTrayParent.transform);
				}
				_goodsSubIcon = UnityEngine.Object.Instantiate(GoodsSubIcon, SubIconTrayParent.transform);
				_requestSubIcon = UnityEngine.Object.Instantiate(RequestSubIcon, SubIconTrayParent.transform);
				buttons = new List<IconObject> { ClothButton, VaginaButton, PenisButton, ExitButton };
				if (ClothButton.isActiveAndEnabled)
				{
					await ClothButton.ManagedStart(ClickAllowed);
					ClothButton.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						if (ClothIconTray.IsShowing)
						{
							ClothIconTray.HideSubIcon();
						}
						else
						{
							ClothIconTray.ShowSubIcon();
						}
					}).AddTo(this);
					if (ClothIconTray.HasSubIcon(SubIconType.SwitchShirt))
					{
						SubIconObject si = await ClothIconTray.GetSubIcon(SubIconType.SwitchShirt, ClickAllowed);
						await UniTask.WaitUntil(() => si.IsLoaded);
						si.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							SwitchShirt(wear: true);
						}).AddTo(this);
					}
					if (ClothIconTray.HasSubIcon(SubIconType.SwitchSkirt))
					{
						(await ClothIconTray.GetSubIcon(SubIconType.SwitchSkirt, ClickAllowed)).OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							SwitchSkirt(wear: true);
						}).AddTo(this);
					}
					if (ClothIconTray.HasSubIcon(SubIconType.SwitchBra))
					{
						SubIconObject bt = await ClothIconTray.GetSubIcon(SubIconType.SwitchBra, ClickAllowed);
						await UniTask.WaitUntil(() => bt.IsLoaded);
						bt.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							SwitchBra();
						}).AddTo(this);
					}
					if (ClothIconTray.HasSubIcon(SubIconType.SwitchPants))
					{
						(await ClothIconTray.GetSubIcon(SubIconType.SwitchPants, ClickAllowed)).OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							SwitchPants();
						}).AddTo(this);
					}
					if (ClothIconTray.HasSubIcon(SubIconType.SwitchSMBlindfold))
					{
						(await ClothIconTray.GetSubIcon(SubIconType.SwitchSMBlindfold, ClickAllowed)).OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							_manager.GetOsawariOf<OsawariCosplay>().SwitchSM(SubIconType.SwitchSMBlindfold);
						}).AddTo(this);
					}
					if (ClothIconTray.HasSubIcon(SubIconType.SwitchSMHandcuffs))
					{
						(await ClothIconTray.GetSubIcon(SubIconType.SwitchSMHandcuffs, ClickAllowed)).OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							_manager.GetOsawariOf<OsawariCosplay>().SwitchSM(SubIconType.SwitchSMHandcuffs);
						}).AddTo(this);
					}
				}
				if (GoodsButton.isActiveAndEnabled)
				{
					await GoodsButton.ManagedStart(ClickAllowed);
					GoodsButton.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						if (_goodsSubIcon.IsShowing)
						{
							_goodsSubIcon.HideSubIcon();
						}
						else
						{
							_goodsSubIcon.ShowSubIcon();
						}
					}).AddTo(this);
					if (_goodsSubIcon.HasSubIcon(SubIconType.RotorBreast))
					{
						SubIconObject bt2 = await _goodsSubIcon.GetSubIcon(SubIconType.RotorBreast, ClickAllowed);
						await UniTask.WaitUntil(() => bt2.IsLoaded);
						bt2.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							OsawariGoods.SwitchRotorAppear(RotorPlace.Breast, bt2.Index == 0);
						}).AddTo(this);
						OsawariGoods.OnRotorEnabled.Where(((RotorPlace, bool) x) => x.Item1 == RotorPlace.Breast && SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_PinkRotor)).Subscribe(delegate((RotorPlace, bool) x)
						{
							bt2.SetEnable(x.Item2);
						}).AddTo(this);
						OsawariGoods.OnRotorAppear.Where(((RotorPlace, bool) x) => x.Item1 == RotorPlace.Breast).Subscribe(delegate((RotorPlace, bool) x)
						{
							bt2.ChangeIcon(x.Item2 ? 1 : 0);
						}).AddTo(this);
						bt2.SetEnable(isAble: false);
					}
					if (_goodsSubIcon.HasSubIcon(SubIconType.RotorPussy))
					{
						SubIconObject bt3 = await _goodsSubIcon.GetSubIcon(SubIconType.RotorPussy, ClickAllowed);
						await UniTask.WaitUntil(() => bt3.IsLoaded);
						bt3.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							OsawariGoods.SwitchRotorAppear(RotorPlace.Kuri, bt3.Index == 0);
						}).AddTo(this);
						OsawariGoods.OnRotorEnabled.Where(((RotorPlace, bool) x) => x.Item1 == RotorPlace.Kuri && SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_PinkRotor)).Subscribe(delegate((RotorPlace, bool) x)
						{
							bt3.SetEnable(x.Item2);
						}).AddTo(this);
						OsawariGoods.OnRotorAppear.Where(((RotorPlace, bool) x) => x.Item1 == RotorPlace.Kuri).Subscribe(delegate((RotorPlace, bool) x)
						{
							bt3.ChangeIcon(x.Item2 ? 1 : 0);
						}).AddTo(this);
						bt3.SetEnable(isAble: false);
					}
					if (_goodsSubIcon.HasSubIcon(SubIconType.Condom))
					{
						SubIconObject bt4 = await _goodsSubIcon.GetSubIcon(SubIconType.Condom, ClickAllowed);
						await UniTask.WaitUntil(() => bt4.IsLoaded);
						bt4.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							_manager.CsManager.OsawariCrossSection.SetCondom(bt4.Index == 0);
							OsawariGoods.SwitchCondom(bt4.Index == 0);
						}).AddTo(this);
						OsawariGoods.OnCondomSet.Subscribe(delegate(bool x)
						{
							bt4.ChangeIcon(x ? 1 : 0);
						}).AddTo(this);
						if (_scene.Name != SceneName.HScene2)
						{
							bt4.SetEnable(isAble: false);
							OsawariGoods.OnCondomEnabled.Subscribe(delegate(bool x)
							{
								bt4.SetEnable(x);
							}).AddTo(this);
							_piston.OnPenisShown.Where((bool _) => _manager.ContextManager.Context == OsawariContext.Osawari).Subscribe(delegate(bool x)
							{
								bt4.SetEnable(x);
							}).AddTo(this);
						}
						else if (!SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_Bath))
						{
							bt4.SetEnable(isAble: true);
						}
					}
					if (_goodsSubIcon.HasSubIcon(SubIconType.RotorOnOff))
					{
						SubIconObject bt5 = await _goodsSubIcon.GetSubIcon(SubIconType.RotorOnOff, ClickAllowed);
						await UniTask.WaitUntil(() => bt5.IsLoaded);
						bt5.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							OsawariGoods.SwitchRotor(bt5.Index == 0);
						}).AddTo(this);
						OsawariGoods.OnRotorEnabled.Where(((RotorPlace, bool) _) => SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_PinkRotor)).Subscribe(delegate
						{
							bt5.SetEnable(OsawariGoods.AnyRotorEnabled);
						}).AddTo(this);
						OsawariGoods.OnRotorMove.Subscribe(delegate(bool x)
						{
							bt5.ChangeIcon(x ? 1 : 0);
						}).AddTo(this);
						bt5.SetEnable(OsawariGoods.AnyRotorEnabled);
					}
					if (_goodsSubIcon.HasSubIcon(SubIconType.VibAppear))
					{
						SubIconObject bt6 = await _goodsSubIcon.GetSubIcon(SubIconType.VibAppear, ClickAllowed);
						await UniTask.WaitUntil(() => bt6.IsLoaded);
						bt6.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							bt6.ChangeIcon((bt6.Index == 0) ? 1 : 0);
							OsawariGoods.AppearVibrator(bt6.Index == 1);
						}).AddTo(this);
						OsawariGoods.OnVibratorEnabled.Subscribe(delegate(bool x)
						{
							bt6.SetEnable(x);
						}).AddTo(this);
						OsawariGoods.OnVibratorAppear.Where((bool x) => !x).Subscribe(delegate
						{
							bt6.ChangeIcon(0);
						});
						bt6.SetEnable(isAble: false);
					}
					if (_goodsSubIcon.HasSubIcon(SubIconType.VibOnOff))
					{
						SubIconObject bt7 = await _goodsSubIcon.GetSubIcon(SubIconType.VibOnOff, ClickAllowed);
						await UniTask.WaitUntil(() => bt7.IsLoaded);
						bt7.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
						{
							OsawariGoods.SwitchVibrator(bt7.Index == 0);
						}).AddTo(this);
						OsawariGoods.OnVibratorAppear.Subscribe(delegate(bool x)
						{
							bt7.SetEnable(x);
						}).AddTo(this);
						OsawariGoods.OnVibratorEnabled.Where((bool x) => !x).Subscribe(delegate
						{
							bt7.SetEnable(isAble: false);
						}).AddTo(this);
						OsawariGoods.OnVibratorStart.Subscribe(delegate(bool x)
						{
							bt7.ChangeIcon(x ? 1 : 0);
						}).AddTo(this);
						bt7.SetEnable(isAble: true);
					}
				}
				RequestButton?.ManagedStart(ClickAllowed);
				RequestButton?.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
				{
					if (_requestSubIcon.IsShowing)
					{
						_requestSubIcon.HideSubIcon();
					}
					else
					{
						_requestSubIcon.ShowSubIcon();
					}
				}).AddTo(this);
				if (_requestSubIcon.HasSubIcon(SubIconType.SwitchFellatio))
				{
					_manager.OsawariFellatio?.OnEnableManPenis.Where((bool _) => _manager.ContextManager.Context == OsawariContext.Fellatio).Subscribe(delegate(bool x)
					{
						PenisButton.SetEnable(x);
					}).AddTo(this);
					SubIconObject bt8 = await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatio, ClickAllowed);
					await UniTask.WaitUntil(() => bt8.IsLoaded);
					bt8.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(async delegate
					{
						bt8.ChangeIcon((bt8.Index == 0) ? 1 : 0);
						_manager.OsawariFellatio?.StartFellatio(bt8.Index == 1);
						SubIconObject sfm = await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatioMove, ClickAllowed);
						SubIconObject ss2 = await _requestSubIcon.GetSubIcon(SubIconType.SwitchSuck, ClickAllowed);
						SubIconObject subIconObject2 = await _requestSubIcon.GetSubIcon(SubIconType.PlayerControl, ClickAllowed);
						if (bt8.Index == 1)
						{
							ss2.ChangeIcon(0);
							ss2.SetEnable(isAble: false);
							sfm?.SetEnable(isAble: true);
							subIconObject2.SetEnable(isAble: true);
						}
						else
						{
							ss2.SetEnable(isAble: true);
							sfm?.SetEnable(isAble: false);
							subIconObject2.SetEnable(isAble: false);
							subIconObject2.ChangeIcon(0);
							sfm?.ChangeIcon(0);
						}
					}).AddTo(this);
					bt8.SetEnable(isAble: false);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.SwitchFellatioMove))
				{
					SubIconObject bt9 = await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatioMove, ClickAllowed);
					SubIconObject fsc = await _requestSubIcon.GetSubIcon(SubIconType.FellatioSpeedChange, ClickAllowed);
					await UniTask.WaitUntil(() => bt9.IsLoaded);
					bt9.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						_manager.OsawariFellatio?.StartFellatioMove(bt9.Index == 0);
					}).AddTo(this);
					_manager.OsawariFellatio?.OnFellatioMove.Subscribe(delegate(bool x)
					{
						bt9.ChangeIcon(x ? 1 : 0);
						fsc.SetEnable(x);
					}).AddTo(this);
					_manager.OsawariFellatio?.OnFellatioMoveEnabled.Subscribe(delegate(bool x)
					{
						bt9.SetEnable(x);
					}).AddTo(this);
					bt9.SetEnable(isAble: false);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.SwitchSuck))
				{
					SubIconObject bt10 = await _requestSubIcon.GetSubIcon(SubIconType.SwitchSuck, ClickAllowed);
					await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatioMove, ClickAllowed);
					SubIconObject sf = await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatio, ClickAllowed);
					bt10.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						sf.SetEnable(bt10.Index == 1);
						_manager.OsawariFellatio?.StartSuck(bt10.Index == 0);
					}).AddTo(this);
					_manager.OsawariFellatio?.OnSuck.Subscribe(delegate(bool x)
					{
						bt10.ChangeIcon(x ? 1 : 0);
					}).AddTo(this);
					bt10.SetEnable(isAble: false);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.FellatioSpeedChange))
				{
					SubIconObject bt11 = await _requestSubIcon.GetSubIcon(SubIconType.FellatioSpeedChange, ClickAllowed);
					bt11.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						_manager.OsawariFellatio?.SwitchSpeed();
					}).AddTo(this);
					bt11.SetEnable(isAble: false);
					_manager.OsawariFellatio?.OnMoveFast.Subscribe(delegate(bool x)
					{
						bt11.ChangeIcon(x ? 1 : 0);
					}).AddTo(this);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.PlayerControl))
				{
					SubIconObject bt12 = await _requestSubIcon.GetSubIcon(SubIconType.PlayerControl, ClickAllowed);
					SubIconObject ss = await _requestSubIcon.GetSubIcon(SubIconType.SwitchSuck, ClickAllowed);
					SubIconObject sf2 = await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatio, ClickAllowed);
					bt12.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						bt12.ChangeIcon((bt12.Index == 0) ? 1 : 0);
						if (bt12.Index == 1)
						{
							ss.SetEnable(isAble: false);
						}
						else
						{
							ss.SetEnable(sf2.Index == 0);
						}
						_manager.OsawariFellatio?.StartPlayerControlMode(bt12.Index == 1);
					}).AddTo(this);
					bt12.SetEnable(isAble: false);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.SwitchMouthOpen))
				{
					SubIconObject bt13 = await _requestSubIcon.GetSubIcon(SubIconType.SwitchMouthOpen, ClickAllowed);
					bt13.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						_manager.OsawariFellatio?.OpenMouth(bt13.Index == 0);
					}).AddTo(this);
					_manager.OsawariFellatio?.OnMouthOpen.Subscribe(delegate(bool x)
					{
						bt13.ChangeIcon(x ? 1 : 0);
					}).AddTo(this);
					bt13.SetEnable(isAble: false);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.SwitchRestrictDrink))
				{
					SubIconObject bt14 = await _requestSubIcon.GetSubIcon(SubIconType.SwitchRestrictDrink, ClickAllowed);
					bt14.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						if (bt14.Index == 0)
						{
							bt14.ChangeIcon(1);
							_manager.OsawariFellatio?.RestrictDrink(restrict: true);
						}
						else
						{
							bt14.ChangeIcon(0);
							_manager.OsawariFellatio?.RestrictDrink(restrict: false);
						}
					}).AddTo(this);
				}
				_manager.OsawariFellatio?.OnWaitingForDrinkOrder.Subscribe(async delegate(bool x)
				{
					(await _requestSubIcon.GetSubIcon(SubIconType.SwitchMouthOpen, ClickAllowed)).SetEnable(x);
				}).AddTo(this);
				_furoBody = _manager.GetOsawariOf<Hscene2WomanBody>();
				Hscene2OsawariPiston piston2 = _manager.GetOsawariOf<Hscene2OsawariPiston>();
				piston2?.OnReset.Subscribe(async delegate
				{
					(await _requestSubIcon.GetSubIcon(SubIconType.MoveSpeedChange, ClickAllowed)).ChangeIcon(0);
					(await _requestSubIcon.GetSubIcon(SubIconType.MoveStop, ClickAllowed)).ChangeIcon(0);
				}).AddTo(this);
				if (_requestSubIcon.HasSubIcon(SubIconType.MoveSpeedChange))
				{
					SubIconObject bt15 = await _requestSubIcon.GetSubIcon(SubIconType.MoveSpeedChange, ClickAllowed);
					bt15.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						bt15.ChangeIcon((bt15.Index == 0) ? 1 : 0);
						piston2.SetMoveFast(bt15.Index == 1);
					}).AddTo(this);
					bt15.SetEnable(isAble: false);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.MoveStop))
				{
					SubIconObject bt16 = await _requestSubIcon.GetSubIcon(SubIconType.MoveStop, ClickAllowed);
					bt16.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(async delegate
					{
						bt16.ChangeIcon((bt16.Index == 0) ? 1 : 0);
						piston2.SetMove(bt16.Index == 1);
						(await _requestSubIcon.GetSubIcon(SubIconType.Hold, ClickAllowed)).SetEnable(bt16.Index == 0);
					}).AddTo(this);
					bt16.SetEnable(isAble: false);
				}
				if (_requestSubIcon.HasSubIcon(SubIconType.Hold))
				{
					SubIconObject bt17 = await _requestSubIcon.GetSubIcon(SubIconType.Hold, ClickAllowed);
					bt17.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						bt17.ChangeIcon((bt17.Index == 0) ? 1 : 0);
						piston2.SetHold(bt17.Index == 1);
						PenisButton.SetEnable(bt17.Index == 0);
					}).AddTo(this);
					bt17.SetEnable(isAble: false);
				}
				if (PenisButton.isActiveAndEnabled)
				{
					await PenisButton.ManagedStart(ClickAllowed);
					(from _ in PenisButton.OnClick
						where ClickAllowed()
						where _piston != null
						select _).Subscribe(delegate
					{
						OnClickPenisButton();
					}).AddTo(this);
				}
				if (VaginaButton.isActiveAndEnabled)
				{
					await VaginaButton.ManagedStart(ClickAllowed);
					VaginaButton.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						OnClickVaginaButton();
					}).AddTo(this);
					(await ClothIconTray.GetSubIcon(SubIconType.WearAll, ClickAllowed))?.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						WearAll();
					}).AddTo(this);
					(await ClothIconTray.GetSubIcon(SubIconType.TakeOffAll, ClickAllowed))?.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						TakeOffAll();
					}).AddTo(this);
				}
				if (ContextButton.isActiveAndEnabled)
				{
					await ContextButton.ManagedStart(ClickAllowed);
					ContextButton.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						DisableCrossSection();
						SwitchContext((_context.Context == OsawariContext.Osawari) ? ContextToChange : OsawariContext.Osawari).Forget();
						OsawariGoods.SwitchContext();
						switch (_context.Context)
						{
						case OsawariContext.Osawari:
							if (_scene.Name == SceneName.HScene1)
							{
								ContextButton.ChangeIcon(0);
							}
							else
							{
								ContextButton.ChangeIcon(2);
							}
							break;
						case OsawariContext.Fellatio:
							ContextButton.ChangeIcon(1);
							break;
						case OsawariContext.Paizuri:
							ContextButton.ChangeIcon(3);
							break;
						}
					}).AddTo(this);
					if (_scene.Name == SceneName.HScene4)
					{
						ContextButton.ChangeIcon(2);
					}
				}
				await ExitButton.ManagedStart(ClickAllowed);
				ExitButton.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(async delegate
				{
					if (await YesNoWindow.WaitForAnswer("終了しますか？"))
					{
						_isShowingCrossSection = false;
						_manager.CsManager.SetVisible(_isShowingCrossSection);
						_manager.FinishScene();
						if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH)
						{
							SingletonManager<SoundManager>.Instance.StopAll();
							SaveLoadManager.ClearUnsavedData();
							await SceneManager.LoadSceneAsync(0);
						}
						else
						{
							if (UseResult)
							{
								try
								{
									_manager.GetScene().SetResultWindowVisible(visible: true);
									if (_scene.Name == SceneName.HScene3)
									{
										SaveLoadManager.UnsavedData.HasStudiedToday = true;
									}
									else if ((_scene.Name == SceneName.HScene2 || _scene.Name == SceneName.HScene1 || _scene.Name == SceneName.HScene4) && !SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_BathFellatio))
									{
										SaveLoadManager.UnsavedData.HasHSceneToday = true;
									}
									if (SaveLoadManager.UnsavedData.Days >= 7)
									{
										try
										{
											await Results.OpenResult(_manager.Result);
										}
										catch (OperationCanceledException)
										{
										}
									}
								}
								catch (Exception ex3)
								{
									Debug.LogError(ex3.StackTrace);
									return;
								}
							}
							if (_scene.Name == SceneName.EndOfDay)
							{
								bool fading = true;
								BlackScreenCG.blocksRaycasts = true;
								DOTween.Sequence().Append(BlackScreenCG.DOFade(1f, 1f)).AppendInterval(2f)
									.OnComplete(delegate
									{
										fading = false;
									})
									.Play();
								await UniTask.WaitUntil(() => !fading);
							}
							if (SaveLoadManager.UnsavedData.Days == 6)
							{
								SaveLoadManager.UnsavedData.HSceneCount = 1;
							}
							_scene.SetActive(active: false);
						}
					}
				}).AddTo(this);
				if (SaveLoadManager.UnsavedData.Days < 3 && SingletonManager<SceneContextManager>.Instance.CurrentSceneContext != SceneContext.FreeH)
				{
					ExitButton.SetEnable(isAble: false);
				}
				if (HitAreaButton.isActiveAndEnabled)
				{
					await HitAreaButton.ManagedStart(ClickAllowed);
					HitAreaButton.OnClick.Where((Unit _) => ClickAllowed()).Subscribe(delegate
					{
						_manager.ShowHitAreaAnimation().Forget();
					}).AddTo(this);
				}
				ClothButton?.OnMouse?.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				GoodsButton?.OnMouse.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				VaginaButton?.OnMouse.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				PenisButton?.OnMouse.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				ContextButton?.OnMouse.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				ExitButton?.OnMouse.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				HitAreaButton?.OnMouse.Subscribe(delegate(string x)
				{
					UnityEngine.Object.FindObjectOfType<HeaderUIPresenter>().SetDescriptionText(x);
				}).AddTo(this);
				RequestButton?.OnMouse.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				ClothButton?.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
				GoodsButton?.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
				VaginaButton?.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
				PenisButton?.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
				ContextButton?.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
				ExitButton?.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
				HitAreaButton?.OnMouseExit.Subscribe(delegate
				{
					UnityEngine.Object.FindObjectOfType<HeaderUIPresenter>().SetDescriptionText("");
				}).AddTo(this);
				RequestButton?.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
				foreach (SubIconType name in Enum.GetValues(typeof(SubIconType)))
				{
					SubIconObject subIconObject = await ClothIconTray.GetSubIcon(name, ClickAllowed);
					if (null != subIconObject)
					{
						SetSubIconRX(subIconObject);
						continue;
					}
					SubIconTray goodsSubIcon = _goodsSubIcon;
					if ((object)goodsSubIcon != null && (bool)goodsSubIcon)
					{
						subIconObject = await _goodsSubIcon.GetSubIcon(name, ClickAllowed);
						if (null != subIconObject)
						{
							SetSubIconRX(subIconObject);
							continue;
						}
					}
					goodsSubIcon = _requestSubIcon;
					if ((object)goodsSubIcon != null && (bool)goodsSubIcon)
					{
						SetSubIconRX(await _requestSubIcon.GetSubIcon(name, ClickAllowed));
					}
				}
				CloseClothSubTray();
				CloseGoodsSubTray();
				CloseRequestSubTray();
				if (_piston != null)
				{
					_piston.ManPenis.Subscribe(delegate(ManPenisStatus x)
					{
						int index = 0;
						switch (x)
						{
						case ManPenisStatus.Enter:
							index = 1;
							if (_scene.Name == SceneName.HScene2)
							{
								index = 3;
							}
							break;
						case ManPenisStatus.Insert:
							index = 2;
							break;
						}
						PenisButton.ChangeIcon(index);
					}).AddTo(this);
					(from x in _piston.OnEnablePenis
						where _manager.ContextManager.Context != OsawariContext.Fellatio
						where x != PenisButton.IsAble
						select x).Subscribe(delegate(bool x)
					{
						PenisButton.SetEnable(x);
					}).AddTo(this);
				}
				await UniTask.WaitUntil(() => null != HeartGauge && null != ShaseiGauge && _insertController.EjaculateCount != null);
				HeartGauge.SetRx(_insertController.WomanExtacy, 100000);
				ShaseiGauge.SetRx(_insertController.ManExtacy, 100);
				_manager.TemporaryStatus.Feelings.Atomosphere.Subscribe(delegate
				{
					AtomosphereIndicator.SetAtomosphereRate(_manager.TemporaryStatus.GetCurrentAtomosphere(), _manager.TemporaryStatus.Feelings.GetAtomosphereRate());
				}).AddTo(this);
				_manager.TemporaryStatus.Feelings.OnAtomosphereChanged.Subscribe(delegate(AtomosphereName x)
				{
					AtomosphereIndicator.SetImage(x);
				}).AddTo(this);
				_manager.CheckLeastAtomosphere();
				_manager.OnHint.Subscribe(delegate(HintButtonName x)
				{
					if (x == HintButtonName.Exit)
					{
						ExitButton.SetEnable(isAble: true);
						ExitButton.Highlight();
					}
				}).AddTo(this);
				_insertController.EjaculateCount.Subscribe(delegate(int x)
				{
					ShaseiCount.text = $" × {x}";
				}).AddTo(this);
				_insertController.OnEjaculate.Subscribe(delegate
				{
					EjaculateFlash.DoFlash();
					if (SaveLoadManager.UnsavedData.Days == 6)
					{
						DOVirtual.Float(0f, 1f, 8f, delegate(float x)
						{
							BlackScreenCG.alpha = x;
						}).SetDelay(3f).OnComplete(delegate
						{
							SaveLoadManager.UnsavedData.HasHSceneToday = true;
							_scene.SetActive(active: false);
						})
							.Play();
					}
				}).AddTo(this);
				_insertController.OnWomanExcite.Subscribe(delegate
				{
					EjaculateFlash.DoFlash(playSe: false);
				}).AddTo(this);
				_scene.UIShown.Subscribe(delegate(bool x)
				{
					UIHolder.SetActive(x);
				}).AddTo(this);
				BubbleManager.OnBubblePublish.Subscribe(async delegate(string x)
				{
					SpeechBubble bubble = UnityEngine.Object.Instantiate(Bubble, BubbleHolder.transform).GetComponent<SpeechBubble>();
					try
					{
						await bubble.Initialize(x);
					}
					catch
					{
						return;
					}
					BubbleHolder.AddBubble(bubble);
				}).AddTo(this);
				_uiShowSeq = DOTween.Sequence().Append(LowerUICG.DOFade(1f, 0.1f)).SetAutoKill(autoKillOnCompletion: false);
				_uiCollapseSeq = DOTween.Sequence().Append(LowerUICG.DOFade(0f, 0.1f)).SetAutoKill(autoKillOnCompletion: false);
				_utage.OnStartPlaying.Subscribe(delegate
				{
					_uiCollapseSeq.Restart();
				}).AddTo(this);
				_utage.OnFinishPlaying.Subscribe(delegate
				{
					_uiShowSeq.Restart();
				}).AddTo(this);
				OsawariGoods?.InitializeRx();
				if ((SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_BathFellatio) || SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.Evt_Fellatio)) && _scene.Name == SceneName.HScene1)
				{
					await SwitchContext(OsawariContext.Fellatio);
				}
				await UniTask.WaitUntil(() => _manager.IsLoaded);
				WearAll();
				IsLoaded = true;
				await UniTask.WaitUntil(() => _manager.AllResourceLoaded && _fadeInEnabled, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				InputManager.IsInOsawari.Subscribe(delegate(bool x)
				{
					SetIsInOsawari(x);
					if (x)
					{
						CloseClothSubTray();
						CloseGoodsSubTray();
						CloseRequestSubTray();
					}
				}).AddTo(this);
				BlackScreenCG.DOFade(0f, 0.5f).Play().OnComplete(delegate
				{
					BlackScreenCG.blocksRaycasts = false;
				});
			}
			catch (Exception ex)
			{
				if (!(ex is OperationCanceledException))
				{
					Debug.LogErrorFormat(ex.StackTrace);
				}
			}
		}

		private void Update()
		{
			if (!IsLoaded)
			{
				return;
			}
			InputManager.IsMouseOnUI = IsMouseOnAnyIcon || YesNoWindow.CG.alpha > 0f;
			InputManager.IsMouseOnVariableSize = IsMouseOnAnyVariableSizeObjects;
			InputManager.IsMouseOnEdgeOfVariableSize = IsMouseOnEdgeOfVariableSizeObject;
			if (!SingletonManager<SceneContextManager>.Instance.AllowOsawari)
			{
				Cursor.SetCursor(NormalMouse, new Vector2(OsawariMouse.width / 2, OsawariMouse.height / 2), CursorMode.Auto);
			}
			else
			{
				switch (InputManager.MouseOn)
				{
				case MouseOn.Osawari:
					Cursor.SetCursor(OsawariMouse, new Vector2(OsawariMouse.width / 2, OsawariMouse.height / 2), CursorMode.Auto);
					break;
				case MouseOn.Edge:
					Cursor.SetCursor(ResizeMouse, new Vector2(ResizeMouse.width / 2, ResizeMouse.height / 2), CursorMode.Auto);
					break;
				case MouseOn.VariableSizeObject:
					Cursor.SetCursor(OsawariMouse, new Vector2(OsawariMouse.width / 2, OsawariMouse.height / 2), CursorMode.Auto);
					break;
				default:
					Cursor.SetCursor(NormalMouse, new Vector2(NormalMouse.width / 2, NormalMouse.height / 2), CursorMode.Auto);
					break;
				}
			}
			DescriptionText.color = SaveLoadManager.GlobalData.GameOption.UIColor;
			ShaseiUICG.alpha = ((_scene.Name == SceneName.HScene1 || _scene.Name == SceneName.HScene2 || (_scene.Name == SceneName.HScene4 && SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Pool_H)) || SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH) ? 1 : 0);
		}

		private void SetSubIconRX(SubIconObject sb)
		{
			if (!(null == sb))
			{
				sb.OnMouse.Subscribe(delegate(string x)
				{
					DescriptionText.text = x;
				}).AddTo(this);
				sb.OnMouseExit.Subscribe(delegate
				{
					DescriptionText.text = "";
				}).AddTo(this);
			}
		}

		public void CloseClothSubTray()
		{
			ClothIconTray.HideSubIcon();
		}

		public void CloseGoodsSubTray()
		{
			_goodsSubIcon.HideSubIcon();
		}

		public void CloseRequestSubTray()
		{
			_requestSubIcon?.HideSubIcon();
		}

		public void SetIsInOsawari(bool isInOsawari)
		{
			foreach (IconObject button in buttons)
			{
				button.SetIsInOsawari(isInOsawari);
			}
		}

		private void SwitchShirt(bool wear)
		{
			_manager.OsawariFellatio?.SwitchCloth(shirt: true);
			_manager.GetEveryInterfaceOf<IShirt>().ForEach(delegate(IShirt x)
			{
				x.SwitchShirt();
			});
			OsawariGoods.SwitchShirt(wear);
		}

		private void SwitchSkirt(bool wear)
		{
			_manager.OsawariFellatio?.SwitchCloth(shirt: false, skirt: true);
			_manager.GetEveryInterfaceOf<ISkirt>().ForEach(delegate(ISkirt x)
			{
				x.SwitchSkirt();
			});
			OsawariGoods.SwitchSkirt(wear);
		}

		private void SwitchPants()
		{
			_manager.OsawariFellatio?.SwitchCloth(shirt: false, skirt: false, bra: false, pants: true);
			_manager.GetEveryInterfaceOf<IPants>().ForEach(delegate(IPants x)
			{
				x.SwitchPants();
			});
			OsawariGoods.SwitchPants();
		}

		private void SwitchBra()
		{
			if (_manager.TemporaryStatus.Cloth == ClothName.Normal || _scene.Name != SceneName.HScene1)
			{
				_manager.OsawariFellatio?.SwitchCloth(shirt: false, skirt: false, bra: true);
				_manager.GetEveryInterfaceOf<IBra>()?.ForEach(delegate(IBra x)
				{
					x.SwitchBra();
				});
				OsawariGoods.SwitchBra();
			}
			else
			{
				_manager.GetOsawariOf<OsawariCosplay>()?.SwitchCosplayBreast(_manager.TemporaryStatus.Cloth);
			}
		}

		private void ForceShirt(bool on)
		{
			if (on)
			{
				_manager.GetEveryInterfaceOf<IShirt>()?.ForEach(delegate(IShirt x)
				{
					x.OnShirt();
				});
				_manager.OsawariFellatio?.SetCloth(1f);
				OsawariGoods.ShirtOn();
			}
			else
			{
				_manager.GetEveryInterfaceOf<IShirt>()?.ForEach(delegate(IShirt x)
				{
					x.OffShirt();
				});
				_manager.OsawariFellatio?.SetCloth(0f);
			}
		}

		private void ForceSkirt(bool on)
		{
			if (on)
			{
				_manager.GetEveryInterfaceOf<ISkirt>()?.ForEach(delegate(ISkirt x)
				{
					x.OnSkirt();
				});
				_manager.GetOsawariOf<OsawariRibbon>()?.OnRibbon();
				_manager.OsawariFellatio?.SetCloth(-1f, 1f);
				OsawariGoods.SkirtOn();
			}
			else
			{
				_manager.GetEveryInterfaceOf<ISkirt>()?.ForEach(delegate(ISkirt x)
				{
					x.OffSkirt();
				});
				_manager.OsawariFellatio?.SetCloth(-1f, 0f);
			}
		}

		private void ForcePants(bool on)
		{
			if (on)
			{
				_manager.GetEveryInterfaceOf<IPants>()?.ForEach(delegate(IPants x)
				{
					x.OnPants();
				});
				_manager.OsawariFellatio?.SetCloth(-1f, -1f, -1f, 1f);
				OsawariGoods.PantsOn();
			}
			else
			{
				_manager.GetEveryInterfaceOf<IPants>()?.ForEach(delegate(IPants x)
				{
					x.OffPants();
				});
				_manager.OsawariFellatio?.SetCloth(-1f, -1f, -1f, 0f);
				OsawariGoods.PantsOff();
			}
		}

		private void ForceBra(bool on)
		{
			if (_manager.TemporaryStatus.Cloth != ClothName.Normal && _scene.Name == SceneName.HScene1)
			{
				_manager.GetOsawariOf<OsawariCosplay>()?.ForceCosplayBreast(_manager.TemporaryStatus.Cloth, on);
			}
			else if (on)
			{
				_manager.GetEveryInterfaceOf<IBra>()?.ForEach(delegate(IBra x)
				{
					x.TakeOnBra();
				});
				_manager.OsawariFellatio?.SetCloth(-1f, -1f, 1f);
				OsawariGoods.BraOn();
			}
			else
			{
				_manager.GetEveryInterfaceOf<IBra>()?.ForEach(delegate(IBra x)
				{
					x.TakeOffBra(exc: true);
				});
				_manager.OsawariFellatio?.SetCloth(-1f, -1f, 0f);
				OsawariGoods.BraOff();
			}
		}

		private void WearAll()
		{
			if (_manager.TemporaryStatus.Cloth != ClothName.Normal && _scene.Name == SceneName.HScene1)
			{
				_manager.GetOsawariOf<OsawariCosplay>()?.WearAll(_manager.TemporaryStatus.Cloth);
				return;
			}
			foreach (SubIconObject subIcon in ClothIconTray.GetSubIcons())
			{
				switch (subIcon.SubIconType)
				{
				case SubIconType.SwitchShirt:
					ForceShirt(on: true);
					break;
				case SubIconType.SwitchSkirt:
					ForceSkirt(on: true);
					break;
				case SubIconType.SwitchBra:
					ForceBra(on: true);
					break;
				case SubIconType.SwitchPants:
					ForcePants(on: true);
					break;
				}
			}
		}

		private void TakeOffAll()
		{
			if (_manager.TemporaryStatus.Cloth != ClothName.Normal && _scene.Name == SceneName.HScene1)
			{
				_manager.GetOsawariOf<OsawariCosplay>().UnwearAll(_manager.TemporaryStatus.Cloth);
				return;
			}
			foreach (SubIconObject subIcon in ClothIconTray.GetSubIcons())
			{
				switch (subIcon.SubIconType)
				{
				case SubIconType.SwitchShirt:
					ForceShirt(on: false);
					break;
				case SubIconType.SwitchSkirt:
					ForceSkirt(on: false);
					break;
				case SubIconType.SwitchBra:
					ForceBra(on: false);
					break;
				case SubIconType.SwitchPants:
					ForcePants(on: false);
					break;
				}
			}
		}

		private void OnClickPenisButton()
		{
			switch (_piston.ManPenis.Value)
			{
			case ManPenisStatus.Unshown:
				ManEnter();
				break;
			case ManPenisStatus.Enter:
				OnNextEnter();
				break;
			case ManPenisStatus.Insert:
				ManEject();
				break;
			}
		}

		private void OnClickVaginaButton()
		{
			_isShowingCrossSection = !_isShowingCrossSection;
			_manager.CsManager.SetVisible(_isShowingCrossSection);
		}

		private void DisableCrossSection()
		{
			_isShowingCrossSection = false;
			_manager.CsManager.SetVisible(_isShowingCrossSection);
		}

		private async void ManEnter()
		{
			_piston.ManEnter().Forget();
			if (_context.Context == OsawariContext.Fellatio)
			{
				(await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatio, ClickAllowed)).SetEnable(isAble: true);
				(await _requestSubIcon.GetSubIcon(SubIconType.SwitchSuck, ClickAllowed)).SetEnable(isAble: true);
			}
			else
			{
				_ = _context.Context;
				_ = 2;
			}
		}

		private async void OnNextEnter()
		{
			if (await _piston.OnNextEnter())
			{
				if (_context.Context == OsawariContext.Fellatio)
				{
					(await _requestSubIcon.GetSubIcon(SubIconType.SwitchFellatio, ClickAllowed)).SetEnable(isAble: false);
					(await _requestSubIcon.GetSubIcon(SubIconType.SwitchSuck, ClickAllowed)).SetEnable(isAble: false);
				}
				(await _requestSubIcon.GetSubIcon(SubIconType.MoveSpeedChange, ClickAllowed))?.SetEnable(isAble: true);
				(await _requestSubIcon.GetSubIcon(SubIconType.MoveStop, ClickAllowed))?.SetEnable(isAble: true);
				(await _requestSubIcon.GetSubIcon(SubIconType.Hold, ClickAllowed))?.SetEnable(isAble: true);
			}
		}

		private async void ManEject()
		{
			_piston.Eject().Forget();
			(await _requestSubIcon.GetSubIcon(SubIconType.MoveSpeedChange, ClickAllowed))?.SetEnable(isAble: false);
			(await _requestSubIcon.GetSubIcon(SubIconType.MoveStop, ClickAllowed))?.SetEnable(isAble: false);
			(await _requestSubIcon.GetSubIcon(SubIconType.Hold, ClickAllowed))?.SetEnable(isAble: false);
			(await _requestSubIcon.GetSubIcon(SubIconType.PlayerControl, ClickAllowed))?.SetEnable(isAble: false);
		}

		private async UniTask SwitchContext(OsawariContext context)
		{
			BlackScreenCG.blocksRaycasts = true;
			BlackScreenCG.alpha = 1f;
			_context.SetContext(context);
			_manager.SwitchContext();
			if (_context.Context == OsawariContext.Osawari)
			{
				VaginaButton.ShowIcon(show: true);
				VaginaButton.SetEnable(isAble: true);
				if (_scene.Name == SceneName.HScene1)
				{
					(await ClothIconTray.GetSubIcon(SubIconType.SwitchBra, ClickAllowed)).SetEnable(isAble: true);
				}
				if (ClothIconTray.HasSubIcon(SubIconType.SwitchSMHandcuffs))
				{
					(await ClothIconTray.GetSubIcon(SubIconType.SwitchSMHandcuffs, ClickAllowed)).SetEnable(isAble: true);
				}
			}
			else if (_context.Context == OsawariContext.Fellatio)
			{
				VaginaButton.ShowIcon(show: false);
				if (_manager.TemporaryStatus.Cloth != ClothName.Normal && _manager.TemporaryStatus.Cloth != ClothName.MicroBikini && _manager.TemporaryStatus.Cloth != ClothName.SM)
				{
					(await ClothIconTray.GetSubIcon(SubIconType.SwitchBra, ClickAllowed)).SetEnable(isAble: false);
				}
				if (ClothIconTray.HasSubIcon(SubIconType.SwitchSMHandcuffs))
				{
					(await ClothIconTray.GetSubIcon(SubIconType.SwitchSMHandcuffs, ClickAllowed)).SetEnable(isAble: false);
				}
			}
			else
			{
				_isShowingCrossSection = false;
				_manager.CsManager.SetVisible(_isShowingCrossSection);
				VaginaButton.SetEnable(isAble: false);
			}
			RequestButton?.ShowIcon(_context.Context == OsawariContext.Fellatio);
			bool isInFading = true;
			BlackScreenCG.DOFade(0f, 1.5f).SetDelay(1f).Play()
				.OnComplete(delegate
				{
					BlackScreenCG.blocksRaycasts = false;
					isInFading = false;
				});
			try
			{
				await UniTask.WaitUntil(() => !isInFading, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch (OperationCanceledException)
			{
			}
		}

		public void SetButtonEnable(ButtonName name, bool enable)
		{
			IconObject iconObject = name switch
			{
				ButtonName.Cloth => ClothButton, 
				ButtonName.Goods => GoodsButton, 
				ButtonName.Request => RequestButton, 
				ButtonName.Context => ContextButton, 
				ButtonName.Exit => ExitButton, 
				ButtonName.HitArea => HitAreaButton, 
				ButtonName.Vagina => VaginaButton, 
				ButtonName.Penis => PenisButton, 
				_ => null, 
			};
			if (null != iconObject)
			{
				iconObject.SetEnable(enable);
			}
		}

		private void OnDestroy()
		{
			Cursor.SetCursor(NormalMouse, new Vector2(OsawariMouse.width / 2, OsawariMouse.height / 2), CursorMode.Auto);
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}
}
