using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvGraphicLayerDefault")]
	public class AdvGraphicLayerDefault : AdvGraphicLayer
	{
		internal override AdvLayerSettingData.LayerType LayerType => base.SettingData.Type;

		private Camera Camera { get; set; }

		private LetterBoxCamera LetterBoxCamera { get; set; }

		private Vector2 GameScreenSize => LetterBoxCamera.CurrentSize;

		internal override void Init(AdvGraphicManager manager)
		{
			base.Manager = manager;
		}

		internal void Init(AdvLayerSettingData settingData)
		{
			base.SettingData = settingData;
			base.Canvas = GetComponent<Canvas>();
			base.Canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;
			if (!string.IsNullOrEmpty(base.SettingData.LayerMask))
			{
				base.Canvas.gameObject.layer = LayerMask.NameToLayer(base.SettingData.LayerMask);
			}
			base.Canvas.sortingOrder = base.SettingData.Order;
			Camera = base.Engine.CameraManager.FindCameraByLayer(base.Canvas.gameObject.layer);
			if (Camera == null)
			{
				Debug.LogError("Cant find camera");
				Camera = base.Engine.CameraManager.FindCameraByLayer(0);
			}
			LetterBoxCamera = Camera.gameObject.GetComponent<LetterBoxCamera>();
			base.Canvas.worldCamera = Camera;
			base.Canvas.gameObject.AddComponent<GraphicRaycaster>().enabled = false;
			base.RootObjects = base.Canvas.transform;
			ResetCanvasRectTransform();
			if (base.Manager.DebugAutoResetCanvasPosition)
			{
				LetterBoxCamera.OnGameScreenSizeChange.AddListener(delegate
				{
					ResetCanvasRectTransform();
				});
			}
		}

		internal override void ResetCanvasRectTransform()
		{
			RectTransform obj = base.Canvas.transform as RectTransform;
			base.SettingData.Horizontal.GetBorderdPositionAndSize(GameScreenSize.x, out var position, out var size);
			base.SettingData.Vertical.GetBorderdPositionAndSize(GameScreenSize.y, out var position2, out var size2);
			obj.localPosition = new Vector3(position, position2, base.SettingData.Z) / base.Manager.PixelsToUnits;
			obj.SetSize(size, size2);
			obj.localScale = base.SettingData.Scale / base.Manager.PixelsToUnits;
			obj.localRotation = Quaternion.identity;
		}
	}
}
