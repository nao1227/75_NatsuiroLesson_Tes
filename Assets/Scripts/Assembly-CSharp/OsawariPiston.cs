using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public abstract class OsawariPiston : OsawariWithAnimation, IPiston
{
	public float EXTACY_MAN = 0.1f;

	protected FloatReactiveProperty _averageSpd;

	protected ReactiveProperty<ManPenisStatus> _manPenis = new ReactiveProperty<ManPenisStatus>(ManPenisStatus.Unshown);

	protected Subject<bool> _onEnablePenis;

	protected InsertController _insertController;

	protected FaceController _faceController;

	protected AverageCalculator _averageCalculator;

	protected Subject<bool> _onInsert;

	protected Subject<bool> _onPiston;

	protected Subject<Unit> _onImpact;

	protected Subject<bool> _onPenisShown;

	public OsawariEvent InsertEvent;

	public List<ImpactEvent> ImpactEvents;

	public int PistonDistanceThreshold;

	protected float _movedDistanceWhilePiston;

	public bool IsInPiston;

	protected ParameterValue _forcePiston;

	protected float _accelPiston;

	protected float _velocityPiston;

	protected float _lastPistonValue;

	public readonly float ABSOLUTE_FORCE_MAX = 15f;

	public PhysicsCalculater PhysicsCalculater;

	protected float _deltaT = 30f;

	public float AutoSpeed = 10f;

	[SerializeField]
	protected float EnforcementFactor = 0.3f;

	[SerializeField]
	protected float PullFactor = 0.4f;

	protected bool _isPistonAutoReturning;

	public float AutoPistonPeriod = 3f;

	protected float _impact;

	protected float _pastImpactTime;

	protected bool _impactFlag;

	protected bool _allowImpactEvent = true;

	public IReadOnlyReactiveProperty<float> AverageSpeed => _averageSpd;

	public IReadOnlyReactiveProperty<ManPenisStatus> ManPenis => _manPenis;

	public IObservable<bool> OnEnablePenis => _onEnablePenis;

	public bool IsMoving { get; protected set; }

	public IObservable<Unit> OnImpact => _onImpact;

	public IObservable<bool> OnPenisShown => _onPenisShown;

	public override bool ClickAllowed()
	{
		return !IsAnimating;
	}

	public abstract bool IsEnter();

	public abstract bool IsInsert();

	public abstract UniTask ManEnter();

	public virtual bool CanManEnter()
	{
		return true;
	}

	public abstract UniTask<bool> OnNextEnter();

	public abstract UniTask Eject();

	public IObservable<bool> OnInsert()
	{
		return _onInsert;
	}

	public IObservable<bool> OnPiston()
	{
		return _onPiston;
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		return 0;
	}

	protected override void InitializeParams()
	{
		_averageCalculator = new AverageCalculator(120);
		_averageSpd = new FloatReactiveProperty();
		_onEnablePenis = new Subject<bool>();
		_onInsert = new Subject<bool>();
		_onPiston = new Subject<bool>();
		_onImpact = new Subject<Unit>();
		_onPenisShown = new Subject<bool>();
		InsertEvent?.Initialize();
		IsGrabbable = false;
		_forcePiston = new ParameterValue(0f, 0f - ABSOLUTE_FORCE_MAX, ABSOLUTE_FORCE_MAX);
		_accelPiston = 0f;
		_velocityPiston = 0f;
		_lastPistonValue = 0f;
		foreach (ImpactEvent impactEvent in ImpactEvents)
		{
			impactEvent.Initialize(this).Forget();
		}
	}

	protected void CheckIsInPiston(Vector3 move)
	{
		if (_movedDistanceWhilePiston > (float)PistonDistanceThreshold && !IsInPiston)
		{
			IsInPiston = true;
			_onPiston.OnNext(value: true);
		}
		else if (_averageSpd.Value <= 0.001f && IsInPiston)
		{
			IsInPiston = false;
			_onPiston.OnNext(value: false);
			_movedDistanceWhilePiston = 0f;
		}
		else
		{
			_movedDistanceWhilePiston += Mathf.Abs(move.y / SensitivityY);
		}
	}

	public virtual bool IsAbleToInsert()
	{
		return true;
	}

	private void Update()
	{
		if (_onEnablePenis != null)
		{
			_onEnablePenis.OnNext(CanManEnter());
		}
	}

	protected void Enforce(ThresholdParameterValue piston, Vector3 move)
	{
		_manager.GetCurrentMousePosition();
		_ = _mousePositionOnLastFrame;
		_velocityPiston = (piston.Value - _lastPistonValue) / _deltaT * 60f / base.fps;
		_accelPiston = _velocityPiston / _deltaT * 10000f;
		_forcePiston = _forcePiston.Update(_accelPiston);
		PhysicsCalculater.Enforce(_forcePiston.Value * EnforcementFactor, move.y < 0f, PullFactor);
	}

	protected void Enforce(ParameterValue piston, Vector3 move)
	{
		_manager.GetCurrentMousePosition();
		_ = _mousePositionOnLastFrame;
		_velocityPiston = (piston.Value - _lastPistonValue) / _deltaT * 60f / base.fps;
		_accelPiston = _velocityPiston / _deltaT * 10000f;
		_forcePiston = _forcePiston.Update(_accelPiston);
		PhysicsCalculater.Enforce(_forcePiston.Value * EnforcementFactor, move.y < 0f, PullFactor);
	}

	protected override void AutoAnimation()
	{
		if (!IsInsert() || _insertController.IsManExtacy)
		{
			IsAuto = false;
		}
	}

	public override void SetAuto()
	{
		base.SetAuto();
		ResetAutoTimeCount();
	}

	public override void OnSpecial()
	{
		if (IsInsert())
		{
			_insertController.AddManExtacy(99999f);
		}
	}

	public override void OnClick(CubismDrawable targetMesh, bool isFirst)
	{
		if (isFirst)
		{
			if (IsAuto)
			{
				IsAuto = false;
			}
			if (GetConstraintsCore())
			{
				OnFirstClick();
				_manager.IsAction = true;
				Hand handToUse = _handManager.GetHandToUse();
				if (IsGrabbable)
				{
					_handManager.Grab(handToUse.HandType, this);
				}
			}
		}
		if (GetConstraintsCore())
		{
			UpdateParams(ConvertMovementVec3ForParams());
		}
	}

	public virtual ManPenisStatus GetManPenisValue()
	{
		return _manPenis.Value;
	}
}
