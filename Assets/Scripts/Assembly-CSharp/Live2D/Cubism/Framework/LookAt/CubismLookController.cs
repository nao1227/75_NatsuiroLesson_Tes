using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.LookAt
{
	public sealed class CubismLookController : MonoBehaviour, ICubismUpdatable
	{
		[SerializeField]
		public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Additive;

		[SerializeField]
		[HideInInspector]
		private Object _target;

		private ICubismLookTarget _targetInterface;

		public Transform Center;

		public float Damping = 0.15f;

		private Vector3 VelocityBuffer;

		public Object Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value.ToNullUnlessImplementsInterface<ICubismLookTarget>();
			}
		}

		private ICubismLookTarget TargetInterface
		{
			get
			{
				if (_targetInterface == null)
				{
					_targetInterface = Target.GetInterface<ICubismLookTarget>();
				}
				return _targetInterface;
			}
		}

		private CubismLookParameter[] Sources { get; set; }

		private CubismParameter[] Destinations { get; set; }

		private Vector3 LastPosition { get; set; }

		private Vector3 GoalPosition { get; set; }

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismLookController;

		public bool NeedsUpdateOnEditing => false;

		public void Refresh()
		{
			CubismModel cubismModel = this.FindCubismModel();
			Component[] parameters = cubismModel.Parameters;
			Sources = parameters.GetComponentsMany<CubismLookParameter>();
			Destinations = new CubismParameter[Sources.Length];
			for (int i = 0; i < Sources.Length; i++)
			{
				Destinations[i] = Sources[i].GetComponent<CubismParameter>();
			}
			HasUpdateController = GetComponent<CubismUpdateController>() != null;
		}

		public void OnLateUpdate()
		{
			if (!base.enabled || Destinations == null)
			{
				return;
			}
			ICubismLookTarget targetInterface = TargetInterface;
			if (targetInterface != null && targetInterface.IsActive())
			{
				Vector3 vector = LastPosition;
				GoalPosition = base.transform.InverseTransformPoint(targetInterface.GetPosition()) - Center.localPosition;
				if (vector != GoalPosition)
				{
					vector = Vector3.SmoothDamp(vector, GoalPosition, ref VelocityBuffer, Damping);
				}
				for (int i = 0; i < Destinations.Length; i++)
				{
					Destinations[i].BlendToValue(BlendMode, Sources[i].TickAndEvaluate(vector));
				}
				LastPosition = vector;
			}
		}

		private void Start()
		{
			if (Center == null)
			{
				Center = base.transform;
			}
			Refresh();
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
