using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class TextMenu : MenuParts
	{
		public Slider slider1;

		public Slider slider2;

		public Image Drop1;

		public Image Drop2;

		public TextMeshProUGUI Text1;

		public TextMeshProUGUI Text2;

		public override void SetUp(MainMenuPresenter presenter)
		{
			slider1.value = SaveLoadManager.GlobalData.GameOption.TextSpeed;
			slider2.value = SaveLoadManager.GlobalData.GameOption.TextAutoSpeed;
			List<int> subMenuWindowIndex = presenter.SubMenuWindowIndex;
			slider1.OnValueChangedAsObservable().Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.GameOption.TextSpeed = x;
			}).AddTo(this);
			slider2.OnValueChangedAsObservable().Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.GameOption.TextAutoSpeed = x;
			}).AddTo(this);
			(from _ in Drop1.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.SkipText;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(subMenuWindowIndex[4]);
				presenter.SubMenuWindow.SetUp(presenter.Context, 2);
			}).AddTo(this);
			(from _ in Drop2.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.StopSkipOnChoice;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(subMenuWindowIndex[5]);
				presenter.SubMenuWindow.SetUp(presenter.Context, 3);
			}).AddTo(this);
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
			}
		}

		public void SetFirstText(SubMenuWindow win)
		{
			GameOption gameOption = SaveLoadManager.GlobalData.GameOption;
			Text1.text = win.SkipText[gameOption.SkipUnread];
			Text2.text = win.StopSkipOnChoiceText[gameOption.StopSkipOnChoice];
		}
	}
}
