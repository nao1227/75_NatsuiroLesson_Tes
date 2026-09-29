using UnityEngine;

public class MotionFinishBehaviourRibbon : StateMachineBehaviour
{
	[SerializeField]
	private string ModelObjectName;

	private GameObject _modelObject;

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	}
}
