using UnityEngine;

namespace Utage
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Utage/Lib/Image Effects/Other/ScreenOverlay")]
	public class ScreenOverlay : ImageEffectSingelShaderBase
	{
		public enum OverlayBlendMode
		{
			Additive = 0,
			ScreenBlend = 1,
			Multiply = 2,
			Overlay = 3,
			AlphaBlend = 4
		}

		public OverlayBlendMode blendMode = OverlayBlendMode.Overlay;

		public float intensity = 1f;

		public Texture2D texture;

		protected override void RenderImage(RenderTexture source, RenderTexture destination)
		{
			Vector4 value = new Vector4(1f, 0f, 0f, 1f);
			base.Material.SetVector("_UV_Transform", value);
			base.Material.SetFloat("_Intensity", intensity);
			base.Material.SetTexture("_Overlay", texture);
			Graphics.Blit(source, destination, base.Material, (int)blendMode);
		}
	}
}
