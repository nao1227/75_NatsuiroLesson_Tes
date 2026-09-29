using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariMizugiHimo : OsawariDoublehanded, IBra, IWearable
{
	public CubismDrawable AfterHodokiMeshLeft;

	public CubismDrawable AfterHodokiMeshRight;

	private SingleThresholdParameterValue _mizugi;

	private BoolParameterValue _mizugiFlag;

	private Live2DAnimator _animator;

	private bool _isAnimating;

	private bool _isMicroBikini;

	public OsawariMizugiUpperPaizuri MizugiPaizuri;

	public MizugiHodokiPhase Phase { get; private set; }

	protected override void AutoAnimation()
	{
	}

	public void SetMicroBikini()
	{
		_isMicroBikini = true;
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		switch (Phase)
		{
		case MizugiHodokiPhase.Wearing:
			if (handType == HandType.Left)
			{
				return ParameterNumbers.GetTable()[ParameterName.WetSuitLeftHandBeforeHodoki];
			}
			return ParameterNumbers.GetTable()[ParameterName.WetSuitRightHandBeforeHodoki];
		case MizugiHodokiPhase.HalfWearing:
			if (handType == HandType.Left)
			{
				return ParameterNumbers.GetTable()[ParameterName.WetSuitLeftHandAfterHodoki];
			}
			return ParameterNumbers.GetTable()[ParameterName.WetSuitRightHandAfterHodoki];
		case MizugiHodokiPhase.TakeOff:
			return 0;
		default:
			return 0;
		}
	}

	protected override void InitializeParams()
	{
		_mizugi = new SingleThresholdParameterValue(parameters[ParameterName.WetSuit], 0.8f);
		_mizugi = _mizugi.MoveToLowerSection(useMinAsDefault: true);
		_mizugiFlag = new BoolParameterValue(parameters[ParameterName.WetSuitUpperFlag]);
		Phase = MizugiHodokiPhase.Wearing;
		_animator = GetComponent<Live2DAnimator>();
		_isAnimating = false;
		StateMachineObservables[] rx = _animator.GetRx();
		for (int i = 0; i < rx.Length; i++)
		{
			rx[i].OnStateExitObservable.Subscribe(delegate((StateID, AnimatorStateInfo) x)
			{
				if (x.Item1 == StateID.WetSuitHodoki1)
				{
					OnHalfHodoki();
					_isAnimating = false;
				}
				else if (x.Item1 == StateID.WetSuitHodoki2)
				{
					Phase = MizugiHodokiPhase.TakeOff;
					_isAnimating = false;
				}
			}).AddTo(this);
		}
	}

	protected override void OnLateUpdate()
	{
		if (_isAnimating)
		{
			_mizugi = _mizugi.Update(parameters[ParameterName.WetSuit].Value);
			InactivateLive2D(ParameterName.WetSuit);
		}
		else
		{
			SetLive2D(ParameterName.WetSuit, _mizugi.Value);
		}
		if (_isMicroBikini)
		{
			SetLive2D(ParameterName.MicroBikiniUpper, _mizugiFlag);
			SetLive2D(ParameterName.WetSuitUpperFlag, 0f);
		}
		else
		{
			SetLive2D(ParameterName.WetSuitUpperFlag, _mizugiFlag);
			SetLive2D(ParameterName.MicroBikiniUpper, 0f);
		}
		MizugiPaizuri.SyncroValue(_mizugiFlag.Value, _mizugi.Value);
	}

	private async UniTask OpenMizugiHimo()
	{
		_animator.SetTrigger("TriggerMizugiHodoki1");
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (_isAnimating)
		{
			return;
		}
		switch (Phase)
		{
		case MizugiHodokiPhase.Wearing:
			_manHand.Appear();
			if (_mizugi.IsOnThresholdMax() && move.y < 0f)
			{
				_isAnimating = true;
				_mizugi = _mizugi.MoveToUpperSection();
				OpenMizugiHimo().Forget();
			}
			else
			{
				_mizugi -= move.y / SensitivityY;
			}
			break;
		case MizugiHodokiPhase.HalfWearing:
		{
			_manHand.Appear();
			bool flag = move.x > 0f;
			if (_targetMesh == AfterHodokiMeshLeft)
			{
				flag = move.x < 0f;
			}
			if (flag)
			{
				_isAnimating = true;
				_animator.SetTrigger("TriggerMizugiHodoki2");
			}
			break;
		}
		case MizugiHodokiPhase.TakeOff:
			break;
		}
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!_isAnimating)
		{
			_manHand.Disappear();
		}
		else
		{
			_manHand.Appear();
		}
	}

	private void ResetHodoki()
	{
		SetTouchableMeshs();
		Phase = MizugiHodokiPhase.Wearing;
		_mizugi = _mizugi.MoveToLowerSection(useMinAsDefault: true);
	}

	private void OnHalfHodoki()
	{
		TouchableMeshs = new CubismDrawable[2] { AfterHodokiMeshLeft, AfterHodokiMeshRight };
		Phase = MizugiHodokiPhase.HalfWearing;
	}

	protected override bool GetConstraintsCore()
	{
		if (Phase != MizugiHodokiPhase.TakeOff)
		{
			return _mizugiFlag.AsBool();
		}
		return false;
	}

	public void TakeOnBra()
	{
		_mizugi = _mizugi.MoveToLowerSection(useMinAsDefault: true);
		SetTouchableMeshs();
		_mizugiFlag = _mizugiFlag.Update(val: true);
		Phase = MizugiHodokiPhase.Wearing;
		_animator.SetTrigger("TriggerResetMizugi");
	}

	public void TakeOffBra(bool exc)
	{
		Phase = MizugiHodokiPhase.TakeOff;
		_mizugiFlag = _mizugiFlag.Update(val: false);
	}

	public void SwitchBra()
	{
		if (_mizugiFlag.AsBool())
		{
			TakeOffBra(exc: true);
		}
		else
		{
			TakeOnBra();
		}
	}

	public void SyncroValue(float flagVal)
	{
	}

	public bool IsWearing()
	{
		if (_mizugiFlag.AsBool())
		{
			return Phase != MizugiHodokiPhase.TakeOff;
		}
		return false;
	}
}
