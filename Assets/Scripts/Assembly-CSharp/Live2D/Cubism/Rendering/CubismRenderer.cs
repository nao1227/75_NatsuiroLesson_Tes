using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering.Masking;
using UnityEngine;
using UnityEngine.Rendering;

namespace Live2D.Cubism.Rendering
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public sealed class CubismRenderer : MonoBehaviour
	{
		private struct SwapInfo
		{
			public bool NewVertexPositions { get; set; }

			public bool NewVertexColors { get; set; }

			public bool DidBecomeVisible { get; set; }

			public bool DidBecomeInvisible { get; set; }

			public bool NewRenderOrder { get; set; }
		}

		[SerializeField]
		[HideInInspector]
		private int _localSortingOrder;

		[SerializeField]
		[HideInInspector]
		private Color _color = Color.white;

		[SerializeField]
		[HideInInspector]
		private bool _isOverwrittenDrawableMultiplyColors;

		[SerializeField]
		[HideInInspector]
		private bool _isOverwrittenDrawableScreenColors;

		[SerializeField]
		[HideInInspector]
		private Color _multiplyColor = Color.white;

		[SerializeField]
		[HideInInspector]
		private Color _screenColor = Color.clear;

		[SerializeField]
		[HideInInspector]
		private Texture2D _mainTexture;

		[NonSerialized]
		private MeshFilter _meshFilter;

		[NonSerialized]
		private MeshRenderer _meshRenderer;

		[SerializeField]
		[HideInInspector]
		private CubismSortingMode _sortingMode;

		[SerializeField]
		[HideInInspector]
		private int _sortingOrder;

		[SerializeField]
		[HideInInspector]
		private int _renderOrder;

		[SerializeField]
		[HideInInspector]
		private float _depthOffset = 1E-05f;

		[SerializeField]
		[HideInInspector]
		private float _opacity;

		private static MaterialPropertyBlock _sharedPropertyBlock;

		public int LocalSortingOrder
		{
			get
			{
				return _localSortingOrder;
			}
			set
			{
				if (value != _localSortingOrder)
				{
					_localSortingOrder = value;
					ApplySorting();
				}
			}
		}

		public Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				if (!(value == _color))
				{
					_color = value;
					ApplyVertexColors();
				}
			}
		}

		public bool OverwriteFlagForDrawableMultiplyColors
		{
			get
			{
				return _isOverwrittenDrawableMultiplyColors;
			}
			set
			{
				_isOverwrittenDrawableMultiplyColors = value;
			}
		}

		public bool LastIsUseUserMultiplyColor { get; set; }

		public bool OverwriteFlagForDrawableScreenColors
		{
			get
			{
				return _isOverwrittenDrawableScreenColors;
			}
			set
			{
				_isOverwrittenDrawableScreenColors = value;
			}
		}

		public bool LastIsUseUserScreenColors { get; set; }

		public Color MultiplyColor
		{
			get
			{
				if (OverwriteFlagForDrawableMultiplyColors || RenderController.OverwriteFlagForModelMultiplyColors)
				{
					return _multiplyColor;
				}
				return Drawable.MultiplyColor;
			}
			set
			{
				if (!(value == _multiplyColor))
				{
					_multiplyColor = value;
				}
			}
		}

		public Color LastMultiplyColor { get; set; }

		public Color ScreenColor
		{
			get
			{
				if (OverwriteFlagForDrawableScreenColors || RenderController.OverwriteFlagForModelScreenColors)
				{
					return _screenColor;
				}
				return Drawable.ScreenColor;
			}
			set
			{
				if (!(value == _screenColor))
				{
					_screenColor = value;
				}
			}
		}

		public Color LastScreenColor { get; set; }

		public Material Material
		{
			get
			{
				return MeshRenderer.material;
			}
			set
			{
				MeshRenderer.material = value;
			}
		}

		public Texture2D MainTexture
		{
			get
			{
				return _mainTexture;
			}
			set
			{
				if (!(value == _mainTexture) || !(_mainTexture != null))
				{
					_mainTexture = ((value != null) ? value : Texture2D.whiteTexture);
					ApplyMainTexture();
				}
			}
		}

		private Mesh[] Meshes { get; set; }

		private int FrontMesh { get; set; }

		private int BackMesh { get; set; }

		public Mesh Mesh => Meshes[FrontMesh];

		public MeshFilter MeshFilter => _meshFilter;

		public MeshRenderer MeshRenderer
		{
			get
			{
				TryInitializeMeshRenderer();
				return _meshRenderer;
			}
		}

		private CubismDrawable Drawable { get; set; }

		private CubismRenderController RenderController { get; set; }

		private CubismSortingMode SortingMode
		{
			get
			{
				return _sortingMode;
			}
			set
			{
				_sortingMode = value;
			}
		}

		private int SortingOrder
		{
			get
			{
				return _sortingOrder;
			}
			set
			{
				_sortingOrder = value;
			}
		}

		private int RenderOrder
		{
			get
			{
				return _renderOrder;
			}
			set
			{
				_renderOrder = value;
			}
		}

		private float DepthOffset
		{
			get
			{
				return _depthOffset;
			}
			set
			{
				_depthOffset = value;
			}
		}

		private float Opacity
		{
			get
			{
				return _opacity;
			}
			set
			{
				_opacity = value;
			}
		}

		private Color[] VertexColors { get; set; }

		private SwapInfo LastSwap { get; set; }

		private SwapInfo ThisSwap { get; set; }

		private static MaterialPropertyBlock SharedPropertyBlock
		{
			get
			{
				if (_sharedPropertyBlock == null)
				{
					_sharedPropertyBlock = new MaterialPropertyBlock();
				}
				return _sharedPropertyBlock;
			}
		}

		public void SwapMeshes()
		{
			BackMesh = FrontMesh;
			FrontMesh = ((FrontMesh == 0) ? 1 : 0);
			Mesh mesh = Meshes[FrontMesh];
			Meshes[BackMesh].colors = VertexColors;
			LastSwap = ThisSwap;
			ResetSwapInfoFlags();
			MeshFilter.mesh = mesh;
		}

		public void UpdateVisibility()
		{
			if (LastSwap.DidBecomeVisible)
			{
				MeshRenderer.enabled = true;
			}
			else if (LastSwap.DidBecomeInvisible)
			{
				MeshRenderer.enabled = false;
			}
			ResetVisibilityFlags();
		}

		public void UpdateRenderOrder()
		{
			if (LastSwap.NewRenderOrder)
			{
				ApplySorting();
			}
			ResetRenderOrderFlag();
		}

		internal void OnControllerSortingLayerDidChange(int newSortingLayer)
		{
			MeshRenderer.sortingLayerID = newSortingLayer;
		}

		internal void OnControllerSortingModeDidChange(CubismSortingMode newSortingMode)
		{
			SortingMode = newSortingMode;
			ApplySorting();
		}

		internal void OnControllerSortingOrderDidChange(int newSortingOrder)
		{
			SortingOrder = newSortingOrder;
			ApplySorting();
		}

		internal void OnControllerDepthOffsetDidChange(float newDepthOffset)
		{
			DepthOffset = newDepthOffset;
			ApplySorting();
		}

		internal void OnDrawableOpacityDidChange(float newOpacity)
		{
			Opacity = newOpacity;
			ApplyVertexColors();
		}

		internal void OnDrawableRenderOrderDidChange(int newRenderOrder)
		{
			RenderOrder = newRenderOrder;
			SetNewRenderOrder();
		}

		internal void OnDrawableVertexPositionsDidChange(Vector3[] newVertexPositions)
		{
			Mesh mesh = Mesh;
			mesh.vertices = newVertexPositions;
			mesh.RecalculateBounds();
			SetNewVertexPositions();
		}

		internal void OnDrawableVisiblityDidChange(bool newVisibility)
		{
			if (newVisibility)
			{
				BecomeVisible();
			}
			else
			{
				BecomeInvisible();
			}
		}

		internal void OnMaskPropertiesDidChange(CubismMaskProperties newMaskProperties)
		{
			MeshRenderer.GetPropertyBlock(SharedPropertyBlock);
			SharedPropertyBlock.SetTexture("cubism_MaskTexture", newMaskProperties.Texture);
			SharedPropertyBlock.SetVector("cubism_MaskTile", newMaskProperties.Tile);
			SharedPropertyBlock.SetVector("cubism_MaskTransform", newMaskProperties.Transform);
			MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
		}

		internal void OnModelOpacityDidChange(float newModelOpacity)
		{
			_meshRenderer.GetPropertyBlock(SharedPropertyBlock);
			SharedPropertyBlock.SetFloat("cubism_ModelOpacity", newModelOpacity);
			MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
		}

		private void ApplyMainTexture()
		{
			MeshRenderer.GetPropertyBlock(SharedPropertyBlock);
			SharedPropertyBlock.SetTexture("_MainTex", MainTexture);
			MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
		}

		private void ApplySorting()
		{
			if (SortingMode.SortByOrder())
			{
				MeshRenderer.sortingOrder = SortingOrder + ((SortingMode == CubismSortingMode.BackToFrontOrder) ? (RenderOrder + LocalSortingOrder) : (-(RenderOrder + LocalSortingOrder)));
				base.transform.localPosition = Vector3.zero;
			}
			else
			{
				float num = ((SortingMode == CubismSortingMode.BackToFrontZ) ? (0f - DepthOffset) : DepthOffset);
				MeshRenderer.sortingOrder = SortingOrder + LocalSortingOrder;
				base.transform.localPosition = new Vector3(0f, 0f, (float)RenderOrder * num);
			}
		}

		public void ApplyVertexColors()
		{
			Color[] vertexColors = VertexColors;
			Color color = Color;
			color.a *= Opacity;
			for (int i = 0; i < vertexColors.Length; i++)
			{
				vertexColors[i] = color;
			}
			SetNewVertexColors();
		}

		public void ApplyMultiplyColor()
		{
			MeshRenderer.GetPropertyBlock(SharedPropertyBlock);
			SharedPropertyBlock.SetColor("cubism_MultiplyColor", MultiplyColor);
			MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
		}

		private void TryInitializeMultiplyColor()
		{
			LastIsUseUserMultiplyColor = false;
			LastMultiplyColor = MultiplyColor;
			ApplyMultiplyColor();
		}

		public void ApplyScreenColor()
		{
			MeshRenderer.GetPropertyBlock(SharedPropertyBlock);
			SharedPropertyBlock.SetColor("cubism_ScreenColor", ScreenColor);
			MeshRenderer.SetPropertyBlock(SharedPropertyBlock);
		}

		private void TryInitializeScreenColor()
		{
			LastIsUseUserScreenColors = false;
			LastScreenColor = ScreenColor;
			ApplyScreenColor();
		}

		private void TryInitializeMeshRenderer()
		{
			if (_meshRenderer == null)
			{
				_meshRenderer = GetComponent<MeshRenderer>();
				if (_meshRenderer == null)
				{
					_meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
					_meshRenderer.hideFlags = HideFlags.HideInInspector;
					_meshRenderer.receiveShadows = false;
					_meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
					_meshRenderer.lightProbeUsage = LightProbeUsage.BlendProbes;
				}
			}
		}

		private void TryInitializeMeshFilter()
		{
			if (_meshFilter == null)
			{
				_meshFilter = GetComponent<MeshFilter>();
				if (_meshFilter == null)
				{
					_meshFilter = base.gameObject.AddComponent<MeshFilter>();
					_meshFilter.hideFlags = HideFlags.HideInInspector;
				}
			}
		}

		private void TryInitializeMesh()
		{
			if (Meshes == null || Mesh.vertexCount <= 0)
			{
				if (Meshes == null)
				{
					Meshes = new Mesh[2];
				}
				for (int i = 0; i < 2; i++)
				{
					Mesh mesh = new Mesh
					{
						name = Drawable.name,
						vertices = Drawable.VertexPositions,
						uv = Drawable.VertexUvs,
						triangles = Drawable.Indices
					};
					mesh.MarkDynamic();
					mesh.RecalculateBounds();
					Meshes[i] = mesh;
				}
			}
		}

		private void TryInitializeVertexColor()
		{
			Mesh mesh = Mesh;
			VertexColors = new Color[mesh.vertexCount];
			for (int i = 0; i < VertexColors.Length; i++)
			{
				VertexColors[i] = Color;
				VertexColors[i].a *= Opacity;
			}
		}

		private void TryInitializeMainTexture()
		{
			if (MainTexture == null)
			{
				MainTexture = null;
			}
			ApplyMainTexture();
		}

		public void TryInitialize(CubismRenderController renderController)
		{
			Drawable = GetComponent<CubismDrawable>();
			RenderController = renderController;
			TryInitializeMeshRenderer();
			TryInitializeMeshFilter();
			TryInitializeMesh();
			TryInitializeVertexColor();
			TryInitializeMainTexture();
			TryInitializeMultiplyColor();
			TryInitializeScreenColor();
			ApplySorting();
		}

		private void SetNewVertexPositions()
		{
			SwapInfo thisSwap = ThisSwap;
			thisSwap.NewVertexPositions = true;
			ThisSwap = thisSwap;
		}

		private void SetNewVertexColors()
		{
			SwapInfo thisSwap = ThisSwap;
			thisSwap.NewVertexColors = true;
			ThisSwap = thisSwap;
		}

		private void BecomeVisible()
		{
			SwapInfo thisSwap = ThisSwap;
			thisSwap.DidBecomeVisible = true;
			ThisSwap = thisSwap;
		}

		private void BecomeInvisible()
		{
			SwapInfo thisSwap = ThisSwap;
			thisSwap.DidBecomeInvisible = true;
			ThisSwap = thisSwap;
		}

		private void SetNewRenderOrder()
		{
			SwapInfo thisSwap = ThisSwap;
			thisSwap.NewRenderOrder = true;
			ThisSwap = thisSwap;
		}

		private void ResetSwapInfoFlags()
		{
			SwapInfo thisSwap = ThisSwap;
			thisSwap.NewVertexColors = false;
			thisSwap.NewVertexPositions = false;
			thisSwap.DidBecomeVisible = false;
			thisSwap.DidBecomeInvisible = false;
			ThisSwap = thisSwap;
		}

		private void ResetVisibilityFlags()
		{
			SwapInfo lastSwap = LastSwap;
			lastSwap.DidBecomeVisible = false;
			lastSwap.DidBecomeInvisible = false;
			LastSwap = lastSwap;
		}

		private void ResetRenderOrderFlag()
		{
			SwapInfo lastSwap = LastSwap;
			lastSwap.NewRenderOrder = false;
			LastSwap = lastSwap;
		}

		private void OnDestroy()
		{
			if (Meshes != null)
			{
				for (int i = 0; i < Meshes.Length; i++)
				{
					UnityEngine.Object.DestroyImmediate(Meshes[i]);
				}
			}
		}
	}
}
