using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class OsawariRibbon : OsawariWithAnimation, ISwitchable, IDoubleHanded
{
	private bool _animeDone;

	private bool _isRibbonShown;

	private ParameterValue _ribbon;

	private float _impact;

	private bool _impactFlag;

	[SerializeField]
	private float RibbonRealmUpper = 2f;

	[SerializeField]
	private float RibbonRealmLower;

	public bool Touchable => _isRibbonShown;

	protected override void InitializeParams()
	{
		_ribbon = new ParameterValue(parameters[ParameterName.Ribbon]);
	}

	protected override void OnLateUpdate()
	{
		if (!IsAnimating)
		{
			SetLive2D(ParameterName.Ribbon, _ribbon);
			if (_impactFlag && _impact > 1f)
			{
				RibbonTakeOff();
			}
		}
		else
		{
			InactivateLive2D(ParameterName.Ribbon);
			_ribbon = _ribbon.Update(parameters[ParameterName.Ribbon].Value);
		}
	}

	private async void RibbonTakeOff()
	{
		await StartAnimation(AnimeName.RibbonTakeOff, on: true);
		SetAnimationStatusOffNoYield(AnimeName.RibbonSwitch);
		await SetAnimationStatusOff(AnimeName.RibbonTakeOff);
		_isRibbonShown = false;
		_impactFlag = false;
	}

	public void OnRibbon()
	{
		StartAnimation(AnimeName.RibbonSwitch, on: true).Forget();
		_ribbon = _ribbon.Update(RibbonRealmLower);
		_isRibbonShown = true;
	}

	public void OffRibbon()
	{
		SetAnimationStatusOff(AnimeName.RibbonSwitch).Forget();
		_ribbon = _ribbon.Update(RibbonRealmUpper);
		_isRibbonShown = false;
	}

	public void SwitchRibbon()
	{
		if (_isRibbonShown)
		{
			OffRibbon();
			return;
		}
		OnRibbon();
		_isRibbonShown = true;
	}

	public bool GetRibbonFlag()
	{
		return _isRibbonShown;
	}

	private IEnumerator _StateListener()
	{
		_animeDone = true;
		yield return null;
	}

	public void StateListener()
	{
		StartCoroutine(_StateListener());
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (!IsAnimating)
		{
			float impact = Vector3.Magnitude(move / Time.deltaTime * 10f * 60f / base.fps);
			if (_manHand.GetValue(HandType.Left) == 1f)
			{
				_impactFlag = true;
				_impact = impact;
			}
			_manHand.Appear();
		}
		else if (IsAnimating)
		{
			_ribbon = _ribbon.Update(parameters[ParameterName.Ribbon].Value);
		}
	}

	protected override void AutoAnimation()
	{
	}

	protected override void UpdateWhileNotClicked()
	{
		_manHand.Disappear();
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return ParameterNumbers.GetTable()[ParameterName.HandOnRibbon];
	}

	protected override bool GetConstraintsCore()
	{
		return _ribbon.Value < 2f;
	}
}
