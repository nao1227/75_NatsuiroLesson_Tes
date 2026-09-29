using System;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	[Serializable]
	public struct WeightedDetailState
	{
		public DetailStateName StateName;

		public AssetReference Voice;

		public int Probability;
	}
}
