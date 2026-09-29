using UnityEngine;

public class MozaicController : MonoBehaviour
{
	[SerializeField]
	private Camera Camera;

	[SerializeField]
	private float DefaultBlursize = 0.5f;

	[SerializeField]
	private float Factor = 0.4f;

	private CameraController _cameraController;

	private Renderer _renderer;

	private float _differenceOrthogonal;

	private void Start()
	{
		_renderer = GetComponent<Renderer>();
		_cameraController = Camera.GetComponent<CameraController>();
		_differenceOrthogonal = _cameraController.GetOrthogonalInMinZoom() - _cameraController.GetOrthogonalInMaxZoom();
	}

	private void Update()
	{
		float num = (_cameraController.GetOrthogonalInMinZoom() - Camera.orthographicSize) / _differenceOrthogonal;
		float value = DefaultBlursize + num * Factor;
		_renderer.material.SetFloat("_blurSizeXY", value);
	}
}
