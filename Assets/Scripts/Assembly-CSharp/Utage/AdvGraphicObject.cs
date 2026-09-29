using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/AdvGraphicObject")]
	[RequireComponent(typeof(RectTransform))]
	public class AdvGraphicObject : MonoBehaviour, IAdvFadeSkippable, IAdvFade
	{
		private AdvGraphicLoader loader;

		protected AdvGraphicLayer layer;

		private AdvEffectColor effectColor;

		private readonly List<AdvGraphicObject> swapFadeObjects = new List<AdvGraphicObject>();

		private const int Version = 2;

		private const int Version1 = 1;

		private const int Version0 = 0;

		public AdvGraphicLoader Loader => this.GetComponentCacheCreateIfMissing(ref loader);

		public AdvGraphicLayer Layer
		{
			get
			{
				return layer;
			}
			internal set
			{
				layer = value;
			}
		}

		public AdvEngine Engine => Layer.Manager.Engine;

		public AdvGraphicInfo LastResource { get; private set; }

		public float PixelsToUnits => Layer.Manager.PixelsToUnits;

		public bool EnableRenderTexture
		{
			get
			{
				if (LastResource != null)
				{
					return LastResource.RenderTextureSetting.EnableRenderTexture;
				}
				return false;
			}
		}

		public AdvGraphicBase TargetObject { get; private set; }

		public AdvGraphicBase RenderObject { get; private set; }

		public AdvRenderTextureSpace RenderTextureSpace { get; private set; }

		private Timer FadeTimer { get; set; }

		public AdvEffectColor EffectColor => this.GetComponentCacheCreateIfMissing(ref effectColor);

		public RectTransform rectTransform { get; private set; }

		internal bool IsFading
		{
			get
			{
				if (TargetObject is IAdvCrossFadeImageObject advCrossFadeImageObject && advCrossFadeImageObject.IsCrossFading)
				{
					return true;
				}
				swapFadeObjects.RemoveAll((AdvGraphicObject x) => x == null);
				if (!FadeTimer.IsPlaying)
				{
					return swapFadeObjects.Count > 0;
				}
				return true;
			}
		}

		public bool CheckType(AdvGraphicObjectType type)
		{
			if (LastResource == null)
			{
				return false;
			}
			switch (type)
			{
			case AdvGraphicObjectType.Character:
				return LastResource.SettingData is AdvCharacterSettingData;
			case AdvGraphicObjectType.Bg:
				if (!(LastResource.SettingData is AdvTextureSettingData advTextureSettingData2))
				{
					return false;
				}
				if (advTextureSettingData2.TextureType != AdvTextureSettingData.Type.Bg)
				{
					return advTextureSettingData2.TextureType == AdvTextureSettingData.Type.Sprite;
				}
				return true;
			default:
				if (!(LastResource.SettingData is AdvTextureSettingData advTextureSettingData))
				{
					return false;
				}
				return advTextureSettingData.TextureType == AdvTextureSettingData.Type.Sprite;
			}
		}

		public virtual void Init(AdvGraphicLayer layer, AdvGraphicInfo graphic)
		{
			this.layer = layer;
			rectTransform = base.transform as RectTransform;
			rectTransform.SetStretch();
			rectTransform.pivot = graphic.Pivot0;
			if (graphic.RenderTextureSetting.EnableRenderTexture)
			{
				InitRenderTextureImage(graphic);
			}
			else
			{
				if (graphic.IsOverridePrefab())
				{
					GameObject gameObject = base.transform.AddChildPrefab(graphic.File.UnityObject as GameObject);
					AdvGraphicBase targetObject = (RenderObject = gameObject.GetComponent<AdvGraphicBase>());
					TargetObject = targetObject;
				}
				else
				{
					GameObject gameObject2 = base.transform.AddChildGameObject(graphic.Key);
					AdvGraphicBase targetObject = (RenderObject = gameObject2.AddComponent(graphic.GetComponentType()) as AdvGraphicBase);
					TargetObject = targetObject;
				}
				TargetObject.Init(this);
			}
			LipSynchBase componentInChildren = TargetObject.GetComponentInChildren<LipSynchBase>();
			if (componentInChildren != null)
			{
				componentInChildren.CharacterLabel = base.gameObject.name;
				componentInChildren.OnCheckTextLipSync.AddListener(delegate(LipSynchBase x)
				{
					x.EnableTextLipSync = x.CharacterLabel == Engine.Page.CharacterLabel && Engine.Page.IsSendChar;
				});
				componentInChildren.OnCheckUpdateingText.AddListener(delegate(LipSynchBase x)
				{
					x.UpdatingText = Engine.Page.UpdatingText;
				});
			}
			FadeTimer = base.gameObject.AddComponent<Timer>();
			effectColor = this.GetComponentCreateIfMissing<AdvEffectColor>();
			effectColor.OnValueChanged.AddListener(RenderObject.OnEffectColorsChange);
		}

		private void InitRenderTextureImage(AdvGraphicInfo graphic)
		{
			AdvGraphicManager manager = Layer.Manager;
			RenderTextureSpace = manager.RenderTextureManager.CreateSpace();
			RenderTextureSpace.Init(graphic, manager.PixelsToUnits);
			AdvGraphicObjectRenderTextureImage advGraphicObjectRenderTextureImage = (AdvGraphicObjectRenderTextureImage)(RenderObject = base.transform.AddChildGameObject(graphic.Key).AddComponent<AdvGraphicObjectRenderTextureImage>());
			advGraphicObjectRenderTextureImage.Init(RenderTextureSpace);
			RenderObject.Init(this);
			if (graphic.IsOverridePrefab())
			{
				TargetObject = RenderTextureSpace.RenderRoot.transform.AddChildPrefab(graphic.File.UnityObject as GameObject).GetComponent<AdvGraphicBase>();
			}
			else
			{
				TargetObject = RenderTextureSpace.RenderRoot.transform.AddChildGameObject(graphic.Key).AddComponent(graphic.GetComponentType()) as AdvGraphicBase;
			}
			TargetObject.Init(this);
		}

		public virtual void Draw(AdvGraphicOperationArg arg, float fadeTime)
		{
			DrawSub(arg.Graphic, fadeTime);
		}

		private void DrawSub(AdvGraphicInfo graphic, float fadeTime)
		{
			TargetObject.name = graphic.File.FileName;
			TargetObject.ChangeResourceOnDraw(graphic, fadeTime);
			if (RenderObject != TargetObject)
			{
				RenderObject.ChangeResourceOnDraw(graphic, fadeTime);
				if (graphic.IsUguiComponentType)
				{
					RenderObject.Scale(graphic);
				}
			}
			else
			{
				TargetObject.Scale(graphic);
			}
			RenderObject.Alignment(Layer.SettingData.Alignment, graphic);
			RenderObject.Flip(Layer.SettingData.FlipX, Layer.SettingData.FlipY);
			LastResource = graphic;
		}

		internal virtual void SetCommandPostion(AdvCommand command)
		{
			bool flag = false;
			Vector3 localPosition = base.transform.localPosition;
			if (command.TryParseCell<float>(AdvColumnName.Arg4, out var val))
			{
				localPosition.x = val;
				flag = true;
			}
			if (command.TryParseCell<float>(AdvColumnName.Arg5, out var val2))
			{
				localPosition.y = val2;
				flag = true;
			}
			if (flag)
			{
				base.transform.localPosition = localPosition;
			}
		}

		public virtual void ChangePattern(string pattern)
		{
			if (TargetObject != null)
			{
				TargetObject.ChangePattern(pattern);
			}
		}

		public virtual bool TryFadeIn(float time)
		{
			if (TargetObject != null)
			{
				FadeIn(time);
				return true;
			}
			return false;
		}

		public void FadeIn(float fadeTime)
		{
			FadeIn(fadeTime, delegate
			{
			});
		}

		public void FadeIn(float fadeTime, Action onComplete)
		{
			float begin = 0f;
			float end = 1f;
			FadeTimer.StartTimer(fadeTime, Engine.Time.Unscaled, delegate(Timer x)
			{
				EffectColor.FadeAlpha = x.GetCurve(begin, end);
			}, delegate
			{
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public virtual void FadeOut(float time)
		{
			FadeOut(time, Clear);
		}

		public void FadeOut(float time, Action onComplete)
		{
			if (TargetObject == null)
			{
				if (onComplete != null)
				{
					onComplete();
				}
				return;
			}
			float begin = EffectColor.FadeAlpha;
			float end = 0f;
			FadeTimer.StartTimer(time, Engine.Time.Unscaled, delegate(Timer x)
			{
				EffectColor.FadeAlpha = x.GetCurve(begin, end);
			}, delegate
			{
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public void SkipFade()
		{
			if (TargetObject is IAdvCrossFadeImageObject advCrossFadeImageObject && advCrossFadeImageObject.IsCrossFading)
			{
				advCrossFadeImageObject.SkipCrossFade();
			}
			FadeTimer.SkipToEnd();
			foreach (AdvGraphicObject swapFadeObject in swapFadeObjects)
			{
				if (swapFadeObject != null)
				{
					swapFadeObject.Clear();
				}
			}
			swapFadeObjects.Clear();
		}

		public void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			if (TargetObject == null)
			{
				onComplete?.Invoke();
			}
			else
			{
				RenderObject.RuleFadeIn(engine, data, onComplete);
			}
		}

		public void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			if (TargetObject == null)
			{
				if (onComplete != null)
				{
					onComplete();
				}
				Clear();
				return;
			}
			RenderObject.RuleFadeOut(engine, data, delegate
			{
				if (onComplete != null)
				{
					onComplete();
				}
				Clear();
			});
		}

		public void SkipRuleFade()
		{
			RenderObject.SkipRuleFade();
		}

		public virtual void Clear()
		{
			RemoveFromLayer();
			base.gameObject.SetActive(value: false);
			UnityEngine.Object.Destroy(base.gameObject);
		}

		protected virtual void OnDestroy()
		{
			RemoveFromLayer();
			if ((bool)RenderTextureSpace)
			{
				UnityEngine.Object.Destroy(RenderTextureSpace.gameObject);
			}
		}

		public virtual void RemoveFromLayer()
		{
			swapFadeObjects.Clear();
			if ((bool)Layer)
			{
				Layer.Remove(this);
			}
		}

		public void AddSwapFadeObject(AdvGraphicObject swapFadeObject)
		{
			swapFadeObjects.Add(swapFadeObject);
		}

		internal void SetPivot(float pivotX, float pivotY, float offsetX, float offsetY, AdvGraphicObjectPivotType pivotType)
		{
			if (!(TargetObject == null))
			{
				if (pivotType == AdvGraphicObjectPivotType.Direct)
				{
					rectTransform.SetPivotKeepRect(new Vector2(pivotX, pivotY));
					return;
				}
				Vector3 pivotTargetWorldPoint = GetPivotTargetWorldPoint(pivotX, pivotY, offsetX, offsetY, pivotType);
				Vector2 pivot = rectTransform.WorldPointToPivot(pivotTargetWorldPoint);
				rectTransform.SetPivotKeepRect(pivot);
			}
		}

		private Vector3 GetPivotTargetWorldPoint(float pivotX, float pivotY, float offsetX, float offsetY, AdvGraphicObjectPivotType pivotType)
		{
			if (pivotType == AdvGraphicObjectPivotType.WorldSpace)
			{
				return GetPivotTargetInWorldSpace(pivotX, pivotY, offsetX, offsetY);
			}
			return GetPivotTargetInSpriteSpace(pivotX, pivotY, offsetX, offsetY, pivotType);
		}

		private Vector3 GetPivotTargetInWorldSpace(float pivotX, float pivotY, float offsetX, float offsetY)
		{
			Camera camera = Engine.CameraManager.FindCameraByLayer(layer.Canvas.gameObject.layer);
			if (camera == null)
			{
				Debug.LogError("Cant find camera");
				camera = Engine.CameraManager.FindCameraByLayer(0);
			}
			LetterBoxCamera component = camera.GetComponent<LetterBoxCamera>();
			Vector3 vector = new Vector3((pivotX - 0.5f) * (float)component.Width + offsetX, (pivotY - 0.5f) * (float)component.Height + offsetY, 0f);
			vector /= (float)component.PixelsToUnits;
			return camera.transform.position + vector;
		}

		private Vector3 GetPivotTargetInSpriteSpace(float pivotX, float pivotY, float offsetX, float offsetY, AdvGraphicObjectPivotType pivotType)
		{
			Transform transform = TargetObject.transform;
			RectTransform rectTransform = transform as RectTransform;
			if (rectTransform == null)
			{
				Debug.LogError(base.gameObject.name + "is not RectTransform type");
				return Vector3.zero;
			}
			Vector3 localPoint = rectTransform.PivotToLocalPoint(new Vector2(pivotX, pivotY));
			switch (pivotType)
			{
			case AdvGraphicObjectPivotType.SpritePos:
			{
				Vector2 vector = rectTransform.localScale;
				if (Mathf.Approximately(0f, vector.x))
				{
					vector.x = 1f;
				}
				if (Mathf.Approximately(0f, vector.y))
				{
					vector.y = 1f;
				}
				localPoint.x += offsetX / vector.x;
				localPoint.y += offsetY / vector.y;
				break;
			}
			case AdvGraphicObjectPivotType.SpritePosLocal:
				localPoint.x += offsetX;
				localPoint.y += offsetY;
				break;
			default:
				Debug.LogError(pivotType.ToString() + " is Failed");
				break;
			case AdvGraphicObjectPivotType.SpritePosNoSize:
				break;
			}
			Vector3 result = transform.LocalPointToWorldPoint(localPoint);
			if (pivotType == AdvGraphicObjectPivotType.SpritePosNoSize)
			{
				result.x += offsetX / Layer.Manager.PixelsToUnits;
				result.y += offsetY / Layer.Manager.PixelsToUnits;
			}
			return result;
		}

		public void ResetPivot()
		{
			if (LastResource != null)
			{
				rectTransform.SetPivotKeepRect(LastResource.Pivot0);
			}
		}

		public virtual void ChangePatternAnimation(string paraString)
		{
			if (LastResource == null)
			{
				Debug.LogError("ChangePatternAnimationError  LastResource is null");
				return;
			}
			if (!(LastResource.SettingData is AdvCharacterSettingData advCharacterSettingData))
			{
				Debug.LogError("ChangePatternAnimationError  characterData is null");
				return;
			}
			string[] array = paraString.Split(',');
			if (array.Length == 0)
			{
				Debug.LogError("ChangePatternAnimationError  argString = " + paraString);
				return;
			}
			float val;
			if (array.Length == 1)
			{
				val = 0f;
			}
			else if (!WrapperUnityVersion.TryParseFloatGlobal(array[1], out val))
			{
				Debug.LogError("ChangePatternAnimationError  " + array[1] + " is not float string");
				return;
			}
			val = Mathf.Max(val, 0.001f);
			string patternLabel = array[0];
			AdvCharacterSettingData characterData = Engine.DataManager.SettingDataManager.CharacterSetting.GetCharacterData(advCharacterSettingData.Name, patternLabel);
			if (characterData == null)
			{
				Debug.LogError("ChangePatternAnimationError  pattern is not pattern name");
				return;
			}
			AdvGraphicInfo main = characterData.Graphic.Main;
			DrawSub(main, Engine.Page.ToSkippedTime(val));
			if (!string.IsNullOrEmpty(main.AnimationState))
			{
				TargetObject.ChangeAnimationState(main.AnimationState, val);
			}
		}

		public virtual bool EnableSaveObject()
		{
			if (LastResource == null)
			{
				return false;
			}
			if (LastResource.DataType == "Capture")
			{
				return false;
			}
			AdvGraphicObjectParticle advGraphicObjectParticle = TargetObject as AdvGraphicObjectParticle;
			if (advGraphicObjectParticle != null)
			{
				return advGraphicObjectParticle.EnableSave();
			}
			return true;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(2);
			writer.WriteRectTransfom(rectTransform);
			writer.WriteBuffer(EffectColor.Write);
			writer.WriteBuffer(delegate(BinaryWriter x)
			{
				AdvITweenPlayer.WriteSaveData(x, base.gameObject);
			});
			writer.WriteBuffer(delegate(BinaryWriter x)
			{
				AdvAnimationPlayer.WriteSaveData(x, base.gameObject);
			});
			writer.WriteBuffer(delegate(BinaryWriter x)
			{
				TargetObject.Write(x);
			});
		}

		public void Read(byte[] buffer, AdvGraphicInfo graphic)
		{
			TargetObject.gameObject.SetActive(value: false);
			Loader.LoadGraphic(graphic, delegate
			{
				TargetObject.gameObject.SetActive(value: true);
				SetGraphicOnSaveDataRead(graphic);
				BinaryUtil.BinaryRead(buffer, Read);
			});
		}

		private void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			switch (num)
			{
			default:
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			case 0:
			case 1:
				reader.ReadLocalTransform(base.transform);
				break;
			case 2:
				reader.ReadRectTransfom(rectTransform);
				break;
			}
			reader.ReadBuffer(EffectColor.Read);
			reader.ReadBuffer(delegate(BinaryReader x)
			{
				AdvITweenPlayer.ReadSaveData(x, base.gameObject, isUnder2DSpace: true, PixelsToUnits, Engine.Time.Unscaled);
			});
			reader.ReadBuffer(delegate(BinaryReader x)
			{
				AdvAnimationPlayer.ReadSaveData(x, base.gameObject, Engine);
			});
			if (num > 0)
			{
				reader.ReadBuffer(delegate(BinaryReader x)
				{
					TargetObject.Read(x);
				});
			}
		}

		internal void InitCaptureImage(AdvGraphicInfo grapic, Camera cachedCamera)
		{
			LastResource = grapic;
			base.gameObject.GetComponentInChildren<AdvGraphicObjectRawImage>().CaptureCamera(cachedCamera);
		}

		private void SetGraphicOnSaveDataRead(AdvGraphicInfo graphic)
		{
			DrawSub(graphic, 0f);
		}
	}
}
