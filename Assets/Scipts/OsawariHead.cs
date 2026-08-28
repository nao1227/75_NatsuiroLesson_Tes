using System;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;
using Stubs;

public class OsawariHead : AbstractOsawari
{
	public int HeadXLower;

	public int HeadXUpper;

	public int HeadYLower;

	public int HeadYUpper;

	public float HeadXBase;

	public float HeadYBase;

	public int HeadXAutoRatio = 10;

	public int HeadYAutoRatio = 10;

	[SerializeField]
	private PhysicsCalculater PhysicsCalculater;

	private ParameterValue _headY;

	private ParameterValue _headX;

	public bool UsePhysics = true;

	public bool InactivateWhileNotClicked;

	private FaceController _face;

	private Subject<Unit> _onStroke = new Subject<Unit>();

	public IObservable<Unit> OnStroke => _onStroke;

	protected override void InitializeParams()
	{
		_face = GetComponent<FaceController>();
		_headY = new ParameterValue(parameters[ParameterName.HeadY]);
		_headX = new ParameterValue(parameters[ParameterName.HeadX]);
	}

	protected override void OnLateUpdate()
	{
		_headX = _headX.Update(Math.Min(Math.Max(_headX.Value, HeadXLower), HeadXUpper));
		_headY = _headY.Update(Math.Min(Math.Max(_headY.Value, HeadYLower), HeadYUpper));
		if (Mathf.Abs(_headX.Value - HeadXBase) < 0.01f && Mathf.Abs(_headY.Value - HeadYBase) < 0.01f)
		{
			InactivateLive2D(ParameterName.HeadY);
			if (InactivateWhileNotClicked)
			{
				InactivateLive2D(ParameterName.HeadX);
			}
		}
		else
		{
			SetLive2D(ParameterName.HeadY, _headY.Value);
			SetLive2D(ParameterName.HeadX, _headX.Value);
		}
	}

	public bool IsReturning()
	{
		if (!(Mathf.Abs(_headY.Value - HeadYBase) > 0f))
		{
			return Mathf.Abs(_headX.Value - HeadXBase) > 0f;
		}
		return true;
	}

	protected override void AutoAnimation()
	{
		if (IsAuto)
		{
			float num = (float)HeadYAutoRatio * Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) - _headY.Value;
			float num2 = (float)HeadXAutoRatio * Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) - _headX.Value;
			UpdateParams(new Vector3(num2 * SensitivityX, num * SensitivityY, 0f));
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return handType switch
		{
			HandType.Right => ParameterNumbers.GetTable()[ParameterName.RightHandOnHead], 
			HandType.Left => ParameterNumbers.GetTable()[ParameterName.LeftHandOnHead], 
			_ => throw new Exception(), 
		};
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (null != _face)
		{
			_face.AllowBlink = false;
		}
		if (move.magnitude > 0f)
		{
			_onStroke.OnNext(Unit.Default);
		}
		_headY += move.y / SensitivityY;
		_headX += move.x / SensitivityX;
		_manHand.Appear(GetActiveHand());
	}

	protected override void UpdateWhileNotClicked()
	{
		if (UsePhysics)
		{
			PhysicsCalculater.InvalidCalc(5, Switch: true);
			PhysicsCalculater.InvalidCalc(6, Switch: true);
			PhysicsCalculater.InvalidCalc(16, Switch: true);
			PhysicsCalculater.InvalidCalc(17, Switch: true);
		}
		if (IsReturning())
		{
			_headY -= (_headY.Value - HeadYBase) * 0.1f;
			_headX -= (_headX.Value - HeadXBase) * 0.1f;
		}
		_manHand.Disappear();
	}

	protected override void OnFirstClickCore()
	{
		base.OnFirstClickCore();
		_headX = _headX.Update(parameters[ParameterName.HeadX].Value);
		_headY = _headY.Update(parameters[ParameterName.HeadY].Value);
	}

	public override void OnMouseUp(bool fromCancel = false)
	{
		base.OnMouseUp(fromCancel);
		if (null != _face)
		{
			_face.AllowBlink = true;
		}
	}
}
