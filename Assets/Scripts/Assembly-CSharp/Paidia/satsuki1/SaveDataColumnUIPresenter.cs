using System;
using Cysharp.Threading.Tasks;
using Paidia.Utils;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class SaveDataColumnUIPresenter : MonoBehaviour
	{
		[NonSerialized]
		public int Index;

		public Image Thumbnail;

		public bool IsLoad;

		public TextMeshProUGUI Relationship;

		public TextMeshProUGUI Playtime;

		public TextMeshProUGUI SaveDate;

		public TextMeshProUGUI SaveDataName;

		public TextMeshProUGUI Days;

		public CanvasGroup ValidData;

		public CanvasGroup InvalidData;

		public Image ClickImage;

		private string AutoColor = "#F55A7F";

		private string NormalColor = "#664D53";

		public Sprite SelectedSprite;

		private Sprite _originalSprite;

		public static bool SelectedAny;

		public Image Background;

		public Sprite AutoSaveImage;

		public IObservable<PointerEventData> OnClick => from x in ClickImage.OnPointerClickAsObservable()
			where x.button == PointerEventData.InputButton.Left
			select x;

		public void SetData(MetaSaveData data)
		{
			Thumbnail.sprite = null;
			TextMeshProUGUI relationship = Relationship;
			relationship.text = data.Relationship switch
			{
				Paidia.satsuki1.Relationship.None => "生徒と先生", 
				Paidia.satsuki1.Relationship.Secret => "ヒミツの関係", 
				Paidia.satsuki1.Relationship.Special => "求め合う関係", 
				Paidia.satsuki1.Relationship.Lovers => "恋人？", 
				Paidia.satsuki1.Relationship.LoveyDovey => "ラブラブな二人", 
				_ => throw new InvalidOperationException(), 
			};
			try
			{
				Playtime.text = new TimeSpan(0, 0, data.PlayTime).ToString("HH\\:mm");
			}
			catch
			{
				Playtime.text = new TimeSpan(0, 0, data.PlayTime).ToString("hh\\:mm");
			}
			SaveDate.text = data.SavedAt;
			Days.text = data.Days.ToString();
			_originalSprite = Background.sprite;
			if (ColorUtility.TryParseHtmlString((data.Index == 0) ? AutoColor : NormalColor, out var color))
			{
				SaveDataName.color = color;
			}
			if (data.Index == 0)
			{
				SaveDataName.text = "Auto";
				Thumbnail.sprite = AutoSaveImage;
			}
			else
			{
				SaveDataName.text = $"セーブデータ{data.Index}";
				try
				{
					SetImage(data.SaveDataName).Forget();
				}
				catch (Exception)
				{
				}
			}
			(from _ in ClickImage.OnPointerEnterAsObservable()
				where !SelectedAny && (data.Index > 0 || IsLoad)
				select _).Subscribe(delegate
			{
				Background.sprite = SelectedSprite;
			}).AddTo(this);
			(from _ in ClickImage.OnPointerExitAsObservable()
				where !SelectedAny
				select _).Subscribe(delegate
			{
				Background.sprite = _originalSprite;
			}).AddTo(this);
		}

		private async UniTask SetImage(string saveDataName)
		{
			Texture2D texture2D = await ExternalResources.ReadImageAsync(SaveLoadManager.GetCurrentPath() + saveDataName + ".png");
			if (null != texture2D)
			{
				Thumbnail.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.zero);
			}
		}

		public void SetActive(bool active)
		{
			if (active)
			{
				ValidData.alpha = 1f;
				InvalidData.alpha = 0f;
			}
			else
			{
				ValidData.alpha = 0f;
				InvalidData.alpha = 1f;
			}
		}

		public void Unselect()
		{
			SelectedAny = false;
			Background.sprite = _originalSprite;
		}
	}
}
