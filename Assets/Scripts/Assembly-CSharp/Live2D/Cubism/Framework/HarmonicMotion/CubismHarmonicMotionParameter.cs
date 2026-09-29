using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.HarmonicMotion
{
	public sealed class CubismHarmonicMotionParameter : MonoBehaviour
	{
		[SerializeField]
		public int Channel;

		[SerializeField]
		public CubismHarmonicMotionDirection Direction;

		[SerializeField]
		[Range(0f, 1f)]
		public float NormalizedOrigin = 0.5f;

		[SerializeField]
		[Range(0f, 1f)]
		public float NormalizedRange = 0.5f;

		[SerializeField]
		[Range(0.01f, 10f)]
		public float Duration = 3f;

		private bool IsInitialized => Mathf.Abs(ValueRange) >= Mathf.Epsilon;

		private float MaximumValue { get; set; }

		private float MinimumValue { get; set; }

		private float ValueRange { get; set; }

		private float T { get; set; }

		private void Initialize()
		{
			CubismParameter component = GetComponent<CubismParameter>();
			MaximumValue = component.MaximumValue;
			MinimumValue = component.MinimumValue;
			ValueRange = MaximumValue - MinimumValue;
		}

		internal void Play(float[] channelTimescales)
		{
			T += Time.deltaTime * channelTimescales[Channel];
			while (T > Duration)
			{
				T -= Duration;
			}
		}

		internal float Evaluate()
		{
			if (!IsInitialized)
			{
				Initialize();
			}
			float origin = MinimumValue + NormalizedOrigin * ValueRange;
			float range = NormalizedRange * ValueRange;
			Clamp(ref origin, ref range);
			return origin + range * Mathf.Sin(T * ((float)Math.PI * 2f) / Duration);
		}

		private void Clamp(ref float origin, ref float range)
		{
			switch (Direction)
			{
			case CubismHarmonicMotionDirection.Left:
				if (origin - range >= MinimumValue)
				{
					range /= 2f;
					origin -= range;
				}
				else
				{
					range = (origin - MinimumValue) / 2f;
					origin = MinimumValue + range;
					NormalizedRange = range * 2f / ValueRange;
				}
				break;
			case CubismHarmonicMotionDirection.Right:
				if (origin + range <= MaximumValue)
				{
					range /= 2f;
					origin += range;
				}
				else
				{
					range = (MaximumValue - origin) / 2f;
					origin = MaximumValue - range;
					NormalizedRange = range * 2f / ValueRange;
				}
				break;
			}
			if (origin - range < MinimumValue)
			{
				range = origin - MinimumValue;
				NormalizedRange = range / ValueRange;
			}
			else if (origin + range > MaximumValue)
			{
				range = MaximumValue - origin;
				NormalizedRange = range / ValueRange;
			}
		}
	}
}
