using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class TutorialPresenter : MonoBehaviour
	{
		[NonSerialized]
		public TutorialName TutorialName;

		public CanvasGroup CG;

		public Image Outside;

		public Image TutorialImage;

		public Image RightArrow;

		public Image LeftArrow;

		public List<Sprite> MainSprites;

		public List<Sprite> StudySprites;

		public List<Sprite> StudySpritesLocked;

		public List<Sprite> HSprites;

		public List<Sprite> PoolSprites;

		public List<Sprite> PoolSpritesLocked;

		public List<Sprite> KissSprites;

		public List<Sprite> BathSprites;

		public List<Sprite> FellatioSprites;

		public List<Sprite> PaizuriSprites;

		private int _index;

		private Action _onComplete;

		private void Start()
		{
			CG.alpha = 0f;
			CG.blocksRaycasts = false;
			CG.interactable = false;
			(from _ in RightArrow.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where HasNext()
				select _).Subscribe(delegate
			{
				GoNext();
			}).AddTo(this);
			(from _ in LeftArrow.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where HasPrevious()
				select _).Subscribe(delegate
			{
				GoNext(reverse: true);
			}).AddTo(this);
			(from x in Outside.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).Subscribe(delegate
			{
				Hide();
			}).AddTo(this);
		}

		public void Show(TutorialName name, Action onComplete = null)
		{
			_onComplete = onComplete;
			TutorialName = name;
			CG.alpha = 1f;
			CG.blocksRaycasts = true;
			CG.interactable = true;
			_index = 0;
			UpdateImage();
		}

		private void Hide()
		{
			CG.alpha = 0f;
			CG.blocksRaycasts = false;
			CG.interactable = false;
			SingletonManager<SceneContextManager>.Instance.AllowUtage = true;
			UnityEngine.Object.FindObjectOfType<HScene>()?.SetModalWindowVisible(visible: false);
			if (_onComplete != null)
			{
				_onComplete();
			}
		}

		private bool HasNext()
		{
			return TutorialName switch
			{
				TutorialName.Tutorial_Main => _index < MainSprites.Count - 1, 
				TutorialName.Tutorial_Study => _index < StudySprites.Count - 1, 
				TutorialName.Tutorial_H => _index < HSprites.Count - 1, 
				TutorialName.Tutorial_Kiss => _index < KissSprites.Count - 1, 
				TutorialName.Tutorial_Bath => _index < BathSprites.Count - 1, 
				TutorialName.Tutorial_Fellatio => _index < FellatioSprites.Count - 1, 
				TutorialName.Tutorial_Paizuri => _index < PaizuriSprites.Count - 1, 
				TutorialName.Tutorial_Pool => _index < PoolSprites.Count - 1 + (PoolSpritesUnlocked() ? 1 : 0), 
				_ => false, 
			};
		}

		private bool HasPrevious()
		{
			return _index > 0;
		}

		public void GoNext(bool reverse = false)
		{
			if (reverse)
			{
				if (HasPrevious())
				{
					_index--;
				}
			}
			else if (HasNext())
			{
				_index++;
			}
			UpdateImage();
		}

		private void UpdateImage()
		{
			switch (TutorialName)
			{
			case TutorialName.Tutorial_Main:
				TutorialImage.sprite = MainSprites[_index];
				break;
			case TutorialName.Tutorial_Study:
				if (StudySpritesUnlocked() && _index == 0)
				{
					TutorialImage.sprite = StudySpritesLocked[0];
				}
				else
				{
					TutorialImage.sprite = StudySprites[_index];
				}
				break;
			case TutorialName.Tutorial_H:
				TutorialImage.sprite = HSprites[_index];
				break;
			case TutorialName.Tutorial_Kiss:
				TutorialImage.sprite = KissSprites[_index];
				break;
			case TutorialName.Tutorial_Pool:
				if (PoolSpritesUnlocked() && _index == 0)
				{
					TutorialImage.sprite = PoolSpritesLocked[0];
				}
				else if (PoolSpritesUnlocked() && _index == 2)
				{
					TutorialImage.sprite = PoolSpritesLocked[1];
				}
				else if (PoolSpritesUnlocked() && _index == 3)
				{
					TutorialImage.sprite = PoolSprites[2];
				}
				else
				{
					TutorialImage.sprite = PoolSprites[_index];
				}
				break;
			case TutorialName.Tutorial_Bath:
				TutorialImage.sprite = BathSprites[_index];
				break;
			case TutorialName.Tutorial_Fellatio:
				TutorialImage.sprite = FellatioSprites[_index];
				break;
			case TutorialName.Tutorial_Paizuri:
				TutorialImage.sprite = PaizuriSprites[_index];
				break;
			}
			RightArrow.color = (HasNext() ? Color.white : Color.gray);
			LeftArrow.color = (HasPrevious() ? Color.white : Color.gray);
		}

		private bool StudySpritesUnlocked()
		{
			return SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Study_H);
		}

		private bool PoolSpritesUnlocked()
		{
			return SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Pool_H);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				List<RaycastResult> raycastResults = new List<RaycastResult>();
				PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
				pointerEventData.position = Input.mousePosition;
				EventSystem.current.RaycastAll(pointerEventData, raycastResults);
			}
		}
	}
}
