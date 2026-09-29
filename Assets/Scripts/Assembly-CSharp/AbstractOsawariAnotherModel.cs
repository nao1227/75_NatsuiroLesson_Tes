using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;

public abstract class AbstractOsawariAnotherModel : AbstractOsawari
{
	public CubismModel AnotherModel;

	public override CubismParameter GetHandParameter(HandType handType)
	{
		return AnotherModel.Parameters[GetHandParamIndex(handType)];
	}

	protected override void SetParameters()
	{
		parameters = new Dictionary<ParameterName, CubismParameter>();
		foreach (KeyValuePair<ParameterName, int> item in ParameterNumbers.GetTable())
		{
			parameters[item.Key] = AnotherModel.Parameters[item.Value];
		}
	}

	protected override void SetLive2D(ParameterName name, float val)
	{
		_manager.Preserver.SetValue(GetParameterNumber(name), val, CubismParameterBlendMode.Override, 1);
	}

	protected override void SetLive2D(ParameterName name, ParameterValue val)
	{
		SetLive2D(name, val.Value);
	}

	protected override void SetLive2D(int parameterId, float val)
	{
		_manager.Preserver.SetValue(parameterId, val, CubismParameterBlendMode.Override, 1);
	}

	protected override void InactivateLive2D(ParameterName name)
	{
		_manager.Preserver.InactivateValue(GetParameterNumber(name), 1);
	}

	protected override void UpdateHandParams()
	{
		foreach (Hand item in GetActiveHand())
		{
			ParameterValue paramValue = _manHand.Get(item.HandType);
			_handManager.MoveHand(item.HandType, paramValue, 1);
		}
	}
}
