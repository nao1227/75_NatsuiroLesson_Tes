using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class WeightedStateList
	{
		public List<WeightedState> States;

		public int NoChangeProbability;

		public int Count => States.Count;

		public WeightedStateList()
		{
			States = new List<WeightedState>();
		}

		public FaceStateName GetRandomState(FaceStateName origin)
		{
			int maxExclusive = NoChangeProbability + States.Sum((WeightedState x) => x.Probability);
			int num = UnityEngine.Random.Range(0, maxExclusive);
			foreach (WeightedState state in States)
			{
				if (state.Probability > num)
				{
					return state.StateName;
				}
				num -= state.Probability;
			}
			return origin;
		}

		public bool Contains(FaceStateName state)
		{
			return States.Count((WeightedState x) => x.StateName == state) > 0;
		}

		public IEnumerator<WeightedState> GetEnumerator()
		{
			foreach (WeightedState state in States)
			{
				yield return state;
			}
		}
	}
}
