using System;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariHeadPaizuri : AbstractOsawariAnotherModel
	{
		public int HeadXLower;

		public int HeadXUpper;

		public int HeadYLower;

		public int HeadYUpper;

		public float HeadXBase;

		public float HeadYBase;

		public int HeadXAutoRatio = 10;

		public int HeadYAutoRatio = 10;

		private PhysicsCalculater _physicsCalculater;

		private ParameterValue _headY;

		private ParameterValue _headX;

		public bool UsePhysics = true;

		public bool InactivateWhileNotClicked;

		private OsawariPaizuri _paizuri;

		private Subject<Unit> _onStroke = new Subject<Unit>();

		public IObservable<Unit> OnStroke => _onStroke;

		protected override void InitializeParams()
		{
			_physicsCalculater = AnotherModel.GetComponent<PhysicsCalculater>();
			_headY = new ParameterValue(parameters[ParameterName.HeadY]);
			_headX = new ParameterValue(parameters[ParameterName.HeadX]);
			_paizuri = _manager.GetOsawariOf<OsawariPaizuri>();
		}

		protected override void OnLateUpdate()
		{
			_headX = _headX.Update(Math.Min(Math.Max(_headX.Value, HeadXLower), HeadXUpper));
			_headY = _headY.Update(Math.Min(Math.Max(_headY.Value, HeadYLower), HeadYUpper));
			if (Mathf.Abs(_headX.Value - HeadXBase) < 0.01f && Mathf.Abs(_headY.Value - HeadYBase) < 0.01f)
			{
				InactivateLive2D(ParameterName.HeadY);
				if (InactivateWhileNotClicked)
				{
					InactivateLive2D(ParameterName.HeadX);
				}
			}
			else
			{
				SetLive2D(ParameterName.HeadY, _headY.Value);
				SetLive2D(ParameterName.HeadX, _headX.Value);
			}
		}

		public bool IsReturning()
		{
			if (!(Mathf.Abs(_headY.Value - HeadYBase) > 0f))
			{
				return Mathf.Abs(_headX.Value - HeadXBase) > 0f;
			}
			return true;
		}

		protected override void AutoAnimation()
		{
			_headY = _headY.Update((float)HeadYAutoRatio * Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			_headX = _headX.Update((float)HeadXAutoRatio * Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			foreach (Hand item in GetActiveHand())
			{
				_manHand.SetValue(item.HandType, 1f);
			}
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return handType switch
			{
				HandType.Right => ParameterNumbers.GetTable()[ParameterName.RightHandOnHead], 
				HandType.Left => ParameterNumbers.GetTable()[ParameterName.LeftHandOnHead], 
				_ => throw new Exception(), 
			};
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (move.magnitude > 0f)
			{
				_onStroke.OnNext(Unit.Default);
			}
			_headY += move.y / SensitivityY;
			_headX += move.x / SensitivityX;
			_manHand.Appear(GetActiveHand());
		}

		protected override void UpdateWhileNotClicked()
		{
			if (IsReturning())
			{
				_headY -= (_headY.Value - HeadYBase) * 0.1f;
				_headX -= (_headX.Value - HeadXBase) * 0.1f;
			}
			_manHand.Disappear();
		}

		protected override void OnFirstClickCore()
		{
			base.OnFirstClickCore();
			_headX = _headX.Update(parameters[ParameterName.HeadX].Value);
			_headY = _headY.Update(parameters[ParameterName.HeadY].Value);
		}

		protected override bool GetConstraintsCore()
		{
			return !_paizuri.IsInPaizuriMode;
		}
	}
}
