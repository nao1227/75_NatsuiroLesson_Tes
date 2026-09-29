using UnityEngine;

public class RandomBehaviour : StateMachineBehaviour
{
	private int hashRandom = Animator.StringToHash("random");

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		animator.SetInteger(hashRandom, Random.Range(0, 1));
	}
}
