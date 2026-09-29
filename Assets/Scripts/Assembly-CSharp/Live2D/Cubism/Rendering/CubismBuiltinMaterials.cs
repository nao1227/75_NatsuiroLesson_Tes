using UnityEngine;

namespace Live2D.Cubism.Rendering
{
	public static class CubismBuiltinMaterials
	{
		private const string ResourcesDirectory = "Live2D/Cubism/Materials";

		public static Material Unlit => LoadUnlitMaterial("Unlit");

		public static Material UnlitAdditive => LoadUnlitMaterial("UnlitAdditive");

		public static Material UnlitMultiply => LoadUnlitMaterial("UnlitMultiply");

		public static Material UnlitMasked => LoadUnlitMaterial("UnlitMasked");

		public static Material UnlitAdditiveMasked => LoadUnlitMaterial("UnlitAdditiveMasked");

		public static Material UnlitMultiplyMasked => LoadUnlitMaterial("UnlitMultiplyMasked");

		public static Material UnlitMaskedInverted => LoadUnlitMaterial("UnlitMaskedInverted");

		public static Material UnlitAdditiveMaskedInverted => LoadUnlitMaterial("UnlitAdditiveMaskedInverted");

		public static Material UnlitMultiplyMaskedInverted => LoadUnlitMaterial("UnlitMultiplyMaskedInverted");

		public static Material UnlitCulling => LoadUnlitMaterial("UnlitCulling");

		public static Material UnlitAdditiveCulling => LoadUnlitMaterial("UnlitAdditiveCulling");

		public static Material UnlitMultiplyCulling => LoadUnlitMaterial("UnlitMultiplyCulling");

		public static Material UnlitMaskedCulling => LoadUnlitMaterial("UnlitMaskedCulling");

		public static Material UnlitAdditiveMaskedCulling => LoadUnlitMaterial("UnlitAdditiveMaskedCulling");

		public static Material UnlitMultiplyMaskedCulling => LoadUnlitMaterial("UnlitMultiplyMaskedCulling");

		public static Material UnlitMaskedInvertedCulling => LoadUnlitMaterial("UnlitMaskedInvertedCulling");

		public static Material UnlitAdditiveMaskedInvertedCulling => LoadUnlitMaterial("UnlitAdditiveMaskedInvertedCulling");

		public static Material UnlitMultiplyMaskedInvertedCulling => LoadUnlitMaterial("UnlitMultiplyMaskedInvertedCulling");

		public static Material Mask => LoadMaskMaterial();

		public static Material MaskCulling => LoadMaskCullingMaterial();

		private static Material LoadUnlitMaterial(string name)
		{
			return Resources.Load<Material>("Live2D/Cubism/Materials/" + name);
		}

		private static Material LoadMaskMaterial()
		{
			return Resources.Load<Material>("Live2D/Cubism/Materials/Mask");
		}

		private static Material LoadMaskCullingMaterial()
		{
			return Resources.Load<Material>("Live2D/Cubism/Materials/MaskCulling");
		}
	}
}
