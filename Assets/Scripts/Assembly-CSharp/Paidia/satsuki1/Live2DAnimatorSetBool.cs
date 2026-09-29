using UnityEngine;

namespace Paidia.satsuki1
{
	public class Live2DAnimatorSetBool : MonoBehaviour
	{
		public Live2DAnimator Animator;

		public void SetBoolOn(string ParameterName)
		{
			Animator.SetBool(ParameterName, on: true);
		}

		public void SetBoolOff(string ParameterName)
		{
			Animator.SetBool(ParameterName, on: false);
		}
	}
}
