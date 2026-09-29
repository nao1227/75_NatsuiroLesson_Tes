using System;
using Live2D.Cubism.Core;
using UnityEngine;

public abstract class OsawariHelper : AbstractOsawari
{
	public virtual void OnUtageAnimation()
	{
	}

	public virtual void OnUtageAnimationFinished()
	{
	}

	public override void OnClick(CubismDrawable targetMesh, bool isFirst)
	{
	}

	public override CubismParameter GetHandParameter(HandType handType)
	{
		return base.GetHandParameter(handType);
	}

	protected override void AutoAnimation()
	{
		throw new NotImplementedException();
	}

	protected override void UpdateWhileNotClicked()
	{
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		throw new NotImplementedException();
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		throw new NotImplementedException();
	}

	protected override void OnLateUpdate()
	{
	}
}
