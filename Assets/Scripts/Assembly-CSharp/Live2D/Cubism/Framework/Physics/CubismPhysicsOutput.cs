using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[Serializable]
	public struct CubismPhysicsOutput
	{
		public delegate float ValueGetter(Vector2 translation, CubismPhysicsParticle[] particles, int particleIndex, Vector2 gravity);

		public delegate float ScaleGetter();

		[SerializeField]
		public string DestinationId;

		[SerializeField]
		public int ParticleIndex;

		[SerializeField]
		public Vector2 TranslationScale;

		[SerializeField]
		public float AngleScale;

		[SerializeField]
		public float Weight;

		[SerializeField]
		public CubismPhysicsSourceComponent SourceComponent;

		[SerializeField]
		public bool IsInverted;

		[NonSerialized]
		public float ValueBelowMinimum;

		[NonSerialized]
		public float ValueExceededMaximum;

		[NonSerialized]
		public CubismParameter Destination;

		[NonSerialized]
		public ValueGetter GetValue;

		[NonSerialized]
		public ScaleGetter GetScale;

		private float GetOutputTranslationX(Vector2 translation, CubismPhysicsParticle[] particles, int particleIndex, Vector2 gravity)
		{
			float num = translation.x;
			if (IsInverted)
			{
				num *= -1f;
			}
			return num;
		}

		private float GetOutputTranslationY(Vector2 translation, CubismPhysicsParticle[] particles, int particleIndex, Vector2 gravity)
		{
			float num = translation.y;
			if (IsInverted)
			{
				num *= -1f;
			}
			return num;
		}

		private float GetOutputAngle(Vector2 translation, CubismPhysicsParticle[] particles, int particleIndex, Vector2 gravity)
		{
			Vector2 zero = Vector2.zero;
			if (CubismPhysics.UseAngleCorrection)
			{
				if (particleIndex < 2)
				{
					zero = gravity;
					zero.y *= -1f;
				}
				else
				{
					zero = particles[particleIndex - 1].Position - particles[particleIndex - 2].Position;
				}
			}
			else
			{
				zero = gravity;
				zero.y *= -1f;
			}
			float num = CubismPhysicsMath.DirectionToRadian(zero, translation);
			if (IsInverted)
			{
				num *= -1f;
			}
			return num;
		}

		private float GetOutputScaleTranslationX()
		{
			return TranslationScale.x;
		}

		private float GetOutputScaleTranslationY()
		{
			return TranslationScale.y;
		}

		private float GetOutputScaleAngle()
		{
			return AngleScale;
		}

		public void InitializeGetter()
		{
			switch (SourceComponent)
			{
			case CubismPhysicsSourceComponent.X:
				GetScale = GetOutputScaleTranslationX;
				GetValue = GetOutputTranslationX;
				break;
			case CubismPhysicsSourceComponent.Y:
				GetScale = GetOutputScaleTranslationY;
				GetValue = GetOutputTranslationY;
				break;
			case CubismPhysicsSourceComponent.Angle:
				GetScale = GetOutputScaleAngle;
				GetValue = GetOutputAngle;
				break;
			}
		}
	}
}
