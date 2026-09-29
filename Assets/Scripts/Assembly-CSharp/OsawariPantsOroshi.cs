using Live2D.Cubism.Framework;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class OsawariPantsOroshi : OsawariWithAnimation, IDoubleHanded
{
	private ParameterValue _oroshi;

	private HScene4OsawariPants _pants;

	private int _reset;

	public OsawariPantsOroshiPaizuri _oroshiPaizuri;

	public bool IsOnHip => _oroshi.Value == 0f;

	protected override void AutoAnimation()
	{
	}

	protected override void SetTouchableMeshs()
	{
	}

	protected bool IsMB()
	{
		return _manager.TemporaryStatus.Cloth == ClothName.MicroBikini;
	}

	protected override int GetHandParamIndex(HandType handType)
	{
		if (!IsMB())
		{
			return ParameterNumbers.GetTable()[ParameterName.DoubleHandOnPants];
		}
		return ParameterNumbers.GetTable()[ParameterName.DoubleHandOnPantsMB];
	}

	protected override void InitializeParams()
	{
		_oroshi = new ParameterValue(parameters[ParameterName.Pants]);
		_pants = _manager.GetOsawariOf<HScene4OsawariPants>();
		StateMachineObservables[] rx = animator.GetRx();
		for (int i = 0; i < rx.Length; i++)
		{
			rx[i].OnStateExitObservable.Where(((StateID, AnimatorStateInfo) x) => x.Item1 == StateID.PantsOroshi).Subscribe(delegate
			{
				_pants.OffPants();
				IsAnimating = false;
			}).AddTo(this);
		}
	}

	protected override void OnLateUpdate()
	{
		if (!IsAnimating)
		{
			parameters[ParameterName.Pants].BlendToValue(CubismParameterBlendMode.Override, _oroshi.Value);
		}
		else
		{
			_oroshi = _oroshi.Update(parameters[ParameterName.Pants].Value);
		}
		_oroshiPaizuri.SynchroValue(_oroshi.Value);
	}

	protected override void UpdateParamsCore(Vector3 move)
	{
		if (!IsAnimating)
		{
			_manHand.Appear();
			if (move.y / SensitivityY < 0f)
			{
				IsAnimating = true;
				animator.SetTrigger("TriggerPantsOroshi");
				_manHand.SetValue(HandType.Left, 1f);
				_manHand.SetValue(HandType.Right, 1f);
			}
		}
	}

	protected override void UpdateWhileNotClicked()
	{
		if (!IsAnimating)
		{
			_manHand.Disappear();
		}
	}

	protected override bool GetConstraintsCore()
	{
		if (_pants.IsClosed)
		{
			return parameters[IsMB() ? ParameterName.MBPantsFlag : ParameterName.PantsFlag].Value == 1f;
		}
		return false;
	}

	public void ResetPantsOroshi()
	{
		if (_oroshi != null)
		{
			_oroshi = _oroshi.Update(0f);
			animator.SetTrigger("TriggerResetPants");
		}
	}

	public void SynchroValue(float value)
	{
		_oroshi = _oroshi.Update(value);
		if (value == 1f)
		{
			_pants.OffPants();
		}
	}

	public override void SetAuto()
	{
	}
}
