using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Utage/Lib/Image Effects/Blur/Blur")]
	public class Blur : ImageEffectSingelShaderBase
	{
		public enum BlurType
		{
			StandardGauss = 0,
			SgxGauss = 1
		}

		[Range(0f, 2f)]
		public float downsample = 1f;

		[Range(0f, 10f)]
		public float blurSize = 3f;

		[Range(1f, 4f)]
		public float blurIterations = 2f;

		public BlurType blurType;

		protected override bool NeedRenderTexture => true;

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			int num = Mathf.FloorToInt(downsample);
			float num2 = 1f / (1f * (float)(1 << num));
			base.Material.SetVector("_Parameter", new Vector4(blurSize * num2, (0f - blurSize) * num2, 0f, 0f));
			source.filterMode = FilterMode.Bilinear;
			int width = source.width >> num;
			int height = source.height >> num;
			RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
			renderTexture.filterMode = FilterMode.Bilinear;
			Graphics.Blit(source, renderTexture, base.Material, 0);
			int num3 = ((blurType != BlurType.StandardGauss) ? 2 : 0);
			for (int i = 0; (float)i < blurIterations; i++)
			{
				float num4 = (float)i * 1f;
				base.Material.SetVector("_Parameter", new Vector4(blurSize * num2 + num4, (0f - blurSize) * num2 - num4, 0f, 0f));
				RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, base.Material, 1 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
				temporary = RenderTexture.GetTemporary(width, height, 0, source.format);
				temporary.filterMode = FilterMode.Bilinear;
				Graphics.Blit(renderTexture, temporary, base.Material, 2 + num3);
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			Graphics.Blit(renderTexture, destination);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}
}
