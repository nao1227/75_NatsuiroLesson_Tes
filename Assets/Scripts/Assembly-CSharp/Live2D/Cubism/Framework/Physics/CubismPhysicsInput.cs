using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[Serializable]
	public struct CubismPhysicsInput
	{
		public delegate void NormalizedParameterValueGetter(ref Vector2 targetTranslation, ref float targetAngle, CubismParameter parameter, ref float parameterValue, CubismPhysicsNormalization normalization, float weight);

		[SerializeField]
		public string SourceId;

		[SerializeField]
		public Vector2 ScaleOfTranslation;

		[SerializeField]
		public float AngleScale;

		[SerializeField]
		public float Weight;

		[SerializeField]
		public CubismPhysicsSourceComponent SourceComponent;

		[SerializeField]
		public bool IsInverted;

		[NonSerialized]
		public CubismParameter Source;

		[NonSerialized]
		public NormalizedParameterValueGetter GetNormalizedParameterValue;

		private void GetInputTranslationXFromNormalizedParameterValue(ref Vector2 targetTranslation, ref float targetAngle, CubismParameter parameter, ref float parameterValue, CubismPhysicsNormalization normalization, float weight)
		{
			targetTranslation.x += CubismPhysicsMath.Normalize(parameter, ref parameterValue, normalization.Position.Minimum, normalization.Position.Maximum, normalization.Position.Default, IsInverted) * weight;
		}

		private void GetInputTranslationYFromNormalizedParameterValue(ref Vector2 targetTranslation, ref float targetAngle, CubismParameter parameter, ref float parameterValue, CubismPhysicsNormalization normalization, float weight)
		{
			targetTranslation.y += CubismPhysicsMath.Normalize(parameter, ref parameterValue, normalization.Position.Minimum, normalization.Position.Maximum, normalization.Position.Default, IsInverted) * weight;
		}

		private void GetInputAngleFromNormalizedParameterValue(ref Vector2 targetTranslation, ref float targetAngle, CubismParameter parameter, ref float parameterValue, CubismPhysicsNormalization normalization, float weight)
		{
			targetAngle += CubismPhysicsMath.Normalize(parameter, ref parameterValue, normalization.Angle.Minimum, normalization.Angle.Maximum, normalization.Angle.Default, IsInverted) * weight;
		}

		public void InitializeGetter()
		{
			switch (SourceComponent)
			{
			case CubismPhysicsSourceComponent.X:
				GetNormalizedParameterValue = GetInputTranslationXFromNormalizedParameterValue;
				break;
			case CubismPhysicsSourceComponent.Y:
				GetNormalizedParameterValue = GetInputTranslationYFromNormalizedParameterValue;
				break;
			case CubismPhysicsSourceComponent.Angle:
				GetNormalizedParameterValue = GetInputAngleFromNormalizedParameterValue;
				break;
			}
		}
	}
}
