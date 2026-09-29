using UnityEngine;

namespace Paidia.satsuki1
{
	public class SubCamera : MonoBehaviour
	{
		private Camera _camera;

		private void Start()
		{
			_camera = GetComponent<Camera>();
		}

		private void Update()
		{
			_camera.rect = Camera.main.rect;
		}
	}
}
