using System;

namespace Paidia.satsuki1
{
	[Serializable]
	public struct EyeDirection
	{
		public float X;

		public float Y;

		public FeelingParameterRange ExciteRange;

		public FeelingParameterRange AtomosphereRange;

		public FeelingParameterRange StimulusRange;

		public bool UseExcite;

		public bool UseAtomosphere;

		public bool UseStimulus;

		public EyeDirection(float x, float y, FeelingParameterRange excite, FeelingParameterRange atomosphere, FeelingParameterRange stimulus)
		{
			X = x;
			Y = y;
			ExciteRange = excite;
			AtomosphereRange = atomosphere;
			StimulusRange = stimulus;
			UseExcite = false;
			UseAtomosphere = false;
			UseStimulus = false;
		}

		public bool Fullfil(TemporaryStatus status)
		{
			if (!UseExcite && !UseAtomosphere && !UseStimulus)
			{
				return false;
			}
			if ((!UseExcite || ExciteRange.IsInRange(status.Feelings.Excite)) && (!UseAtomosphere || AtomosphereRange.IsInRange(status.Feelings.Atomosphere.Value)))
			{
				if (UseStimulus)
				{
					return StimulusRange.IsInRange(status.Feelings.Stimulus);
				}
				return true;
			}
			return false;
		}
	}
}
