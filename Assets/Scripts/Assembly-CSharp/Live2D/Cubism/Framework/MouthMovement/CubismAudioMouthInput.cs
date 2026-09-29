using UnityEngine;

namespace Live2D.Cubism.Framework.MouthMovement
{
	[RequireComponent(typeof(CubismMouthController))]
	public sealed class CubismAudioMouthInput : MonoBehaviour
	{
		[SerializeField]
		public AudioSource AudioInput;

		[SerializeField]
		public CubismAudioSamplingQuality SamplingQuality;

		[Range(1f, 10f)]
		public float Gain = 1f;

		[Range(0f, 1f)]
		public float Smoothing;

		private float VelocityBuffer;

		private float[] Samples { get; set; }

		private float LastRms { get; set; }

		private CubismMouthController Target { get; set; }

		private bool IsInitialized => Samples != null;

		private void TryInitialize()
		{
			if (!IsInitialized)
			{
				switch (SamplingQuality)
				{
				case CubismAudioSamplingQuality.VeryHigh:
					Samples = new float[256];
					break;
				case CubismAudioSamplingQuality.Maximum:
					Samples = new float[512];
					break;
				default:
					Samples = new float[256];
					break;
				}
				Target = GetComponent<CubismMouthController>();
			}
		}

		private void Update()
		{
			if (!(AudioInput == null))
			{
				float num = 0f;
				AudioInput.GetOutputData(Samples, 0);
				for (int i = 0; i < Samples.Length; i++)
				{
					float num2 = Samples[i];
					num += num2 * num2;
				}
				float value = Mathf.Sqrt(num / (float)Samples.Length) * Gain;
				value = Mathf.Clamp(value, 0f, 1f);
				value = Mathf.SmoothDamp(LastRms, value, ref VelocityBuffer, Smoothing * 0.1f);
				Target.MouthOpening = value;
				LastRms = value;
			}
		}

		private void OnEnable()
		{
			TryInitialize();
		}
	}
}
