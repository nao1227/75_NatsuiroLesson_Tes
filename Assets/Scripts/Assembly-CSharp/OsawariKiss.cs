using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariKiss : OsawariWithoutHand
{
	private enum Phase
	{
		Default = 0,
		Prepare = 1,
		StickOut = 2,
		Kiss = 3
	}

	protected IEnumerable<OsawariAction> _actions;

	private SingleThresholdParameterValue _tongueParam;

	private ParameterValue _kissParam;

	private FaceController _faceController;

	public bool StopPoseAnimationWhileKissing;

	public AnimationClip StopAnimationClip;

	private InsertController _insertController;

	private bool _clickFlag;

	private bool _onlyWhileInExtacy;

	private Phase phase;

	public Subject<Unit> OnKissStart;

	public Subject<Unit> OnKissEnd;

	private CancellationTokenSource _studyHCountCTS;

	[SerializeField]
	private float YmoveRealmLower = -1f;

	public bool IsKissing => phase == Phase.Kiss;

	protected override void InitializeParams()
	{
		_insertController = GetComponent<InsertController>();
		OnKissStart = new Subject<Unit>();
		OnKissEnd = new Subject<Unit>();
		_faceController = GetComponent<FaceController>();
		_tongueParam = new SingleThresholdParameterValue(parameters[ParameterName.Tongue], YmoveRealmLower);
		_kissParam = new ParameterValue(parameters[ParameterName.Kiss]);
		_actions = _manager.ActionManager.GetActionOf<IKissTrigger>();
	}

	public void CancellAnimaion()
	{
		if (_clickFlag)
		{
			Cursor.visible = true;
		}
		_clickFlag = false;
		IsAuto = false;
	}

	protected override void AutoAnimation()
	{
		if (_onlyWhileInExtacy && !_insertController.IsWomanExtacy)
		{
			_onlyWhileInExtacy = false;
			IsAuto = false;
			OnMouseUp();
		}
		else
		{
			_tongueParam = _tongueParam.MoveToUpperSection();
			float y = Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) - _tongueParam.Value;
			UpdateParams(new Vector3(0f, y, 0f) * SensitivityY);
		}
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		throw new Exception("No hand should be called on this class");
	}

	protected override void OnLateUpdate()
	{
		if (phase != Phase.Default && !IsAuto)
		{
			_faceController.SetDirectionFixed(Bool: true);
			_faceController.SetFaceDirection(FaceController.FaceDirection.Front);
		}
		SetLive2D(ParameterName.Tongue, _tongueParam.Value);
		SetLive2D(ParameterName.Kiss, _kissParam);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		_insertController.SetIsKissing(isKissing: true);
		switch (phase)
		{
		case Phase.Default:
			phase = Phase.Prepare;
			OnKissStart.OnNext(Unit.Default);
			break;
		case Phase.Prepare:
			_kissParam += 0.4f;
			if (_kissParam.IsMax())
			{
				phase = Phase.StickOut;
			}
			break;
		case Phase.StickOut:
			_tongueParam += 0.2f;
			if (_tongueParam.IsOnThresholdMax())
			{
				_tongueParam = _tongueParam.MoveToUpperSection();
				phase = Phase.Kiss;
			}
			break;
		case Phase.Kiss:
			_tongueParam += move.y / SensitivityY;
			{
				foreach (OsawariAction action in _actions)
				{
					action.StartAction();
				}
				break;
			}
		default:
			throw new Exception();
		}
	}

	protected override void UpdateWhileNotClicked()
	{
		if (IsAuto)
		{
			return;
		}
		_insertController.SetIsKissing(isKissing: false);
		switch (phase)
		{
		case Phase.Default:
			_manager.IsAction = false;
			OnKissEnd.OnNext(Unit.Default);
			{
				foreach (OsawariAction action in _actions)
				{
					action.FinishAction();
				}
				break;
			}
		case Phase.Prepare:
			_kissParam -= 0.1f;
			if (_kissParam.IsMin())
			{
				phase = Phase.Default;
				_faceController.SetDirectionFixed(Bool: false);
				_faceController.SetFaceDirection(FaceController.FaceDirection.Default);
				_faceController.StateListener();
			}
			break;
		case Phase.StickOut:
			_tongueParam -= 0.1f;
			if (_tongueParam.IsOnThresholdMin())
			{
				phase = Phase.Prepare;
			}
			break;
		case Phase.Kiss:
			_tongueParam -= 0.1f;
			if (_tongueParam.IsOnThresholdMin())
			{
				_tongueParam = _tongueParam.MoveToLowerSection();
				phase = Phase.StickOut;
			}
			break;
		default:
			throw new Exception();
		}
	}

	protected override void OnFirstClickCore()
	{
		if (StopPoseAnimationWhileKissing)
		{
			_studyHCountCTS?.Cancel();
		}
	}

	public override async void OnMouseUp(bool fromCancel = false)
	{
		if (IsAuto)
		{
			return;
		}
		if (_insertController?.IsWomanExtacy ?? false)
		{
			SetAuto();
			_onlyWhileInExtacy = true;
		}
		base.OnMouseUp(fromCancel);
		if (!StopPoseAnimationWhileKissing)
		{
			return;
		}
		try
		{
			_studyHCountCTS?.Cancel();
			_studyHCountCTS = new CancellationTokenSource();
			await UniTask.Delay(5000, ignoreTimeScale: false, PlayerLoopTiming.Update, _studyHCountCTS.Token);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	protected override bool GetConstraintsCore()
	{
		return !_insertController.IsWomanExtacy;
	}

	private void OnDestroy()
	{
		_studyHCountCTS?.Cancel();
	}
}
