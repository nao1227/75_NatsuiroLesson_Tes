using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.MouthMovement
{
	public sealed class CubismMouthController : MonoBehaviour, ICubismUpdatable
	{
		[SerializeField]
		public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Multiply;

		[SerializeField]
		[Range(0f, 1f)]
		public float MouthOpening = 1f;

		private CubismParameter[] Destinations { get; set; }

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismMouthController;

		public bool NeedsUpdateOnEditing => false;

		public void Refresh()
		{
			CubismModel cubismModel = this.FindCubismModel();
			if (!(cubismModel == null))
			{
				Component[] parameters = cubismModel.Parameters;
				CubismMouthParameter[] componentsMany = parameters.GetComponentsMany<CubismMouthParameter>();
				Destinations = new CubismParameter[componentsMany.Length];
				for (int i = 0; i < componentsMany.Length; i++)
				{
					Destinations[i] = componentsMany[i].GetComponent<CubismParameter>();
				}
				HasUpdateController = GetComponent<CubismUpdateController>() != null;
			}
		}

		public void OnLateUpdate()
		{
			if (base.enabled && Destinations != null)
			{
				Destinations.BlendToValue(BlendMode, MouthOpening);
			}
		}

		private void Start()
		{
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
