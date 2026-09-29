using System;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.Expression
{
	[Serializable]
	public class CubismPlayingExpression
	{
		[SerializeField]
		public string Type;

		[SerializeField]
		public float FadeInTime;

		[SerializeField]
		public float FadeOutTime;

		[SerializeField]
		[Range(0f, 1f)]
		public float Weight;

		[SerializeField]
		public float ExpressionUserTime;

		[SerializeField]
		public float ExpressionEndTime;

		[SerializeField]
		public CubismParameter[] Destinations;

		[SerializeField]
		public float[] Value;

		[SerializeField]
		public CubismParameterBlendMode[] Blend;

		public static CubismPlayingExpression Create(CubismModel model, CubismExpressionData expressionData)
		{
			if (model == null || expressionData == null)
			{
				return null;
			}
			CubismPlayingExpression cubismPlayingExpression = new CubismPlayingExpression();
			cubismPlayingExpression.Type = expressionData.Type;
			cubismPlayingExpression.FadeInTime = ((expressionData.FadeInTime < 0f) ? 1f : expressionData.FadeInTime);
			cubismPlayingExpression.FadeOutTime = ((expressionData.FadeOutTime < 0f) ? 1f : expressionData.FadeOutTime);
			cubismPlayingExpression.Weight = 0f;
			cubismPlayingExpression.ExpressionUserTime = 0f;
			cubismPlayingExpression.ExpressionEndTime = 0f;
			int num = expressionData.Parameters.Length;
			cubismPlayingExpression.Destinations = new CubismParameter[num];
			cubismPlayingExpression.Value = new float[num];
			cubismPlayingExpression.Blend = new CubismParameterBlendMode[num];
			for (int i = 0; i < num; i++)
			{
				cubismPlayingExpression.Destinations[i] = model.Parameters.FindById(expressionData.Parameters[i].Id);
				cubismPlayingExpression.Value[i] = expressionData.Parameters[i].Value;
				cubismPlayingExpression.Blend[i] = expressionData.Parameters[i].Blend;
			}
			return cubismPlayingExpression;
		}
	}
}
