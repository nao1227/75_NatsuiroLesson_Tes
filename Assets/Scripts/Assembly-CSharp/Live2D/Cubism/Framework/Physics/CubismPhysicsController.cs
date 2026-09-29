using System;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Rendering.Masking;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[CubismMoveOnReimportCopyComponentsOnly]
	public class CubismPhysicsController : MonoBehaviour, ICubismUpdatable
	{
		[SerializeField]
		private CubismPhysicsRig _rig;

		private CubismPhysicsRig Rig
		{
			get
			{
				return _rig;
			}
			set
			{
				_rig = value;
			}
		}

		public CubismParameter[] Parameters { get; private set; }

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismPhysicsController;

		public bool NeedsUpdateOnEditing => false;

		public void OnLateUpdate()
		{
			float deltaTime = Time.deltaTime;
			if (CubismPhysics.UseFixedDeltaTime)
			{
				deltaTime = Time.fixedDeltaTime;
			}
			Rig.Evaluate(deltaTime);
		}

		public void Stabilization()
		{
			Rig.Stabilization();
			CubismRenderController component = base.gameObject.GetComponent<CubismRenderController>();
			CubismMaskController component2 = base.gameObject.GetComponent<CubismMaskController>();
			component.OnLateUpdate();
			if ((bool)component2)
			{
				component2.OnLateUpdate();
			}
		}

		public void Initialize(CubismPhysicsRig rig)
		{
			Rig = rig;
			Awake();
		}

		public void SetPhysicsSubRigOutputAngleScaleRatio(CubismPhysicsSubRig subRig, float ratio)
		{
			if (subRig != null)
			{
				for (int i = 0; i < subRig.Output.Length; i++)
				{
					CubismPhysicsOutput cubismPhysicsOutput = subRig.OriginalOutput[i];
					subRig.Output[i].AngleScale = Math.Max(cubismPhysicsOutput.AngleScale * ratio, 0f);
					subRig.Output[i].InitializeGetter();
				}
			}
		}

		public void SetPhysicsSubRigOutputIsInverted(CubismPhysicsSubRig subRig, bool isInvert)
		{
			if (subRig != null)
			{
				for (int i = 0; i < subRig.Output.Length; i++)
				{
					CubismPhysicsOutput cubismPhysicsOutput = subRig.OriginalOutput[i];
					subRig.Output[i].IsInverted = (isInvert ? (!cubismPhysicsOutput.IsInverted) : cubismPhysicsOutput.IsInverted);
					subRig.Output[i].InitializeGetter();
				}
			}
		}

		public void Awake()
		{
			if (Rig != null)
			{
				Rig.Controller = this;
				for (int i = 0; i < Rig.SubRigs.Length; i++)
				{
					Rig.SubRigs[i].Rig = Rig;
				}
				Parameters = this.FindCubismModel().Parameters;
				Rig.Initialize();
			}
		}

		public void Start()
		{
			HasUpdateController = GetComponent<CubismUpdateController>() != null;
		}

		private void LateUpdate()
		{
			if (!HasUpdateController)
			{
				OnLateUpdate();
			}
		}
	}
}
