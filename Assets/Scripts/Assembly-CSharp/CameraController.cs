using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Transform tf;

	private Camera cam;

	[SerializeField]
	private float Zoom = 0.1f;

	[SerializeField]
	private float Move = 0.1f;

	[SerializeField]
	private float OrthogonalInMaxZoom = 0.223371f;

	[SerializeField]
	private float OrthogonalInMinZoom = 0.4513705f;

	[SerializeField]
	private float OrthogonalDefault = 0.4003706f;

	public float GetOrthogonalDefault()
	{
		return OrthogonalDefault;
	}

	public float GetOrthogonalInMaxZoom()
	{
		return OrthogonalInMaxZoom;
	}

	public float GetOrthogonalInMinZoom()
	{
		return OrthogonalInMinZoom;
	}

	private void Start()
	{
		tf = base.gameObject.GetComponent<Transform>();
		cam = base.gameObject.GetComponent<Camera>();
		cam.orthographicSize = OrthogonalDefault;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.I))
		{
			float num = cam.orthographicSize - Zoom;
			if (num <= OrthogonalInMaxZoom)
			{
				num = OrthogonalInMaxZoom;
			}
			else if (num >= OrthogonalInMinZoom)
			{
				num = OrthogonalInMinZoom;
			}
			cam.orthographicSize = num;
		}
		else if (Input.GetKey(KeyCode.O))
		{
			float num2 = cam.orthographicSize + Zoom;
			if (num2 <= OrthogonalInMaxZoom)
			{
				num2 = OrthogonalInMaxZoom;
			}
			else if (num2 >= OrthogonalInMinZoom)
			{
				num2 = OrthogonalInMinZoom;
			}
			cam.orthographicSize = num2;
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			tf.position += new Vector3(0f, Move, 0f);
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			tf.position += new Vector3(0f, 0f - Move, 0f);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			tf.position += new Vector3(0f - Move, 0f, 0f);
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			tf.position += new Vector3(Move, 0f, 0f);
		}
	}
}
