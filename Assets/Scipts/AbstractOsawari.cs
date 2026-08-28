using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;

using Paidia.satsuki1;
using UnityEngine;
using Stubs;    

public abstract class AbstractOsawari : MonoBehaviour
{
	protected Dictionary<ParameterName, CubismParameter> parameters;

	protected OsawariManager _manager;

	protected HandManager _handManager;

	public bool IsGrabbable = true;

	[SerializeField]
	protected float ParameterAngle;

	[SerializeField]
	protected ParameterDictionary ParameterNumbers;

	[SerializeField]
	public CubismDrawable Mesh;

	[SerializeField]
	protected float SensitivityX = 50f;

	[SerializeField]
	protected float SensitivityY = 50f;

	[SerializeField]
	protected float AnimationSpeed = 1f;

	[SerializeField]
	public int Priority;

	public CubismDrawable[] TouchableMeshs;

	protected Vector3 _exdash;

	protected Vector3 _eydash;

	protected Vector3 _mousePositionOnLastFrame;

	protected HandParamValue _manHand;

	private bool initialized;

	protected CancellationToken _token;

	public SpeedRange ModerateExciteSpeed;

	public SpeedRange ModerateAtomosphereSpeed;

	public float AtomospherePenaltySpeedThreshold;

	public float StimulusSpeedThreshold;

	public int ExciteIncrement = 8;

	public int AtomosphereIncrement = 4;

	public int AtomospherePenaltyDecrement = 8;

	public int StimulusIncrementFirstImpact = 500;

	public int StimulusIncrement = 3;

	public int StimulusDecrement = 3;

	public int StimulusIncreaseTime = 1500;

	public int LimitMagnitude = 50;

	public List<OsawariEvent> OnTouchEvents;

	public List<OsawariEvent> TriggeredEvents;

	public List<OsawariEvent> OnMouseUpEvents;

	public List<OsawariBlocker> Blockers;

	public int HitAreaFlagParameter = -1;

	protected float _lastValue;

	protected OsawariConditions _conditions;

	protected FPSChecker fpsChecker;

	public Easing.Ease EasingType = Easing.Ease.OutSine;

	protected float _autoTime;

	protected float _lastTimeAuto;

	protected CubismDrawable _targetMesh;

	public float AutoPeriod = 3f;

	public virtual bool IsAnimating
	{
		get
		{
			return IsAuto;
		}
		protected set
		{
		}
	}

	public virtual bool IsAuto { get; protected set; }

	protected float fps => fpsChecker?.GetFps() ?? 60f;

	public bool IsPistonMoving => _manager.IsPistonMoving;

	public virtual bool ClickAllowed()
	{
		return true;
	}

	public virtual void ManagedStart(OsawariManager osawariManager, CancellationToken token)
	{
		fpsChecker = GameObject.Find("UI_fps")?.GetComponent<FPSChecker>();
		_conditions = OsawariConditions.Empty;
		_manager = osawariManager;
		_token = token;
		_handManager = _manager.HandManager;
		_mousePositionOnLastFrame = Vector3.zero;
		SetParameters();
		_exdash = new Vector3(Mathf.Cos(ParameterAngle * (float)Math.PI / 180f), Mathf.Sin(ParameterAngle * (float)Math.PI / 180f), 0f);
		_eydash = new Vector3(0f - Mathf.Sin(ParameterAngle * (float)Math.PI / 180f), Mathf.Cos(ParameterAngle * (float)Math.PI / 180f), 0f);
		_manHand = new HandParamValue();
		InitializeParams();
		SetTouchableMeshs();
		foreach (OsawariEvent onTouchEvent in OnTouchEvents)
		{
			onTouchEvent.Initialize(this).Forget();
		}
		foreach (OsawariEvent triggeredEvent in TriggeredEvents)
		{
			triggeredEvent.Initialize(this).Forget();
		}
		foreach (OsawariEvent onMouseUpEvent in OnMouseUpEvents)
		{
			onMouseUpEvent.Initialize(this).Forget();
		}
		initialized = true;
	}

	protected virtual List<Hand> GetActiveHand()
	{
		return _handManager.GetHandGrabbing(this);
	}

	protected bool IsRightHandOn()
	{
		return GetActiveHand().Count((Hand x) => x.HandType == HandType.Right) > 0;
	}

	protected bool IsLeftHandOn()
	{
		return GetActiveHand().Count((Hand x) => x.HandType == HandType.Left) > 0;
	}

	protected Vector3 GetMouseMove()
	{
		return _manager.OsawariInput.GetMouseMove();
	}

	protected Vector3 ConvertMovementVec3ForParams()
	{
		Vector3 mouseMove = GetMouseMove();
		Vector3 result = Vector3.Project(mouseMove, _exdash) + Vector3.Project(mouseMove, _eydash);
		if (result.magnitude > (float)LimitMagnitude)
		{
			result /= result.magnitude / (float)LimitMagnitude;
		}
		return result;
	}

	public void ManagedLateUpdate()
	{
		if (initialized)
		{
			OnLateUpdate();
			UpdateHandParams();
			SetLive2D(HitAreaFlagParameter, IsHintAreaEnabled() ? 1 : 0);
		}
	}

	protected virtual void UpdateHandParams()
	{
		foreach (Hand item in GetActiveHand())
		{
			ParameterValue paramValue = _manHand.Get(item.HandType);
			_handManager.MoveHand(item.HandType, paramValue);
		}
	}

	protected int GetParameterNumber(ParameterName name)
	{
		return ParameterNumbers.GetTable()[name];
	}

	protected virtual void SetLive2D(ParameterName name, float val)
	{
		_manager.Preserver.SetValue(GetParameterNumber(name), val);
	}

	protected virtual void SetLive2D(ParameterName name, ParameterValue val)
	{
		SetLive2D(name, val.Value);
	}

	protected virtual void SetLive2D(int parameterId, float val)
	{
		_manager.Preserver.SetValue(parameterId, val);
	}

	protected virtual void InactivateLive2D(ParameterName name)
	{
		try
		{
			_manager.Preserver.InactivateValue(GetParameterNumber(name));
		}
		catch (Exception)
		{
		}
	}

	public virtual void ManagedUpdate()
	{
		if (!initialized)
		{
			return;
		}
		_conditions.IsPistonMoving = _manager.IsPistonMoving;
		if (IsAuto)
		{
			_conditions.UpdateTime();
			UtageManager utageManager = _manager.UtageManager;
			if ((object)utageManager != null && utageManager.IsPlaying)
			{
				Cancel();
				foreach (OsawariEvent item in TriggeredEvents.Where((OsawariEvent x) => x.IsInvoked))
				{
					item.Cancel();
				}
			}
			else
			{
				_conditions.Move = Vector3.zero;
				foreach (OsawariEvent triggeredEvent in TriggeredEvents)
				{
					if (triggeredEvent.IsFullfillCondition(_manager.TemporaryStatus, _conditions))
					{
						if (!triggeredEvent.IsInvoked)
						{
							triggeredEvent.InvokeEvent(_manager.TemporaryStatus, _conditions).Forget();
						}
					}
					else if (triggeredEvent.IsInvoked)
					{
						triggeredEvent.Cancel();
					}
				}
				AutoAnimation();
			}
		}
		if (!IsAuto && !_manager.IsClickingAt(this))
		{
			OnWhileNotClicked();
		}
		else if (_manager.IsClickingAt(this) || IsAuto)
		{
			_conditions.UpdateTime();
			UpdateFeelingParams();
		}
		UpdateHands();
		_mousePositionOnLastFrame = _manager.GetCurrentMousePosition();
	}

	public void ManagedUpdateWhileNotActive()
	{
		if (initialized)
		{
			OnWhileNotClicked();
			UpdateHands();
			UpdateHandParams();
		}
	}

	protected virtual void UpdateFeelingParams()
	{
		Vector3 vector = _manager.MainCamera.ScreenToWorldPoint(_mousePositionOnLastFrame);
		Vector3 vector2 = _manager.MainCamera.ScreenToWorldPoint(_manager.GetCurrentMousePosition());
		_conditions.Speed = (vector - vector2).magnitude / Time.deltaTime;
		_conditions.MovedDistance += _conditions.Speed;
		if (ModerateExciteSpeed.IsInRange(_conditions.Speed) || IsAuto)
		{
			_manager.TemporaryStatus.AddExciteValue(ExciteIncrement);
		}
		if (ModerateAtomosphereSpeed.IsInRange(_conditions.Speed) || IsAuto)
		{
			_manager.TemporaryStatus.Feelings.AddAtomosphere(AtomosphereIncrement);
		}
		else if (_conditions.Speed > AtomospherePenaltySpeedThreshold && !IsAuto)
		{
			_manager.TemporaryStatus.Feelings.AddAtomosphere(-AtomospherePenaltyDecrement);
		}
		if (_conditions.Speed > StimulusSpeedThreshold && !IsAuto)
		{
			_manager.TemporaryStatus.Feelings.Stimulus += StimulusIncrement;
		}
		else
		{
			_manager.TemporaryStatus.Feelings.Stimulus -= StimulusDecrement;
		}
	}

	public virtual void Cancel()
	{
		IsAuto = false;
		OnMouseUp(fromCancel: true);
	}

	public virtual bool CanTouchMesh(CubismDrawable targetMesh)
	{
		return TouchableMeshs.Contains(targetMesh);
	}

	public virtual CubismParameter GetHandParameter(HandType handType)
	{
		return _manager.Model.Parameters[GetHandParamIndex(handType)];
	}

	public virtual void OnClick(CubismDrawable targetMesh, bool isFirst)
	{
		if (targetMesh != null)
		{
			_targetMesh = targetMesh;
		}
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
				Hand hand = ((this is IParticularHand) ? ((IParticularHand)this).GetParticularHand() : _handManager.GetHandToUse());
				if (IsGrabbable)
				{
					_handManager.Grab(hand.HandType, this);
				}
			}
		}
		if (GetConstraintsCore() && (_handManager.IsGrabbing(this) || (!IsGrabbable && !IsAnimating)))
		{
			UpdateParams(ConvertMovementVec3ForParams());
		}
	}

	public virtual void SetAuto()
	{
		IsAuto = true;
	}

	public virtual bool IsAnimePlaying()
	{
		return IsAuto;
	}

	protected void OnFirstClick()
	{
		_manager.AddFirstStimulus(StimulusIncrementFirstImpact);
		foreach (OsawariEvent onTouchEvent in OnTouchEvents)
		{
			if (onTouchEvent.IsFullfillCondition(_manager.TemporaryStatus, _conditions))
			{
				onTouchEvent.InvokeEvent(_manager.TemporaryStatus, _conditions).Forget();
			}
		}
		_conditions.MovedDistance = 0f;
		OnFirstClickCore();
	}

	protected virtual void OnFirstClickCore()
	{
	}

	protected virtual void OnWhileNotClicked()
	{
		_conditions.ResetTime();
		UpdateWhileNotClicked();
	}

	protected virtual void UpdateHands()
	{
		if (IsRightHandOn() && _manHand.Get(HandType.Right).IsAlmostZero() && !_manHand.Locked)
		{
			_handManager.Release(HandType.Right);
			_manHand.SetValue(HandType.Right, 0f);
		}
		if (IsLeftHandOn() && _manHand.Get(HandType.Left).IsAlmostZero() && !_manHand.Locked)
		{
			_handManager.Release(HandType.Left);
			_manHand.SetValue(HandType.Left, 0f);
		}
	}

	protected virtual void UpdateParams(Vector3 move)
	{
		foreach (OsawariEvent triggeredEvent in TriggeredEvents)
		{
			if (triggeredEvent.IsFullfillCondition(_manager.TemporaryStatus, _conditions))
			{
				if (!triggeredEvent.IsInvoked)
				{
					triggeredEvent.InvokeEvent(_manager.TemporaryStatus, _conditions).Forget();
				}
			}
			else if (triggeredEvent.IsInvoked)
			{
				triggeredEvent.Cancel();
			}
		}
		if (!GetRestricted())
		{
			UpdateParamsCore(move);
		}
	}

	protected virtual void SetParameters()
	{
		parameters = new Dictionary<ParameterName, CubismParameter>();
		foreach (KeyValuePair<ParameterName, int> item in ParameterNumbers.GetTable())
		{
			if (item.Value != -1)
			{
				parameters[item.Key] = _manager.Model.Parameters[item.Value];
			}
		}
	}

	protected virtual void SetTouchableMeshs()
	{
		TouchableMeshs = new CubismDrawable[1] { Mesh };
	}

	public virtual void OnSpecial()
	{
	}

	public virtual void OnSpecialMouseUp()
	{
	}

	public virtual async void OnMouseUp(bool fromCancel = false)
	{
		if (GetRestricted())
		{
			return;
		}
		if (!IsAuto && !fromCancel)
		{
			foreach (OsawariEvent item in TriggeredEvents.Where((OsawariEvent x) => x.IsInvoked || x is SerialEvent || x is HScene3ModeChangeEvent || x is DetailAnimationEvent))
			{
				item.Cancel();
			}
			foreach (OsawariEvent onMouseUpEvent in OnMouseUpEvents)
			{
				await onMouseUpEvent.InvokeEvent(_manager.TemporaryStatus, _conditions);
			}
		}
		if (_conditions != null)
		{
			_conditions.Move = Vector3.zero;
		}
	}

	protected bool GetRestricted()
	{
		return GetRestrictedCore();
	}

	protected virtual bool IsHintAreaEnabled()
	{
		return GetConstraints();
	}

	protected virtual bool GetConstraintsCore()
	{
		return true;
	}

	protected virtual bool GetRestrictedCore()
	{
		return false;
	}

	public virtual bool GetConstraints()
	{
		UtageManager utageManager = _manager.UtageManager;
		if (((object)utageManager == null || !utageManager.IsPlaying) && GetConstraintsCore())
		{
			return Blockers.Count((OsawariBlocker x) => x.IsBlocked()) == 0;
		}
		return false;
	}

	public virtual void SwitchContext()
	{
	}

	protected abstract void InitializeParams();

	public virtual void PostInitialize()
	{
	}

	protected abstract void AutoAnimation();

	protected abstract void UpdateWhileNotClicked();

	protected abstract void UpdateParamsCore(Vector3 move);

	protected abstract int GetHandParamIndex(HandType handType);

	protected abstract void OnLateUpdate();

	protected void ResetAutoTimeCount()
	{
		_autoTime = 0f;
		_lastTimeAuto = 0f;
	}

	protected void UpdateAutoTimeCount()
	{
		_lastTimeAuto = _autoTime;
		_autoTime += Time.deltaTime;
		if (_autoTime >= AutoPeriod)
		{
			_autoTime = AutoPeriod;
		}
	}

	protected float GetEasedValue(float x)
	{
		float arg = x / AutoPeriod;
		return Easing.GetEasingMethod(EasingType)(arg);
	}
}
