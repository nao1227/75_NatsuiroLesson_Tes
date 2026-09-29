using Live2D.Cubism.Core;

public class Hand
{
	public readonly HandType HandType;

	private bool isAuto;

	public CubismParameter Parameter;

	public AbstractOsawari GrabbingTarget;

	public bool IsGrabbing => GrabbingTarget != null;

	public Hand(HandType handType)
	{
		HandType = handType;
	}

	public void Grab(AbstractOsawari target)
	{
		GrabbingTarget = target;
		Parameter = target.GetHandParameter(HandType);
	}

	public void Release()
	{
		GrabbingTarget = null;
	}
}
