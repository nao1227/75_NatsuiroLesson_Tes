using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class MouseMenu : MenuParts
	{
		private class MouseOption
		{
			public int Decision;

			public int Auto;

			public int Special;

			public MouseOption(int dec, int auto, int sp)
			{
				Decision = dec;
				Auto = auto;
				Special = sp;
			}

			public void SetDecision(int MouseButtonNum)
			{
				if (Decision != MouseButtonNum)
				{
					if (Auto == MouseButtonNum)
					{
						Auto = Decision;
						Decision = MouseButtonNum;
					}
					else
					{
						Special = Decision;
						Decision = MouseButtonNum;
					}
				}
			}

			public void SetAuto(int MouseButtonNum)
			{
				if (Decision == MouseButtonNum)
				{
					Decision = Auto;
					Auto = MouseButtonNum;
				}
				else if (Auto != MouseButtonNum)
				{
					Special = Auto;
					Auto = MouseButtonNum;
				}
			}

			public void SetSpecial(int MouseButtonNum)
			{
				if (Decision == MouseButtonNum)
				{
					Decision = Special;
					Special = MouseButtonNum;
				}
				else if (Auto == MouseButtonNum)
				{
					Auto = Special;
					Special = MouseButtonNum;
				}
			}
		}

		public Image Drop1;

		public Image Drop2;

		public Image Drop3;

		public TextMeshProUGUI Text1;

		public TextMeshProUGUI Text2;

		public TextMeshProUGUI Text3;

		public Slider MouseSensitivitySlider;

		private MouseOption _mouseButton;

		public override void SetUp(MainMenuPresenter presenter)
		{
			MouseSensitivitySlider.value = SaveLoadManager.GlobalData.GameOption.MouseSensitivity;
			GameOption gameOption = SaveLoadManager.GlobalData.GameOption;
			_mouseButton = new MouseOption(gameOption.MouseButtonDecision, gameOption.MouseButtonAuto, gameOption.MouseButtonSpecial);
			_ = presenter.Context;
			_ = presenter.SubMenuWindowIndex;
			(from _ in Drop1.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.MouseLeftButton;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(GetMouseRoleBound(0));
				presenter.SubMenuWindow.SetUp(presenter.Context, 0);
			}).AddTo(this);
			(from _ in Drop2.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.MouseWheel;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(GetMouseRoleBound(2));
				presenter.SubMenuWindow.SetUp(presenter.Context, 2);
			}).AddTo(this);
			(from _ in Drop3.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.MouseRightButton;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(GetMouseRoleBound(1));
				presenter.SubMenuWindow.SetUp(presenter.Context, 3);
			}).AddTo(this);
			MouseSensitivitySlider.OnValueChangedAsObservable().Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.GameOption.MouseSensitivity = x;
			}).AddTo(this);
			SetText(presenter.SubMenuWindow);
		}

		public void SetText(SubMenuWindow win)
		{
			_ = SaveLoadManager.GlobalData.GameOption;
			Text1.text = win.MouseButtonText[GetMouseRoleBound(0)];
			Text2.text = win.MouseButtonText[GetMouseRoleBound(2)];
			Text3.text = win.MouseButtonText[GetMouseRoleBound(1)];
		}

		private int GetMouseRoleBound(int index)
		{
			GameOption gameOption = SaveLoadManager.GlobalData.GameOption;
			if (gameOption.MouseButtonDecision == index)
			{
				return 0;
			}
			if (gameOption.MouseButtonSpecial == index)
			{
				return 1;
			}
			return 2;
		}

		public void SetLeftButton(int num)
		{
			SetMouseRole(0, num);
		}

		public void SetWheel(int num)
		{
			SetMouseRole(2, num);
		}

		public void SetRightButton(int num)
		{
			SetMouseRole(1, num);
		}

		private void SetMouseRole(int index, int num)
		{
			switch (num)
			{
			case 0:
				_mouseButton.SetDecision(index);
				break;
			case 1:
				_mouseButton.SetSpecial(index);
				break;
			case 2:
				_mouseButton.SetAuto(index);
				break;
			}
			Save();
		}

		private void Save()
		{
			SaveLoadManager.GlobalData.GameOption.MouseButtonDecision = _mouseButton.Decision;
			SaveLoadManager.GlobalData.GameOption.MouseButtonAuto = _mouseButton.Auto;
			SaveLoadManager.GlobalData.GameOption.MouseButtonSpecial = _mouseButton.Special;
		}
	}
}
