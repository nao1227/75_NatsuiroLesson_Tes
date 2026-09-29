using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[Serializable]
	public struct CubismPhysicsNormalization
	{
		[SerializeField]
		public CubismPhysicsNormalizationTuplet Position;

		[SerializeField]
		public CubismPhysicsNormalizationTuplet Angle;
	}
}
