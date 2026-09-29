using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class Hscene2OsawariPiston : OsawariPiston
{
	public float ImpactThreshold;

	private ParameterValue _piston;

	private Hscene2WomanBody _body;

	private HScene2OsawariKiss _kiss;

	private bool _insert;

	private bool _isPistonStarted;

	private FaceAnimationControllerOnOsawariScene2 _fac;

	public float WomanMoveExtacyFactor = 1f;

	private Subject<Unit> _onReset = new Subject<Unit>();

	private float _lastWomanBody;

	private HScene2OsawariBreast _breast;

	public OsawariEvent NoCondomEvent;

	public bool IsWomanMoving { get; private set; }

	public IObservable<Unit> OnReset => _onReset;

	public bool WomanMoveFast { get; private set; }

	protected override async void InitializeParams()
	{
		base.InitializeParams();
		await NoCondomEvent.Initialize(this);
		_insertController = GetComponent<InsertController>();
		_faceController = GetComponent<FaceController>();
		_piston = new ParameterValue(parameters[ParameterName.Piston]);
		_body = _manager.GetOsawariOf<Hscene2WomanBody>();
		_breast = _manager.GetOsawariOf<HScene2OsawariBreast>();
		_kiss = _manager.GetOsawariOf<HScene2OsawariKiss>();
		_manPenis.Value = ManPenisStatus.Enter;
		_fac = UnityEngine.Object.FindObjectOfType<FaceAnimationControllerOnOsawariScene2>();
		StateMachineObservables[] rx = animator.GetRx();
		for (int i = 0; i < rx.Length; i++)
		{
			rx[i].OnStateEnterObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
			{
				if (x.Item1 == StateID.Idle || x.Item1 == StateID.Insert || x.Item1 == StateID.Hold)
				{
					IsAnimating = false;
				}
			}).AddTo(this);
		}
	}

	protected override void AutoAnimation()
	{
		base.AutoAnimation();
		if (IsAuto && !IsAnimating)
		{
			float easedValue = GetEasedValue(_lastTimeAuto);
			UpdateAutoTimeCount();
			float y = (GetEasedValue(_autoTime) - easedValue) * SensitivityY;
			Vector3 move = new Vector3(0f, y, 0f) * ((!_isPistonAutoReturning) ? 1 : (-1));
			UpdateParamsCore(move);
			if (_piston.IsMax() || _piston.IsMin() || _autoTime == AutoPeriod)
			{
				ResetAutoTimeCount();
				_isPistonAutoReturning = !_isPistonAutoReturning;
			}
		}
	}

	protected override void UpdateWhileNotClicked()
	{
		if (IsAuto)
		{
			return;
		}
		if (_body.IsHolding)
		{
			PhysicsCalculater.InvalidCalc(1, Switch: false);
			PhysicsCalculater.InvalidCalc(2, Switch: false);
			PhysicsCalculater.InvalidCalc(3, Switch: false);
			PhysicsCalculater.InvalidCalc(4, Switch: false);
		}
		else
		{
			PhysicsCalculater.InvalidCalc(1, Switch: true);
			PhysicsCalculater.InvalidCalc(2, Switch: true);
			PhysicsCalculater.InvalidCalc(3, Switch: true);
			PhysicsCalculater.InvalidCalc(4, Switch: true);
		}
		if (IsWomanMoving)
		{
			foreach (OsawariEvent item in TriggeredEvents.Where((OsawariEvent x) => x.InvokedWhileWomanMoving))
			{
				if (item.IsFullfillCondition(_manager.TemporaryStatus, _conditions))
				{
					if (!item.IsInvoked)
					{
						item.InvokeEvent(_manager.TemporaryStatus, _conditions).Forget();
					}
				}
				else if (item.IsInvoked)
				{
					item.Cancel();
				}
			}
			if (!_body.IsHolding)
			{
				EnforceByWomanBody();
			}
			_insertController.AddManExtacy(EXTACY_MAN * WomanMoveExtacyFactor);
		}
		else
		{
			foreach (OsawariEvent item2 in TriggeredEvents.Where((OsawariEvent x) => x.InvokedWhileWomanMoving))
			{
				if (item2.IsInvoked)
				{
					item2.Cancel();
				}
			}
		}
		if (_isPistonStarted)
		{
			_onPiston.OnNext(value: false);
			_isPistonStarted = false;
		}
		_averageCalculator.Add(0f);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_piston += move.y / SensitivityY;
		if (_body.IsHolding)
		{
			_piston = _piston.Update(0f);
		}
		else
		{
			_averageCalculator.Add(move.y / SensitivityY);
		}
		_isPistonStarted = _averageSpd.Value > 0f && _insert;
		if (_piston.IsMax() && parameters[ParameterName.Piston].Value != parameters[ParameterName.Piston].MaximumValue && _averageCalculator.GetAverageOf(3, isAbs: true) > ImpactThreshold)
		{
			_onImpact.OnNext(Unit.Default);
			foreach (ImpactEvent impactEvent in ImpactEvents)
			{
				impactEvent.InvokeEvent(_manager.TemporaryStatus, _conditions).Forget();
			}
		}
		if (!IsAnimating)
		{
			_manager.CsManager.OsawariCrossSection.SynchroPistonFromHScene2(_piston.Value - _body.GetCSValue());
		}
		if (IsInsert())
		{
			CheckIsInPiston(move);
			Enforce(_piston, move);
			_insertController.AddManExtacy(EXTACY_MAN * move.magnitude);
		}
		_lastPistonValue = _piston.Value;
	}

	protected override int GetHandParamIndex(HandType hand)
	{
		return 0;
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.Piston, _piston);
		_manager.CsManager.OsawariCrossSection.SynchroPistonFromHScene2(_piston.Value - _body.GetCSValue());
		if (IsWomanMoving)
		{
			_manager.TemporaryStatus.Feelings.AddAtomosphere(AtomosphereIncrement * ((!WomanMoveFast) ? 1 : 2));
			_manager.TemporaryStatus.AddExciteValue(ExciteIncrement * ((!WomanMoveFast) ? 1 : 2));
		}
		_averageSpd.Value = _averageCalculator.AbsAverage;
	}

	public override bool IsEnter()
	{
		return false;
	}

	public override bool IsInsert()
	{
		return _insert;
	}

	public override async UniTask ManEnter()
	{
		await UniTask.Yield();
	}

	public override async UniTask<bool> OnNextEnter()
	{
		if (NoCondomEvent.IsFullfillCondition(_manager.TemporaryStatus, _conditions))
		{
			await NoCondomEvent.InvokeEvent(_manager.TemporaryStatus, _conditions);
			return false;
		}
		IsAnimating = true;
		animator.SetTrigger("TriggerInsert");
		_onInsert.OnNext(value: true);
		await UniTask.Yield();
		_insert = true;
		_manPenis.Value = ManPenisStatus.Insert;
		return true;
	}

	public override async UniTask Eject()
	{
		IsAnimating = true;
		animator.SetTrigger("TriggerEject");
		_onInsert.OnNext(value: false);
		await UniTask.Yield();
		_insert = false;
		_manPenis.Value = ManPenisStatus.Enter;
		Reset();
	}

	private void Reset()
	{
		SetMove(move: false);
		SetMoveFast(moveFast: false);
		_onReset.OnNext(Unit.Default);
	}

	protected override bool GetConstraintsCore()
	{
		if (base.GetConstraintsCore() && !_body.IsHolding)
		{
			return _insert;
		}
		return false;
	}

	public void SetMove(bool move)
	{
		animator.SetBool("Move", move);
		IsWomanMoving = move;
		_fac.ResetFace();
	}

	public void SetMoveFast(bool moveFast)
	{
		WomanMoveFast = moveFast;
		animator.SetBool("MoveFast", moveFast);
		_fac.ResetFace();
	}

	public void SetHold(bool hold)
	{
		_piston = _piston.Update(0f);
		_kiss.SetIsHolding(hold);
		IsAnimating = true;
		animator.SetBool("Hold", hold);
		_breast.Cancel();
	}

	private float GetWomanBodyValue()
	{
		return _manager.Model.Parameters[GetParameterNumber(ParameterName.WomanBodyY)].Value;
	}

	private void EnforceByWomanBody()
	{
		_velocityPiston = (GetWomanBodyValue() - _lastWomanBody) / _deltaT * 60f / base.fps;
		_lastWomanBody = GetWomanBodyValue();
		_accelPiston = _velocityPiston / _deltaT * 10000f;
		_forcePiston = _forcePiston.Update(_accelPiston);
		PhysicsCalculater.Enforce(_forcePiston.Value * EnforcementFactor, GetWomanBodyValue() > _lastWomanBody, PullFactor);
	}
}
