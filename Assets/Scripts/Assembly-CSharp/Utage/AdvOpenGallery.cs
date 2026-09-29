using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Extra/AdvOpenGallery")]
	public class AdvOpenGallery : MonoBehaviour
	{
		[SerializeField]
		protected AdvEngine engine;

		public AdvEngine Engine => this.GetComponentCacheFindIfMissing(ref engine);

		public void OpenAll()
		{
			OpenAllSceneGallery();
			OpenAllCgGallery();
		}

		public void OpenAllSceneGallery()
		{
			AdvGallerySaveData galleryData = Engine.SystemSaveData.GalleryData;
			foreach (AdvSceneGallerySettingData item in Engine.DataManager.SettingDataManager.SceneGallerySetting.List)
			{
				galleryData.AddSceneLabel(item.ScenarioLabel);
			}
		}

		public void OpenAllCgGallery()
		{
			AdvGallerySaveData galleryData = Engine.SystemSaveData.GalleryData;
			foreach (AdvTextureSettingData item in Engine.DataManager.SettingDataManager.TextureSetting.List)
			{
				if (item.TextureType == AdvTextureSettingData.Type.Event && !string.IsNullOrEmpty(item.ThumbnailPath))
				{
					galleryData.AddCgLabel(item.Key);
				}
			}
		}
	}
}
