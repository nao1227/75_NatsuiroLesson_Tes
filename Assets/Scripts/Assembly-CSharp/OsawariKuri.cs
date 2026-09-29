using Paidia.satsuki1;
using UnityEngine;

public class OsawariKuri : AbstractOsawari
{
	private ParameterValue _kuriX;

	private ParameterValue _kuriY;

	protected override void InitializeParams()
	{
		_kuriX = new ParameterValue(parameters[ParameterName.KuriX]);
		_kuriY = new ParameterValue(parameters[ParameterName.KuriY]);
	}

	protected override void AutoAnimation()
	{
		_kuriX = _kuriX.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
		_kuriY = _kuriY.Update(Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAuto)
		{
			_manHand.Disappear();
		}
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (!IsAuto)
		{
			_kuriX += move.x / SensitivityX;
			_kuriY += move.y / SensitivityY;
			_manHand.Appear(GetActiveHand());
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (handType == HandType.Left)
		{
			return ParameterNumbers.GetTable()[ParameterName.LeftHandOnKuri];
		}
		return ParameterNumbers.GetTable()[ParameterName.RightHandOnKuri];
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.KuriX, _kuriX);
		SetLive2D(ParameterName.KuriY, _kuriY);
	}

	public void CancellAnimaion()
	{
		IsAuto = false;
	}
}
