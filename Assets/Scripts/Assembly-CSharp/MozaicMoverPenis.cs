using Live2D.Cubism.Rendering;
using UnityEngine;

public class MozaicMoverPenis : MonoBehaviour
{
	[SerializeField]
	private GameObject PenisObject;

	[SerializeField]
	private Camera Camera;

	[SerializeField]
	private float PositionYFactor = -0.08f;

	[SerializeField]
	private bool Scaling = true;

	[SerializeField]
	private float ScalingYFactor = 0.4f;

	[SerializeField]
	private double MaxPosition = -0.1621856;

	[SerializeField]
	private double MinPosition = -0.3896137;

	private double _scale;

	private CubismRenderer _cubismRenderer;

	private CameraController _cameraController;

	private void Start()
	{
		_cubismRenderer = PenisObject.GetComponent<CubismRenderer>();
		_cameraController = Camera.GetComponent<CameraController>();
		Bounds bounds = _cubismRenderer.Mesh.bounds;
		base.transform.position = new Vector3(bounds.center.x, bounds.center.y + PositionYFactor, -1f);
		_scale = MaxPosition - MinPosition;
	}

	private void Update()
	{
		Bounds bounds = _cubismRenderer.Mesh.bounds;
		base.transform.position = new Vector3(bounds.center.x, bounds.center.y + PositionYFactor, -1f);
		if (Scaling)
		{
			float y = base.transform.position.y;
			double num = ((double)y - MinPosition) / _scale;
			if ((double)y <= MinPosition)
			{
				num = 0.0;
			}
			base.transform.localScale = new Vector3(1f, (float)((double)(ScalingYFactor - 1f) * num * num * num * num + 1.0), 1f);
		}
	}
}
