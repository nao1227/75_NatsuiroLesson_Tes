using System.Collections.Generic;
using UnityEngine;
using UtageExtensions;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/AdvEffectManager")]
	public class AdvEffectManager : MonoBehaviour
	{
		public enum TargetType
		{
			Default = 0,
			Camera = 1,
			Graphics = 2,
			MessageWindow = 3
		}

		private AdvEngine engine;

		[SerializeField]
		private List<Texture2D> ruleTextureList = new List<Texture2D>();

		public AdvEngine Engine => this.GetComponentCacheInParent(ref engine);

		public List<Texture2D> RuleTextureList
		{
			get
			{
				return ruleTextureList;
			}
			set
			{
				ruleTextureList = value;
			}
		}

		internal Texture2D FindRuleTexture(string name)
		{
			foreach (Texture2D ruleTexture in ruleTextureList)
			{
				if (!(ruleTexture == null) && ruleTexture.name == name)
				{
					return ruleTexture;
				}
			}
			Debug.LogErrorFormat("Not Found Rule Texture [ {0} ]", name);
			return null;
		}

		internal GameObject FindTarget(AdvCommandEffectBase command)
		{
			return FindTarget(command.Target, command.TargetName);
		}

		internal GameObject FindTarget(TargetType targetType, string targetName)
		{
			switch (targetType)
			{
			case TargetType.MessageWindow:
				return Engine.MessageWindowManager.UiMessageWindowManager.gameObject;
			case TargetType.Graphics:
				return Engine.GraphicManager.gameObject;
			case TargetType.Camera:
			{
				if (string.IsNullOrEmpty(targetName) || targetName == TargetType.Camera.ToString())
				{
					return Engine.CameraManager.gameObject;
				}
				CameraRoot cameraRoot = Engine.CameraManager.FindCameraRoot(targetName);
				if (cameraRoot == null)
				{
					return null;
				}
				return cameraRoot.gameObject;
			}
			default:
			{
				GameObject gameObject = Engine.GraphicManager.FindObjectOrLayer(targetName);
				if (gameObject != null)
				{
					return gameObject;
				}
				if (Engine.MessageWindowManager.UiMessageWindowManager.AllWindows.TryGetValue(targetName, out var value))
				{
					return value.gameObject;
				}
				return null;
			}
			}
		}
	}
}
