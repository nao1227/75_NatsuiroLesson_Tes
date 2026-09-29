using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utage
{
	[ExecuteInEditMode]
	[AddComponentMenu("Utage/Lib/UI/UguiLocalizeImage")]
	public class UguiLocalizeImage : UguiLocalizeBase
	{
		[Serializable]
		public class LocalizeSprite
		{
			public string language = "";

			public Sprite sprite;
		}

		[SerializeField]
		private List<LocalizeSprite> localizeSprites = new List<LocalizeSprite>();

		[NonSerialized]
		protected Sprite defaultSprite;

		private Image cachedImage;

		private List<LocalizeSprite> LocalizeSprites => localizeSprites;

		protected Image CachedImage
		{
			get
			{
				if (null == cachedImage)
				{
					cachedImage = GetComponent<Image>();
				}
				return cachedImage;
			}
		}

		protected override void RefreshSub()
		{
			if (CachedImage != null && !LanguageManagerBase.Instance.IgnoreLocalizeUiText)
			{
				CachedImage.sprite = FindSprite(LanguageManagerBase.Instance.CurrentLanguage);
			}
		}

		private Sprite FindSprite(string language)
		{
			LocalizeSprite localizeSprite = LocalizeSprites.Find((LocalizeSprite x) => x.language == language);
			if (localizeSprite != null)
			{
				return localizeSprite.sprite;
			}
			if (defaultSprite != null)
			{
				return defaultSprite;
			}
			if (LocalizeSprites.Count > 0)
			{
				return LocalizeSprites[0].sprite;
			}
			return null;
		}

		protected override void InitDefault()
		{
			if (CachedImage != null)
			{
				defaultSprite = CachedImage.sprite;
			}
		}

		public override void ResetDefault()
		{
			if (CachedImage != null)
			{
				CachedImage.sprite = defaultSprite;
			}
		}
	}
}
