using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class DisplayMenu : MenuParts
	{
		public Image Drop1;

		public Image Drop2;

		public Image Drop3;

		public Image Drop4;

		public TextMeshProUGUI Text1;

		public TextMeshProUGUI Text2;

		public TextMeshProUGUI Text3;

		public TextMeshProUGUI Text4;

		public Image ColorPalette;

		public List<Color> UIColorCandidates;

		public override void SetUp(MainMenuPresenter presenter)
		{
			MainMenuPresenter.SubMenuWindowContext context = presenter.Context;
			List<int> subMenuWindowIndex = presenter.SubMenuWindowIndex;
			(from _ in Drop1.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.WindowSize;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(subMenuWindowIndex[0]);
				presenter.SubMenuWindow.SetUp(presenter.Context, 0);
			}).AddTo(this);
			(from _ in Drop2.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				where !SingletonManager<ScreenManager>.Instance.IsFullScreen
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.Resolution;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(subMenuWindowIndex[1]);
				presenter.SubMenuWindow.SetUp(presenter.Context, 1);
			}).AddTo(this);
			(from _ in Drop3.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = ((presenter.Scene.Phase.Value == BaseScene.MenuPhase.Display) ? MainMenuPresenter.SubMenuWindowContext.Quality : MainMenuPresenter.SubMenuWindowContext.SkipText);
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(subMenuWindowIndex[2]);
				presenter.SubMenuWindow.SetUp(presenter.Context, 2);
			}).AddTo(this);
			(from _ in Drop4.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = ((presenter.Scene.Phase.Value == BaseScene.MenuPhase.Display) ? MainMenuPresenter.SubMenuWindowContext.FrameRate : MainMenuPresenter.SubMenuWindowContext.SkipText);
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(subMenuWindowIndex[3]);
				presenter.SubMenuWindow.SetUp(presenter.Context, 3);
			}).AddTo(this);
			(from x in ColorPalette.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				GameOption gameOption = SaveLoadManager.GlobalData.GameOption;
				for (int i = 0; i < UIColorCandidates.Count; i++)
				{
					if (UIColorCandidates[i] == gameOption.UIColor)
					{
						gameOption.UIColor = UIColorCandidates[(i + 1) % UIColorCandidates.Count];
						ColorPalette.color = gameOption.UIColor;
						break;
					}
				}
			}).AddTo(this);
			ColorPalette.color = SaveLoadManager.GlobalData.GameOption.UIColor;
			SetFirstText(presenter.SubMenuWindow);
		}

		public void SetText(int index, string text)
		{
			switch (index)
			{
			case 0:
				Text1.text = text;
				break;
			case 1:
				Text2.text = text;
				break;
			case 2:
				Text3.text = text;
				break;
			case 3:
				Text4.text = text;
				break;
			}
		}

		public void SetFirstText(SubMenuWindow win)
		{
			GameOption gameOption = SaveLoadManager.GlobalData.GameOption;
			Text1.text = win.WindowSizeText[SaveLoadManager.GlobalData.FullScreen];
			Text2.text = win.ResolutionText[SaveLoadManager.GlobalData.ScreenSize];
			Text3.text = win.QualityText[gameOption.ImageQuality];
			Text4.text = win.FrameRateText[gameOption.FrameRate];
		}
	}
}
