using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.HarmonicMotion
{
	public sealed class CubismHarmonicMotionController : MonoBehaviour, ICubismUpdatable
	{
		private const int DefaultChannelCount = 1;

		[SerializeField]
		public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Additive;

		[SerializeField]
		public float[] ChannelTimescales;

		private CubismHarmonicMotionParameter[] Sources { get; set; }

		private CubismParameter[] Destinations { get; set; }

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismHarmonicMotionController;

		public bool NeedsUpdateOnEditing => false;

		public void Refresh()
		{
			CubismModel cubismModel = this.FindCubismModel();
			Component[] parameters = cubismModel.Parameters;
			Sources = parameters.GetComponentsMany<CubismHarmonicMotionParameter>();
			Destinations = new CubismParameter[Sources.Length];
			for (int i = 0; i < Sources.Length; i++)
			{
				Destinations[i] = Sources[i].GetComponent<CubismParameter>();
			}
			HasUpdateController = GetComponent<CubismUpdateController>() != null;
		}

		public void OnLateUpdate()
		{
			if (base.enabled && Sources != null)
			{
				for (int i = 0; i < Sources.Length; i++)
				{
					Sources[i].Play(ChannelTimescales);
					Destinations[i].BlendToValue(BlendMode, Sources[i].Evaluate());
				}
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

		private void Reset()
		{
			ChannelTimescales = new float[1];
			for (int i = 0; i < 1; i++)
			{
				ChannelTimescales[i] = 1f;
			}
		}
	}
}
