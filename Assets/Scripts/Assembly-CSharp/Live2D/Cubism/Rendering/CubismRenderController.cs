using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;

namespace Live2D.Cubism.Rendering
{
	[ExecuteInEditMode]
	[CubismDontMoveOnReimport]
	public sealed class CubismRenderController : MonoBehaviour, ICubismUpdatable
	{
		[SerializeField]
		[HideInInspector]
		public float Opacity = 1f;

		[SerializeField]
		[HideInInspector]
		private float _lastOpacity;

		[SerializeField]
		[HideInInspector]
		private bool _isOverwrittenModelMultiplyColors;

		[SerializeField]
		[HideInInspector]
		private bool _isOverwrittenModelScreenColors;

		[SerializeField]
		[HideInInspector]
		private Color _modelMultiplyColor;

		[SerializeField]
		[HideInInspector]
		private Color _modelScreenColor;

		[SerializeField]
		[HideInInspector]
		private int _sortingLayerId;

		[SerializeField]
		[HideInInspector]
		private CubismSortingMode _sortingMode;

		[SerializeField]
		[HideInInspector]
		private int _sortingOrder;

		[SerializeField]
		public Camera CameraToFace;

		[SerializeField]
		[HideInInspector]
		private UnityEngine.Object _drawOrderHandler;

		[NonSerialized]
		private ICubismDrawOrderHandler _drawOrderHandlerInterface;

		[SerializeField]
		[HideInInspector]
		private UnityEngine.Object _opacityHandler;

		private ICubismOpacityHandler _opacityHandlerInterface;

		[SerializeField]
		[HideInInspector]
		private UnityEngine.Object _multiplyColorHandler;

		private ICubismBlendColorHandler _multiplyColorHandlerInterface;

		[SerializeField]
		[HideInInspector]
		private UnityEngine.Object _screenColorHandler;

		private ICubismBlendColorHandler _screenColorHandlerInterface;

		[SerializeField]
		[HideInInspector]
		public float _depthOffset = 1E-05f;

		private Transform _drawablesRootTransform;

		[NonSerialized]
		private CubismRenderer[] _renderers;

		private float LastOpacity
		{
			get
			{
				return _lastOpacity;
			}
			set
			{
				_lastOpacity = value;
			}
		}

		public bool OverwriteFlagForModelMultiplyColors
		{
			get
			{
				return _isOverwrittenModelMultiplyColors;
			}
			set
			{
				_isOverwrittenModelMultiplyColors = value;
			}
		}

		public bool OverwriteFlagForModelScreenColors
		{
			get
			{
				return _isOverwrittenModelScreenColors;
			}
			set
			{
				_isOverwrittenModelScreenColors = value;
			}
		}

		public Color ModelMultiplyColor
		{
			get
			{
				return _modelMultiplyColor;
			}
			set
			{
				_modelMultiplyColor = value;
			}
		}

		public Color ModelScreenColor
		{
			get
			{
				return _modelScreenColor;
			}
			set
			{
				_modelScreenColor = value;
			}
		}

		public string SortingLayer
		{
			get
			{
				return UnityEngine.SortingLayer.IDToName(SortingLayerId);
			}
			set
			{
				SortingLayerId = UnityEngine.SortingLayer.NameToID(value);
			}
		}

		public int SortingLayerId
		{
			get
			{
				return _sortingLayerId;
			}
			set
			{
				if (value != _sortingLayerId)
				{
					_sortingLayerId = value;
					CubismRenderer[] renderers = Renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						renderers[i].OnControllerSortingLayerDidChange(_sortingLayerId);
					}
				}
			}
		}

		public CubismSortingMode SortingMode
		{
			get
			{
				return _sortingMode;
			}
			set
			{
				if (value != _sortingMode)
				{
					_sortingMode = value;
					CubismRenderer[] renderers = Renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						renderers[i].OnControllerSortingModeDidChange(_sortingMode);
					}
				}
			}
		}

		public int SortingOrder
		{
			get
			{
				return _sortingOrder;
			}
			set
			{
				if (value != _sortingOrder)
				{
					_sortingOrder = value;
					CubismRenderer[] renderers = Renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						renderers[i].OnControllerSortingOrderDidChange(SortingOrder);
					}
				}
			}
		}

		public UnityEngine.Object DrawOrderHandler
		{
			get
			{
				return _drawOrderHandler;
			}
			set
			{
				_drawOrderHandler = value.ToNullUnlessImplementsInterface<ICubismDrawOrderHandler>();
			}
		}

		private ICubismDrawOrderHandler DrawOrderHandlerInterface
		{
			get
			{
				if (_drawOrderHandlerInterface == null)
				{
					_drawOrderHandlerInterface = DrawOrderHandler.GetInterface<ICubismDrawOrderHandler>();
				}
				return _drawOrderHandlerInterface;
			}
		}

		public UnityEngine.Object OpacityHandler
		{
			get
			{
				return _opacityHandler;
			}
			set
			{
				_opacityHandler = value.ToNullUnlessImplementsInterface<ICubismOpacityHandler>();
			}
		}

		private ICubismOpacityHandler OpacityHandlerInterface
		{
			get
			{
				if (_opacityHandlerInterface == null)
				{
					_opacityHandlerInterface = OpacityHandler.GetInterface<ICubismOpacityHandler>();
				}
				return _opacityHandlerInterface;
			}
		}

		public UnityEngine.Object MultiplyColorHandler
		{
			get
			{
				return _multiplyColorHandler;
			}
			set
			{
				_multiplyColorHandler = value.ToNullUnlessImplementsInterface<ICubismBlendColorHandler>();
			}
		}

		private ICubismBlendColorHandler MultiplyColorHandlerInterface
		{
			get
			{
				if (_multiplyColorHandlerInterface == null)
				{
					_multiplyColorHandlerInterface = MultiplyColorHandler?.GetInterface<ICubismBlendColorHandler>();
				}
				return _multiplyColorHandlerInterface;
			}
		}

		public UnityEngine.Object ScreenColorHandler
		{
			get
			{
				return _screenColorHandler;
			}
			set
			{
				_screenColorHandler = value.ToNullUnlessImplementsInterface<ICubismBlendColorHandler>();
			}
		}

		private ICubismBlendColorHandler ScreenColorHandlerInterface
		{
			get
			{
				if (_screenColorHandlerInterface == null)
				{
					_screenColorHandlerInterface = ScreenColorHandler?.GetInterface<ICubismBlendColorHandler>();
				}
				return _screenColorHandlerInterface;
			}
		}

		public float DepthOffset
		{
			get
			{
				return _depthOffset;
			}
			set
			{
				if (!(Mathf.Abs(value - _depthOffset) < Mathf.Epsilon))
				{
					_depthOffset = value;
					CubismRenderer[] renderers = Renderers;
					for (int i = 0; i < renderers.Length; i++)
					{
						renderers[i].OnControllerDepthOffsetDidChange(_depthOffset);
					}
				}
			}
		}

		private CubismModel Model => this.FindCubismModel();

		private Transform DrawablesRootTransform
		{
			get
			{
				if (_drawablesRootTransform == null)
				{
					_drawablesRootTransform = Model.Drawables[0].transform.parent;
				}
				return _drawablesRootTransform;
			}
		}

		public CubismRenderer[] Renderers
		{
			get
			{
				if (_renderers == null)
				{
					Component[] drawables = Model.Drawables;
					_renderers = drawables.GetComponentsMany<CubismRenderer>();
				}
				return _renderers;
			}
			private set
			{
				_renderers = value;
			}
		}

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismRenderController;

		public bool NeedsUpdateOnEditing => true;

		private void TryInitializeRenderers()
		{
			CubismRenderer[] array = Renderers;
			if (array == null || array.Length == 0)
			{
				Component[] drawables = this.FindCubismModel().Drawables;
				array = (Renderers = drawables.AddComponentEach<CubismRenderer>());
			}
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					array[i].TryInitialize(this);
				}
				_sortingLayerId = array[0].MeshRenderer.sortingLayerID;
			}
		}

		private void UpdateOpacity()
		{
			if (Mathf.Abs(Opacity - LastOpacity) < Mathf.Epsilon)
			{
				return;
			}
			Opacity = Mathf.Clamp(Opacity, 0f, 1f);
			LastOpacity = Opacity;
			if ((OpacityHandlerInterface == null || Opacity > 1f - Mathf.Epsilon) && Renderers != null)
			{
				CubismRenderer[] renderers = Renderers;
				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].OnModelOpacityDidChange(Opacity);
				}
			}
			if (OpacityHandlerInterface != null)
			{
				OpacityHandlerInterface.OnOpacityDidChange(this, Opacity);
			}
		}

		private void UpdateBlendColors()
		{
			if (Renderers == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			Color[] array = new Color[Renderers.Length];
			Color[] array2 = new Color[Renderers.Length];
			for (int i = 0; i < Renderers.Length; i++)
			{
				bool flag3 = Renderers[i].OverwriteFlagForDrawableMultiplyColors || OverwriteFlagForModelMultiplyColors;
				if (flag3)
				{
					if (!Renderers[i].LastIsUseUserMultiplyColor)
					{
						Renderers[i].MultiplyColor = Renderers[i].LastMultiplyColor;
						Renderers[i].ApplyMultiplyColor();
						flag = true;
					}
					else if (Renderers[i].LastMultiplyColor != Renderers[i].MultiplyColor)
					{
						Renderers[i].ApplyMultiplyColor();
						flag = true;
					}
					Renderers[i].LastMultiplyColor = Renderers[i].MultiplyColor;
				}
				else if (Renderers[i].LastIsUseUserMultiplyColor)
				{
					Renderers[i].MultiplyColor = Renderers[i].LastMultiplyColor;
					Renderers[i].ApplyMultiplyColor();
					flag = true;
				}
				array[i] = Renderers[i].MultiplyColor;
				Renderers[i].LastIsUseUserMultiplyColor = flag3;
				bool flag4 = Renderers[i].OverwriteFlagForDrawableScreenColors || OverwriteFlagForModelScreenColors;
				if (flag4)
				{
					if (!Renderers[i].LastIsUseUserScreenColors)
					{
						Renderers[i].ScreenColor = Renderers[i].LastScreenColor;
						Renderers[i].ApplyScreenColor();
						flag2 = true;
					}
					else if (Renderers[i].LastScreenColor != Renderers[i].ScreenColor)
					{
						Renderers[i].ApplyScreenColor();
						flag2 = true;
					}
					Renderers[i].LastScreenColor = Renderers[i].ScreenColor;
				}
				else if (Renderers[i].LastIsUseUserScreenColors)
				{
					Renderers[i].ScreenColor = Renderers[i].LastScreenColor;
					Renderers[i].ApplyScreenColor();
					flag2 = true;
				}
				array2[i] = Renderers[i].ScreenColor;
				Renderers[i].LastIsUseUserScreenColors = flag4;
			}
			if (MultiplyColorHandler != null && flag)
			{
				MultiplyColorHandlerInterface.OnBlendColorDidChange(this, array);
			}
			if (ScreenColorHandler != null && flag2)
			{
				ScreenColorHandlerInterface.OnBlendColorDidChange(this, array2);
			}
		}

		public void OnLateUpdate()
		{
			if (base.enabled)
			{
				UpdateOpacity();
				UpdateBlendColors();
				if (!(CameraToFace == null))
				{
					DrawablesRootTransform.rotation = Quaternion.LookRotation(CameraToFace.transform.forward, Vector3.up);
				}
			}
		}

		private void Start()
		{
			HasUpdateController = GetComponent<CubismUpdateController>() != null;
		}

		private void OnEnable()
		{
			if (!(Model == null))
			{
				TryInitializeRenderers();
				Model.OnDynamicDrawableData += OnDynamicDrawableData;
			}
		}

		private void OnDisable()
		{
			if (!(Model == null))
			{
				Model.OnDynamicDrawableData -= OnDynamicDrawableData;
			}
		}

		private void LateUpdate()
		{
			if (!HasUpdateController)
			{
				OnLateUpdate();
			}
		}

		private void OnDynamicDrawableData(CubismModel sender, CubismDynamicDrawableData[] data)
		{
			CubismDrawable[] drawables = sender.Drawables;
			CubismRenderer[] renderers = Renderers;
			for (int i = 0; i < data.Length; i++)
			{
				bool flag = false;
				renderers[i].UpdateVisibility();
				renderers[i].UpdateRenderOrder();
				if (data[i].IsAnyDirty)
				{
					if (data[i].IsVisibilityDirty)
					{
						renderers[i].OnDrawableVisiblityDidChange(data[i].IsVisible);
						flag = true;
					}
					if (data[i].IsRenderOrderDirty)
					{
						renderers[i].OnDrawableRenderOrderDidChange(data[i].RenderOrder);
						flag = true;
					}
					if (data[i].IsOpacityDirty)
					{
						renderers[i].OnDrawableOpacityDidChange(data[i].Opacity);
						flag = true;
					}
					if (data[i].AreVertexPositionsDirty)
					{
						renderers[i].OnDrawableVertexPositionsDidChange(data[i].VertexPositions);
						flag = true;
					}
					if (flag)
					{
						renderers[i].SwapMeshes();
					}
				}
			}
			ICubismDrawOrderHandler drawOrderHandlerInterface = DrawOrderHandlerInterface;
			if (drawOrderHandlerInterface != null)
			{
				for (int j = 0; j < data.Length; j++)
				{
					if (data[j].IsDrawOrderDirty)
					{
						drawOrderHandlerInterface.OnDrawOrderDidChange(this, drawables[j], data[j].DrawOrder);
					}
				}
			}
			bool flag2 = false;
			bool flag3 = false;
			Color[] array = new Color[Renderers.Length];
			Color[] array2 = new Color[Renderers.Length];
			for (int k = 0; k < data.Length; k++)
			{
				bool flag4 = !renderers[k].OverwriteFlagForDrawableMultiplyColors && !OverwriteFlagForModelMultiplyColors;
				if (data[k].IsBlendColorDirty && flag4)
				{
					renderers[k].ApplyMultiplyColor();
					flag2 = true;
				}
				array[k] = renderers[k].MultiplyColor;
			}
			for (int l = 0; l < data.Length; l++)
			{
				bool flag5 = !renderers[l].OverwriteFlagForDrawableScreenColors && !OverwriteFlagForModelScreenColors;
				if (data[l].IsBlendColorDirty && flag5)
				{
					renderers[l].ApplyScreenColor();
					flag3 = true;
				}
				array2[l] = renderers[l].ScreenColor;
			}
			ICubismBlendColorHandler multiplyColorHandlerInterface = MultiplyColorHandlerInterface;
			ICubismBlendColorHandler screenColorHandlerInterface = ScreenColorHandlerInterface;
			if (MultiplyColorHandler != null && flag2)
			{
				multiplyColorHandlerInterface.OnBlendColorDidChange(this, array);
			}
			if (ScreenColorHandler != null && flag3)
			{
				screenColorHandlerInterface.OnBlendColorDidChange(this, array2);
			}
		}
	}
}
