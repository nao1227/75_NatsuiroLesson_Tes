using System;
using UnityEngine;
using UnityEngine.UI;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/Internal/GraphicObject/AdvGraphicObjectRenderTextureImage")]
	public class AdvGraphicObjectRenderTextureImage : AdvGraphicObjectUguiBase, IAdvCrossFadeImageObject
	{
		private RenderTexture copyTemporary;

		protected override Material Material
		{
			get
			{
				return RawImage.material;
			}
			set
			{
				RawImage.material = value;
			}
		}

		public AdvRenderTextureSpace RenderTextureSpace { get; private set; }

		private UguiCrossFadeRawImage CrossFade { get; set; }

		private RawImage RawImage { get; set; }

		public bool IsCrossFading
		{
			get
			{
				if (CrossFade == null)
				{
					return false;
				}
				return true;
			}
		}

		private void ReleaseTemporary()
		{
			if (copyTemporary != null)
			{
				RenderTexture.ReleaseTemporary(copyTemporary);
				copyTemporary = null;
			}
			if (CrossFade != null)
			{
				CrossFade.RemoveComponentMySelf();
				CrossFade = null;
			}
		}

		private void OnDestroy()
		{
			if (copyTemporary != null)
			{
				RenderTexture.ReleaseTemporary(copyTemporary);
				copyTemporary = null;
			}
		}

		protected override void AddGraphicComponentOnInit()
		{
		}

		internal void Init(AdvRenderTextureSpace renderTextureSpace)
		{
			RenderTextureSpace = renderTextureSpace;
			RawImage = base.gameObject.GetComponentCreateIfMissing<RawImage>();
			if (renderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image)
			{
				Material = new Material(ShaderManager.DrawByRenderTexture);
			}
			RawImage.texture = RenderTextureSpace.RenderTexture;
			RawImage.SetNativeSize();
			RawImage.rectTransform.localScale = Vector3.one;
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo graphic)
		{
			return false;
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo graphic, float fadeTime)
		{
			bool num = TryCreateCrossFadeImage(fadeTime, graphic);
			if (!num)
			{
				ReleaseTemporary();
			}
			RawImage.texture = RenderTextureSpace.RenderTexture;
			AdvRenderTextureSetting setting = RenderTextureSpace.Setting;
			RawImage.rectTransform.SetWidth(setting.RenderTextureSize.x / setting.RenderTextureScale);
			RawImage.rectTransform.SetHeight(setting.RenderTextureSize.y / setting.RenderTextureScale);
			if (!num && base.LastResource == null)
			{
				base.ParentObject.FadeIn(fadeTime);
			}
		}

		public override void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			UguiTransition transition = base.gameObject.AddComponent<UguiTransition>();
			transition.UnscaledTime = base.Engine.Time.Unscaled;
			transition.RuleFadeIn(engine.EffectManager.FindRuleTexture(data.TextureName), data.Vague, RenderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image, data.GetSkippedTime(engine), delegate
			{
				transition.RemoveComponentMySelf(immediate: false);
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		public override void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			UguiTransition transition = base.gameObject.AddComponent<UguiTransition>();
			transition.UnscaledTime = base.Engine.Time.Unscaled;
			transition.RuleFadeOut(engine.EffectManager.FindRuleTexture(data.TextureName), data.Vague, RenderTextureSpace.RenderTextureType == AdvRenderTextureMode.Image, data.GetSkippedTime(engine), delegate
			{
				transition.RemoveComponentMySelf(immediate: false);
				RawImage.SetAlpha(0f);
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}

		protected bool TryCreateCrossFadeImage(float time, AdvGraphicInfo graphic)
		{
			if (base.LastResource == null)
			{
				return false;
			}
			if (RawImage.texture == null)
			{
				return false;
			}
			ReleaseTemporary();
			Material material = Material;
			copyTemporary = RenderTextureSpace.RenderTexture.CreateCopyTemporary(0);
			CrossFade = base.gameObject.AddComponent<UguiCrossFadeRawImage>();
			CrossFade.Timer.Unscaled = base.Engine.Time.Unscaled;
			CrossFade.Material = material;
			CrossFade.CrossFade(copyTemporary, time, delegate
			{
				ReleaseTemporary();
			});
			return true;
		}

		public void RestartCrossFade(float fadeTime, Action onComplete)
		{
			if (CrossFade == null)
			{
				Debug.LogError("CrossFadeComponent is not found", this);
				return;
			}
			CrossFade.Restart(fadeTime, delegate
			{
				ReleaseTemporary();
				onComplete();
			});
		}

		public void SkipCrossFade()
		{
			if (CrossFade == null)
			{
				Debug.LogError("CrossFadeComponent is not found", this);
			}
			else
			{
				CrossFade.Timer.SkipToEnd();
			}
		}
	}
}
