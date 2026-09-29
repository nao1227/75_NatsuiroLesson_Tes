using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvGraphicManager")]
	public class AdvGraphicManager : MonoBehaviour, IBinaryIO
	{
		[SerializeField]
		private float pixelsToUnits = 100f;

		[SerializeField]
		private float sortOderToZUnits = 100f;

		[SerializeField]
		private string bgSpriteName = "BG";

		[SerializeField]
		private bool resetCharacterTransformOnChangeLayer = true;

		[SerializeField]
		private bool ignoreKeepPositionOnCrossFade;

		[SerializeField]
		private AdvGraphicRenderTextureManager renderTextureManager;

		[SerializeField]
		private AdvVideoManager videoManager;

		[SerializeField]
		private List<AdvGraphicLayer> layerList = new List<AdvGraphicLayer>();

		private bool isEventMode;

		protected Dictionary<AdvLayerSettingData.LayerType, AdvGraphicGroup> Groups = new Dictionary<AdvLayerSettingData.LayerType, AdvGraphicGroup>();

		protected AdvEngine engine;

		private const int Version = 0;

		public float PixelsToUnits => pixelsToUnits;

		public float SortOderToZUnits => sortOderToZUnits;

		public string BgSpriteName => bgSpriteName;

		public bool ResetCharacterTransformOnChangeLayer => resetCharacterTransformOnChangeLayer;

		public bool DebugAutoResetCanvasPosition => false;

		public bool IgnoreKeepPositionOnCrossFade => ignoreKeepPositionOnCrossFade;

		public AdvGraphicRenderTextureManager RenderTextureManager
		{
			get
			{
				if (renderTextureManager == null)
				{
					renderTextureManager = base.transform.parent.AddChildGameObjectComponent<AdvGraphicRenderTextureManager>("GraphicRenderTextureManager");
				}
				return renderTextureManager;
			}
		}

		public AdvVideoManager VideoManager
		{
			get
			{
				if (videoManager == null)
				{
					videoManager = base.transform.parent.AddChildGameObjectComponent<AdvVideoManager>("VideoManager");
				}
				return videoManager;
			}
		}

		public List<AdvGraphicLayer> LayerList => layerList;

		public bool IsEventMode
		{
			get
			{
				return isEventMode;
			}
			set
			{
				isEventMode = value;
			}
		}

		public AdvGraphicGroup CharacterManager => Groups[AdvLayerSettingData.LayerType.Character];

		public AdvGraphicGroup SpriteManager => Groups[AdvLayerSettingData.LayerType.Sprite];

		public AdvGraphicGroup BgManager => Groups[AdvLayerSettingData.LayerType.Bg];

		internal AdvEngine Engine => engine;

		internal bool IsLoading
		{
			get
			{
				foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
				{
					if (group.Value.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string SaveKey => "AdvGraphicManager";

		public virtual void BootInit(AdvEngine engine, AdvLayerSetting setting)
		{
			this.engine = engine;
			Groups.Clear();
			foreach (AdvLayerSettingData.LayerType value2 in Enum.GetValues(typeof(AdvLayerSettingData.LayerType)))
			{
				if (value2 != AdvLayerSettingData.LayerType.Dummy)
				{
					AdvGraphicGroup value = new AdvGraphicGroup(value2, setting, this);
					Groups.Add(value2, value);
				}
			}
		}

		public void EmbedLayer(AdvGraphicLayer layer)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				if (group.Value.Type == layer.LayerType)
				{
					group.Value.EmbedLayer(layer);
				}
			}
		}

		public void RemoveEmbedLayer(AdvGraphicLayer layer)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				if (group.Value.Type == layer.LayerType)
				{
					group.Value.RemoveLayer(layer);
				}
			}
		}

		public void Remake(AdvLayerSetting setting)
		{
			foreach (AdvGraphicGroup value2 in Groups.Values)
			{
				value2.DestroyAll();
			}
			Groups.Clear();
			foreach (AdvLayerSettingData.LayerType value3 in Enum.GetValues(typeof(AdvLayerSettingData.LayerType)))
			{
				AdvGraphicGroup value = new AdvGraphicGroup(value3, setting, this);
				Groups.Add(value3, value);
			}
		}

		internal void Clear()
		{
			foreach (AdvGraphicGroup value in Groups.Values)
			{
				value.Clear();
			}
		}

		internal AdvGraphicLayer FindLayer(string layerName)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				AdvGraphicLayer advGraphicLayer = group.Value.FindLayer(layerName);
				if (advGraphicLayer != null)
				{
					return advGraphicLayer;
				}
			}
			return null;
		}

		internal AdvGraphicLayer FindLayerByObjectName(string name)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				AdvGraphicLayer advGraphicLayer = group.Value.FindLayerFromObjectName(name);
				if (advGraphicLayer != null)
				{
					return advGraphicLayer;
				}
			}
			return null;
		}

		internal void ResetAllLayerRectTransform()
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.ResetAllLayerRectTransform();
			}
		}

		internal AdvGraphicObject FindObject(string name)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				AdvGraphicObject advGraphicObject = group.Value.FindObject(name);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		internal GameObject FindObjectOrLayer(string targetName)
		{
			AdvGraphicObject advGraphicObject = FindObject(targetName);
			if (advGraphicObject != null)
			{
				return advGraphicObject.gameObject;
			}
			AdvGraphicLayer advGraphicLayer = FindLayer(targetName);
			if (advGraphicLayer != null)
			{
				return advGraphicLayer.gameObject;
			}
			return null;
		}

		internal List<AdvGraphicObject> AllGraphics()
		{
			List<AdvGraphicObject> list = new List<AdvGraphicObject>();
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.AddAllGraphics(list);
			}
			return list;
		}

		internal void DrawObject(string layerName, string label, AdvGraphicOperationArg graphicOperationArg)
		{
			FindLayer(layerName).Draw(label, graphicOperationArg);
		}

		public void ChangeLayer(string objectName, string layerName, AdvChangeLayerRepositionType repositionType, float fadeOutTime)
		{
			AdvGraphicLayer advGraphicLayer = FindLayer(layerName);
			if (advGraphicLayer == null)
			{
				Debug.LogErrorFormat("{0} is not found", layerName);
				return;
			}
			AdvGraphicGroup advGraphicGroup = null;
			AdvGraphicObject advGraphicObject = null;
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				advGraphicGroup = group.Value;
				advGraphicObject = advGraphicGroup.FindObject(objectName);
				if (advGraphicObject != null)
				{
					break;
				}
			}
			if (advGraphicObject == null)
			{
				Debug.LogErrorFormat("{0} is not found", objectName);
				return;
			}
			AdvGraphicLayer layer = advGraphicObject.Layer;
			if (!(layer == advGraphicLayer))
			{
				bool isDefaultObject = layer.DefaultObject == advGraphicObject;
				if (advGraphicGroup.Type == AdvLayerSettingData.LayerType.Sprite)
				{
					isDefaultObject = false;
				}
				advGraphicLayer.ChangeLayer(isDefaultObject, advGraphicObject, repositionType, fadeOutTime);
				layer.Remove(advGraphicObject);
			}
		}

		internal void FadeOutParticle(string targetName, AdvParticleStopType stopType)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.FadeOutParticle(targetName, stopType);
			}
		}

		internal void FadeOutAllParticle(AdvParticleStopType stopType)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.FadeOutAllParticle(stopType);
			}
		}

		public AdvGraphicObject FindParticle(string targetName)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				AdvGraphicObject advGraphicObject = group.Value.FindParticle(targetName);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		public bool IsFading(string targetName)
		{
			AdvGraphicObject advGraphicObject = FindObjectIncludeFadeOuting(targetName);
			if (advGraphicObject != null)
			{
				return advGraphicObject.IsFading;
			}
			AdvGraphicLayer advGraphicLayer = FindLayer(targetName);
			if (advGraphicLayer != null)
			{
				return advGraphicLayer.IsFading;
			}
			return false;
		}

		public void SkipFade(string targetName)
		{
			AdvGraphicObject advGraphicObject = FindObjectIncludeFadeOuting(targetName);
			if (advGraphicObject != null)
			{
				advGraphicObject.SkipFade();
				return;
			}
			AdvGraphicLayer advGraphicLayer = FindLayer(targetName);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.SkipFade();
			}
			else
			{
				Debug.LogError(targetName + " is not found in all objects");
			}
		}

		internal void FadeOutAllObjects(AdvGraphicObjectType objectType, float fadeTime)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.FadeOutAllObjects(objectType, fadeTime);
			}
		}

		public bool IsFadingObjects(AdvGraphicObjectType objectType)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				if (group.Value.IsFadingObjects(objectType))
				{
					return true;
				}
			}
			return false;
		}

		public void SkipFadeObjects(AdvGraphicObjectType objectType)
		{
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				group.Value.SkipFadeObjects(objectType);
			}
		}

		private AdvGraphicObject FindObjectIncludeFadeOuting(string targetName)
		{
			AdvGraphicObject advGraphicObject = FindObject(targetName);
			if (advGraphicObject != null)
			{
				return advGraphicObject;
			}
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				advGraphicObject = group.Value.FindFadeOutingObject(targetName);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		internal void CreateCaptureImageObject(string name, string cameraName, string layerName)
		{
			AdvGraphicLayer advGraphicLayer = FindLayer(layerName);
			if (advGraphicLayer == null)
			{
				Debug.LogError(layerName + " is not layer name");
				return;
			}
			CameraRoot cameraRoot = Engine.CameraManager.FindCameraRoot(cameraName);
			if (cameraRoot == null)
			{
				Debug.LogError(cameraName + " is not camera name");
				return;
			}
			AdvGraphicInfo grapic = new AdvGraphicInfo("Capture", name, "2D");
			advGraphicLayer.GetObjectCreateIfMissing(name, grapic).InitCaptureImage(grapic, cameraRoot.LetterBoxCamera.CachedCamera);
		}

		internal void RemoveClickEvent(string name)
		{
			AdvGraphicObject advGraphicObject = FindObject(name);
			if (!(advGraphicObject == null))
			{
				advGraphicObject.gameObject.GetComponentInChildren<IAdvClickEvent>()?.RemoveClickEvent();
			}
		}

		internal void AddClickEvent(string name, bool isPolygon, StringGridRow row, UnityAction<BaseEventData> action)
		{
			AdvGraphicObject advGraphicObject = FindObject(name);
			if (advGraphicObject == null)
			{
				Debug.LogError("can't find Graphic object" + name);
				return;
			}
			IAdvClickEvent componentInChildren = advGraphicObject.gameObject.GetComponentInChildren<IAdvClickEvent>();
			if (componentInChildren == null)
			{
				Debug.LogError("can't find IAdvClickEvent Interface in " + name);
			}
			else
			{
				componentInChildren.AddClickEvent(isPolygon, row, action);
			}
		}

		public virtual void OnWrite(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(isEventMode);
			writer.Write(Groups.Count);
			foreach (KeyValuePair<AdvLayerSettingData.LayerType, AdvGraphicGroup> group in Groups)
			{
				writer.Write((int)group.Key);
				writer.WriteBuffer(group.Value.Write);
			}
		}

		public virtual void OnRead(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			isEventMode = reader.ReadBoolean();
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				AdvLayerSettingData.LayerType key = (AdvLayerSettingData.LayerType)reader.ReadInt32();
				reader.ReadBuffer(Groups[key].Read);
			}
		}
	}
}
