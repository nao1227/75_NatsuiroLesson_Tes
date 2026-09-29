using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class HScene3OsawariAibu : OsawariWithoutHand, IAibu
	{
		private ParameterValue _aibu;

		private ParameterValue _manLeg;

		private HScene3OsawariPants _pants;

		private OsawariLeg _leg;

		private HScene3OsawariHelper _helper;

		private OsawariVibrator _vib;

		private AverageCalculator _average;

		private FloatReactiveProperty _ave;

		public IReadOnlyReactiveProperty<float> GetAverageSpeed()
		{
			return _ave;
		}

		protected override void AutoAnimation()
		{
			float value = _aibu.Value;
			_aibu = _aibu.Update(Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime) + 1f);
			_average.Add(_aibu.Value - value);
		}

		protected override void InitializeParams()
		{
			_aibu = new ParameterValue(parameters[ParameterName.Aibu]);
			_manLeg = new ParameterValue(parameters[ParameterName.ManLeg]);
			_pants = _manager.GetOsawariOf<HScene3OsawariPants>();
			_helper = _manager.GetOsawariOf<HScene3OsawariHelper>();
			_leg = _manager.GetOsawariOf<OsawariLeg>();
			_vib = _manager.GetOsawariOf<OsawariVibrator>();
			_leg.OnLegClosing.Subscribe(delegate
			{
				Cancel();
			});
			_average = new AverageCalculator(120);
			_ave = new FloatReactiveProperty(0f);
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Aibu, _aibu);
			SetLive2D(ParameterName.ManLeg, _manLeg);
			_ave.Value = _average.AbsAverage;
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (!IsAuto)
			{
				_aibu += move.y / SensitivityY;
				_manLeg += 0.1f;
				_average.Add(move.y / SensitivityY);
			}
		}

		protected override void UpdateWhileNotClicked()
		{
			_aibu -= 0.1f;
			_manLeg -= 0.1f;
			_average.Add(0f);
		}

		protected override bool GetConstraintsCore()
		{
			if (_pants.IsOpen || (_leg.IsOpen && _helper.ClothStatus == ClothStatus.Naked))
			{
				return !_vib.IsVibAppeared;
			}
			return false;
		}
	}
}
