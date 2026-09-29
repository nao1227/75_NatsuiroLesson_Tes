using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.Physics
{
	[Serializable]
	public struct CubismPhysicsNormalizationTuplet
	{
		[SerializeField]
		public float Maximum;

		[SerializeField]
		public float Minimum;

		[SerializeField]
		public float Default;
	}
}
