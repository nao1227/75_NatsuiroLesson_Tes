using System;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	[Serializable]
	public struct WeightedState
	{
		public FaceStateName StateName;

		public AssetReference Voice;

		public int Probability;
	}
}
