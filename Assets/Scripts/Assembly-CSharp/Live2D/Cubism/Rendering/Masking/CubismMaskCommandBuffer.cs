using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Live2D.Cubism.Rendering.Masking
{
	[ExecuteInEditMode]
	public sealed class CubismMaskCommandBuffer : MonoBehaviour
	{
		private static List<ICubismMaskCommandSource> Sources { get; set; }

		private static CommandBuffer Buffer { get; set; }

		private static bool ContainsSources
		{
			get
			{
				if (Sources != null)
				{
					return Sources.Count > 0;
				}
				return false;
			}
		}

		private static void Initialize()
		{
			if (Sources == null)
			{
				Sources = new List<ICubismMaskCommandSource>();
			}
			if (Buffer == null)
			{
				Buffer = new CommandBuffer
				{
					name = "cubism_MaskCommandBuffer"
				};
			}
			GameObject gameObject = GameObject.Find("cubism_MaskCommandBuffer");
			if (gameObject == null)
			{
				gameObject = new GameObject("cubism_MaskCommandBuffer")
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				if (!Application.isEditor || Application.isPlaying)
				{
					Object.DontDestroyOnLoad(gameObject);
				}
				gameObject.AddComponent<CubismMaskCommandBuffer>();
			}
		}

		internal static void AddSource(ICubismMaskCommandSource source)
		{
			Initialize();
			if (!Sources.Contains(source))
			{
				Sources.Add(source);
			}
		}

		internal static void RemoveSource(ICubismMaskCommandSource source)
		{
			Initialize();
			Sources.RemoveAll((ICubismMaskCommandSource s) => s == source);
		}

		private static void RefreshCommandBuffer()
		{
			Buffer.Clear();
			for (int i = 0; i < Sources.Count; i++)
			{
				Sources[i].AddToCommandBuffer(Buffer);
			}
		}

		private void LateUpdate()
		{
			if (ContainsSources)
			{
				RefreshCommandBuffer();
				Graphics.ExecuteCommandBuffer(Buffer);
			}
		}
	}
}
