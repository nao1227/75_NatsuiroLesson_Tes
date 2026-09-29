using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	public static class CubismBuiltinPickers
	{
		public static Material MaterialPicker(CubismModel3Json sender, CubismDrawable drawable)
		{
			if (drawable.IsDoubleSided)
			{
				if (drawable.BlendAdditive)
				{
					if (!drawable.IsMasked)
					{
						return CubismBuiltinMaterials.UnlitAdditive;
					}
					if (!drawable.IsInverted)
					{
						return CubismBuiltinMaterials.UnlitAdditiveMasked;
					}
					return CubismBuiltinMaterials.UnlitAdditiveMaskedInverted;
				}
				if (drawable.MultiplyBlend)
				{
					if (!drawable.IsMasked)
					{
						return CubismBuiltinMaterials.UnlitMultiply;
					}
					if (!drawable.IsInverted)
					{
						return CubismBuiltinMaterials.UnlitMultiplyMasked;
					}
					return CubismBuiltinMaterials.UnlitMultiplyMaskedInverted;
				}
				if (!drawable.IsMasked)
				{
					return CubismBuiltinMaterials.Unlit;
				}
				if (!drawable.IsInverted)
				{
					return CubismBuiltinMaterials.UnlitMasked;
				}
				return CubismBuiltinMaterials.UnlitMaskedInverted;
			}
			if (drawable.BlendAdditive)
			{
				if (!drawable.IsMasked)
				{
					return CubismBuiltinMaterials.UnlitAdditiveCulling;
				}
				if (!drawable.IsInverted)
				{
					return CubismBuiltinMaterials.UnlitAdditiveMaskedCulling;
				}
				return CubismBuiltinMaterials.UnlitAdditiveMaskedInvertedCulling;
			}
			if (drawable.MultiplyBlend)
			{
				if (!drawable.IsMasked)
				{
					return CubismBuiltinMaterials.UnlitMultiplyCulling;
				}
				if (!drawable.IsInverted)
				{
					return CubismBuiltinMaterials.UnlitMultiplyMaskedCulling;
				}
				return CubismBuiltinMaterials.UnlitMultiplyMaskedInvertedCulling;
			}
			if (!drawable.IsMasked)
			{
				return CubismBuiltinMaterials.UnlitCulling;
			}
			if (!drawable.IsInverted)
			{
				return CubismBuiltinMaterials.UnlitMaskedCulling;
			}
			return CubismBuiltinMaterials.UnlitMaskedInvertedCulling;
		}

		public static Texture2D TexturePicker(CubismModel3Json sender, CubismDrawable drawable)
		{
			return sender.Textures[drawable.TextureIndex];
		}
	}
}
