using System;
using UnityEngine;

namespace Live2D.Cubism.Framework
{
	public sealed class CubismAutoEyeBlinkInput : MonoBehaviour
	{
		private enum Phase
		{
			Idling = 0,
			ClosingEyes = 1,
			OpeningEyes = 2
		}

		[SerializeField]
		[Range(1f, 10f)]
		public float Mean = 2.5f;

		[SerializeField]
		[Range(0.5f, 5f)]
		public float MaximumDeviation = 2f;

		[SerializeField]
		[Range(1f, 20f)]
		public float Timescale = 10f;

		private CubismEyeBlinkController Controller { get; set; }

		private float T { get; set; }

		private Phase CurrentPhase { get; set; }

		private float LastValue { get; set; }

		public void Reset()
		{
			T = 0f;
		}

		private void Start()
		{
			Controller = GetComponent<CubismEyeBlinkController>();
		}

		private void LateUpdate()
		{
			if (Controller == null)
			{
				return;
			}
			if (CurrentPhase == Phase.Idling)
			{
				T -= Time.deltaTime;
				if (!(T < 0f))
				{
					return;
				}
				T = -(float)Math.PI / 2f;
				LastValue = 1f;
				CurrentPhase = Phase.ClosingEyes;
			}
			T += Time.deltaTime * Timescale;
			float num = Mathf.Abs(Mathf.Sin(T));
			if (CurrentPhase == Phase.ClosingEyes && num > LastValue)
			{
				CurrentPhase = Phase.OpeningEyes;
			}
			else if (CurrentPhase == Phase.OpeningEyes && num < LastValue)
			{
				num = 1f;
				CurrentPhase = Phase.Idling;
				T = Mean + UnityEngine.Random.Range(0f - MaximumDeviation, MaximumDeviation);
			}
			Controller.EyeOpening = num;
			LastValue = num;
		}
	}
}
