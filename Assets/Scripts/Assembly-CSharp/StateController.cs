using UnityEngine;

public class StateController : StateMachineBehaviour
{
	[SerializeField]
	private string ModelObjectName;

	[SerializeField]
	private string[] MotionName;

	private GameObject _modelObject;

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		_modelObject = GameObject.Find(ModelObjectName);
		for (int i = 0; i < MotionName.Length; i++)
		{
			stateInfo.IsName(MotionName[i]);
		}
	}
}
