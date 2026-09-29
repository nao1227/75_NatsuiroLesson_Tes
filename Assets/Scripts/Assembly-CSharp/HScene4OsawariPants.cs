using Paidia.satsuki1;

public class HScene4OsawariPants : OsawariPants, IPants
{
	private OsawariPantsOroshi _oroshi;

	private bool _isMicroBikini;

	public OsawariPantsPaizuri _pantsPaizuri;

	private OsawariAibu _aibu;

	public bool IsClosed => _pants.Value == 0f;

	public void SetMicroBikini()
	{
		_isMicroBikini = true;
	}

	protected override void InitializeParams()
	{
		base.InitializeParams();
		_aibu = _manager.GetOsawariOf<OsawariAibu>();
		_pantsFlag = _pantsFlag.Update(val: true);
		_oroshi = _manager.GetOsawariOf<OsawariPantsOroshi>();
		_piston = _manager.GetOsawariOf<HScene4OsawariPiston>();
		OnPants();
	}

	protected override void OnLateUpdate()
	{
		SetLive2D(ParameterName.Pants, _pants);
		if (_isMicroBikini)
		{
			SetLive2D(ParameterName.MicroBikiniLower, _pantsFlag);
			SetLive2D(ParameterName.PantsFlag, 0f);
		}
		else
		{
			SetLive2D(ParameterName.MicroBikiniLower, 0f);
			SetLive2D(ParameterName.PantsFlag, _pantsFlag);
		}
		_pantsPaizuri.SynchroValue(_pantsFlag.Value, _pants.Value);
	}

	protected override bool GetConstraintsCore()
	{
		if (_pantsFlag.AsBool() && _oroshi.IsOnHip && _piston.ManPenis.Value == ManPenisStatus.Unshown)
		{
			return !_aibu.IsAnimating;
		}
		return false;
	}

	public override bool IsAbleToInsert()
	{
		if (_pantsFlag.AsBool() && _oroshi.IsOnHip)
		{
			return _pants.Value > 0f;
		}
		return true;
	}

	public override void OnPants()
	{
		base.OnPants();
		if (_piston.GetManPenisValue() != ManPenisStatus.Unshown)
		{
			_pants = _pants.Update(1f);
		}
		_oroshi.ResetPantsOroshi();
	}

	public void SynchroValue(float flag, float value)
	{
	}
}
