using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariClothes : OsawariWithAnimation, ISwitchable, IWearable, IBra, IDoubleHanded
	{
		private OsawariShirt _osawariShirt;

		private FaceController _faceController;

		private OsawariArms _osawariArms;

		private bool _animeDone;

		[SerializeField]
		private bool _braFlag;

		private float _lastBraValue;

		private float _pastImpactBraTime;

		[SerializeField]
		private int ParameterBraFlagNumber = 53;

		[SerializeField]
		private int ParameterNobraFlagNumber = 55;

		[SerializeField]
		private int ParameterManHandTakeOffBraNumber = 74;

		[SerializeField]
		private float BraAnimeRealmUpper = 4f;

		[SerializeField]
		private float BraTouchRealmUpper = 2f;

		[SerializeField]
		private float BraOpenThreshold = 1.1f;

		[SerializeField]
		private float BraDownSpeed = 0.01f;

		[SerializeField]
		private float BraTouchRealmLower = 1f;

		[SerializeField]
		private float BraClickRealmLower;

		[SerializeField]
		private float BraInvalidRealmLower = -1f;

		[SerializeField]
		private float BraOpenSpeed = 0.05f;

		[SerializeField]
		private string BraTakeOffAnimeName = "Bra_TakeOff";

		[SerializeField]
		private string BraTakeOffSoundName = "bra_takeoff";

		[SerializeField]
		private string BraSwitchAnimeName = "Bra_Switch";

		[SerializeField]
		private string BraSwitchOnAnimeName = "Bra_SwitchOn";

		[SerializeField]
		private string BraSwitchOffAnimeName = "Bra_SwitchOff";

		[SerializeField]
		private string BraTakeOffFaceName = "Face_BraTakeOff";

		private ThresholdParameterValue _bra;

		private bool _isBraOpenAnimation;

		public bool IsBraTakingOff;

		public bool IsWearingBra => _braFlag;

		public bool Touchable => _braFlag;

		public bool IsWearing()
		{
			return _braFlag;
		}

		protected override void InitializeParams()
		{
			_osawariShirt = _manager.GetOsawariOf<OsawariShirt>();
			_faceController = GetComponent<FaceController>();
			_osawariArms = _manager.GetOsawariOf<OsawariArms>();
			float[] thresholds = new float[3] { BraClickRealmLower, BraTouchRealmLower, BraTouchRealmUpper };
			_bra = new ThresholdParameterValue(parameters[ParameterName.Bra], thresholds);
			_lastBraValue = _bra.Value;
		}

		public void TakeOnBra()
		{
			_manager.GetEveryOsawariOf<OsawariBrest>().ForEach(delegate(OsawariBrest x)
			{
				x.Cancel();
			});
			if (!_osawariShirt.GetShirtFlag() || _osawariShirt.NippleAllowed)
			{
				_manager.GetEveryOsawariOf<OsawariBrest>().ForEach(delegate(OsawariBrest x)
				{
					x.Cancel();
				});
				_manager.GetEveryOsawariOf<OsawariNipple>().ForEach(delegate(OsawariNipple x)
				{
					x.Cancel();
				});
				StartAnimation(AnimeName.BraSwitch, on: true).Forget();
				_bra = _bra.MoveSectionTo(1);
				SetLive2D(ParameterName.Bra, _bra.Value);
				_lastBraValue = _bra.Value;
				_braFlag = true;
			}
		}

		public void TakeOffBra(bool exc)
		{
			_manager.GetEveryOsawariOf<OsawariBrest>().ForEach(delegate(OsawariBrest x)
			{
				x.Cancel();
			});
			if (!_osawariShirt.GetShirtFlag() || _osawariShirt.NippleAllowed || exc)
			{
				StartAnimation(AnimeName.BraSwitch, on: false).Forget();
				_bra = _bra.MoveSectionTo(0);
				SetLive2D(ParameterName.Bra, _bra.Value);
				_lastBraValue = _bra.Value;
				_braFlag = false;
			}
		}

		public void SwitchBra()
		{
			List<OsawariBrest> everyOsawariOf = _manager.GetEveryOsawariOf<OsawariBrest>();
			if (!everyOsawariOf[0].IsAnimePlaying() && !everyOsawariOf[1].IsAnimePlaying() && _osawariShirt.CanBraTakeOff())
			{
				if (_braFlag)
				{
					TakeOffBra(exc: true);
				}
				else
				{
					TakeOnBra();
				}
			}
		}

		public bool GetBraFlag()
		{
			return _braFlag;
		}

		private IEnumerator _StateListener()
		{
			_animeDone = true;
			yield return null;
		}

		public void StateListener()
		{
			StartCoroutine(_StateListener());
		}

		protected override void AutoAnimation()
		{
			throw new NotImplementedException();
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return ParameterManHandTakeOffBraNumber;
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Bra, _bra.Value);
			_lastBraValue = _bra.Value;
		}

		protected override void UpdateWhileNotClicked()
		{
			_manHand.Disappear();
			if (_bra.Value > BraTouchRealmLower && _bra.Value <= BraOpenThreshold)
			{
				_bra = _bra.Update(Mathf.Max(_bra.Value - BraDownSpeed, BraTouchRealmLower));
			}
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (IsAnimating)
			{
				return;
			}
			if (_bra.Value > BraOpenThreshold && _bra.Value <= BraTouchRealmUpper)
			{
				if (_manager.GetMove().y > 0f)
				{
					AutoBraOpen().Forget();
				}
				_manHand.Appear(GetActiveHand());
			}
			else
			{
				_bra += move.y / SensitivityY;
				_manHand.Appear(GetActiveHand());
			}
		}

		private async UniTask AutoBraOpen()
		{
			if (_isBraOpenAnimation)
			{
				return;
			}
			_isBraOpenAnimation = true;
			while (_isBraOpenAnimation)
			{
				_bra += BraOpenSpeed;
				if (_bra.IsOnThresholdMax())
				{
					await TakeOffBraAnimation();
					_isBraOpenAnimation = false;
				}
				_manHand.Appear(GetActiveHand());
				await UniTask.Yield();
			}
			_manager.GetEveryOsawariOf<OsawariBrest>().ForEach(delegate(OsawariBrest x)
			{
				x.Cancel();
			});
		}

		protected override bool GetConstraintsCore()
		{
			if (_osawariShirt.CanBraTakeOff())
			{
				return !_osawariArms.IsArmClosed;
			}
			return false;
		}

		public override bool GetConstraints()
		{
			if (_braFlag)
			{
				return GetConstraintsCore();
			}
			return false;
		}

		protected override async void OnFirstClickCore()
		{
			if (_bra.Value < BraTouchRealmLower)
			{
				await HockAnimation();
			}
		}

		private async UniTask HockAnimation()
		{
			IsAnimating = true;
			while (!_bra.IsOnThresholdMax())
			{
				_bra += 0.1f;
				await UniTask.Yield(_token);
			}
			_bra = _bra.MoveSectionTo(2);
			IsAnimating = false;
		}

		private async UniTask TakeOffBraAnimation()
		{
			IsBraTakingOff = true;
			_faceController.FacialAnimation(BraTakeOffFaceName);
			animator.SetBool(StringsManager.GetAnimeName(AnimeName.BraSwitch), on: false);
			_bra = _bra.MoveSectionTo(3, useMinAsDefault: false);
			await StartAnimation(AnimeName.BraTakeOff, on: true, singleTime: true);
			_braFlag = false;
			IsBraTakingOff = false;
			_manager.OsawariFellatio.SetCloth(-1f, -1f, 0f);
			UnityEngine.Object.FindObjectOfType<OsawariGoods>().BraOff();
		}
	}
}
