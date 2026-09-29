using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.MotionFade;
using UnityEngine;

namespace Live2D.Cubism.Framework.Expression
{
	public class CubismExpressionController : MonoBehaviour, ICubismUpdatable
	{
		[SerializeField]
		public CubismExpressionList ExpressionsList;

		private CubismModel _model;

		private List<CubismPlayingExpression> _playingExpressions = new List<CubismPlayingExpression>();

		[SerializeField]
		public int CurrentExpressionIndex = -1;

		private int _lastExpressionIndex = -1;

		[HideInInspector]
		public bool HasUpdateController { get; set; }

		public int ExecutionOrder => CubismUpdateExecutionOrder.CubismExpressionController;

		public bool NeedsUpdateOnEditing => false;

		private void StartExpression()
		{
			if (ExpressionsList == null || ExpressionsList.CubismExpressionObjects == null)
			{
				return;
			}
			_lastExpressionIndex = CurrentExpressionIndex;
			if (_playingExpressions.Count > 0)
			{
				CubismPlayingExpression cubismPlayingExpression = _playingExpressions[_playingExpressions.Count - 1];
				cubismPlayingExpression.ExpressionEndTime = cubismPlayingExpression.ExpressionUserTime + cubismPlayingExpression.FadeOutTime;
				_playingExpressions[_playingExpressions.Count - 1] = cubismPlayingExpression;
			}
			if (CurrentExpressionIndex >= 0 && CurrentExpressionIndex < ExpressionsList.CubismExpressionObjects.Length)
			{
				CubismPlayingExpression cubismPlayingExpression2 = CubismPlayingExpression.Create(_model, ExpressionsList.CubismExpressionObjects[CurrentExpressionIndex]);
				if (cubismPlayingExpression2 != null)
				{
					_playingExpressions.Add(cubismPlayingExpression2);
				}
			}
		}

		public void OnLateUpdate()
		{
			if (!base.enabled || _model == null)
			{
				return;
			}
			if (CurrentExpressionIndex != _lastExpressionIndex)
			{
				StartExpression();
			}
			for (int i = 0; i < _playingExpressions.Count; i++)
			{
				CubismPlayingExpression cubismPlayingExpression = _playingExpressions[i];
				cubismPlayingExpression.ExpressionUserTime += Time.deltaTime;
				float num = ((Mathf.Abs(cubismPlayingExpression.FadeInTime) < float.Epsilon) ? 1f : CubismFadeMath.GetEasingSine(cubismPlayingExpression.ExpressionUserTime / cubismPlayingExpression.FadeInTime));
				float num2 = ((Mathf.Abs(cubismPlayingExpression.ExpressionEndTime) < float.Epsilon || cubismPlayingExpression.ExpressionEndTime < 0f) ? 1f : CubismFadeMath.GetEasingSine((cubismPlayingExpression.ExpressionEndTime - cubismPlayingExpression.ExpressionUserTime) / cubismPlayingExpression.FadeOutTime));
				cubismPlayingExpression.Weight = num * num2;
				for (int j = 0; j < cubismPlayingExpression.Destinations.Length; j++)
				{
					if (!(cubismPlayingExpression.Destinations[j] == null))
					{
						switch (cubismPlayingExpression.Blend[j])
						{
						case CubismParameterBlendMode.Additive:
							cubismPlayingExpression.Destinations[j].AddToValue(cubismPlayingExpression.Value[j], cubismPlayingExpression.Weight);
							break;
						case CubismParameterBlendMode.Multiply:
							cubismPlayingExpression.Destinations[j].MultiplyValueBy(cubismPlayingExpression.Value[j], cubismPlayingExpression.Weight);
							break;
						case CubismParameterBlendMode.Override:
							cubismPlayingExpression.Destinations[j].Value = cubismPlayingExpression.Destinations[j].Value * (1f - cubismPlayingExpression.Weight) + cubismPlayingExpression.Value[j] * cubismPlayingExpression.Weight;
							break;
						}
					}
				}
				_playingExpressions[i] = cubismPlayingExpression;
			}
			for (int num3 = _playingExpressions.Count - 1; num3 >= 0; num3--)
			{
				if (!(_playingExpressions[num3].Weight > 0f))
				{
					_playingExpressions.RemoveAt(num3);
				}
			}
		}

		private void OnEnable()
		{
			_model = this.FindCubismModel();
			HasUpdateController = GetComponent<CubismUpdateController>() != null;
		}

		private void LateUpdate()
		{
			if (!HasUpdateController)
			{
				OnLateUpdate();
			}
		}
	}
}
