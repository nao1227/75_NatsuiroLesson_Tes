using Live2D.Cubism.Framework.Json;
using UnityEngine;

namespace Live2D.Cubism.Framework.MotionFade
{
	public class CubismFadeMotionData : ScriptableObject
	{
		[SerializeField]
		public string MotionName;

		[SerializeField]
		public float FadeInTime;

		[SerializeField]
		public float FadeOutTime;

		[SerializeField]
		public string[] ParameterIds;

		[SerializeField]
		public AnimationCurve[] ParameterCurves;

		[SerializeField]
		public float[] ParameterFadeInTimes;

		[SerializeField]
		public float[] ParameterFadeOutTimes;

		[SerializeField]
		public float MotionLength;

		public static CubismFadeMotionData CreateInstance(CubismMotion3Json motion3Json, string motionName, float motionLength, bool shouldImportAsOriginalWorkflow = false, bool isCallFromModelJson = false)
		{
			CubismFadeMotionData cubismFadeMotionData = ScriptableObject.CreateInstance<CubismFadeMotionData>();
			int num = motion3Json.Curves.Length;
			cubismFadeMotionData.ParameterIds = new string[num];
			cubismFadeMotionData.ParameterFadeInTimes = new float[num];
			cubismFadeMotionData.ParameterFadeOutTimes = new float[num];
			cubismFadeMotionData.ParameterCurves = new AnimationCurve[num];
			return CreateInstance(cubismFadeMotionData, motion3Json, motionName, motionLength, shouldImportAsOriginalWorkflow, isCallFromModelJson);
		}

		public static CubismFadeMotionData CreateInstance(CubismFadeMotionData fadeMotion, CubismMotion3Json motion3Json, string motionName, float motionLength, bool shouldImportAsOriginalWorkflow = false, bool isCallFormModelJson = false)
		{
			fadeMotion.MotionName = motionName;
			fadeMotion.MotionLength = motionLength;
			fadeMotion.FadeInTime = ((motion3Json.Meta.FadeInTime < 0f) ? 1f : motion3Json.Meta.FadeInTime);
			fadeMotion.FadeOutTime = ((motion3Json.Meta.FadeOutTime < 0f) ? 1f : motion3Json.Meta.FadeOutTime);
			for (int i = 0; i < motion3Json.Curves.Length; i++)
			{
				CubismMotion3Json.SerializableCurve serializableCurve = motion3Json.Curves[i];
				if (!(serializableCurve.Target == "PartOpacity" && shouldImportAsOriginalWorkflow) || isCallFormModelJson)
				{
					fadeMotion.ParameterIds[i] = serializableCurve.Id;
					fadeMotion.ParameterFadeInTimes[i] = ((serializableCurve.FadeInTime < 0f) ? (-1f) : serializableCurve.FadeInTime);
					fadeMotion.ParameterFadeOutTimes[i] = ((serializableCurve.FadeOutTime < 0f) ? (-1f) : serializableCurve.FadeOutTime);
					fadeMotion.ParameterCurves[i] = new AnimationCurve(CubismMotion3Json.ConvertCurveSegmentsToKeyframes(serializableCurve.Segments));
				}
			}
			return fadeMotion;
		}
	}
}
