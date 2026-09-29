using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariAibuPaizuri : AbstractOsawariAnotherModel, IAibu
	{
		private ParameterValue _aibu;

		private OsawariPantsPaizuri _pants;

		private OsawariPaizuri _paizuri;

		private bool _isAutoAnimationReturning;

		public float AutoSpeed = 1f;

		private AverageCalculator _average;

		private FloatReactiveProperty _ave;

		public IReadOnlyReactiveProperty<float> GetAverageSpeed()
		{
			return _ave;
		}

		protected override void AutoAnimation()
		{
			if (_aibu.IsMax() || _aibu.IsMin())
			{
				_isAutoAnimationReturning = !_isAutoAnimationReturning;
			}
			Vector3 move = new Vector3(0f, _isAutoAnimationReturning ? (0f - AutoSpeed) : AutoSpeed, 0f);
			UpdateParamsCore(move);
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return GetParameterNumber((handType == HandType.Left) ? ParameterName.LeftHandOnAibu : ParameterName.RighthandOnAibu);
		}

		protected override void InitializeParams()
		{
			_aibu = new ParameterValue(parameters[ParameterName.Aibu]);
			_paizuri = _manager.OsawariPaizuri;
			_pants = _manager.GetOsawariOf<OsawariPantsPaizuri>();
			_average = new AverageCalculator(120);
			_ave = new FloatReactiveProperty(0f);
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Aibu, _aibu.Value);
			_ave.Value = _average.AbsAverage;
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			_aibu += move.y / SensitivityY;
			_average.Add(move.y / SensitivityY);
			_manHand.Appear(GetActiveHand());
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!IsAuto)
			{
				_manHand.Disappear();
				_aibu += 0.9f;
				_average.Add(0f);
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (_pants.IsAbleToInsert())
			{
				return !_paizuri.IsInPaizuriMode;
			}
			return false;
		}
	}
}
