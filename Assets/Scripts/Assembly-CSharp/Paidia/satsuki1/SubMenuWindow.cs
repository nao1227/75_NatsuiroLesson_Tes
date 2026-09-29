using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class SubMenuWindow : MonoBehaviour
	{
		public List<SubMenuText> Candidates;

		public Image SelectionBar;

		public Image Drop;

		private Vector3 SelectionBarOriginalVec;

		public List<string> WindowSizeText;

		public List<string> ResolutionText;

		public List<string> QualityText;

		public List<string> FrameRateText;

		public List<string> SkipText;

		public List<string> MouseButtonText;

		public List<string> StopSkipOnChoiceText;

		public List<string> EjaculationCountOptionText;

		private Vector3 OriginalPosition;

		private readonly Vector3 MOVE = new Vector3(0f, -150f, 0f);

		public int Index { get; private set; }

		public void ManagedStart()
		{
			Index = 0;
			SelectionBarOriginalVec = SelectionBar.transform.localPosition;
			OriginalPosition = base.gameObject.transform.localPosition;
		}

		public void SetUp(MainMenuPresenter.SubMenuWindowContext context, int vert)
		{
			List<string> list = context switch
			{
				MainMenuPresenter.SubMenuWindowContext.WindowSize => WindowSizeText, 
				MainMenuPresenter.SubMenuWindowContext.Resolution => ResolutionText, 
				MainMenuPresenter.SubMenuWindowContext.Quality => QualityText, 
				MainMenuPresenter.SubMenuWindowContext.FrameRate => FrameRateText, 
				MainMenuPresenter.SubMenuWindowContext.SkipText => SkipText, 
				MainMenuPresenter.SubMenuWindowContext.StopSkipOnChoice => StopSkipOnChoiceText, 
				MainMenuPresenter.SubMenuWindowContext.MouseLeftButton => MouseButtonText, 
				MainMenuPresenter.SubMenuWindowContext.MouseWheel => MouseButtonText, 
				MainMenuPresenter.SubMenuWindowContext.MouseRightButton => MouseButtonText, 
				MainMenuPresenter.SubMenuWindowContext.EjaculationCountOption => EjaculationCountOptionText, 
				_ => throw new NotImplementedException(), 
			};
			for (int i = 0; i < Candidates.Count; i++)
			{
				if (i < list.Count)
				{
					Candidates[i].SetActive(active: true);
					Candidates[i].SetText(list[i]);
				}
				else
				{
					Candidates[i].SetText("");
					Candidates[i].SetActive(active: false);
				}
			}
			Vector3 vector = Vector3.zero;
			if (context == MainMenuPresenter.SubMenuWindowContext.MouseLeftButton || context == MainMenuPresenter.SubMenuWindowContext.MouseWheel || context == MainMenuPresenter.SubMenuWindowContext.MouseRightButton)
			{
				vector = new Vector3(-776f, 0f, 0f);
			}
			base.gameObject.transform.localPosition = OriginalPosition + MOVE * vert + vector;
		}

		public void SetIndex(int index)
		{
			Index = index;
			SelectionBar.transform.localPosition = new Vector3(SelectionBar.transform.localPosition.x, (0f - SelectionBar.rectTransform.sizeDelta.y) * (float)index, 0f);
			for (int i = 0; i < 4; i++)
			{
				Candidates[i].Select(i == index);
			}
		}

		public void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
		}
	}
}
