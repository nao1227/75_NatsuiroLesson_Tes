using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[Serializable]
	public class CubismPhysicsSubRig
	{
		private struct SubRigPhysicsOutput
		{
			public float[] Output;
		}

		[SerializeField]
		public string Name;

		[SerializeField]
		public CubismPhysicsInput[] Input;

		[NonSerialized]
		public CubismPhysicsInput[] OriginalInput;

		[SerializeField]
		public CubismPhysicsOutput[] Output;

		[NonSerialized]
		public CubismPhysicsOutput[] OriginalOutput;

		[SerializeField]
		public CubismPhysicsParticle[] Particles;

		[SerializeField]
		public CubismPhysicsNormalization Normalization;

		[NonSerialized]
		private CubismPhysicsRig _rig;

		[NonSerialized]
		private SubRigPhysicsOutput _currentRigOutput;

		[NonSerialized]
		private SubRigPhysicsOutput _previousRigOutput;

		public CubismPhysicsRig Rig
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

		public void Interpolate(float weight)
		{
			for (int i = 0; i < Output.Length; i++)
			{
				if (Output[i].Destination == null)
				{
					CubismParameter cubismParameter = Rig.Controller.Parameters.FindById(Output[i].DestinationId);
					if (cubismParameter == null)
					{
						continue;
					}
					Output[i].Destination = cubismParameter;
				}
				UpdateOutputParameterValue(Output[i].Destination, ref Output[i].Destination.Value, _previousRigOutput.Output[i] * (1f - weight) + _currentRigOutput.Output[i] * weight, Output[i]);
			}
		}

		private void UpdateOutputParameterValue(CubismParameter parameter, ref float parameterValue, float translation, CubismPhysicsOutput output)
		{
			float num = 1f;
			num = output.GetScale();
			float num2 = translation * num;
			if (num2 < parameter.MinimumValue)
			{
				if (num2 < output.ValueBelowMinimum)
				{
					output.ValueBelowMinimum = num2;
				}
				num2 = parameter.MinimumValue;
			}
			else if (num2 > parameter.MaximumValue)
			{
				if (num2 > output.ValueExceededMaximum)
				{
					output.ValueExceededMaximum = num2;
				}
				num2 = parameter.MaximumValue;
			}
			float num3 = output.Weight / CubismPhysics.MaximumWeight;
			if (num3 >= 1f)
			{
				parameterValue = num2;
				return;
			}
			num2 = parameterValue * (1f - num3) + num2 * num3;
			parameterValue = num2;
		}

		private void UpdateParticles(CubismPhysicsParticle[] strand, Vector2 totalTranslation, float totalAngle, Vector2 wind, float thresholdValue, float deltaTime)
		{
			strand[0].Position = totalTranslation;
			Vector2 vector = CubismPhysicsMath.RadianToDirection(CubismPhysicsMath.DegreesToRadian(totalAngle));
			vector.Normalize();
			for (int i = 1; i < strand.Length; i++)
			{
				strand[i].Force = vector * strand[i].Acceleration + wind;
				strand[i].LastPosition = strand[i].Position;
				float num = strand[i].Delay * deltaTime * 30f;
				Vector2 vector2 = strand[i].Position - strand[i - 1].Position;
				float f = CubismPhysicsMath.DirectionToRadian(strand[i].LastGravity, vector) / CubismPhysics.AirResistance;
				vector2.x = Mathf.Cos(f) * vector2.x - vector2.y * Mathf.Sin(f);
				vector2.y = Mathf.Sin(f) * vector2.x + vector2.y * Mathf.Cos(f);
				strand[i].Position = strand[i - 1].Position + vector2;
				Vector2 vector3 = strand[i].Velocity * num;
				Vector2 vector4 = strand[i].Force * num * num;
				strand[i].Position = strand[i].Position + vector3 + vector4;
				Vector2 vector5 = strand[i].Position - strand[i - 1].Position;
				vector5.Normalize();
				strand[i].Position = strand[i - 1].Position + vector5 * strand[i].Radius;
				if (Mathf.Abs(strand[i].Position.x) < thresholdValue)
				{
					strand[i].Position.x = 0f;
				}
				if (num != 0f)
				{
					strand[i].Velocity = (strand[i].Position - strand[i].LastPosition) / num * strand[i].Mobility;
				}
				strand[i].Force = Vector2.zero;
				strand[i].LastGravity = vector;
			}
		}

		private void UpdateParticlesForStabilization(CubismPhysicsParticle[] strand, Vector2 totalTranslation, float totalAngle, Vector2 wind, float thresholdValue)
		{
			strand[0].Position = totalTranslation;
			Vector2 vector = CubismPhysicsMath.RadianToDirection(CubismPhysicsMath.DegreesToRadian(totalAngle));
			vector.Normalize();
			for (int i = 1; i < strand.Length; i++)
			{
				strand[i].Force = vector * strand[i].Acceleration + wind;
				strand[i].LastPosition = strand[i].Position;
				strand[i].Velocity = Vector2.zero;
				Vector2 force = strand[i].Force;
				force.Normalize();
				strand[i].Position = strand[i - 1].Position + force * strand[i].Radius;
				if (Mathf.Abs(strand[i].Position.x) < thresholdValue)
				{
					strand[i].Position.x = 0f;
				}
				strand[i].Force = Vector2.zero;
				strand[i].LastGravity = vector;
			}
		}

		public void Initialize()
		{
			CubismPhysicsParticle[] particles = Particles;
			particles[0].InitialPosition = Vector2.zero;
			particles[0].LastPosition = particles[0].InitialPosition;
			particles[0].LastGravity = Rig.Gravity;
			particles[0].LastGravity.y *= -1f;
			for (int i = 1; i < particles.Length; i++)
			{
				Vector2 zero = Vector2.zero;
				zero.y = particles[i].Radius;
				particles[i].InitialPosition = particles[i - 1].InitialPosition + zero;
				particles[i].Position = particles[i].InitialPosition;
				particles[i].LastPosition = particles[i].InitialPosition;
				particles[i].LastGravity = Rig.Gravity;
				particles[i].LastGravity.y *= -1f;
			}
			OriginalInput = new CubismPhysicsInput[Input.Length];
			for (int j = 0; j < Input.Length; j++)
			{
				OriginalInput[j] = Input[j];
				Input[j].InitializeGetter();
			}
			_previousRigOutput = default(SubRigPhysicsOutput);
			_currentRigOutput = default(SubRigPhysicsOutput);
			Array.Resize(ref _previousRigOutput.Output, Output.Length);
			Array.Resize(ref _currentRigOutput.Output, Output.Length);
			OriginalOutput = new CubismPhysicsOutput[Output.Length];
			for (int k = 0; k < Output.Length; k++)
			{
				OriginalOutput[k] = Output[k];
				Output[k].InitializeGetter();
			}
		}

		public void Evaluate(float deltaTime)
		{
			float targetAngle = 0f;
			Vector2 targetTranslation = Vector2.zero;
			for (int i = 0; i < Input.Length; i++)
			{
				float weight = Input[i].Weight / CubismPhysics.MaximumWeight;
				if (Input[i].Source == null)
				{
					Input[i].Source = Rig.Controller.Parameters.FindById(Input[i].SourceId);
				}
				int num = Array.IndexOf(Rig.Controller.Parameters, Input[i].Source);
				CubismParameter source = Input[i].Source;
				Input[i].GetNormalizedParameterValue(ref targetTranslation, ref targetAngle, source, ref Rig.ParametersCache[num], Normalization, weight);
			}
			float f = CubismPhysicsMath.DegreesToRadian(0f - targetAngle);
			targetTranslation.x = targetTranslation.x * Mathf.Cos(f) - targetTranslation.y * Mathf.Sin(f);
			targetTranslation.y = targetTranslation.x * Mathf.Sin(f) + targetTranslation.y * Mathf.Cos(f);
			UpdateParticles(Particles, targetTranslation, targetAngle, Rig.Wind, 0.001f * Normalization.Position.Maximum, deltaTime);
			for (int j = 0; j < Output.Length; j++)
			{
				_previousRigOutput.Output[j] = _currentRigOutput.Output[j];
				if (Output[j].Destination == null)
				{
					CubismParameter cubismParameter = Rig.Controller.Parameters.FindById(Output[j].DestinationId);
					if (cubismParameter == null)
					{
						continue;
					}
					Output[j].Destination = cubismParameter;
				}
				int particleIndex = Output[j].ParticleIndex;
				if (particleIndex >= 1 && particleIndex < Particles.Length)
				{
					int num2 = Array.IndexOf(Rig.Controller.Parameters, Output[j].Destination);
					Vector2 translation = Particles[particleIndex].Position - Particles[particleIndex - 1].Position;
					CubismParameter destination = Output[j].Destination;
					float num3 = Output[j].GetValue(translation, Particles, particleIndex, Rig.Gravity);
					_currentRigOutput.Output[j] = num3;
					UpdateOutputParameterValue(destination, ref Rig.ParametersCache[num2], num3, Output[j]);
				}
			}
		}

		public void Stabilization()
		{
			float targetAngle = 0f;
			Vector2 targetTranslation = Vector2.zero;
			for (int i = 0; i < Input.Length; i++)
			{
				float weight = Input[i].Weight / CubismPhysics.MaximumWeight;
				if (Input[i].Source == null)
				{
					Input[i].Source = Rig.Controller.Parameters.FindById(Input[i].SourceId);
				}
				int num = Array.IndexOf(Rig.Controller.Parameters, Input[i].Source);
				CubismParameter source = Input[i].Source;
				Input[i].GetNormalizedParameterValue(ref targetTranslation, ref targetAngle, source, ref Input[i].Source.Value, Normalization, weight);
				Rig.ParametersCache[num] = Input[i].Source.Value;
			}
			float f = CubismPhysicsMath.DegreesToRadian(0f - targetAngle);
			targetTranslation.x = targetTranslation.x * Mathf.Cos(f) - targetTranslation.y * Mathf.Sin(f);
			targetTranslation.y = targetTranslation.x * Mathf.Sin(f) + targetTranslation.y * Mathf.Cos(f);
			UpdateParticlesForStabilization(Particles, targetTranslation, targetAngle, Rig.Wind, 0.001f * Normalization.Position.Maximum);
			for (int j = 0; j < Output.Length; j++)
			{
				_previousRigOutput.Output[j] = _currentRigOutput.Output[j];
				if (Output[j].Destination == null)
				{
					CubismParameter cubismParameter = Rig.Controller.Parameters.FindById(Output[j].DestinationId);
					if (cubismParameter == null)
					{
						continue;
					}
					Output[j].Destination = cubismParameter;
				}
				int particleIndex = Output[j].ParticleIndex;
				if (particleIndex >= 1 && particleIndex < Particles.Length)
				{
					int num2 = Array.IndexOf(Rig.Controller.Parameters, Output[j].Destination);
					Vector2 translation = Particles[particleIndex].Position - Particles[particleIndex - 1].Position;
					CubismParameter destination = Output[j].Destination;
					float num3 = Output[j].GetValue(translation, Particles, particleIndex, Rig.Gravity);
					_currentRigOutput.Output[j] = num3;
					_previousRigOutput.Output[j] = num3;
					UpdateOutputParameterValue(destination, ref Output[j].Destination.Value, num3, Output[j]);
					Rig.ParametersCache[num2] = Output[j].Destination.Value;
				}
			}
		}
	}
}
