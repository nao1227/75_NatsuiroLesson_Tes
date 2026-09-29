using Live2D.Cubism.Core;
using UnityEngine;
using UnityEngine.Rendering;

namespace Live2D.Cubism.Rendering.Masking
{
	internal sealed class CubismMaskRenderer
	{
		private MaterialPropertyBlock MaskProperties { get; set; }

		private CubismRenderer MainRenderer { get; set; }

		private Material MaskMaterial { get; set; }

		private Material MaskCullingMaterial { get; set; }

		private bool IsCulling { get; set; }

		internal Bounds MeshBounds => MainRenderer.Mesh.bounds;

		public CubismMaskRenderer()
		{
			MaskProperties = new MaterialPropertyBlock();
			MaskMaterial = CubismBuiltinMaterials.Mask;
			MaskCullingMaterial = CubismBuiltinMaterials.MaskCulling;
		}

		internal CubismMaskRenderer SetMainRenderer(CubismRenderer value)
		{
			MainRenderer = value;
			IsCulling = !MainRenderer.gameObject.GetComponent<CubismDrawable>().IsDoubleSided;
			return this;
		}

		internal CubismMaskRenderer SetMaskTile(CubismMaskTile value)
		{
			MaskProperties.SetVector("cubism_MaskTile", value);
			return this;
		}

		internal CubismMaskRenderer SetMaskTransform(CubismMaskTransform value)
		{
			MaskProperties.SetVector("cubism_MaskTransform", value);
			return this;
		}

		internal void AddToCommandBuffer(CommandBuffer buffer)
		{
			Texture2D mainTexture = MainRenderer.MainTexture;
			Mesh mesh = MainRenderer.Mesh;
			MaskProperties.SetTexture("_MainTex", mainTexture);
			buffer.DrawMesh(mesh, Matrix4x4.identity, IsCulling ? MaskCullingMaterial : MaskMaterial, 0, 0, MaskProperties);
		}
	}
}
