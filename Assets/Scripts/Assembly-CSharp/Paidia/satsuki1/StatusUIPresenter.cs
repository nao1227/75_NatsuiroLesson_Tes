using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtageExtensions;

namespace Paidia.satsuki1
{
	public class StatusUIPresenter : MonoBehaviour
	{
		public CanvasGroup CG;

		public Image Background;

		public Image Still;

		public TextMeshProUGUI RelationText;

		public TextMeshProUGUI StatsText;

		public List<Image> Favourability;

		public Image FavMax;

		public List<Image> Sensitivity;

		public Image SensivityMax;

		public List<Sprite> StillSprites;

		private CompositeDisposable _disposable;

		public TextMeshProUGUI FavBonusText;

		public TextMeshProUGUI SensitivityBonusText;

		public void DrawData(Action onComplete = null)
		{
			ClearWindow();
			LocalData unsavedData = SaveLoadManager.UnsavedData;
			TextMeshProUGUI relationText = RelationText;
			relationText.text = unsavedData.PersistantStatus.Relationship switch
			{
				Relationship.None => "生徒と先生", 
				Relationship.Secret => "ヒミツの関係", 
				Relationship.Special => "求め合う関係", 
				Relationship.Lovers => "恋人？", 
				Relationship.LoveyDovey => "ラブラブな二人", 
				_ => throw new InvalidOperationException(), 
			};
			Still.sprite = StillSprites[(int)unsavedData.PersistantStatus.Relationship];
			for (int i = 0; i < unsavedData.PersistantStatus.GetFavorabilityLevel(); i++)
			{
				Favourability[i].SetAlpha(1f);
			}
			if (unsavedData.PersistantStatus.GetFavorabilityLevel() == 7)
			{
				FavMax.SetAlpha(1f);
			}
			for (int j = 0; j < unsavedData.PersistantStatus.GetSensitivityLevel(); j++)
			{
				Sensitivity[j].SetAlpha(1f);
			}
			if (unsavedData.PersistantStatus.GetSensitivityLevel() == 6)
			{
				SensivityMax.SetAlpha(1f);
			}
			FavBonusText.text = $"+{unsavedData.PersistantStatus.GetAtomosphereBonusByLevel()}%";
			SensitivityBonusText.text = $"+{unsavedData.PersistantStatus.GetExciteBonusByLevel()}%";
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(unsavedData.HSceneCount).AppendLine();
			stringBuilder.Append(unsavedData.WomanExtacyCount).AppendLine();
			stringBuilder.Append(unsavedData.EjaculationInVaginaCount).AppendLine();
			stringBuilder.Append(unsavedData.FellatioEjaculationCount).AppendLine();
			stringBuilder.Append(unsavedData.KissCount).AppendLine();
			stringBuilder.Append(unsavedData.BreastCount).AppendLine();
			stringBuilder.Append(unsavedData.FellaSceneCount);
			StatsText.text = stringBuilder.ToString();
			_disposable = new CompositeDisposable();
			(from x in Background.OnPointerClickAsObservable()
				where x.button == PointerEventData.InputButton.Left
				select x).First().Subscribe(delegate
			{
				ClearWindow();
				_disposable.Dispose();
				SingletonManager<SceneContextManager>.Instance.AllowUtage = true;
				UnityEngine.Object.FindObjectOfType<HScene>()?.SetModalWindowVisible(visible: false);
				if (onComplete != null)
				{
					onComplete();
				}
			}).AddTo(_disposable);
			CG.alpha = 1f;
			CG.blocksRaycasts = true;
		}

		public void ClearWindow()
		{
			RelationText.text = "";
			foreach (Image item in Favourability)
			{
				item.SetAlpha(0f);
			}
			FavMax.SetAlpha(0f);
			foreach (Image item2 in Sensitivity)
			{
				item2.SetAlpha(0f);
			}
			SensivityMax.SetAlpha(0f);
			StatsText.text = "0\n0\n0\n0\n0\n0\n0";
			CG.alpha = 0f;
			CG.blocksRaycasts = false;
		}

		private void OnDestroy()
		{
			if (_disposable != null && !_disposable.IsDisposed)
			{
				_disposable.Dispose();
			}
		}
	}
}
