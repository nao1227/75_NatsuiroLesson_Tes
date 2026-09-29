using UnityEngine;

namespace Paidia.satsuki1
{
	public class VFXCameraController : MonoBehaviour
	{
		public Camera MainCamera;

		private Camera _vfxCamera;

		private void Start()
		{
			_vfxCamera = GetComponent<Camera>();
		}

		private void Update()
		{
			_vfxCamera.orthographicSize = MainCamera.orthographicSize;
		}
	}
}
