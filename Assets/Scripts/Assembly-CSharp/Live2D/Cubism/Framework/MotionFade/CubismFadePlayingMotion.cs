using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.MotionFade
{
	public struct CubismFadePlayingMotion
	{
		[SerializeField]
		public float StartTime;

		[SerializeField]
		public float EndTime;

		[SerializeField]
		public float FadeInStartTime;

		[SerializeField]
		[Range(0f, float.MaxValue)]
		public float Speed;

		[SerializeField]
		public CubismFadeMotionData Motion;

		[SerializeField]
		public bool IsLooping;

		[NonSerialized]
		public float Weight;
	}
}
