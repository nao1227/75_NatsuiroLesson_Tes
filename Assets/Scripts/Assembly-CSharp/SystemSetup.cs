using UnityEngine;

public class SystemSetup : MonoBehaviour
{
	private void Awake()
	{
		Application.targetFrameRate = 60;
	}

	public void ChangeFrameRate(int _fps)
	{
		Application.targetFrameRate = _fps;
	}
}
