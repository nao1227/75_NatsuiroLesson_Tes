using System;
using Live2D.Cubism.Framework.Physics;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	[Serializable]
	public sealed class CubismPhysics3Json
	{
		[Serializable]
		public struct SerializableVector2
		{
			[SerializeField]
			public float X;

			[SerializeField]
			public float Y;
		}

		[Serializable]
		public struct SerializableNormalizationValue
		{
			[SerializeField]
			public float Minimum;

			[SerializeField]
			public float Default;

			[SerializeField]
			public float Maximum;
		}

		[Serializable]
		public struct SerializableParameter
		{
			[SerializeField]
			public string Target;

			[SerializeField]
			public string Id;
		}

		[Serializable]
		public struct SerializableInput
		{
			[SerializeField]
			public SerializableParameter Source;

			[SerializeField]
			public float Weight;

			[SerializeField]
			public string Type;

			[SerializeField]
			public bool Reflect;
		}

		[Serializable]
		public struct SerializableOutput
		{
			[SerializeField]
			public SerializableParameter Destination;

			[SerializeField]
			public int VertexIndex;

			[SerializeField]
			public float Scale;

			[SerializeField]
			public float Weight;

			[SerializeField]
			public string Type;

			[SerializeField]
			public bool Reflect;
		}

		[Serializable]
		public struct SerializableVertex
		{
			[SerializeField]
			public SerializableVector2 Position;

			[SerializeField]
			public float Mobility;

			[SerializeField]
			public float Delay;

			[SerializeField]
			public float Acceleration;

			[SerializeField]
			public float Radius;
		}

		[Serializable]
		public struct SerializableNormalization
		{
			[SerializeField]
			public SerializableNormalizationValue Position;

			[SerializeField]
			public SerializableNormalizationValue Angle;
		}

		[Serializable]
		public struct PhysicsDictionaryItem
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public string Name;
		}

		[Serializable]
		public struct SerializablePhysicsSettings
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public SerializableInput[] Input;

			[SerializeField]
			public SerializableOutput[] Output;

			[SerializeField]
			public SerializableVertex[] Vertices;

			[SerializeField]
			public SerializableNormalization Normalization;
		}

		[Serializable]
		public struct SerializableMeta
		{
			[SerializeField]
			public int PhysicsSettingCount;

			[SerializeField]
			public int TotalInputCount;

			[SerializeField]
			public int TotalOutputCount;

			[SerializeField]
			public int TotalVertexCount;

			[SerializeField]
			public SerializableEffectiveForces EffectiveForces;

			[SerializeField]
			public float Fps;

			[SerializeField]
			public PhysicsDictionaryItem[] PhysicsDictionary;
		}

		[Serializable]
		public struct SerializableEffectiveForces
		{
			[SerializeField]
			public SerializableVector2 Gravity;

			[SerializeField]
			public SerializableVector2 Wind;
		}

		[SerializeField]
		public int Version;

		[SerializeField]
		public SerializableMeta Meta;

		[SerializeField]
		public SerializablePhysicsSettings[] PhysicsSettings;

		public static CubismPhysics3Json LoadFrom(string physics3Json)
		{
			if (!string.IsNullOrEmpty(physics3Json))
			{
				return JsonUtility.FromJson<CubismPhysics3Json>(physics3Json);
			}
			return null;
		}

		public static CubismPhysics3Json LoadFrom(TextAsset physics3JsonAsset)
		{
			if (!(physics3JsonAsset == null))
			{
				return LoadFrom(physics3JsonAsset.text);
			}
			return null;
		}

		public CubismPhysicsRig ToRig()
		{
			CubismPhysicsRig cubismPhysicsRig = new CubismPhysicsRig();
			cubismPhysicsRig.Gravity.x = Meta.EffectiveForces.Gravity.X;
			cubismPhysicsRig.Gravity.y = Meta.EffectiveForces.Gravity.Y;
			cubismPhysicsRig.Wind.x = Meta.EffectiveForces.Wind.X;
			cubismPhysicsRig.Wind.y = Meta.EffectiveForces.Wind.Y;
			cubismPhysicsRig.Fps = Meta.Fps;
			cubismPhysicsRig.SubRigs = new CubismPhysicsSubRig[Meta.PhysicsSettingCount];
			PhysicsDictionaryItem[] physicsDictionary = Meta.PhysicsDictionary;
			for (int i = 0; i < cubismPhysicsRig.SubRigs.Length; i++)
			{
				cubismPhysicsRig.SubRigs[i] = new CubismPhysicsSubRig
				{
					Name = physicsDictionary[i].Name,
					Input = ReadInput(PhysicsSettings[i].Input),
					Output = ReadOutput(PhysicsSettings[i].Output),
					Particles = ReadParticles(PhysicsSettings[i].Vertices),
					Normalization = ReadNormalization(PhysicsSettings[i].Normalization)
				};
			}
			return cubismPhysicsRig;
		}

		private CubismPhysicsInput[] ReadInput(SerializableInput[] source)
		{
			CubismPhysicsInput[] array = new CubismPhysicsInput[source.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new CubismPhysicsInput
				{
					SourceId = source[i].Source.Id,
					AngleScale = 0f,
					ScaleOfTranslation = Vector2.zero,
					Weight = source[i].Weight,
					SourceComponent = (CubismPhysicsSourceComponent)Enum.Parse(typeof(CubismPhysicsSourceComponent), source[i].Type),
					IsInverted = source[i].Reflect
				};
			}
			return array;
		}

		private CubismPhysicsOutput[] ReadOutput(SerializableOutput[] source)
		{
			CubismPhysicsOutput[] array = new CubismPhysicsOutput[source.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new CubismPhysicsOutput
				{
					DestinationId = source[i].Destination.Id,
					ParticleIndex = source[i].VertexIndex,
					TranslationScale = Vector2.zero,
					AngleScale = source[i].Scale,
					Weight = source[i].Weight,
					SourceComponent = (CubismPhysicsSourceComponent)Enum.Parse(typeof(CubismPhysicsSourceComponent), source[i].Type),
					IsInverted = source[i].Reflect,
					ValueBelowMinimum = 0f,
					ValueExceededMaximum = 0f
				};
			}
			return array;
		}

		private CubismPhysicsParticle[] ReadParticles(SerializableVertex[] source)
		{
			CubismPhysicsParticle[] array = new CubismPhysicsParticle[source.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new CubismPhysicsParticle
				{
					InitialPosition = 
					{
						x = source[i].Position.X,
						y = source[i].Position.Y
					},
					Mobility = source[i].Mobility,
					Delay = source[i].Delay,
					Acceleration = source[i].Acceleration,
					Radius = source[i].Radius,
					Position = Vector2.zero,
					LastPosition = Vector2.zero,
					LastGravity = Vector2.down,
					Force = Vector2.zero,
					Velocity = Vector2.zero
				};
			}
			return array;
		}

		private CubismPhysicsNormalization ReadNormalization(SerializableNormalization source)
		{
			return new CubismPhysicsNormalization
			{
				Position = 
				{
					Maximum = source.Position.Maximum,
					Minimum = source.Position.Minimum,
					Default = source.Position.Default
				},
				Angle = 
				{
					Maximum = source.Angle.Maximum,
					Minimum = source.Angle.Minimum,
					Default = source.Angle.Default
				}
			};
		}
	}
}
