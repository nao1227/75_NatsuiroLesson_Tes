using UnityEngine;

public class MotionRandomizer : StateMachineBehaviour
{
	[SerializeField]
	private string ModelObjectName;

	private GameObject _modelObject;

	public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
	{
		_modelObject = GameObject.Find(ModelObjectName);
		_modelObject.GetComponent<Animator>().SetInteger("Random", Random.Range(0, 2));
	}
}
