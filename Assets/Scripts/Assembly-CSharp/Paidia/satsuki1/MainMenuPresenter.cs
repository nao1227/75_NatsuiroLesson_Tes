using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class MainMenuPresenter : MonoBehaviour
	{
		public enum SubMenuWindowContext
		{
			None = 0,
			WindowSize = 1,
			Resolution = 2,
			Quality = 3,
			FrameRate = 4,
			SkipText = 5,
			MouseLeftButton = 6,
			MouseWheel = 7,
			MouseRightButton = 8,
			StopSkipOnChoice = 9,
			EjaculationCountOption = 10
		}

		[NonSerialized]
		public SubMenuWindowContext Context;

		public GameObject MainMenu;

		public Image StickerDisplay;

		public Image StickerSound;

		public Image StickerGame;

		public Image StickerText;

		public Image StickerMouse;

		public Image StickerShadow;

		public SubMenuWindow SubMenuWindow;

		public BaseScene Scene;

		public MonoBehaviour Scenee;

		public DisplayMenu DisplayMenu;

		public SoundMenu SoundMenu;

		public GameMenu GameMenu;

		public MouseMenu MouseMenu;

		public TextMenu TextMenu;

		public Image OutsideImg;

		public Image BackgroundImg;

		private readonly Vector3 STICKER_MOVE = new Vector3(268f, 0f, 0f);

		private Vector3 _stickyShadowDefault;

		[NonSerialized]
		public List<int> SubMenuWindowIndex;

		private Camera _vfxCamera;

		protected IOption _scene
		{
			get
			{
				if (!(Scenee is IOption result))
				{
					return null;
				}
				return result;
			}
		}

		private async void Start()
		{
			Context = SubMenuWindowContext.None;
			_stickyShadowDefault = StickerShadow.transform.localPosition;
			try
			{
				await UniTask.WaitUntil(() => _scene.IsLoaded(), PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch
			{
				return;
			}
			RestoreSubMenuWindowIndex();
			SetMenuImageActive(active: true);
			SubMenuWindow.SetActive(active: true);
			SubMenuWindow.ManagedStart();
			_scene.GetMenuPhase().Subscribe(delegate(BaseScene.MenuPhase x)
			{
				if (x == BaseScene.MenuPhase.None)
				{
					if (null != _vfxCamera)
					{
						_vfxCamera.gameObject.SetActive(value: true);
					}
				}
				else if (null == _vfxCamera)
				{
					_vfxCamera = GameObject.Find("VFXCamera")?.GetComponent<Camera>();
				}
				MainMenu.SetActive(x != BaseScene.MenuPhase.None);
				SingletonManager<SceneContextManager>.Instance.AllowUtage = x == BaseScene.MenuPhase.None;
				_scene.SetActiveSceneModalWindowVisible(x != BaseScene.MenuPhase.None);
				_stickyShadowDefault = new Vector3(_stickyShadowDefault.x, StickerDisplay.transform.localPosition.y - 75f, _stickyShadowDefault.z);
				switch (x)
				{
				case BaseScene.MenuPhase.Display:
					StickerShadow.transform.localPosition = _stickyShadowDefault;
					SetMenuImageActive(active: false);
					DisplayMenu.SetActive(active: true);
					ProcessSubMenu(Context, SubMenuWindow.Index);
					break;
				case BaseScene.MenuPhase.Sound:
					StickerShadow.transform.localPosition = _stickyShadowDefault + STICKER_MOVE;
					SetMenuImageActive(active: false);
					SoundMenu.SetActive(active: true);
					ProcessSubMenu(Context, SubMenuWindow.Index);
					break;
				case BaseScene.MenuPhase.Mouse:
					StickerShadow.transform.localPosition = _stickyShadowDefault + STICKER_MOVE * 2f;
					SetMenuImageActive(active: false);
					MouseMenu.SetActive(active: true);
					ProcessSubMenu(Context, SubMenuWindow.Index);
					break;
				case BaseScene.MenuPhase.Game:
					StickerShadow.transform.localPosition = _stickyShadowDefault + STICKER_MOVE * 3f;
					SetMenuImageActive(active: false);
					GameMenu.SetActive(active: true);
					ProcessSubMenu(Context, SubMenuWindow.Index);
					break;
				case BaseScene.MenuPhase.Text:
					StickerShadow.transform.localPosition = _stickyShadowDefault + STICKER_MOVE * 4f;
					SetMenuImageActive(active: false);
					TextMenu.SetActive(active: true);
					ProcessSubMenu(Context, SubMenuWindow.Index);
					break;
				case BaseScene.MenuPhase.Save:
					break;
				}
			}).AddTo(this);
			(from x in StickerDisplay.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				_scene.GetMenuPhase().Value = BaseScene.MenuPhase.Display;
			}).AddTo(this);
			(from x in StickerSound.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				_scene.GetMenuPhase().Value = BaseScene.MenuPhase.Sound;
			}).AddTo(this);
			(from x in StickerGame.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				_scene.GetMenuPhase().Value = BaseScene.MenuPhase.Game;
			}).AddTo(this);
			(from x in StickerText.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				_scene.GetMenuPhase().Value = BaseScene.MenuPhase.Text;
			}).AddTo(this);
			(from x in StickerMouse.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				_scene.GetMenuPhase().Value = BaseScene.MenuPhase.Mouse;
			}).AddTo(this);
			for (int num = 0; num < 4; num++)
			{
				int tmp = num;
				SubMenuWindow.Candidates[num].OnClick.Subscribe(delegate
				{
					SubMenuWindow.SetIndex(tmp);
					SubMenuEvent(tmp);
					ProcessSubMenu(Context, SubMenuWindow.Index);
				}).AddTo(this);
			}
			(from x in SubMenuWindow.Drop.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				ProcessSubMenu(Context, SubMenuWindow.Index);
			}).AddTo(this);
			(from x in BackgroundImg.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				ProcessSubMenu(Context, SubMenuWindow.Index);
			}).AddTo(this);
			(from x in OutsideImg.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				SaveLoadManager.SaveGlobalData();
				_scene.GetMenuPhase().Value = BaseScene.MenuPhase.None;
			}).AddTo(this);
			DisplayMenu.SetUp(this);
			SoundMenu.SetUp(this);
			GameMenu.SetUp(this);
			MouseMenu.SetUp(this);
			TextMenu.SetUp(this);
			SubMenuWindow.SetActive(active: false);
			SetMenuImageActive(active: false);
		}

		private void SubMenuEvent(int index)
		{
			BaseScene.MenuPhase value = _scene.GetMenuPhase().Value;
			if (value != BaseScene.MenuPhase.Display)
			{
				_ = 4;
			}
		}

		private void ProcessSubMenu(SubMenuWindowContext context, int index)
		{
			switch (context)
			{
			case SubMenuWindowContext.None:
				return;
			case SubMenuWindowContext.WindowSize:
				SaveLoadManager.GlobalData.FullScreen = index;
				SaveLoadManager.SaveGlobalData();
				SingletonManager<ScreenManager>.Instance.SetFullScreen(index == 1);
				SubMenuWindowIndex[0] = index;
				DisplayMenu.SetText(0, SubMenuWindow.WindowSizeText[index]);
				break;
			case SubMenuWindowContext.Resolution:
			{
				SaveLoadManager.GlobalData.ScreenSize = index;
				SaveLoadManager.SaveGlobalData();
				ScreenSize size = index switch
				{
					0 => ScreenSize.Large, 
					1 => ScreenSize.Mid, 
					2 => ScreenSize.Mid_Low, 
					3 => ScreenSize.Low, 
					_ => throw new NotImplementedException(), 
				};
				SingletonManager<ScreenManager>.Instance.ChangeScreenResolution(size);
				SubMenuWindowIndex[1] = index;
				DisplayMenu.SetText(1, SubMenuWindow.ResolutionText[index]);
				break;
			}
			case SubMenuWindowContext.Quality:
				SubMenuWindowIndex[2] = index;
				DisplayMenu.SetText(2, SubMenuWindow.QualityText[index]);
				break;
			case SubMenuWindowContext.FrameRate:
				SubMenuWindowIndex[3] = index;
				DisplayMenu.SetText(3, SubMenuWindow.FrameRateText[index]);
				break;
			case SubMenuWindowContext.MouseLeftButton:
				SubMenuWindowIndex[6] = index;
				MouseMenu.SetLeftButton(index);
				MouseMenu.SetText(SubMenuWindow);
				break;
			case SubMenuWindowContext.MouseWheel:
				SubMenuWindowIndex[7] = index;
				MouseMenu.SetWheel(index);
				MouseMenu.SetText(SubMenuWindow);
				break;
			case SubMenuWindowContext.MouseRightButton:
				SubMenuWindowIndex[8] = index;
				MouseMenu.SetRightButton(index);
				MouseMenu.SetText(SubMenuWindow);
				break;
			case SubMenuWindowContext.SkipText:
				SaveLoadManager.GlobalData.GameOption.SkipUnread = index;
				SubMenuWindowIndex[4] = index;
				TextMenu.SetText(0, SubMenuWindow.SkipText[index]);
				break;
			case SubMenuWindowContext.StopSkipOnChoice:
				SaveLoadManager.GlobalData.GameOption.StopSkipOnChoice = index;
				SubMenuWindowIndex[5] = index;
				TextMenu.SetText(1, SubMenuWindow.StopSkipOnChoiceText[index]);
				break;
			case SubMenuWindowContext.EjaculationCountOption:
				SaveLoadManager.GlobalData.GameOption.CountEjaculationWithCondom = index == 0;
				SubMenuWindowIndex[9] = index;
				GameMenu.SetText(SubMenuWindow.EjaculationCountOptionText[index]);
				break;
			}
			Context = SubMenuWindowContext.None;
			SubMenuWindow.SetActive(active: false);
		}

		private void RestoreSubMenuWindowIndex()
		{
			GameOption gameOption = SaveLoadManager.GlobalData.GameOption;
			SubMenuWindowIndex = new List<int>
			{
				SaveLoadManager.GlobalData.FullScreen,
				SaveLoadManager.GlobalData.ScreenSize,
				gameOption.ImageQuality,
				gameOption.FrameRate,
				gameOption.SkipUnread,
				gameOption.StopSkipOnChoice,
				gameOption.MouseButtonDecision,
				gameOption.MouseButtonAuto,
				gameOption.MouseButtonSpecial,
				(!gameOption.CountEjaculationWithCondom) ? 1 : 0
			};
		}

		private void SetMenuImageActive(bool active)
		{
			DisplayMenu.SetActive(active);
			SoundMenu.SetActive(active);
			GameMenu.SetActive(active);
			MouseMenu.SetActive(active);
			TextMenu.SetActive(active);
		}
	}
}
