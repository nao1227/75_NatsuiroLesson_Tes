using System;
using Live2D.Cubism.Framework.Json;
using UnityEngine;

namespace Live2D.Cubism.Framework.Expression
{
	public class CubismExpressionData : ScriptableObject
	{
		[Serializable]
		public struct SerializableExpressionParameter
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public float Value;

			[SerializeField]
			public CubismParameterBlendMode Blend;
		}

		[SerializeField]
		public string Type;

		[SerializeField]
		public float FadeInTime;

		[SerializeField]
		public float FadeOutTime;

		[SerializeField]
		public SerializableExpressionParameter[] Parameters;

		public static CubismExpressionData CreateInstance(CubismExp3Json json)
		{
			return CreateInstance(ScriptableObject.CreateInstance<CubismExpressionData>(), json);
		}

		public static CubismExpressionData CreateInstance(CubismExpressionData expressionData, CubismExp3Json json)
		{
			expressionData.Type = json.Type;
			expressionData.FadeInTime = json.FadeInTime;
			expressionData.FadeOutTime = json.FadeOutTime;
			expressionData.Parameters = new SerializableExpressionParameter[json.Parameters.Length];
			for (int i = 0; i < json.Parameters.Length; i++)
			{
				expressionData.Parameters[i].Id = json.Parameters[i].Id;
				expressionData.Parameters[i].Value = json.Parameters[i].Value;
				switch (json.Parameters[i].Blend)
				{
				case "Add":
					expressionData.Parameters[i].Blend = CubismParameterBlendMode.Additive;
					break;
				case "Multiply":
					expressionData.Parameters[i].Blend = CubismParameterBlendMode.Multiply;
					break;
				case "Overwrite":
					expressionData.Parameters[i].Blend = CubismParameterBlendMode.Override;
					break;
				default:
					expressionData.Parameters[i].Blend = CubismParameterBlendMode.Additive;
					break;
				}
			}
			return expressionData;
		}
	}
}
