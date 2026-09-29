using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework
{
	public class CubismParameterStore : MonoBehaviour, ICubismUpdatable
	{
		private float[] _parameterValues;

		private float[] _partOpacities;

		private CubismParameter[] DestinationParameters { get; set; }

		private CubismPart[] DestinationParts { get; set; }

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismParameterStoreSaveParameters;

		public bool NeedsUpdateOnEditing => false;

		public void Refresh()
		{
			if (DestinationParameters == null)
			{
				DestinationParameters = this.FindCubismModel().Parameters;
			}
			if (DestinationParts == null)
			{
				DestinationParts = this.FindCubismModel().Parts;
			}
			HasUpdateController = GetComponent<CubismUpdateController>() != null;
			SaveParameters();
		}

		public void OnLateUpdate()
		{
			if (HasUpdateController)
			{
				SaveParameters();
			}
		}

		public void SaveParameters()
		{
			if (!base.enabled)
			{
				return;
			}
			if (DestinationParameters != null && _parameterValues == null)
			{
				_parameterValues = new float[DestinationParameters.Length];
			}
			if (_parameterValues != null)
			{
				for (int i = 0; i < _parameterValues.Length; i++)
				{
					_parameterValues[i] = DestinationParameters[i].Value;
				}
			}
			if (DestinationParts != null && _partOpacities == null)
			{
				_partOpacities = new float[DestinationParts.Length];
			}
			if (_partOpacities != null)
			{
				for (int j = 0; j < _partOpacities.Length; j++)
				{
					_partOpacities[j] = DestinationParts[j].Opacity;
				}
			}
		}

		public void RestoreParameters()
		{
			if (!base.enabled)
			{
				return;
			}
			if (_parameterValues != null)
			{
				for (int i = 0; i < _parameterValues.Length; i++)
				{
					DestinationParameters[i].Value = _parameterValues[i];
				}
			}
			if (_partOpacities != null)
			{
				for (int j = 0; j < _partOpacities.Length; j++)
				{
					DestinationParts[j].Opacity = _partOpacities[j];
				}
			}
		}

		private void OnEnable()
		{
			Refresh();
		}
	}
}
