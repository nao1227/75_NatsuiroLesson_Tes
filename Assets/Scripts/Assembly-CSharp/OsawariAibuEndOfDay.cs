using Paidia.satsuki1;
using UnityEngine;

public class OsawariAibuEndOfDay : AbstractOsawari
{
	private ParameterValue _aibu;

	protected override void AutoAnimation()
	{
		_ = _aibu.Value;
		_aibu = _aibu.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) + 1f);
		foreach (Hand item in GetActiveHand())
		{
			_manHand.SetValue(item.HandType, 1f);
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (handType == HandType.Left)
		{
			return GetParameterNumber(ParameterName.LeftHandOnAibu);
		}
		return GetParameterNumber(ParameterName.RighthandOnAibu);
	}

	protected override void InitializeParams()
	{
		_aibu = new ParameterValue(parameters[ParameterName.Aibu]);
	}

	protected override void OnLateUpdate()
	{
		if (GetActiveHand().Count > 0 && GetActiveHand()[0].HandType == HandType.Left)
		{
			SetLive2D(ParameterName.Aibu, _aibu);
		}
		else
		{
			SetLive2D(ParameterName.AibuRight, _aibu);
		}
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (!IsAuto)
		{
			_aibu += move.y / SensitivityY;
			_manHand.Appear(GetActiveHand());
		}
	}

	protected override void UpdateWhileNotClicked()
	{
		_manHand.Disappear();
		_aibu -= 0.1f;
	}
}
