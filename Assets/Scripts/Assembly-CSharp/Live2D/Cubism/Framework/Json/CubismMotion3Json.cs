using System;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Rendering;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	[Serializable]
	public sealed class CubismMotion3Json
	{
		private delegate void SegmentParser(float[] segments, List<Keyframe> result, ref int position);

		[Serializable]
		public struct SerializableMeta
		{
			[SerializeField]
			public float Duration;

			[SerializeField]
			public float Fps;

			[SerializeField]
			public bool Loop;

			[SerializeField]
			public int CurveCount;

			[SerializeField]
			public int TotalSegmentCount;

			[SerializeField]
			public int TotalPointCount;

			[SerializeField]
			public bool AreBeziersRestricted;

			[SerializeField]
			public int UserDataCount;

			[SerializeField]
			public int TotalUserDataSize;

			[SerializeField]
			public float FadeInTime;

			[SerializeField]
			public float FadeOutTime;
		}

		[Serializable]
		public struct SerializableCurve
		{
			[SerializeField]
			public string Target;

			[SerializeField]
			public string Id;

			[SerializeField]
			public float[] Segments;

			[SerializeField]
			public float FadeInTime;

			[SerializeField]
			public float FadeOutTime;
		}

		[Serializable]
		public struct SerializableUserData
		{
			[SerializeField]
			public float Time;

			[SerializeField]
			public string Value;
		}

		[SerializeField]
		public int Version;

		[SerializeField]
		public SerializableMeta Meta;

		[SerializeField]
		public SerializableCurve[] Curves;

		[SerializeField]
		public SerializableUserData[] UserData;

		private const float OffsetGranularity = 0.01f;

		private static Dictionary<float, SegmentParser> Parsers = new Dictionary<float, SegmentParser>
		{
			{ 0f, ParseLinearSegment },
			{ 1f, ParseBezierSegment },
			{ 2f, ParseSteppedSegment },
			{ 3f, ParseInverseSteppedSegment }
		};

		public static CubismMotion3Json LoadFrom(string motion3Json)
		{
			if (string.IsNullOrEmpty(motion3Json))
			{
				return null;
			}
			CubismMotion3Json cubismMotion3Json = JsonUtility.FromJson<CubismMotion3Json>(motion3Json);
			cubismMotion3Json.Meta.FadeInTime = -1f;
			cubismMotion3Json.Meta.FadeOutTime = -1f;
			for (int i = 0; i < cubismMotion3Json.Curves.Length; i++)
			{
				cubismMotion3Json.Curves[i].FadeInTime = -1f;
				cubismMotion3Json.Curves[i].FadeOutTime = -1f;
			}
			JsonUtility.FromJsonOverwrite(motion3Json, cubismMotion3Json);
			return cubismMotion3Json;
		}

		public static CubismMotion3Json LoadFrom(TextAsset motion3JsonAsset)
		{
			if (!(motion3JsonAsset == null))
			{
				return LoadFrom(motion3JsonAsset.text);
			}
			return null;
		}

		private CubismMotion3Json()
		{
		}

		public static Keyframe[] ConvertCurveSegmentsToKeyframes(float[] segments)
		{
			if (segments.Length < 1)
			{
				return new Keyframe[0];
			}
			List<Keyframe> list = new List<Keyframe>
			{
				new Keyframe(segments[0], segments[1])
			};
			int position = 2;
			while (position < segments.Length)
			{
				Parsers[segments[position]](segments, list, ref position);
			}
			return list.ToArray();
		}

		public static AnimationCurve ConvertSteppedCurveToLinerCurver(SerializableCurve curve, float poseFadeInTime)
		{
			poseFadeInTime = ((poseFadeInTime < 0f) ? 0.5f : poseFadeInTime);
			float[] array = curve.Segments;
			int num = 2;
			for (int i = 2; i < curve.Segments.Length; i += 3)
			{
				bool num2 = curve.Segments[i] == 2f;
				bool num3 = i == curve.Segments.Length - 3;
				bool flag = !num3 && curve.Segments[i + 3] == 2f;
				bool flag2 = !num3 && i + 3 == curve.Segments.Length - 3;
				if (num2 && (flag || flag2))
				{
					Array.Resize(ref array, array.Length + 3);
					array[num] = 0f;
					array[num + 1] = curve.Segments[i + 1];
					array[num + 2] = curve.Segments[i - 1];
					array[num + 3] = 0f;
					array[num + 4] = curve.Segments[i + 1] + poseFadeInTime;
					array[num + 5] = curve.Segments[i + 2];
					num += 6;
				}
				else if (curve.Segments[i] == 1f)
				{
					array[num] = curve.Segments[i];
					array[num + 1] = curve.Segments[i + 1];
					array[num + 2] = curve.Segments[i + 2];
					array[num + 3] = curve.Segments[i + 3];
					array[num + 4] = curve.Segments[i + 4];
					array[num + 5] = curve.Segments[i + 5];
					array[num + 6] = curve.Segments[i + 6];
					i += 4;
					num += 7;
				}
				else
				{
					array[num] = curve.Segments[i];
					array[num + 1] = curve.Segments[i + 1];
					array[num + 2] = curve.Segments[i + 2];
					num += 3;
				}
			}
			return new AnimationCurve(ConvertCurveSegmentsToKeyframes(array));
		}

		public AnimationClip ToAnimationClip(bool shouldImportAsOriginalWorkflow = false, bool shouldClearAnimationCurves = false, bool isCallFormModelJson = false, CubismPose3Json poseJson = null)
		{
			if (!Meta.AreBeziersRestricted)
			{
				Debug.LogWarning("Béziers are not restricted and curves might be off. Please export motions from Cubism in restricted mode for perfect match.");
			}
			AnimationClip animationClip = new AnimationClip
			{
				frameRate = Meta.Fps,
				legacy = true,
				wrapMode = (Meta.Loop ? WrapMode.Loop : WrapMode.Default)
			};
			return ToAnimationClip(animationClip, shouldImportAsOriginalWorkflow, shouldClearAnimationCurves, isCallFormModelJson, poseJson);
		}

		public AnimationClip ToAnimationClip(AnimationClip animationClip, bool shouldImportAsOriginalWorkflow = false, bool shouldClearAnimationCurves = false, bool isCallFormModelJson = false, CubismPose3Json poseJson = null)
		{
			if (shouldClearAnimationCurves && (!shouldImportAsOriginalWorkflow || (isCallFormModelJson && shouldImportAsOriginalWorkflow)))
			{
				animationClip.ClearCurves();
			}
			for (int i = 0; i < Curves.Length; i++)
			{
				SerializableCurve curve = Curves[i];
				if (curve.Target == "PartOpacity" && shouldImportAsOriginalWorkflow && !isCallFormModelJson)
				{
					continue;
				}
				string relativePath = string.Empty;
				Type type = null;
				string propertyName = string.Empty;
				AnimationCurve curve2 = new AnimationCurve(ConvertCurveSegmentsToKeyframes(curve.Segments));
				if (curve.Target == "Model")
				{
					if (curve.Id == "Opacity")
					{
						relativePath = string.Empty;
						propertyName = "Opacity";
						type = typeof(CubismRenderController);
					}
					else if (curve.Id == "EyeBlink")
					{
						relativePath = string.Empty;
						propertyName = "EyeOpening";
						type = typeof(CubismEyeBlinkController);
					}
					else if (curve.Id == "LipSync")
					{
						relativePath = string.Empty;
						propertyName = "MouthOpening";
						type = typeof(CubismMouthController);
					}
				}
				else if (curve.Target == "Parameter")
				{
					relativePath = "Parameters/" + curve.Id;
					propertyName = "Value";
					type = typeof(CubismParameter);
				}
				else if (curve.Target == "PartOpacity")
				{
					relativePath = "Parts/" + curve.Id;
					propertyName = "Opacity";
					type = typeof(CubismPart);
					if (shouldImportAsOriginalWorkflow && poseJson != null && poseJson.FadeInTime != 0f)
					{
						curve2 = ConvertSteppedCurveToLinerCurver(curve, poseJson.FadeInTime);
					}
				}
				animationClip.SetCurve(relativePath, type, propertyName, curve2);
			}
			return animationClip;
		}

		private static void ParseLinearSegment(float[] segments, List<Keyframe> result, ref int position)
		{
			float num = segments[position + 1] - result[result.Count - 1].time;
			float num2 = (segments[position + 2] - result[result.Count - 1].value) / num;
			float inTangent = num2;
			Keyframe value = new Keyframe(result[result.Count - 1].time, result[result.Count - 1].value, result[result.Count - 1].inTangent, num2);
			result[result.Count - 1] = value;
			value = new Keyframe(segments[position + 1], segments[position + 2], inTangent, 0f);
			result.Add(value);
			position += 3;
		}

		private static void ParseBezierSegment(float[] segments, List<Keyframe> result, ref int position)
		{
			float num = Mathf.Abs(result[result.Count - 1].time - segments[position + 5]) * 0.333333f;
			float outTangent = (segments[position + 2] - result[result.Count - 1].value) / num;
			float inTangent = (segments[position + 6] - segments[position + 4]) / num;
			Keyframe value = new Keyframe(result[result.Count - 1].time, result[result.Count - 1].value, result[result.Count - 1].inTangent, outTangent);
			result[result.Count - 1] = value;
			value = new Keyframe(segments[position + 5], segments[position + 6], inTangent, 0f);
			result.Add(value);
			position += 7;
		}

		private static void ParseSteppedSegment(float[] segments, List<Keyframe> result, ref int position)
		{
			result.Add(new Keyframe(segments[position + 1], segments[position + 2])
			{
				inTangent = float.PositiveInfinity
			});
			position += 3;
		}

		private static void ParseInverseSteppedSegment(float[] segments, List<Keyframe> result, ref int position)
		{
			Keyframe value = result[result.Count - 1];
			float inTangent = (value.outTangent = (float)Math.Atan2(segments[position + 2] - value.value, segments[position + 1] - value.time));
			result[result.Count - 1] = value;
			result.Add(new Keyframe(value.time + 0.01f, segments[position + 2])
			{
				inTangent = inTangent,
				outTangent = 0f
			});
			result.Add(new Keyframe(segments[position + 1], segments[position + 2])
			{
				inTangent = 0f
			});
			position += 3;
		}
	}
}
