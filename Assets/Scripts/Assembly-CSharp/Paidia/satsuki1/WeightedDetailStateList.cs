using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class WeightedDetailStateList
	{
		public List<WeightedDetailState> States;

		public int NoChangeProbability;

		public int Count => States.Count;

		public DetailStateName GetRandomState(DetailStateName origin)
		{
			int num = NoChangeProbability;
			if (States.Count((WeightedDetailState x) => x.StateName == origin) > 0)
			{
				num -= States.First((WeightedDetailState x) => x.StateName == origin).Probability;
			}
			int maxExclusive = num + States.Sum((WeightedDetailState x) => x.Probability);
			int num2 = UnityEngine.Random.Range(0, maxExclusive);
			foreach (WeightedDetailState state in States)
			{
				if (state.Probability > num2)
				{
					return state.StateName;
				}
				num2 -= state.Probability;
			}
			return origin;
		}

		public bool Contains(DetailStateName state)
		{
			return States.Count((WeightedDetailState x) => x.StateName == state) > 0;
		}

		public IEnumerator<WeightedDetailState> GetEnumerator()
		{
			foreach (WeightedDetailState state in States)
			{
				yield return state;
			}
		}
	}
}
