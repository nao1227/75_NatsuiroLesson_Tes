using Live2D.Cubism.Core;

public abstract class OsawariDoublehanded : AbstractOsawari
{
	public override void OnClick(CubismDrawable targetMesh, bool isFirst)
	{
		if (isFirst)
		{
			if (IsAuto)
			{
				IsAuto = false;
			}
			if (GetConstraintsCore() && _manager.CanGrab(this))
			{
				OnFirstClick();
				_manager.IsAction = true;
				if (IsGrabbable)
				{
					_handManager.Grab(HandType.Left, this);
					_handManager.Grab(HandType.Right, this);
				}
			}
		}
		if (GetConstraintsCore() && (_handManager.IsGrabbing(this) || (!IsGrabbable && !IsAnimating)))
		{
			UpdateParams(ConvertMovementVec3ForParams());
		}
	}
}
