using System;
using Live2D.Cubism.Core;

public abstract class OsawariWithoutHand : AbstractOsawari
{
	public override void OnClick(CubismDrawable targetMesh, bool isFirst)
	{
		if (isFirst)
		{
			if (GetConstraintsCore() && !IsAuto)
			{
				OnFirstClick();
				_manager.IsAction = true;
			}
			else if (IsAuto && !_manager.IsAction)
			{
				_manager.IsAction = true;
				IsAuto = false;
			}
		}
		if (GetConstraintsCore())
		{
			UpdateParams(ConvertMovementVec3ForParams());
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		throw new NotImplementedException();
	}
}
