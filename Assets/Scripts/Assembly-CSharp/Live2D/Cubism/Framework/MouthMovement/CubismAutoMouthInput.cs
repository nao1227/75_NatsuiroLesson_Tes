using UnityEngine;

namespace Live2D.Cubism.Framework.MouthMovement
{
	public sealed class CubismAutoMouthInput : MonoBehaviour
	{
		[SerializeField]
		public float Timescale = 10f;

		private CubismMouthController Controller { get; set; }

		private float T { get; set; }

		public void Reset()
		{
			T = 0f;
		}

		private void Start()
		{
			Controller = GetComponent<CubismMouthController>();
		}

		private void LateUpdate()
		{
			if (!(Controller == null))
			{
				T += Time.deltaTime * Timescale;
				Controller.MouthOpening = Mathf.Abs(Mathf.Sin(T));
			}
		}
	}
}
