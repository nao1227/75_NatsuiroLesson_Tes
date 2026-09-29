using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariShirt : OsawariWithAnimation, ISwitchable, IWearable, IShirt, ISkirt
	{
		private enum ButtonNumbers
		{
			Three = 0,
			Five = 1
		}

		private OsawariClothes _osawariClothes;

		private OsawariRibbon _osawariRibbon;

		private OsawariArms _osawariArms;

		private bool _onShirtAnimeFlag;

		private bool _offShirtAnimeFlag;

		private bool _onSkirtAnimeFlag;

		private bool _offSkirtAnimeFlag;

		private bool _animePlaying;

		private bool _animeDone;

		private float _pastShirtValue;

		private float _shirt;

		private BoolParameterValue _shirtFlag;

		private BoolParameterValue _skirtFlag;

		private int _openButtonCount;

		private Dictionary<ButtonNumbers, bool> _openShirtStatus;

		private float _pastImpactTime;

		public List<OsawariEvent> OnShirtsOpenEvents;

		public List<OsawariEvent> OnButtonOpenEvents;

		[SerializeField]
		private string ButtonTakeOffAnimeName = "Shirt_Button";

		[SerializeField]
		private string ShirtOpenAnimeName = "Shirt_Open";

		[SerializeField]
		private string ShirtSwitchAnimeName = "Shirt_Switch";

		[SerializeField]
		private List<CubismDrawable> ButtonMesh;

		private float deltaT;

		private bool _lockOpen;

		[SerializeField]
		private MeshDictionary _touchableMeshesWName;

		private bool _shirtOpen;

		public bool Touchable
		{
			get
			{
				if (_shirtFlag.AsBool())
				{
					return !_openShirtStatus[ButtonNumbers.Five];
				}
				return false;
			}
		}

		public bool NippleAllowed
		{
			get
			{
				if (!_shirtOpen)
				{
					return !_shirtFlag.AsBool();
				}
				return true;
			}
		}

		public bool IsWearing()
		{
			if (!_skirtFlag.AsBool())
			{
				return _shirtFlag.AsBool();
			}
			return true;
		}

		protected override void InitializeParams()
		{
			_manager = GetComponent<OsawariManager>();
			_osawariClothes = _manager.GetOsawariOf<OsawariClothes>();
			_osawariRibbon = _manager.GetOsawariOf<OsawariRibbon>();
			_osawariArms = _manager.GetOsawariOf<OsawariArms>();
			_shirt = 0f;
			_shirtOpen = false;
			_shirtFlag = new BoolParameterValue();
			_skirtFlag = new BoolParameterValue();
			_openShirtStatus = RefreshShirtStatus();
			_targetMesh = null;
			_lockOpen = false;
			foreach (OsawariEvent onShirtsOpenEvent in OnShirtsOpenEvents)
			{
				onShirtsOpenEvent.Initialize(this).Forget();
			}
			foreach (OsawariEvent onButtonOpenEvent in OnButtonOpenEvents)
			{
				onButtonOpenEvent.Initialize(this).Forget();
			}
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.ShirtFlag, _shirtFlag);
			SetLive2D(ParameterName.SkirtFlag, _skirtFlag);
			GetHandParameter(HandType.Left).Value = _manHand.GetValue(HandType.Right);
		}

		public void OnSkirt()
		{
			_onSkirtAnimeFlag = true;
			_offSkirtAnimeFlag = false;
			_skirtFlag = _skirtFlag.Update(val: true);
		}

		public void OffSkirt()
		{
			_offSkirtAnimeFlag = true;
			_onSkirtAnimeFlag = false;
			_skirtFlag = _skirtFlag.Update(val: false);
		}

		public void SwitchSkirt()
		{
			if (!_animePlaying)
			{
				if (_skirtFlag.AsBool())
				{
					OffSkirt();
				}
				else
				{
					OnSkirt();
				}
			}
		}

		public void OnShirt()
		{
			ResetButton().Forget();
			_osawariClothes.TakeOnBra();
			_openButtonCount = 0;
			StartAnimation(AnimeName.ShirtSwitch, on: true).Forget();
			StartAnimation(AnimeName.ShirtOpen, on: false).Forget();
			_openShirtStatus = RefreshShirtStatus();
			_osawariRibbon.OnRibbon();
			_shirtFlag = _shirtFlag.Update(val: true);
			_lockOpen = true;
		}

		public void OffShirt()
		{
			_manager.GetEveryOsawariOf<OsawariBrest>().ForEach(delegate(OsawariBrest x)
			{
				x.Cancel();
			});
			_osawariRibbon.OffRibbon();
			StartAnimation(AnimeName.ShirtSwitch, on: false).Forget();
			_shirtFlag = _shirtFlag.Update(val: false);
		}

		public void SwitchShirt()
		{
			if (!_animePlaying)
			{
				if (_shirtFlag.AsBool())
				{
					OffShirt();
				}
				else
				{
					OnShirt();
				}
			}
		}

		public bool GetShirtFlag()
		{
			return _shirtFlag.AsBool();
		}

		public int GetButtonNumberFlag()
		{
			return _openButtonCount;
		}

		public bool CanBraTakeOff()
		{
			if ((_openButtonCount != 3 && _openButtonCount != 4) || !_shirtFlag.AsBool())
			{
				if (_shirtFlag.AsBool())
				{
					return NippleAllowed;
				}
				return true;
			}
			return false;
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
		}

		protected override async void UpdateParamsCore(Vector3 move)
		{
			if (!_lockOpen)
			{
				_shirt += move.y / SensitivityY;
			}
			Dictionary<MeshName, CubismDrawable> tbl = _touchableMeshesWName.GetTable();
			if ((double)Time.time > (double)_pastImpactTime + 0.5 && (_shirt - _pastShirtValue) / deltaT * 60f / base.fps >= 0.01f && !IsOpen(ButtonNumbers.Three) && !IsOpen(ButtonNumbers.Five))
			{
				if (_openButtonCount == 3)
				{
					_openShirtStatus[ButtonNumbers.Three] = true;
				}
				else if (_openButtonCount == 5)
				{
					_openShirtStatus[ButtonNumbers.Five] = true;
				}
				foreach (OsawariEvent onShirtsOpenEvent in OnShirtsOpenEvents)
				{
					onShirtsOpenEvent.InvokeEvent(_manager.TemporaryStatus, _conditions);
				}
				await StartAnimation(AnimeName.ShirtOpen, on: true);
				_shirtOpen = true;
			}
			if (_targetMesh == tbl[MeshName.ShirtOpen3Left] || _targetMesh == tbl[MeshName.ShirtOpen3Right] || _targetMesh == tbl[MeshName.ShirtOpen5Left] || _targetMesh == tbl[MeshName.ShirtOpen5Right] || _targetMesh == tbl[MeshName.ShirtOpen35Left] || _targetMesh == tbl[MeshName.ShirtOpen35Right])
			{
				_manHand.Appear();
			}
			_pastShirtValue = _shirt;
		}

		protected override void UpdateWhileNotClicked()
		{
			if (_shirtFlag.AsBool())
			{
				_shirt *= 0.9f;
				_pastShirtValue = _shirt;
			}
			_manHand.Disappear();
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			if (CanOpen(ButtonNumbers.Five))
			{
				if (CanOpen(ButtonNumbers.Three))
				{
					return ParameterNumbers.GetTable()[ParameterName.ManHand35Flag];
				}
				return ParameterNumbers.GetTable()[ParameterName.ManHand5Flag];
			}
			if (_openButtonCount == 5)
			{
				return 0;
			}
			return ParameterNumbers.GetTable()[ParameterName.ManHand3Flag];
		}

		protected override void SetTouchableMeshs()
		{
			List<CubismDrawable> list = new List<CubismDrawable>();
			foreach (CubismDrawable item in ButtonMesh)
			{
				list.Add(item);
			}
			foreach (CubismDrawable value in _touchableMeshesWName.GetTable().Values)
			{
				list.Add(value);
			}
			TouchableMeshs = list.ToArray();
		}

		private bool CanOpen(ButtonNumbers num)
		{
			return num switch
			{
				ButtonNumbers.Three => isButtonCountBetween(3, 4) && !IsOpen(ButtonNumbers.Three), 
				ButtonNumbers.Five => _openButtonCount >= 5 && !IsOpen(ButtonNumbers.Five), 
				_ => throw new Exception(), 
			};
		}

		private bool IsOpen(ButtonNumbers num)
		{
			return _openShirtStatus[num];
		}

		protected override bool GetConstraintsCore()
		{
			if (_shirtFlag.AsBool() && !_handManager.IsGrabbingAny && !IsAnimating && !_osawariRibbon.GetRibbonFlag())
			{
				return !_osawariArms.IsArmClosed;
			}
			return false;
		}

		public override void OnClick(CubismDrawable targetMesh, bool isFirst)
		{
			if (targetMesh != null)
			{
				_targetMesh = targetMesh;
			}
			if (isFirst)
			{
				if (IsAuto)
				{
					IsAuto = false;
				}
				if (GetConstraintsCore() && _manager.CanGrab(this) && CanTouchMesh(targetMesh))
				{
					OnFirstClick();
					_manager.IsAction = true;
					_handManager.Grab(HandType.Left, this);
					_handManager.Grab(HandType.Right, this);
					OpenButtonIfMeshIsButton(targetMesh);
				}
			}
			if (_handManager.IsGrabbing(this))
			{
				UpdateParams(ConvertMovementVec3ForParams());
			}
		}

		public override void OnMouseUp(bool fromCancel = false)
		{
			_targetMesh = null;
			base.OnMouseUp(fromCancel);
		}

		private bool CanTouch(CubismDrawable targetMesh)
		{
			bool flag = false;
			if (ButtonMesh.Contains(targetMesh))
			{
				if (_openButtonCount > 2 && _skirtFlag.AsBool())
				{
					return false;
				}
				for (int i = 0; i < 5; i++)
				{
					if (targetMesh == ButtonMesh[i] && _openButtonCount == i)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return CanOpenAtClickedMesh(targetMesh);
			}
			return true;
		}

		private async void OpenButtonIfMeshIsButton(CubismDrawable targetMesh)
		{
			for (int i = 0; i <= 4; i++)
			{
				if (!(targetMesh == ButtonMesh[i]) || _openButtonCount != i)
				{
					continue;
				}
				foreach (OsawariEvent onButtonOpenEvent in OnButtonOpenEvents)
				{
					onButtonOpenEvent.InvokeEvent(_manager.TemporaryStatus, _conditions);
				}
				await StartAnimation(AnimeName.ButtonTakeOff, 1);
				_openButtonCount++;
			}
		}

		private bool isButtonCountBetween(int min, int max)
		{
			if (_openButtonCount >= min)
			{
				return _openButtonCount <= max;
			}
			return false;
		}

		private Dictionary<ButtonNumbers, bool> RefreshShirtStatus()
		{
			return new Dictionary<ButtonNumbers, bool>
			{
				{
					ButtonNumbers.Three,
					false
				},
				{
					ButtonNumbers.Five,
					false
				}
			};
		}

		private bool CanOpenAtClickedMesh(CubismDrawable mesh)
		{
			bool result = false;
			Dictionary<MeshName, CubismDrawable> table = _touchableMeshesWName.GetTable();
			if (mesh == table[MeshName.ShirtOpen3Left] || mesh == table[MeshName.ShirtOpen3Right])
			{
				result = CanOpen(ButtonNumbers.Three);
			}
			else if (mesh == table[MeshName.ShirtOpen5Left] || mesh == table[MeshName.ShirtOpen5Right])
			{
				result = CanOpen(ButtonNumbers.Five);
			}
			else if (mesh == table[MeshName.ShirtOpen35Left] || mesh == table[MeshName.ShirtOpen35Right])
			{
				result = CanOpen(ButtonNumbers.Three) && CanOpen(ButtonNumbers.Five);
			}
			return result;
		}

		protected virtual async UniTask ResetButton()
		{
			_shirtOpen = false;
			string animeName = StringsManager.GetAnimeName(AnimeName.ButtonTakeOff);
			animator.SetInteger(animeName, 0);
			IsAnimating = true;
			await UniTask.Yield(_token);
			await UniTask.Delay(TimeSpan.FromSeconds(animator.GetCurrentAnimatorStateInfo(0).length), ignoreTimeScale: false, PlayerLoopTiming.Update, _token);
			IsAnimating = false;
		}

		protected override void OnFirstClickCore()
		{
			_lockOpen = isButtonCountBetween(1, 2) || _openButtonCount == 4;
		}

		public override bool CanTouchMesh(CubismDrawable targetMesh)
		{
			if (!CanTouch(targetMesh))
			{
				return false;
			}
			Dictionary<MeshName, CubismDrawable> table = _touchableMeshesWName.GetTable();
			if (targetMesh == table[MeshName.ShirtOpen3Left] || targetMesh == table[MeshName.ShirtOpen3Right])
			{
				return _openButtonCount == 3;
			}
			if (targetMesh == table[MeshName.ShirtOpen5Left] || targetMesh == table[MeshName.ShirtOpen5Right])
			{
				return _openButtonCount == 5;
			}
			if (targetMesh == table[MeshName.ShirtOpen35Left] || targetMesh == table[MeshName.ShirtOpen35Right])
			{
				return _openButtonCount == 5;
			}
			if (ButtonMesh.Contains(targetMesh))
			{
				return ButtonMesh.FindIndex((CubismDrawable x) => x == targetMesh) == _openButtonCount;
			}
			return base.CanTouchMesh(targetMesh);
		}

		public override void SwitchContext()
		{
			if (_shirtFlag.AsBool() && _manager.ContextManager.Context == OsawariContext.Fellatio)
			{
				_manager.OsawariFellatio.SetShirtButton(_openButtonCount);
			}
		}
	}
}
