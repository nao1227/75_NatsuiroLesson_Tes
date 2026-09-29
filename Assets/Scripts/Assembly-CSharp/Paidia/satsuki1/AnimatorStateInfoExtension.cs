using System;
using UnityEngine;

namespace Paidia.satsuki1
{
	public static class AnimatorStateInfoExtension
	{
		public static StateType GetStateName<StateType>(this AnimatorStateInfo info) where StateType : Enum
		{
			foreach (StateType value in Enum.GetValues(typeof(StateType)))
			{
				if (info.IsName(value.ToString()))
				{
					return value;
				}
			}
			return default(StateType);
		}
	}
}
