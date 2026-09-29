using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public struct TargetValues
	{
		public float FaceDirection;

		public EyeDirection EyeDirection;

		public float EyeOpen;

		public float EyeSize;

		public float EyeBrowType;

		public float EyeBrowUpDown;

		public float MouseType;

		public float MouseOpen;

		public float BreathSpeed;

		public float GetValue(FaceParamNames name)
		{
			return name switch
			{
				FaceParamNames.FaceDirection => FaceDirection, 
				FaceParamNames.EyeDirection => throw new ArgumentException("Eye direction is not supported."), 
				FaceParamNames.EyeOpen => EyeOpen, 
				FaceParamNames.EyeSize => EyeSize, 
				FaceParamNames.EyeBrowType => EyeBrowType, 
				FaceParamNames.EyeBrowUpDown => EyeBrowUpDown, 
				FaceParamNames.MouseType => MouseType, 
				FaceParamNames.MouseOpen => MouseOpen, 
				FaceParamNames.BreathSpeed => BreathSpeed, 
				_ => throw new NotSupportedException(), 
			};
		}
	}
}
