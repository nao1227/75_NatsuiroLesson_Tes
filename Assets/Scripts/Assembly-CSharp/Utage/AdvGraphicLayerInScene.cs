using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvGraphicLayerInScene")]
	public class AdvGraphicLayerInScene : AdvGraphicLayer
	{
		[SerializeField]
		private AdvLayerSettingData.LayerType layerType;

		[SerializeField]
		private Alignment alignment;

		[SerializeField]
		private bool flipX;

		[SerializeField]
		private bool flipY;

		[SerializeField]
		private Transform rootObjects;

		private Vector3 defaultPosition;

		private Vector2 defaultSize;

		private Vector3 defaultScale;

		private Quaternion defaultRotation;

		internal override AdvLayerSettingData.LayerType LayerType => layerType;

		internal override void Init(AdvGraphicManager manager)
		{
			base.Manager = manager;
			base.Canvas = GetComponent<Canvas>();
			base.RootObjects = ((rootObjects == null) ? base.transform : rootObjects);
			base.SettingData = new AdvLayerSettingData();
			base.SettingData.InitFromCanvas(base.Canvas, layerType, alignment, flipX, flipY);
			RectTransform rectTransform = base.transform as RectTransform;
			defaultPosition = rectTransform.localPosition;
			defaultSize = rectTransform.GetSize();
			defaultScale = rectTransform.localScale;
			defaultRotation = rectTransform.rotation;
		}

		internal override void ResetCanvasRectTransform()
		{
			RectTransform obj = base.transform as RectTransform;
			obj.localPosition = defaultPosition;
			obj.SetSize(defaultSize);
			obj.localScale = defaultScale;
			obj.rotation = defaultRotation;
		}
	}
}
