using System;
using UnityEngine.AddressableAssets;

namespace Paidia.satsuki1
{
	[Serializable]
	public class WeightedSE
	{
		public AssetReference SE;

		public int Weight;

		public int Channel = 1;
	}
}
