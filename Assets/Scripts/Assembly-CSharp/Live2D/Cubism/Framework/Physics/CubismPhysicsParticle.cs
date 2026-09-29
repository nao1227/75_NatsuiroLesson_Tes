using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[Serializable]
	public struct CubismPhysicsParticle
	{
		[SerializeField]
		public Vector2 InitialPosition;

		[SerializeField]
		public float Mobility;

		[SerializeField]
		public float Delay;

		[SerializeField]
		public float Acceleration;

		[SerializeField]
		public float Radius;

		[NonSerialized]
		public Vector2 Position;

		[NonSerialized]
		public Vector2 LastPosition;

		[NonSerialized]
		public Vector2 LastGravity;

		[NonSerialized]
		public Vector2 Force;

		[NonSerialized]
		public Vector2 Velocity;
	}
}
