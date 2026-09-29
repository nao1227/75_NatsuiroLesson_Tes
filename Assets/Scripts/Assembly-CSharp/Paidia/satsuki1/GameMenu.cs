using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtageExtensions;

namespace Paidia.satsuki1
{
	public class GameMenu : MenuParts
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

		public TextMeshProUGUI ChangeNameText;

		public YesNoWindowPresenter YesNoWindow;

		public List<Color> UIColorCandidates;

		public CanvasGroup HighlightNameCG;

		public Image Drop;

		public Slider HeartGaugeSlider;

		public Slider AtomosphereGaugeSlider;

		public Slider EjaculationGaugeSlider;

		public TextMeshProUGUI HeartGaugeText;

		public TextMeshProUGUI AtomosphereGaugeText;

		public TextMeshProUGUI EjaculationGaugeText;

		public TextMeshProUGUI EjaculationCountText;

		public TMP_InputField NameInput;

		public TextMeshProUGUI ConfirmName;

		public CanvasGroup InvalidName;

		public Image NameInputConfirmUnderline;

		public CanvasGroup NameInputImageCG;

		public override void SetUp(MainMenuPresenter presenter)
		{
			_ = SaveLoadManager.GlobalData.GameOption;
			_ = presenter.Context;
			List<int> subMenuWindowIndex = presenter.SubMenuWindowIndex;
			HeartGaugeSlider.value = (SaveLoadManager.GlobalData.GameOption.HeartGaugeCorrection - 0.01f) / 0.99f;
			AtomosphereGaugeSlider.value = (SaveLoadManager.GlobalData.GameOption.AtomosphereGaugeCorrection - 0.01f) / 0.99f;
			EjaculationGaugeSlider.value = (SaveLoadManager.GlobalData.GameOption.EjaculationGaugeCorrection - 0.01f) / 0.99f;
			(from x in ChangeNameText.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				NameInput.text = SaveLoadManager.UnsavedData.PlayerName;
				NameInputImageCG.blocksRaycasts = true;
				DOVirtual.Float(0f, 1f, 1f, delegate(float f)
				{
					NameInputImageCG.alpha = f;
				}).Play();
			}).AddTo(this);
			ChangeNameText.OnPointerEnterAsObservable().Subscribe(delegate
			{
				HighlightNameCG.alpha = 1f;
			}).AddTo(this);
			ChangeNameText.OnPointerExitAsObservable().Subscribe(delegate
			{
				HighlightNameCG.alpha = 0f;
			}).AddTo(this);
			(from x in ConfirmName.OnPointerEnterAsObservable()
				where NameInput.text.Length < 6
				select x).Subscribe(delegate
			{
				NameInputConfirmUnderline.color = Color.white;
			}).AddTo(this);
			ConfirmName.OnPointerExitAsObservable().Subscribe(delegate
			{
				NameInputConfirmUnderline.color = Color.clear;
			}).AddTo(this);
			(from x in ConfirmName.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				where NameInput.text.Length < 6
				select x).Subscribe(async delegate
			{
				bool yesNo = false;
				try
				{
					yesNo = await YesNoWindow.WaitForAnswer("これでよろしいですか？");
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
				if (yesNo)
				{
					NameInputImageCG.blocksRaycasts = false;
					if (!NameInput.text.IsNullOrEmpty() && !(NameInput.text == "\u200b"))
					{
						_ = NameInput.text;
					}
					SaveLoadManager.UnsavedData.PlayerName = NameInput.text;
					DOVirtual.Float(1f, 0f, 1f, delegate(float f)
					{
						NameInputImageCG.alpha = f;
					}).Play();
				}
			}).AddTo(this);
			(from _ in ConfirmName.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where NameInput.text.Length >= 6
				select _).Subscribe(async delegate
			{
				await YesNoWindow.WaitForYes("入力できる文字数は５文字以下です。");
			}).AddTo(this);
			(from _ in Drop.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where presenter.Context == MainMenuPresenter.SubMenuWindowContext.None
				select _).Subscribe(delegate
			{
				presenter.Context = MainMenuPresenter.SubMenuWindowContext.EjaculationCountOption;
				presenter.SubMenuWindow.SetActive(active: true);
				presenter.SubMenuWindow.SetIndex(subMenuWindowIndex[9]);
				presenter.SubMenuWindow.SetUp(presenter.Context, 0);
			}).AddTo(this);
			(from x in HeartGaugeSlider.OnValueChangedAsObservable()
				select x * 0.99f + 0.01f).Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.GameOption.HeartGaugeCorrection = x;
				HeartGaugeText.text = $"{x * 100f:0}%";
			}).AddTo(this);
			(from x in AtomosphereGaugeSlider.OnValueChangedAsObservable()
				select x * 0.99f + 0.01f).Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.GameOption.AtomosphereGaugeCorrection = x;
				AtomosphereGaugeText.text = $"{x * 100f:0}%";
			}).AddTo(this);
			(from x in EjaculationGaugeSlider.OnValueChangedAsObservable()
				select x * 0.99f + 0.01f).Subscribe(delegate(float x)
			{
				SaveLoadManager.GlobalData.GameOption.EjaculationGaugeCorrection = x;
				EjaculationGaugeText.text = $"{x * 100f:0}%";
			}).AddTo(this);
			SetText(presenter.SubMenuWindow.EjaculationCountOptionText[(!SaveLoadManager.GlobalData.GameOption.CountEjaculationWithCondom) ? 1 : 0]);
		}

		private void Update()
		{
			InvalidName.alpha = ((NameInput.text.Length >= 6) ? 1 : 0);
		}

		public void SetText(string text)
		{
			EjaculationCountText.text = text;
		}
	}
}
