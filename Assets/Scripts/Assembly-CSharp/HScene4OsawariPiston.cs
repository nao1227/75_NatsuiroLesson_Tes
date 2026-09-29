using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class HScene4OsawariPiston : OsawariPiston
{
	private ThresholdParameterValue _piston;

	private ParameterValue _manEnter;

	private OsawariPants _pants;

	private bool _previous = true;

	private bool _ejectAnimating;

	private OsawariGoods _goods;

	private OsawariPaizuri _paizuri;

	private List<OsawariHip> _hips;

	private ManPenisStatus _savedManPenis;

	private OsawariAibu _aibu;

	public PenisFollowerHScene4Normal PenisFollower;

	public HScene4VaginaMosaic VaginaMosaic;

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
			if (_autoTime == AutoPistonPeriod)
			{
				ResetAutoTimeCount();
				_isPistonAutoReturning = !_isPistonAutoReturning;
			}
			else if (_piston.IsOnThresholdMax())
			{
				ResetAutoTimeCount();
				_isPistonAutoReturning = true;
			}
			else if (_piston.Value <= 0f)
			{
				ResetAutoTimeCount();
				_isPistonAutoReturning = false;
				_piston = _piston.Update(0f);
			}
		}
	}

	public override bool CanManEnter()
	{
		if (_pants.IsAbleToInsert())
		{
			return !_aibu.IsAuto;
		}
		return false;
	}

	protected override void InitializeParams()
	{
		base.InitializeParams();
		_aibu = _manager.GetOsawariOf<OsawariAibu>();
		_savedManPenis = _manPenis.Value;
		_goods = GetComponent<OsawariGoods>();
		_manEnter = new ParameterValue(0f);
		_piston = new ThresholdParameterValue(parameters[ParameterName.Piston], new float[1] { -0.5f });
		_paizuri = _manager.GetOsawariOf<OsawariPaizuri>();
		_hips = _manager.GetEveryOsawariOf<OsawariHip>();
		_insertController = GetComponent<InsertController>();
		StateMachineObservables[] rx = animator.GetRx();
		foreach (StateMachineObservables obj in rx)
		{
			obj.OnStateExitObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Insert).Subscribe(delegate
			{
				_piston = _piston.Update(0f);
				IsAnimating = false;
				_manPenis.Value = ManPenisStatus.Insert;
			}).AddTo(this);
			obj.OnStateExitObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Eject).Subscribe(delegate
			{
				_piston = _piston.MoveSectionTo(0);
				_piston = _piston.Update(-1f);
				IsAnimating = false;
				_ejectAnimating = false;
				_manPenis.Value = ManPenisStatus.Enter;
			}).AddTo(this);
			obj.OnStateExitObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.Impact).Subscribe(delegate
			{
				_allowImpactEvent = true;
			}).AddTo(this);
		}
		_pants = _manager.GetOsawariOf<OsawariPants>();
	}

	public override async UniTask Eject()
	{
		IsAnimating = true;
		_ejectAnimating = true;
		animator.SetTrigger("TriggerEject");
		_onInsert.OnNext(value: false);
	}

	public override bool IsEnter()
	{
		return _manEnter.Value == 1f;
	}

	public override bool IsInsert()
	{
		if (_piston != null)
		{
			return _piston.Value >= -0.5f;
		}
		return false;
	}

	public override async UniTask ManEnter()
	{
		if (_manager.ContextManager.Context == OsawariContext.Osawari)
		{
			_hips.ForEach(delegate(OsawariHip x)
			{
				x.Cancel();
			});
			IsAnimating = true;
			PenisFollower.SetEnabled(enabled: true);
			DOVirtual.Float(0f, 1f, 0.5f, delegate(float x)
			{
				_manEnter = _manEnter.Update(x);
			}).OnComplete(delegate
			{
				IsAnimating = false;
				_onPenisShown.OnNext(value: true);
				_manPenis.Value = ManPenisStatus.Enter;
			}).Play();
		}
		else if (_manager.ContextManager.Context == OsawariContext.Paizuri)
		{
			_paizuri.EnterPenis(delegate
			{
				IsAnimating = false;
				_manPenis.Value = ManPenisStatus.Enter;
			});
		}
		await UniTask.Yield();
	}

	public override async UniTask<bool> OnNextEnter()
	{
		if (IsAnimating)
		{
			return false;
		}
		IsAnimating = true;
		if (_manager.ContextManager.Context == OsawariContext.Osawari)
		{
			_hips.ForEach(delegate(OsawariHip x)
			{
				x.Cancel();
			});
			if (_manPenis.Value == ManPenisStatus.Enter)
			{
				DOVirtual.Float(1f, 0f, 0.5f, delegate(float x)
				{
					_manEnter = _manEnter.Update(x);
				}).OnComplete(delegate
				{
					IsAnimating = false;
					_onPenisShown.OnNext(value: false);
					_piston = _piston.MoveSectionTo(0);
					_manPenis.Value = ManPenisStatus.Unshown;
					PenisFollower.SetEnabled(enabled: false);
				}).Play();
			}
		}
		else
		{
			_manager.OsawariPaizuri.ExitPenis(delegate
			{
				IsAnimating = false;
				_manPenis.Value = ManPenisStatus.Unshown;
			});
		}
		await UniTask.Yield();
		return true;
	}

	protected override void OnLateUpdate()
	{
		if (IsAnimating)
		{
			_manager.Preserver.InactivateValue(GetParameterNumber(ParameterName.Piston));
			_piston = _piston.Update(parameters[ParameterName.Piston].Value);
		}
		if (!_ejectAnimating)
		{
			SetLive2D(ParameterName.Piston, _piston.Value);
		}
		_averageSpd.Value = _averageCalculator.AbsAverage;
		SetLive2D(ParameterName.Penis, _manEnter);
		PenisFollower.SetPistonValue(_piston.Value);
		_manager.CsManager.OsawariCrossSection.SynchroPistonFromHScene4(_piston.Value);
	}

	private void Update()
	{
		if (null == _pants || _onEnablePenis == null)
		{
			return;
		}
		if (_manager.ContextManager.Context == OsawariContext.Osawari)
		{
			if (CanManEnter() != _previous || !CanManEnter())
			{
				_onEnablePenis.OnNext(CanManEnter());
				_previous = CanManEnter();
			}
		}
		else
		{
			_onEnablePenis.OnNext(value: true);
		}
	}

	public override bool IsAbleToInsert()
	{
		return _pants.IsAbleToInsert();
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (IsAnimating)
		{
			return;
		}
		float num = move.y / SensitivityY;
		if (_piston.Value < -0.5f)
		{
			num *= -1f;
		}
		_piston += num;
		if (_piston.Value == -0.5f && _piston.IsOnThresholdMax())
		{
			IsAnimating = true;
			_piston = _piston.MoveSectionTo(1);
			animator.SetTrigger("TriggerInsert");
			_onInsert.OnNext(value: true);
		}
		else if (_piston.Value >= -0.5f && _piston.Value < 0f)
		{
			_piston = _piston.Update(0f);
		}
		if (IsInsert())
		{
			CheckIsInPiston(move);
			if (_piston.Value >= 0f)
			{
				float time = Time.time;
				if (time > _pastImpactTime)
				{
					float impact = move.y / SensitivityY;
					_impact = impact;
					_impactFlag = true;
					_pastImpactTime = time;
				}
			}
			_averageCalculator.Add(move.y / SensitivityY);
			if (!IsAnimating)
			{
				_manager.CsManager.OsawariCrossSection.SynchroPistonFromHScene4(parameters[ParameterName.Piston].Value);
			}
			if (_impactFlag && _allowImpactEvent)
			{
				foreach (IImpactTrigger item in _manager.ActionManager.GetActionOf<IImpactTrigger>())
				{
					item.OnImpact(_impact);
				}
				foreach (ImpactEvent item2 in ImpactEvents.Where((ImpactEvent x) => x.IsFullfillCondition(_manager.TemporaryStatus, _conditions) && x.IsInRange(_piston.Value)))
				{
					_allowImpactEvent = false;
					item2.InvokeEvent(_manager.TemporaryStatus, _conditions);
				}
				_impactFlag = false;
			}
			Enforce(_piston, move);
			_insertController.AddManExtacy(EXTACY_MAN * move.magnitude);
		}
		_lastPistonValue = _piston.Value;
	}

	protected override void UpdateWhileNotClicked()
	{
		if (IsAuto)
		{
			return;
		}
		_averageCalculator.Add(0f);
		_allowImpactEvent = true;
		if (_averageSpd.Value == 0f)
		{
			_movedDistanceWhilePiston = 0f;
			if (IsInPiston)
			{
				IsInPiston = false;
				_onPiston.OnNext(value: false);
			}
		}
	}

	protected override bool GetRestrictedCore()
	{
		if (!_goods.IsCondomSet)
		{
			return !SaveLoadManager.UnsavedData.GlobalFlags.IsOn(FlagEnum.AllowCondomPutOff);
		}
		return false;
	}

	public override void SwitchContext()
	{
		VaginaMosaic.SetContext(_manager.ContextManager.Context);
		PenisFollower.SetContext(_manager.ContextManager.Context);
		if (_manager.ContextManager.Context == OsawariContext.Osawari)
		{
			_manPenis.Value = _savedManPenis;
			_onPenisShown.OnNext(_manPenis.Value == ManPenisStatus.Enter || _manPenis.Value == ManPenisStatus.Insert);
		}
		else
		{
			_onPenisShown.OnNext(_manager.OsawariPaizuri.IsInPaizuriMode);
			_savedManPenis = _manPenis.Value;
			_manPenis.Value = (_manager.OsawariPaizuri.IsInPaizuriMode ? ManPenisStatus.Enter : ManPenisStatus.Unshown);
		}
	}

	public override ManPenisStatus GetManPenisValue()
	{
		if (_manager.ContextManager.Context == OsawariContext.Osawari)
		{
			return _manPenis.Value;
		}
		return _savedManPenis;
	}
}
