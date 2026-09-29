using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public abstract class AdvGraphicLayer : MonoBehaviour, IAdvGraphicLayer
	{
		private Dictionary<string, AdvGraphicObject> currentGraphics = new Dictionary<string, AdvGraphicObject>();

		private List<AdvGraphicObject> fadeOutingObjets = new List<AdvGraphicObject>();

		private const int Version = 0;

		public AdvLayerSettingData SettingData { get; protected set; }

		internal abstract AdvLayerSettingData.LayerType LayerType { get; }

		public AdvEngine Engine => Manager.Engine;

		public AdvGraphicManager Manager { get; protected set; }

		protected Transform RootObjects { get; set; }

		public AdvGraphicObject DefaultObject { get; protected set; }

		public Dictionary<string, AdvGraphicObject> CurrentGraphics => currentGraphics;

		public Canvas Canvas { get; protected set; }

		internal bool IsLoading
		{
			get
			{
				foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in currentGraphics)
				{
					if (currentGraphic.Value == null)
					{
						Debug.LogError("");
					}
					if (currentGraphic.Value.Loader.IsLoading)
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
				foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in currentGraphics)
				{
					if (currentGraphic.Value.IsFading)
					{
						return true;
					}
				}
				fadeOutingObjets.RemoveAll((AdvGraphicObject x) => x == null);
				return fadeOutingObjets.Count > 0;
			}
		}

		internal abstract void Init(AdvGraphicManager manager);

		internal abstract void ResetCanvasRectTransform();

		internal void Add(AdvGraphicObject obj)
		{
			CurrentGraphics.Add(obj.name, obj);
		}

		internal void Remove(AdvGraphicObject obj)
		{
			if (CurrentGraphics.ContainsValue(obj))
			{
				CurrentGraphics.Remove(obj.name);
			}
			if (DefaultObject == obj)
			{
				DefaultObject = null;
			}
		}

		internal AdvGraphicObject Draw(string name, AdvGraphicOperationArg arg)
		{
			AdvGraphicObject obj = GetObjectCreateIfMissing(name, arg.Graphic);
			obj.Loader.LoadGraphic(arg.Graphic, delegate
			{
				obj.Draw(arg, arg.GetSkippedFadeTime(Engine));
			});
			return obj;
		}

		internal AdvGraphicObject DrawToDefault(string name, AdvGraphicOperationArg arg)
		{
			bool flag = false;
			bool flag2 = false;
			Vector3 localPosition = Vector3.zero;
			if (DefaultObject != null && DefaultObject.LastResource != null)
			{
				if (DefaultObject.name != name)
				{
					flag = true;
				}
				else if (CheckFailedCrossFade(arg))
				{
					flag = true;
					flag2 = true;
					localPosition = DefaultObject.transform.localPosition;
				}
				else
				{
					flag = false;
				}
			}
			AdvGraphicObject advGraphicObject = null;
			if (flag)
			{
				advGraphicObject = DefaultObject;
				Remove(DefaultObject);
			}
			DefaultObject = Draw(name, arg);
			if (flag)
			{
				DefaultObject.AddSwapFadeObject(advGraphicObject);
				float skippedFadeTime = arg.GetSkippedFadeTime(Engine);
				if (LayerType == AdvLayerSettingData.LayerType.Bg)
				{
					StartCoroutine(CoDelayOut(advGraphicObject, skippedFadeTime));
				}
				else
				{
					advGraphicObject.FadeOut(skippedFadeTime);
				}
			}
			if (flag2 && !Manager.IgnoreKeepPositionOnCrossFade)
			{
				DefaultObject.transform.localPosition = localPosition;
			}
			return DefaultObject;
		}

		private IEnumerator CoDelayOut(AdvGraphicObject obj, float delay)
		{
			yield return Engine.Time.WaitForSeconds(delay);
			if (obj != null)
			{
				obj.Clear();
			}
		}

		protected virtual bool CheckFailedCrossFade(AdvGraphicOperationArg arg)
		{
			if (arg.Graphic.CheckFailedCrossFade(DefaultObject.LastResource))
			{
				return true;
			}
			return DefaultObject.TargetObject.CheckFailedCrossFade(arg.Graphic);
		}

		internal AdvGraphicObject GetObjectCreateIfMissing(string name, AdvGraphicInfo grapic)
		{
			if (grapic == null)
			{
				Debug.LogError(name + " grapic is null");
				return null;
			}
			if (!currentGraphics.TryGetValue(name, out var value))
			{
				return CreateObject(name, grapic);
			}
			return value;
		}

		protected virtual AdvGraphicObject CreateObject(string name, AdvGraphicInfo grapic, bool resetOnFirst = true)
		{
			AdvGraphicObject advGraphicObject;
			if (grapic.TryGetAdvGraphicObjectPrefab(out var prefab))
			{
				GameObject obj = Object.Instantiate(prefab);
				obj.name = name;
				advGraphicObject = obj.GetComponent<AdvGraphicObject>();
				RootObjects.AddChild(advGraphicObject.gameObject);
			}
			else
			{
				advGraphicObject = RootObjects.AddChildGameObjectComponent<AdvGraphicObject>(name);
			}
			advGraphicObject.Init(this, grapic);
			if (resetOnFirst && currentGraphics.Count == 0)
			{
				ResetCanvasRectTransform();
			}
			Add(advGraphicObject);
			return advGraphicObject;
		}

		public void ChangeLayer(bool isDefaultObject, AdvGraphicObject targetObject, AdvChangeLayerRepositionType repositionType, float fadeOutTime)
		{
			if (isDefaultObject)
			{
				if (DefaultObject != null)
				{
					FadeOut(DefaultObject.name, fadeOutTime);
				}
				DefaultObject = targetObject;
			}
			Transform transform = targetObject.transform;
			switch (repositionType)
			{
			case AdvChangeLayerRepositionType.KeepLocal:
				targetObject.transform.SetParent(base.transform, worldPositionStays: false);
				break;
			case AdvChangeLayerRepositionType.ResetLocal:
				targetObject.transform.SetParent(base.transform);
				transform.localPosition = Vector3.zero;
				transform.localEulerAngles = Vector3.zero;
				transform.localScale = Vector3.one;
				break;
			default:
				targetObject.transform.SetParent(base.transform);
				break;
			}
			targetObject.gameObject.layer = base.gameObject.layer;
			targetObject.Layer = this;
			Add(targetObject);
		}

		internal void FadeOut(string name, float fadeTime)
		{
			if (currentGraphics.TryGetValue(name, out var value))
			{
				value.FadeOut(fadeTime);
				fadeOutingObjets.Add(value);
				Remove(value);
			}
		}

		internal void FadeOutAll(float fadeTime)
		{
			foreach (AdvGraphicObject item in new List<AdvGraphicObject>(currentGraphics.Values))
			{
				item.FadeOut(fadeTime);
				fadeOutingObjets.Add(item);
			}
			currentGraphics.Clear();
			DefaultObject = null;
		}

		internal void FadeOutParticle(string targetName, AdvParticleStopType stopType)
		{
			if (currentGraphics.TryGetValue(targetName, out var value))
			{
				FadOutParticle(value, stopType);
			}
		}

		internal void FadeOutAllParticle(AdvParticleStopType stopType)
		{
			foreach (AdvGraphicObject item in new List<AdvGraphicObject>(currentGraphics.Values))
			{
				FadOutParticle(item, stopType);
			}
		}

		private void FadOutParticle(AdvGraphicObject obj, AdvParticleStopType stopType)
		{
			AdvGraphicObjectParticle advGraphicObjectParticle = obj.TargetObject as AdvGraphicObjectParticle;
			if (advGraphicObjectParticle != null)
			{
				advGraphicObjectParticle.Stop(stopType);
				fadeOutingObjets.Add(obj);
				Remove(obj);
			}
		}

		public AdvGraphicObject FindParticle(string targetName)
		{
			AdvGraphicObject advGraphicObject = Find(targetName);
			if (advGraphicObject != null && advGraphicObject.TargetObject is AdvGraphicObjectParticle)
			{
				return advGraphicObject;
			}
			return null;
		}

		internal void Clear()
		{
			foreach (AdvGraphicObject item in new List<AdvGraphicObject>(currentGraphics.Values))
			{
				item.Clear();
			}
			currentGraphics.Clear();
			foreach (AdvGraphicObject fadeOutingObjet in fadeOutingObjets)
			{
				if (fadeOutingObjet != null)
				{
					fadeOutingObjet.Clear();
				}
			}
			DefaultObject = null;
		}

		internal bool IsEqualDefaultGraphicName(string name)
		{
			if (DefaultObject != null)
			{
				return DefaultObject.name == name;
			}
			return false;
		}

		internal bool Contains(string name)
		{
			return currentGraphics.ContainsKey(name);
		}

		internal AdvGraphicObject Find(string name)
		{
			if (currentGraphics.TryGetValue(name, out var value))
			{
				return value;
			}
			return null;
		}

		internal AdvGraphicObject FindFadeOutingObject(string name)
		{
			foreach (AdvGraphicObject fadeOutingObjet in fadeOutingObjets)
			{
				if (fadeOutingObjet != null && fadeOutingObjet.name == name)
				{
					return fadeOutingObjet;
				}
			}
			return null;
		}

		internal void AddAllGraphics(List<AdvGraphicObject> graphics)
		{
			graphics.AddRange(currentGraphics.Values);
		}

		internal void SkipFade()
		{
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in currentGraphics)
			{
				currentGraphic.Value.SkipFade();
			}
			foreach (AdvGraphicObject fadeOutingObjet in fadeOutingObjets)
			{
				if (fadeOutingObjet != null)
				{
					fadeOutingObjet.Clear();
				}
			}
			fadeOutingObjets.Clear();
		}

		public void FadeOutAllObjects(AdvGraphicObjectType objectType, float fadeTime)
		{
			foreach (AdvGraphicObject item in new List<AdvGraphicObject>(currentGraphics.Values))
			{
				if (item.CheckType(objectType))
				{
					item.FadeOut(fadeTime);
					fadeOutingObjets.Add(item);
					Remove(item);
				}
			}
		}

		public bool IsFadingObjects(AdvGraphicObjectType objectType)
		{
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in currentGraphics)
			{
				AdvGraphicObject value = currentGraphic.Value;
				if (value.CheckType(objectType) && value.IsFading)
				{
					return true;
				}
			}
			fadeOutingObjets.RemoveAll((AdvGraphicObject x) => x == null);
			return fadeOutingObjets.Exists((AdvGraphicObject x) => x.CheckType(objectType));
		}

		public void SkipFadeObjects(AdvGraphicObjectType objectType)
		{
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in currentGraphics)
			{
				if (currentGraphic.Value.CheckType(objectType))
				{
					currentGraphic.Value.SkipFade();
				}
			}
			foreach (AdvGraphicObject fadeOutingObjet in fadeOutingObjets)
			{
				if (fadeOutingObjet != null && fadeOutingObjet.CheckType(objectType))
				{
					fadeOutingObjet.Clear();
				}
			}
		}

		public virtual void Write(BinaryWriter writer)
		{
			writer.Write(0);
			writer.WriteLocalTransform(base.transform);
			int num = 0;
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic in CurrentGraphics)
			{
				if (currentGraphic.Value.EnableSaveObject())
				{
					num++;
				}
			}
			writer.Write(num);
			foreach (KeyValuePair<string, AdvGraphicObject> currentGraphic2 in CurrentGraphics)
			{
				if (currentGraphic2.Value.EnableSaveObject())
				{
					writer.Write(currentGraphic2.Key);
					writer.WriteBuffer(currentGraphic2.Value.LastResource.OnWrite);
					writer.WriteBuffer(currentGraphic2.Value.Write);
				}
			}
			writer.Write((DefaultObject == null) ? "" : DefaultObject.name);
		}

		public virtual void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 0)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			reader.ReadLocalTransform(base.transform);
			int num2 = reader.ReadInt32();
			for (int i = 0; i < num2; i++)
			{
				string text = reader.ReadString();
				AdvGraphicInfo graphic = null;
				reader.ReadBuffer(delegate(BinaryReader x)
				{
					graphic = AdvGraphicInfo.ReadGraphicInfo(Engine, x);
				});
				byte[] buffer = reader.ReadBuffer();
				CreateObject(text, graphic, resetOnFirst: false).Read(buffer, graphic);
			}
			string text2 = reader.ReadString();
			DefaultObject = Find(text2);
		}
	}
}
