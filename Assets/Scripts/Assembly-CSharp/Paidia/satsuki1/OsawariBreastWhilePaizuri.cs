using DG.Tweening;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariBreastWhilePaizuri : AbstractOsawariAnotherModel, IDoubleHanded, IBreast
	{
		private ParameterValue _breastX;

		private ParameterValue _breastY;

		private BoolReactiveProperty _grab = new BoolReactiveProperty(initialValue: false);

		private bool _breastMoving;

		private Tween _xTweener;

		private Tween _yTweener;

		public bool isInAction;

		private OsawariPaizuri _paizuri;

		private OsawariMizugiUpperPaizuri _mizugi;

		private bool _isMovingFromAnother;

		private bool _special;

		private bool _fromAnotherLastFrame;

		public float EnforcementFactor;

		public float PullFactor;

		private float _deltaT = 30f;

		public PhysicsCalculater PhysicsCalculater;

		public float EXTACY_MAN = 0.1f;

		public InsertController InsertController;

		public IReadOnlyReactiveProperty<bool> GetIsGrabbing()
		{
			return _grab;
		}

		protected override void SetTouchableMeshs()
		{
		}

		protected override void AutoAnimation()
		{
			_breastX = _breastX.Update(0f - Mathf.Sin(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			_breastY = _breastY.Update(Mathf.Cos(AnimationSpeed * SingletonManager<TimeManager>.Instance.ModifiedPassedTime));
			InsertController.AddManExtacy(EXTACY_MAN * 0.1f);
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return 0;
		}

		protected override void InitializeParams()
		{
			_breastX = new ParameterValue(parameters[ParameterName.BreastX]);
			_breastY = new ParameterValue(parameters[ParameterName.BreastY]);
			_mizugi = _manager.GetOsawariOf<OsawariMizugiUpperPaizuri>();
			_paizuri = _manager.GetOsawariOf<OsawariPaizuri>();
		}

		protected override void OnLateUpdate()
		{
			if (_breastMoving || _isMovingFromAnother || _fromAnotherLastFrame)
			{
				SetLive2D(ParameterName.BreastX, _breastX.Value);
				SetLive2D(ParameterName.BreastY, _breastY.Value);
			}
			else
			{
				InactivateLive2D(ParameterName.BreastX);
				InactivateLive2D(ParameterName.BreastY);
			}
			if (!_isMovingFromAnother && _fromAnotherLastFrame)
			{
				OnMouseUp();
			}
			_fromAnotherLastFrame = _isMovingFromAnother;
			_isMovingFromAnother = false;
			_special = false;
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (!IsAuto)
			{
				bool flag = _targetMesh == TouchableMeshs[1];
				_breastX += move.x / SensitivityX * (float)(flag ? 1 : (-1));
				_breastY += move.y / SensitivityY * (float)((!_isMovingFromAnother) ? 1 : (-1));
				_manHand.Lock();
				if (_special)
				{
					_paizuri.SynchroMove(move);
					return;
				}
				if (!_isMovingFromAnother)
				{
					Enforce(_breastY, move);
					InsertController.AddManExtacy(EXTACY_MAN * move.magnitude);
				}
			}
			_grab.Value = true;
			_lastValue = _breastY.Value;
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!_isMovingFromAnother && !IsAuto)
			{
				_manHand.Unlock();
				_grab.Value = false;
			}
		}

		protected override bool GetConstraintsCore()
		{
			return _paizuri.IsInPaizuriMode;
		}

		public override void OnMouseUp(bool fromCancel = false)
		{
			if (!IsAuto)
			{
				_xTweener = DOVirtual.Float(_breastX.Value, 0f, 1f, delegate(float x)
				{
					_breastX = _breastX.Update(x);
				}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).SetDelay(0.05f);
				_yTweener = DOVirtual.Float(_breastY.Value, 0f, 1f, delegate(float x)
				{
					_breastY = _breastY.Update(x);
				}).SetEase(Ease.OutElastic, 1.70158f, -0.3f).OnComplete(delegate
				{
					_breastMoving = false;
				});
				_breastMoving = true;
				_xTweener.Play();
				_yTweener.Play();
				base.OnMouseUp(fromCancel);
			}
		}

		protected override void OnFirstClickCore()
		{
			base.OnFirstClickCore();
			_xTweener?.Kill();
			_yTweener?.Kill();
			_breastMoving = true;
		}

		public override void OnSpecial()
		{
			_special = true;
		}

		public void SynchroMove(Vector3 move)
		{
			_isMovingFromAnother = true;
			UpdateParamsCore(move);
		}

		protected void Enforce(ParameterValue piston, Vector3 move)
		{
			_manager.GetCurrentMousePosition();
			_ = _mousePositionOnLastFrame;
			float num = (piston.Value - _lastValue) / _deltaT * 60f / base.fps / _deltaT * 10000f;
			PhysicsCalculater.Enforce(num * EnforcementFactor, move.y < 0f, PullFactor);
		}
	}
}
