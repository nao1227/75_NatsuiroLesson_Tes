using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public class AdvGraphicGroup
	{
		protected AdvGraphicManager manager;

		private Dictionary<string, AdvGraphicLayer> layers = new Dictionary<string, AdvGraphicLayer>();

		private const int Version = 0;

		internal AdvLayerSettingData.LayerType Type { get; private set; }

		internal AdvGraphicLayer DefaultLayer { get; set; }

		internal bool IsLoading
		{
			get
			{
				foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
				{
					if (layer.Value.IsLoading)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal bool IsFading
		{
			get
			{
				foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
				{
					if (layer.Value.IsFading)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal AdvGraphicGroup(AdvLayerSettingData.LayerType type, AdvLayerSetting setting, AdvGraphicManager manager)
		{
			Type = type;
			this.manager = manager;
			foreach (AdvGraphicLayer layer in manager.LayerList)
			{
				if (layer.LayerType == type)
				{
					layer.Init(manager);
					AddLayer(layer.name, layer);
				}
			}
			foreach (AdvLayerSettingData item in setting.List)
			{
				if (item.Type == type)
				{
					GameObject gameObject = new GameObject(item.Name, typeof(RectTransform), typeof(Canvas));
					manager.transform.AddChild(gameObject);
					AdvGraphicLayerDefault advGraphicLayerDefault = gameObject.AddComponent<AdvGraphicLayerDefault>();
					advGraphicLayerDefault.Init(manager);
					advGraphicLayerDefault.Init(item);
					AddLayer(item.Name, advGraphicLayerDefault);
				}
			}
		}

		private void AddLayer(string name, AdvGraphicLayer layer)
		{
			if (layers.ContainsKey(name))
			{
				Debug.LogError(name + " is already exists in layers");
				return;
			}
			layers.Add(name, layer);
			if (layer.SettingData.IsDefault)
			{
				DefaultLayer = layer;
			}
		}

		internal void EmbedLayer(AdvGraphicLayer layer)
		{
			layer.Init(manager);
			string name = layer.gameObject.name;
			if (layers.ContainsKey(name))
			{
				layers[name] = layer;
			}
			else
			{
				AddLayer(name, layer);
			}
		}

		internal void RemoveLayer(AdvGraphicLayer layer)
		{
			string name = layer.gameObject.name;
			if (layers.ContainsKey(name))
			{
				layers.Remove(name);
			}
			else
			{
				Debug.LogError(name + " is not find");
			}
		}

		internal virtual void Clear()
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.Clear();
			}
		}

		internal void DestroyAll()
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				AdvGraphicLayer value = layer.Value;
				value.Clear();
				if (value is AdvGraphicLayerDefault)
				{
					Object.Destroy(layer.Value.gameObject);
				}
			}
			layers.Clear();
			DefaultLayer = null;
		}

		public virtual AdvGraphicObject Draw(string layerName, string name, AdvGraphicOperationArg arg)
		{
			return FindLayerOrDefault(layerName).Draw(name, arg);
		}

		public virtual AdvGraphicObject DrawToDefault(string name, AdvGraphicOperationArg arg)
		{
			return DefaultLayer.DrawToDefault(name, arg);
		}

		internal AdvGraphicObject DrawCharacter(string layerName, string name, AdvGraphicOperationArg arg)
		{
			AdvGraphicLayer advGraphicLayer = null;
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				if (layer.Value.IsEqualDefaultGraphicName(name))
				{
					advGraphicLayer = layer.Value;
					break;
				}
			}
			AdvGraphicLayer advGraphicLayer2 = FindLayer(layerName);
			if (advGraphicLayer2 == null)
			{
				advGraphicLayer2 = ((advGraphicLayer == null) ? DefaultLayer : advGraphicLayer);
			}
			if (!(advGraphicLayer != advGraphicLayer2) || !(advGraphicLayer != null))
			{
				return advGraphicLayer2.DrawToDefault(name, arg);
			}
			Vector3 localScale = Vector3.one;
			Vector3 localPosition = Vector3.zero;
			Quaternion localRotation = Quaternion.identity;
			if (advGraphicLayer.CurrentGraphics.TryGetValue(name, out var value))
			{
				localScale = value.rectTransform.localScale;
				localPosition = value.rectTransform.localPosition;
				localRotation = value.rectTransform.localRotation;
				advGraphicLayer.FadeOut(name, arg.GetSkippedFadeTime(manager.Engine));
			}
			AdvGraphicObject advGraphicObject = advGraphicLayer2.DrawToDefault(name, arg);
			if (!manager.ResetCharacterTransformOnChangeLayer)
			{
				advGraphicObject.rectTransform.localScale = localScale;
				advGraphicObject.rectTransform.localPosition = localPosition;
				advGraphicObject.rectTransform.localRotation = localRotation;
			}
			return advGraphicObject;
		}

		internal List<AdvGraphicLayer> AllGraphicsLayers()
		{
			List<AdvGraphicLayer> list = new List<AdvGraphicLayer>();
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				if (layer.Value.CurrentGraphics.Count > 0)
				{
					list.Add(layer.Value);
				}
			}
			return list;
		}

		internal virtual void FadeOut(string name, float fadeTime)
		{
			AdvGraphicLayer advGraphicLayer = FindLayerFromObjectName(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOut(name, fadeTime);
			}
		}

		internal virtual void FadeOutAll(float fadeTime)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.FadeOutAll(fadeTime);
			}
		}

		internal void FadeOutParticle(string targetName, AdvParticleStopType stopType)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.FadeOutParticle(targetName, stopType);
			}
		}

		internal void FadeOutAllParticle(AdvParticleStopType stopType)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.FadeOutAllParticle(stopType);
			}
		}

		public AdvGraphicObject FindParticle(string targetName)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				AdvGraphicObject advGraphicObject = layer.Value.FindParticle(targetName);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		internal bool IsContians(string layerName, string name)
		{
			if (string.IsNullOrEmpty(layerName))
			{
				return FindObject(name) != null;
			}
			AdvGraphicLayer advGraphicLayer = FindLayer(layerName);
			if (advGraphicLayer != null)
			{
				return advGraphicLayer.Find(name) != null;
			}
			return false;
		}

		internal AdvGraphicLayer FindLayerFromObjectName(string name)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				if (layer.Value.Contains(name))
				{
					return layer.Value;
				}
			}
			return null;
		}

		internal AdvGraphicLayer FindLayer(string name)
		{
			if (layers.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}

		internal AdvGraphicLayer FindLayerOrDefault(string name)
		{
			AdvGraphicLayer advGraphicLayer = FindLayer(name);
			if (advGraphicLayer == null)
			{
				return DefaultLayer;
			}
			return advGraphicLayer;
		}

		internal AdvGraphicObject FindObject(string name)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				AdvGraphicObject advGraphicObject = layer.Value.Find(name);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		public AdvGraphicObject FindFadeOutingObject(string name)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				AdvGraphicObject advGraphicObject = layer.Value.FindFadeOutingObject(name);
				if (advGraphicObject != null)
				{
					return advGraphicObject;
				}
			}
			return null;
		}

		internal List<AdvGraphicObject> AllGraphics()
		{
			List<AdvGraphicObject> list = new List<AdvGraphicObject>();
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.AddAllGraphics(list);
			}
			return list;
		}

		internal void AddAllGraphics(List<AdvGraphicObject> graphics)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.AddAllGraphics(graphics);
			}
		}

		internal void SkipFade()
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.SkipFade();
			}
		}

		internal void FadeOutAllObjects(AdvGraphicObjectType objectType, float fadeTime)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.FadeOutAllObjects(objectType, fadeTime);
			}
		}

		public bool IsFadingObjects(AdvGraphicObjectType objectType)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				if (layer.Value.IsFadingObjects(objectType))
				{
					return true;
				}
			}
			return false;
		}

		public void SkipFadeObjects(AdvGraphicObjectType objectType)
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.SkipFadeObjects(objectType);
			}
		}

		public void ResetAllLayerRectTransform()
		{
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				layer.Value.ResetCanvasRectTransform();
			}
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.Write(layers.Count);
			foreach (KeyValuePair<string, AdvGraphicLayer> layer in layers)
			{
				writer.Write(layer.Key);
				writer.WriteBuffer(layer.Value.Write);
			}
		}

		public void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string name = reader.ReadString();
				AdvGraphicLayer advGraphicLayer = FindLayer(name);
				if (advGraphicLayer != null)
				{
					reader.ReadBuffer(advGraphicLayer.Read);
				}
				else
				{
					reader.SkipBuffer();
				}
			}
		}
	}
}
