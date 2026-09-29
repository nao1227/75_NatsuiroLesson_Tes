using System.Collections.Generic;

public class HandParamValue
{
	private const float APPEAR_SPEED = 0.1f;

	private const float DISAPPER_RATIO = 0.5f;

	private ParameterValue rightHandParameterValue;

	private ParameterValue leftHandParameterValue;

	public bool Locked { get; private set; }

	public HandParamValue()
	{
		rightHandParameterValue = ParameterValue.GetHandParameterValue();
		leftHandParameterValue = ParameterValue.GetHandParameterValue();
	}

	public float GetValue(HandType handType)
	{
		return Get(handType).Value;
	}

	public void SetValue(HandType handType, float val)
	{
		if (handType == HandType.Left)
		{
			leftHandParameterValue = leftHandParameterValue.Update(val);
		}
		else
		{
			rightHandParameterValue = rightHandParameterValue.Update(val);
		}
	}

	public ParameterValue Get(HandType handType)
	{
		if (handType == HandType.Left)
		{
			return leftHandParameterValue;
		}
		return rightHandParameterValue;
	}

	public void Appear(List<Hand> hands)
	{
		foreach (Hand hand in hands)
		{
			if (hand.HandType == HandType.Left)
			{
				leftHandParameterValue += 0.1f;
			}
			else
			{
				rightHandParameterValue += 0.1f;
			}
		}
	}

	public void Appear(HandType handType)
	{
		if (handType == HandType.Left)
		{
			leftHandParameterValue += 0.1f;
		}
		else
		{
			rightHandParameterValue += 0.1f;
		}
	}

	public void Appear()
	{
		leftHandParameterValue += 0.1f;
		rightHandParameterValue += 0.1f;
	}

	public void Disappear()
	{
		leftHandParameterValue *= 0.5f;
		rightHandParameterValue *= 0.5f;
	}

	public void Lock()
	{
		Locked = true;
	}

	public void Unlock()
	{
		Locked = false;
	}
}
