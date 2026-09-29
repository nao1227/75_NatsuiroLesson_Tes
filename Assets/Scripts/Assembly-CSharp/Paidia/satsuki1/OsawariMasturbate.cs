using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariMasturbate : AbstractOsawariAnotherModel
	{
		public InsertController InsertController;

		public float ManExtacy;

		private ParameterValue _masturbate;

		private bool _isInFellatio;

		private bool _isInSuck;

		private bool _isReturning;

		private OsawariFellatio _fellatio;

		protected override void AutoAnimation()
		{
			if (IsAuto)
			{
				float easedValue = GetEasedValue(_lastTimeAuto);
				UpdateAutoTimeCount();
				float y = (GetEasedValue(_autoTime) - easedValue) * SensitivityY;
				Vector3 move = new Vector3(0f, y, 0f) * ((!_isReturning) ? 1 : (-1));
				UpdateParamsCore(move);
				if (_masturbate.IsMax() || _masturbate.IsMin() || _autoTime == AutoPeriod)
				{
					ResetAutoTimeCount();
					_isReturning = !_isReturning;
				}
			}
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			if (handType == HandType.Left)
			{
				return ParameterNumbers.GetTable()[ParameterName.LeftHandOnMasturbate];
			}
			return ParameterNumbers.GetTable()[ParameterName.RightHandOnMasturbate];
		}

		protected override void InitializeParams()
		{
			_masturbate = new ParameterValue(parameters[ParameterName.Masturbate]);
			_isInFellatio = false;
			_isInSuck = false;
			_fellatio = _manager.OsawariFellatio;
			_manager.OsawariFellatio.OnSuck.Subscribe(delegate(bool x)
			{
				_isInSuck = x;
			}).AddTo(this);
			_manager.OsawariFellatio.OnFellatio.Subscribe(delegate(bool x)
			{
				_isInFellatio = x;
			}).AddTo(this);
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Masturbate, _masturbate);
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!IsAuto)
			{
				_manHand.Disappear();
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (!_isInFellatio && !_isInSuck)
			{
				return !_fellatio.IsRestrictButton;
			}
			return false;
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			float num = move.y / SensitivityY;
			_masturbate += num;
			_manHand.Appear(GetActiveHand());
			if (Mathf.Abs(num) > 0.001f)
			{
				InsertController.AddManExtacy(ManExtacy);
			}
		}
	}
}
