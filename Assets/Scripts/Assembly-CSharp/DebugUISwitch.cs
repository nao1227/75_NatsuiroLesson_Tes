using UnityEngine;

public class DebugUISwitch : MonoBehaviour
{
	public GameObject DebugUI;

	private void Start()
	{
		DebugUI.SetActive(value: false);
	}

	public void SwitchDebugUIShow()
	{
		DebugUI.SetActive(!DebugUI.activeSelf);
	}

	private void Update()
	{
	}
}
