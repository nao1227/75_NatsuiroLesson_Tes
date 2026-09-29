using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	[Serializable]
	public class FaceParamCorrection
	{
		public TargetValues ExciteValues;

		public TargetValues AtomosphereValues;

		public TargetValues StimulusValues;

		public int EyeDirectionExciteThreshold = 500;

		public int EyeDirectionAtomosphereThreshold = 500;

		public int EyeDirectionStimulusThreshold = 500;

		public List<EyeDirection> EyeDirections;

		public float GetCorrectedValue(FaceParamNames name, float original, TemporaryStatus status)
		{
			int excite = status.Feelings.Excite;
			int value = status.Feelings.Atomosphere.Value;
			int stimulus = status.Feelings.Stimulus;
			float value2 = ExciteValues.GetValue(name);
			float value3 = AtomosphereValues.GetValue(name);
			float value4 = StimulusValues.GetValue(name);
			float num = new List<float> { value2, value3, value4 }.Count((float x) => x >= -90f);
			float num2 = 0f;
			float num3 = 0f;
			if (value2 > -90f)
			{
				num3 += value2 * (float)excite;
				num2 += (float)excite;
			}
			if (value3 > -90f)
			{
				num3 += value3 * (float)value;
				num2 += (float)value;
			}
			if (value4 > -90f)
			{
				num3 += value4 * (float)stimulus;
				num2 += (float)stimulus;
			}
			if (num > 0f)
			{
				num2 /= num * 100000f;
			}
			return (1f - num2) * original + num2 * num3 / 100000f;
		}

		public float GetEyeBallY(float original, TemporaryStatus status, float headY, bool sight)
		{
			if (sight)
			{
				return original;
			}
			if (status.Feelings.Excite > EyeDirectionExciteThreshold || status.Feelings.Stimulus > EyeDirectionStimulusThreshold)
			{
				return 1f;
			}
			if (status.Feelings.Atomosphere.Value > EyeDirectionAtomosphereThreshold)
			{
				return 0f;
			}
			return original;
		}

		public EyeDirection GetEyeDirection(float originalX, float originalY, TemporaryStatus status, bool sight)
		{
			FeelingParameterRange feelingParameterRange = default(FeelingParameterRange);
			EyeDirection result = new EyeDirection(originalX, originalY, feelingParameterRange, feelingParameterRange, feelingParameterRange);
			if (sight)
			{
				return result;
			}
			foreach (EyeDirection eyeDirection in EyeDirections)
			{
				if (eyeDirection.Fullfil(status))
				{
					return eyeDirection;
				}
			}
			return result;
		}
	}
}
