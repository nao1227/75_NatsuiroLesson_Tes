using UnityEngine;
using UnityEngine.Rendering;

namespace Live2D.Cubism.Rendering.Masking
{
	internal sealed class CubismMaskMaskedJunction
	{
		private static CubismMaskProperties SharedMaskProperties { get; set; }

		private CubismMaskRenderer[] Masks { get; set; }

		private CubismRenderer[] Maskeds { get; set; }

		private CubismMaskTexture MaskTexture { get; set; }

		private CubismMaskTile MaskTile { get; set; }

		private CubismMaskTransform MaskTransform { get; set; }

		public CubismMaskMaskedJunction()
		{
			if (SharedMaskProperties == null)
			{
				SharedMaskProperties = new CubismMaskProperties();
			}
		}

		public CubismMaskMaskedJunction SetMasks(CubismMaskRenderer[] value)
		{
			Masks = value;
			return this;
		}

		public CubismMaskMaskedJunction SetMaskeds(CubismRenderer[] value)
		{
			Maskeds = value;
			return this;
		}

		public CubismMaskMaskedJunction SetMaskTexture(CubismMaskTexture value)
		{
			MaskTexture = value;
			return this;
		}

		public CubismMaskMaskedJunction SetMaskTile(CubismMaskTile value)
		{
			MaskTile = value;
			return this;
		}

		public void AddToCommandBuffer(CommandBuffer buffer)
		{
			RecalculateMaskTransform();
			for (int i = 0; i < Masks.Length; i++)
			{
				Masks[i].SetMaskTile(MaskTile).SetMaskTransform(MaskTransform).AddToCommandBuffer(buffer);
			}
		}

		internal void Update()
		{
			RecalculateMaskTransform();
			for (int i = 0; i < Masks.Length; i++)
			{
				Masks[i].SetMaskTransform(MaskTransform);
			}
			CubismMaskProperties sharedMaskProperties = SharedMaskProperties;
			sharedMaskProperties.Texture = MaskTexture;
			sharedMaskProperties.Tile = MaskTile;
			sharedMaskProperties.Transform = MaskTransform;
			for (int j = 0; j < Maskeds.Length; j++)
			{
				Maskeds[j].OnMaskPropertiesDidChange(sharedMaskProperties);
			}
		}

		private void RecalculateMaskTransform()
		{
			Bounds bounds = Masks.GetBounds();
			float num = ((bounds.size.x > bounds.size.y) ? bounds.size.x : bounds.size.y);
			MaskTransform = new CubismMaskTransform
			{
				Offset = bounds.center,
				Scale = 1f / num
			};
		}
	}
}
