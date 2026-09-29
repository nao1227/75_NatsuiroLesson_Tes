using UnityEngine;

public class MotionFinishBehaviourFace : StateMachineBehaviour
{
	[SerializeField]
	private string ModelObjectName;

	private GameObject _modelObject;

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_modelObject = GameObject.Find(ModelObjectName);
		_modelObject.GetComponent<FaceController>().StateListener();
	}
}
