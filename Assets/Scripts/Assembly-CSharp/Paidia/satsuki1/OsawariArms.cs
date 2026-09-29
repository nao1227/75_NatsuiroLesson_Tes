using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariArms : OsawariDoublehanded
	{
		private ParameterValue _arms;

		public float UpperValue = 2f;

		public float MiddleValue = 1.4f;

		public float LowerValue = -1.9f;

		public float DefaultValue;

		public int ArmDownWaitMillsec = 1000;

		public int ArmDownTimeMillsec = 1000;

		public float ArmAutoOpenSpeed = 0.05f;

		private CancellationTokenSource _cts;

		private bool _isWaitingArmDown;

		private bool _isArmAutoOpening;

		private OsawariCosplay _cosplay;

		public bool IsArmClosed => _arms.Value < DefaultValue;

		protected override void AutoAnimation()
		{
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			if (handType == HandType.Left)
			{
				return ParameterNumbers.GetTable()[ParameterName.LeftHandOnArms];
			}
			return ParameterNumbers.GetTable()[ParameterName.RightHandOnArms];
		}

		protected override void SetTouchableMeshs()
		{
		}

		protected override void InitializeParams()
		{
			_arms = new ParameterValue(parameters[ParameterName.ArmLeft]);
			_cts = new CancellationTokenSource();
			_cosplay = _manager.GetOsawariOf<OsawariCosplay>();
		}

		protected override void OnLateUpdate()
		{
			if (!(null == _cosplay))
			{
				if (_cosplay.IsHandCuffOn && _arms.Value < 1.8f)
				{
					_arms = _arms.Update(1.8f);
				}
				SetLive2D(ParameterName.ArmLeft, _arms);
				SetLive2D(ParameterName.ArmRight, 0f - _arms.Value);
			}
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (IsArmClosed && move.y > 0f)
			{
				AutoOpen().Forget();
			}
			else if (!IsArmClosed)
			{
				_arms += move.y / SensitivityY;
				if (IsArmClosed)
				{
					_arms += DefaultValue - _arms.Value;
				}
			}
			_manHand.Appear();
			if (_isWaitingArmDown)
			{
				_cts.Cancel();
				_cts = new CancellationTokenSource();
				_isWaitingArmDown = false;
			}
		}

		private async UniTask AutoOpen()
		{
			if (!_isArmAutoOpening)
			{
				_isArmAutoOpening = true;
				while (IsArmClosed)
				{
					_arms += ArmAutoOpenSpeed;
					_manHand.Appear();
					await UniTask.Yield(_cts.Token);
				}
				_arms = _arms.Update(DefaultValue);
				_isArmAutoOpening = false;
			}
		}

		protected override void UpdateWhileNotClicked()
		{
			_manHand.Disappear();
			ArmDownAsync().Forget();
		}

		private async UniTask ArmDownAsync()
		{
			if (!_isWaitingArmDown && !_arms.IsMin())
			{
				_isWaitingArmDown = true;
				await UniTask.Delay(ArmDownWaitMillsec, ignoreTimeScale: false, PlayerLoopTiming.Update, _cts.Token);
				float targetValue = LowerValue;
				float startArmValue = _arms.Value;
				if (startArmValue > MiddleValue)
				{
					targetValue = MiddleValue;
				}
				else if (startArmValue > DefaultValue)
				{
					targetValue = DefaultValue;
				}
				for (int i = 0; i < ArmDownTimeMillsec / 20; i++)
				{
					_arms -= (startArmValue - targetValue) / (float)(ArmDownTimeMillsec / 20);
					await UniTask.Delay(20, ignoreTimeScale: false, PlayerLoopTiming.Update, _cts.Token);
				}
				_arms = _arms.Update(targetValue);
				_isWaitingArmDown = false;
			}
		}

		private void OnDestroy()
		{
			_cts?.Cancel();
		}
	}
}
