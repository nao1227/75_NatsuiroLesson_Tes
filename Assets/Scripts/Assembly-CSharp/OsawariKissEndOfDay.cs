using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariKissEndOfDay : OsawariKiss
{
	private ParameterValue _kissX;

	private ParameterValue _kissY;

	private ParameterValue _kissZ;

	private ParameterValue _tongue;

	private bool _isKissing;

	protected override void AutoAnimation()
	{
		_kissX = _kissX.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
		_kissY = _kissX.Update(Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return 0;
	}

	protected override void InitializeParams()
	{
		_kissX = new ParameterValue(parameters[ParameterName.KissX]);
		_kissY = new ParameterValue(parameters[ParameterName.KissY]);
		_kissZ = new ParameterValue(parameters[ParameterName.KissZ]);
		_tongue = new ParameterValue(parameters[ParameterName.Tongue]);
		OnKissStart = new Subject<Unit>();
		OnKissEnd = new Subject<Unit>();
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.KissX, _kissX);
		SetLive2D(ParameterName.KissY, _kissY);
		SetLive2D(ParameterName.Tongue, _tongue);
		if (!_isKissing)
		{
			SetLive2D(ParameterName.KissZ, 0f);
		}
		else
		{
			InactivateLive2D(ParameterName.KissZ);
		}
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (!_isKissing)
		{
			OnKissStart.OnNext(Unit.Default);
		}
		_kissX += move.x / SensitivityX;
		_kissY += move.y / SensitivityY;
		_isKissing = true;
		_tongue = _tongue.Update(1f - (1f - _tongue.Value) * 0.9f);
	}

	protected override void UpdateWhileNotClicked()
	{
		_kissX *= 0.9f;
		_kissY *= 0.9f;
		_tongue *= 0.9f;
		_isKissing = false;
		OnKissEnd.OnNext(Unit.Default);
	}

	protected override bool GetRestrictedCore()
	{
		if (!SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Mouth_Day5))
		{
			return SaveLoadManager.UnsavedData.Days < 6;
		}
		return false;
	}

	protected override bool GetConstraintsCore()
	{
		return true;
	}
}
