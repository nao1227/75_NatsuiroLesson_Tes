using System;
using System.IO;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	public abstract class AdvGraphicObjectPrefabBase : AdvGraphicBase
	{
		private enum SaveType
		{
			Animator = 0,
			Other = 1
		}

		protected GameObject currentObject;

		private Animator animator;

		private const int Version = 1;

		private string AnimationStateName { get; set; }

		public override void Init(AdvGraphicObject parentObject)
		{
			AnimationStateName = "";
			base.Init(parentObject);
		}

		internal override bool CheckFailedCrossFade(AdvGraphicInfo grapic)
		{
			if (IsOnlyChangeAnimationState(grapic))
			{
				return false;
			}
			return true;
		}

		internal override void ChangeResourceOnDraw(AdvGraphicInfo grapic, float fadeTime)
		{
			if (!IsOnlyChangeAnimationState(grapic))
			{
				currentObject = UnityEngine.Object.Instantiate(grapic.File.UnityObject) as GameObject;
				Vector3 localPosition = currentObject.transform.localPosition;
				Vector3 localEulerAngles = currentObject.transform.localEulerAngles;
				Vector3 localScale = currentObject.transform.localScale;
				currentObject.transform.SetParent(base.transform);
				currentObject.transform.localPosition = localPosition;
				currentObject.transform.localScale = localScale;
				currentObject.transform.localEulerAngles = localEulerAngles;
				currentObject.ChangeLayerDeep(base.gameObject.layer);
				currentObject.gameObject.SetActive(value: true);
				animator = GetComponentInChildren<Animator>();
				ChangeResourceOnDrawSub(grapic);
			}
			if (base.LastResource == null)
			{
				base.ParentObject.FadeIn(fadeTime);
			}
		}

		protected bool IsOnlyChangeAnimationState(AdvGraphicInfo grapic)
		{
			if (base.LastResource == null)
			{
				return false;
			}
			if (base.LastResource == grapic)
			{
				return true;
			}
			if (base.LastResource.File == grapic.File && base.LastResource.AnimationState != grapic.AnimationState)
			{
				return true;
			}
			return false;
		}

		protected abstract void ChangeResourceOnDrawSub(AdvGraphicInfo grapic);

		internal override void Scale(AdvGraphicInfo graphic)
		{
			base.transform.localScale = graphic.Scale * base.Layer.Manager.PixelsToUnits;
		}

		internal override void Alignment(Alignment alignment, AdvGraphicInfo graphic)
		{
			base.transform.localPosition = graphic.Position;
		}

		internal override void Flip(bool flipX, bool flipY)
		{
		}

		internal override void SetCommandArg(AdvCommand command)
		{
			string animationStateName = GetAnimationStateName(command);
			float fadeTime = command.ParseCellOptional(AdvColumnName.Arg6, 0.2f);
			ChangeAnimationState(animationStateName, fadeTime);
		}

		internal override void ChangeAnimationState(string animationStateName, float fadeTime)
		{
			AnimationStateName = animationStateName;
			if (string.IsNullOrEmpty(AnimationStateName))
			{
				return;
			}
			if ((bool)animator)
			{
				animator.CrossFadeInFixedTime(AnimationStateName, fadeTime);
				return;
			}
			Animation componentInChildren = GetComponentInChildren<Animation>();
			if (componentInChildren != null)
			{
				componentInChildren.CrossFade(AnimationStateName, fadeTime);
			}
		}

		private string GetAnimationStateName(AdvCommand command)
		{
			if (!string.IsNullOrEmpty(base.LastResource.AnimationState))
			{
				return base.LastResource.AnimationState;
			}
			return AdvCharacterInfo.ParsePatternOnly(command);
		}

		private bool IsAnimationState(string patternName)
		{
			if (string.IsNullOrEmpty(patternName))
			{
				return false;
			}
			if ((bool)animator)
			{
				int layerCount = animator.layerCount;
				for (int i = 0; i < layerCount; i++)
				{
					if (animator.HasState(i, Animator.StringToHash(patternName)))
					{
						return true;
					}
				}
			}
			else
			{
				Animation componentInChildren = GetComponentInChildren<Animation>();
				if (componentInChildren != null)
				{
					foreach (AnimationState item in componentInChildren)
					{
						if (item.name == patternName)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public override void RuleFadeIn(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			Debug.LogError(base.gameObject.name + " is not support RuleFadeIn", base.gameObject);
			onComplete?.Invoke();
		}

		public override void RuleFadeOut(AdvEngine engine, AdvTransitionArgs data, Action onComplete)
		{
			Debug.LogError(base.gameObject.name + " is not support RuleFadeOut", base.gameObject);
			onComplete?.Invoke();
		}

		public override void Write(BinaryWriter writer)
		{
			writer.Write(1);
			if (animator != null)
			{
				writer.Write(SaveType.Animator.ToString());
				int layerCount = animator.layerCount;
				writer.Write(layerCount);
				for (int i = 0; i < layerCount; i++)
				{
					AnimatorStateInfo animatorStateInfo = (animator.IsInTransition(i) ? animator.GetNextAnimatorStateInfo(i) : animator.GetCurrentAnimatorStateInfo(i));
					writer.Write(animatorStateInfo.fullPathHash);
					writer.Write(animatorStateInfo.normalizedTime);
				}
			}
			else
			{
				writer.Write(SaveType.Other.ToString());
				writer.Write(AnimationStateName);
			}
		}

		public override void Read(BinaryReader reader)
		{
			int num = reader.ReadInt32();
			if (num < 0 || num > 1)
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
				return;
			}
			string value = reader.ReadString();
			switch ((SaveType)Enum.Parse(typeof(SaveType), value))
			{
			case SaveType.Animator:
			{
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					int stateNameHash = reader.ReadInt32();
					int layer = i;
					float normalizedTime = reader.ReadSingle();
					animator.Play(stateNameHash, layer, normalizedTime);
				}
				break;
			}
			default:
			{
				string animationState = reader.ReadString();
				ChangeAnimationState(animationState, 0f);
				break;
			}
			}
		}
	}
}
