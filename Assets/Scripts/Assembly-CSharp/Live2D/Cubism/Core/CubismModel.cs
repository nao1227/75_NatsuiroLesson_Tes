using System;
using Live2D.Cubism.Framework;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace Live2D.Cubism.Core
{
	[ExecuteInEditMode]
	[CubismDontMoveOnReimport]
	public sealed class CubismModel : MonoBehaviour
	{
		public delegate void DynamicDrawableDataHandler(CubismModel sender, CubismDynamicDrawableData[] data);

		[SerializeField]
		[HideInInspector]
		private CubismMoc _moc;

		[NonSerialized]
		private CubismParameter[] _parameters;

		[NonSerialized]
		private CubismPart[] _parts;

		[NonSerialized]
		private CubismDrawable[] _drawables;

		[NonSerialized]
		private CubismCanvasInformation _canvasInformation;

		private CubismParameterStore _parameterStore;

		[NonSerialized]
		private static Action _modelUpdateFunctions;

		public CubismMoc Moc
		{
			get
			{
				return _moc;
			}
			private set
			{
				_moc = value;
			}
		}

		private CubismTaskableModel TaskableModel { get; set; }

		public CubismParameter[] Parameters
		{
			get
			{
				if (_parameters == null)
				{
					Revive();
				}
				return _parameters;
			}
			private set
			{
				_parameters = value;
			}
		}

		public CubismPart[] Parts
		{
			get
			{
				if (_parts == null)
				{
					Revive();
				}
				return _parts;
			}
			private set
			{
				_parts = value;
			}
		}

		public CubismDrawable[] Drawables
		{
			get
			{
				if (_drawables == null)
				{
					Revive();
				}
				return _drawables;
			}
			private set
			{
				_drawables = value;
			}
		}

		public CubismCanvasInformation CanvasInformation
		{
			get
			{
				if (_canvasInformation == null)
				{
					Revive();
				}
				return _canvasInformation;
			}
			private set
			{
				_canvasInformation = value;
			}
		}

		public bool IsRevived => TaskableModel != null;

		private bool CanRevive => Moc != null;

		private bool WasAttachedModelUpdateFunction { get; set; }

		private bool WasJustEnabled { get; set; }

		private int LastTick { get; set; }

		public event DynamicDrawableDataHandler OnDynamicDrawableData;

		public static CubismModel InstantiateFrom(CubismMoc moc)
		{
			if (moc == null)
			{
				return null;
			}
			CubismModel cubismModel = new GameObject(moc.name).AddComponent<CubismModel>();
			cubismModel.Reset(moc);
			return cubismModel;
		}

		public static void ResetMocReference(CubismModel model, CubismMoc moc)
		{
			model.Moc = moc;
		}

		private void Revive()
		{
			if (!IsRevived && CanRevive)
			{
				TaskableModel = new CubismTaskableModel(Moc);
				if (TaskableModel != null && TaskableModel.UnmanagedModel != null)
				{
					Parameters = GetComponentsInChildren<CubismParameter>();
					Parts = GetComponentsInChildren<CubismPart>();
					Drawables = GetComponentsInChildren<CubismDrawable>();
					Parameters.Revive(TaskableModel.UnmanagedModel);
					Parts.Revive(TaskableModel.UnmanagedModel);
					Drawables.Revive(TaskableModel.UnmanagedModel);
					CanvasInformation = new CubismCanvasInformation(TaskableModel.UnmanagedModel);
					_parameterStore = GetComponent<CubismParameterStore>();
				}
			}
		}

		private void Reset(CubismMoc moc)
		{
			Moc = moc;
			base.name = moc.name;
			TaskableModel = new CubismTaskableModel(moc);
			if (TaskableModel != null && TaskableModel.UnmanagedModel != null)
			{
				GameObject gameObject = CubismParameter.CreateParameters(TaskableModel.UnmanagedModel);
				GameObject gameObject2 = CubismPart.CreateParts(TaskableModel.UnmanagedModel);
				GameObject gameObject3 = CubismDrawable.CreateDrawables(TaskableModel.UnmanagedModel);
				gameObject.transform.SetParent(base.transform);
				gameObject2.transform.SetParent(base.transform);
				gameObject3.transform.SetParent(base.transform);
				Parameters = gameObject.GetComponentsInChildren<CubismParameter>();
				Parts = gameObject2.GetComponentsInChildren<CubismPart>();
				Drawables = gameObject3.GetComponentsInChildren<CubismDrawable>();
				CanvasInformation = new CubismCanvasInformation(TaskableModel.UnmanagedModel);
			}
		}

		public void ForceUpdateNow()
		{
			WasJustEnabled = true;
			LastTick = -1;
			Revive();
			OnModelUpdate();
		}

		private static void OnModelsUpdate()
		{
			if (_modelUpdateFunctions != null)
			{
				_modelUpdateFunctions();
			}
		}

		[RuntimeInitializeOnLoadMethod]
		private static void RegisterCallbackFunction()
		{
			PlayerLoopSystem playerLoopSystem = new PlayerLoopSystem
			{
				type = typeof(CubismModel),
				updateDelegate = OnModelsUpdate
			};
			PlayerLoopSystem currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
			int num = -1;
			for (int i = 0; i < currentPlayerLoop.subSystemList.Length; i++)
			{
				if (!(currentPlayerLoop.subSystemList[i].type != typeof(PreLateUpdate)))
				{
					num = i;
					break;
				}
			}
			if (num < 0)
			{
				Debug.LogError("CubismModel : Failed to add processing to PlayerLoop.");
				return;
			}
			PlayerLoopSystem playerLoopSystem2 = currentPlayerLoop.subSystemList[num];
			PlayerLoopSystem[] array = playerLoopSystem2.subSystemList;
			Array.Resize(ref array, array.Length + 1);
			array[array.Length - 1] = playerLoopSystem;
			playerLoopSystem2.subSystemList = array;
			currentPlayerLoop.subSystemList[num] = playerLoopSystem2;
			PlayerLoop.SetPlayerLoop(currentPlayerLoop);
		}

		private void Update()
		{
			if (!WasAttachedModelUpdateFunction)
			{
				_modelUpdateFunctions = (Action)Delegate.Combine(_modelUpdateFunctions, new Action(OnModelUpdate));
				WasAttachedModelUpdateFunction = true;
			}
			if (!WasJustEnabled && IsRevived && TaskableModel.DidExecute)
			{
				TaskableModel.TryReadParameters(Parameters);
				if (_parameterStore != null)
				{
					_parameterStore.RestoreParameters();
				}
				if (this.OnDynamicDrawableData != null)
				{
					this.OnDynamicDrawableData(this, TaskableModel.DynamicDrawableData);
				}
			}
		}

		private void OnRenderObject()
		{
		}

		private void OnModelUpdate()
		{
			if (!IsRevived || (LastTick == Time.frameCount && Application.isPlaying))
			{
				return;
			}
			LastTick = Time.frameCount;
			TaskableModel.TryWriteParametersAndParts(Parameters, Parts);
			if (!TaskableModel.IsExecuting)
			{
				if (WasJustEnabled)
				{
					TaskableModel.UpdateNow();
					WasJustEnabled = false;
					Update();
				}
				else
				{
					TaskableModel.Update();
				}
			}
		}

		private void OnEnable()
		{
			WasJustEnabled = true;
			Revive();
		}

		private void OnDisable()
		{
			if (WasAttachedModelUpdateFunction)
			{
				_modelUpdateFunctions = (Action)Delegate.Remove(_modelUpdateFunctions, new Action(OnModelUpdate));
				WasAttachedModelUpdateFunction = false;
			}
		}

		private void OnDestroy()
		{
			if (IsRevived)
			{
				TaskableModel.ReleaseUnmanaged();
				TaskableModel = null;
			}
		}

		private void OnValidate()
		{
			OnEnable();
		}
	}
}
