using UnityEngine;
using UnityEngine.UI;

public class FPSChecker : MonoBehaviour
{
	private int frameCount;

	private float prevTime;

	private float fps;

	private Text _textComponent;

	private void Start()
	{
		frameCount = 0;
		prevTime = 0f;
		fps = 60f;
		_textComponent = GetComponent<Text>();
		_textComponent.enabled = false;
	}

	public float GetFps()
	{
		return fps;
	}

	private void Update()
	{
	}
}
