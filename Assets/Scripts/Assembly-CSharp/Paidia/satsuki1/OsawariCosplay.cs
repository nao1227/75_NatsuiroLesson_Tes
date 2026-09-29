using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariCosplay : AbstractOsawari, IWearable
	{
		private ClothName _cloth;

		private OsawariPants _pants;

		private OsawariPiston _piston;

		private SingleThresholdParameterValue _bunny;

		private SingleThresholdParameterValue _wetSuitUpper;

		private ParameterValue _wetSuitLower;

		private BoolParameterValue _sm;

		private BoolParameterValue _smNipple;

		private BoolParameterValue _smBlindfold;

		private BoolParameterValue _smHandcuffs;

		private SingleThresholdParameterValue _cat;

		public List<ClothNameFloatPair> _upperDict;

		public List<ClothNameFloatPair> _lowerDict;

		public CubismDrawable WetSuitMesh;

		public CubismDrawable CatMeshLeft;

		public CubismDrawable CatMeshRight;

		public CubismDrawable CatMeshPants;

		public CubismDrawable BunnyMeshLeft;

		public CubismDrawable BunnyMeshRight;

		public CubismDrawable BunnyMeshPants;

		public CubismDrawable SMMesh;

		private OsawariArms _arms;

		private bool _isAbleToInsert;

		private bool _isAutoOpeningWetSuitUpper;

		private bool _isAutoOpeningCat;

		public OsawariGoods Goods;

		public bool IsLoaded { get; private set; }

		public bool IsHandCuffOn
		{
			get
			{
				if (_cloth == ClothName.SM && _smHandcuffs != null)
				{
					return _smHandcuffs.AsBool();
				}
				return false;
			}
		}

		public bool IsWearingBunnyBra
		{
			get
			{
				if (_cloth == ClothName.Bunny)
				{
					return _bunny.Value == _bunny.GetMinValueOfSection(1);
				}
				return false;
			}
		}

		public bool IsAbleToInsert()
		{
			if (_cloth == ClothName.Normal || _cloth == ClothName.MicroBikini)
			{
				return false;
			}
			return _isAbleToInsert;
		}

		protected override void SetTouchableMeshs()
		{
			switch (_cloth)
			{
			case ClothName.Normal:
			case ClothName.SM:
				TouchableMeshs = new CubismDrawable[1] { SMMesh };
				break;
			case ClothName.Bunny:
				TouchableMeshs = new CubismDrawable[1] { BunnyMeshPants };
				break;
			case ClothName.MicroBikini:
				TouchableMeshs = new CubismDrawable[1] { WetSuitMesh };
				break;
			case ClothName.Cat:
				TouchableMeshs = new CubismDrawable[3] { CatMeshLeft, CatMeshRight, CatMeshPants };
				break;
			}
		}

		protected override void AutoAnimation()
		{
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return _cloth switch
			{
				ClothName.Bunny => ParameterNumbers.GetTable()[ParameterName.BunnyHand], 
				ClothName.Cat => ParameterNumbers.GetTable()[ParameterName.CatHand], 
				ClothName.MicroBikini => ParameterNumbers.GetTable()[ParameterName.WetSuitHand], 
				ClothName.SM => ParameterNumbers.GetTable()[ParameterName.SMHand], 
				_ => throw new NotImplementedException(), 
			};
		}

		protected override void InitializeParams()
		{
			_bunny = new SingleThresholdParameterValue(parameters[ParameterName.BunnyFlag], _lowerDict.First((ClothNameFloatPair x) => x.Cloth == ClothName.Bunny).Value);
			_wetSuitUpper = new SingleThresholdParameterValue(parameters[ParameterName.WetSuitUpperFlag], _lowerDict.First((ClothNameFloatPair x) => x.Cloth == ClothName.MicroBikini).Value);
			_wetSuitLower = new ParameterValue(parameters[ParameterName.WetSuitLowerFlag]);
			_sm = new BoolParameterValue(parameters[ParameterName.SMFlag]);
			_smNipple = new BoolParameterValue(parameters[ParameterName.SM_Nipples]);
			_smBlindfold = new BoolParameterValue(parameters[ParameterName.SM_Bindfold]);
			_smHandcuffs = new BoolParameterValue(parameters[ParameterName.SM_Handcuffs]);
			_cat = new SingleThresholdParameterValue(parameters[ParameterName.CatFlag], _lowerDict.First((ClothNameFloatPair x) => x.Cloth == ClothName.Cat).Value);
			_bunny = _bunny.Update(1f);
			_wetSuitLower = _wetSuitLower.Update(1f);
			_wetSuitUpper = _wetSuitUpper.MoveToUpperSection();
			_wetSuitUpper = _wetSuitUpper.Update(0f);
			_cat = _cat.MoveToUpperSection();
			_cat = _cat.Update(1f);
			_cloth = _manager.TemporaryStatus.Cloth;
			_arms = _manager.GetOsawariOf<OsawariArms>();
			_pants = _manager.GetOsawariOf<OsawariPants>();
			if (_cloth == ClothName.MicroBikini)
			{
				_pants.SetPantsName(GetParameterNumber(ParameterName.WetSuitLowerFlag));
				_pants.OnPants();
			}
			else if (_cloth != ClothName.Normal)
			{
				_pants.InactivatePants();
				_pants.OnPants();
			}
			_piston = _manager.GetOsawariOf<OsawariPiston>();
			IsLoaded = true;
		}

		protected override void OnLateUpdate()
		{
			switch (_manager.TemporaryStatus.Cloth)
			{
			case ClothName.Bunny:
				SetLive2D(ParameterName.BunnyFlag, _bunny.Value);
				if (_bunny.Value == 2f || _bunny.Value == 0f)
				{
					Goods.BraOff();
				}
				else
				{
					Goods.BraOn();
				}
				break;
			case ClothName.MicroBikini:
				SetLive2D(ParameterName.WetSuitUpperFlag, _wetSuitUpper.Value);
				if (_wetSuitUpper.Value == -1f || _wetSuitUpper.Value == 2f)
				{
					Goods.BraOff();
				}
				else
				{
					Goods.BraOn();
				}
				break;
			case ClothName.Cat:
				SetLive2D(ParameterName.CatFlag, _cat.Value);
				if (_cat.Value == 2f || _cat.Value == 0f)
				{
					Goods.BraOff();
				}
				else
				{
					Goods.BraOn();
				}
				break;
			case ClothName.SM:
				SetLive2D(ParameterName.SMFlag, _sm);
				SetLive2D(ParameterName.SM_Bindfold, _smBlindfold);
				SetLive2D(ParameterName.SM_Nipples, _smNipple);
				if (_smNipple.AsBool() && _sm.AsBool())
				{
					Goods.BraOn();
				}
				else
				{
					Goods.BraOff();
				}
				SetLive2D(ParameterName.SM_Handcuffs, _smHandcuffs);
				break;
			case ClothName.Normal:
				break;
			}
		}

		private bool IsPants()
		{
			if (!(_targetMesh == BunnyMeshPants) && !(_targetMesh == CatMeshPants))
			{
				return _targetMesh == SMMesh;
			}
			return true;
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			_manHand.Unlock();
			if (!IsPants() && _handManager.IsGrabbingAny)
			{
				AbstractOsawari grabbingTarget = _handManager.RightHand.GrabbingTarget;
				AbstractOsawari grabbingTarget2 = _handManager.LeftHand.GrabbingTarget;
				grabbingTarget?.Cancel();
				grabbingTarget2?.Cancel();
			}
			switch (_cloth)
			{
			case ClothName.Bunny:
				if (_targetMesh == BunnyMeshPants)
				{
					_manHand.Lock();
					_pants.ForceUpdateParams(move, this);
				}
				else if (_bunny.Value >= _lowerDict.First((ClothNameFloatPair x) => x.Cloth == ClothName.Bunny).Value)
				{
					_bunny -= move.y / SensitivityY;
				}
				break;
			case ClothName.MicroBikini:
				if (_wetSuitUpper.Value >= _lowerDict.First((ClothNameFloatPair x) => x.Cloth == ClothName.MicroBikini).Value && !_wetSuitUpper.IsOnThresholdMax())
				{
					if (_wetSuitUpper.Value > 0f)
					{
						AutoOpenWetSuitUpper();
					}
					else
					{
						_wetSuitUpper += move.y / SensitivityY;
					}
				}
				break;
			case ClothName.Cat:
			{
				if (_targetMesh == CatMeshPants)
				{
					_manHand.Lock();
					_pants.ForceUpdateParams(move, this);
					break;
				}
				if (_isAutoOpeningCat)
				{
					return;
				}
				int num = ((!(_targetMesh == CatMeshLeft)) ? 1 : (-1));
				_cat -= (float)num * move.x / SensitivityX;
				if (_cat.Value >= 1.3f)
				{
					AutoOpenCat();
				}
				break;
			}
			case ClothName.SM:
				_manHand.Lock();
				_pants.ForceUpdateParams(move, this);
				break;
			}
			if (!IsPants())
			{
				_manHand.Appear();
			}
		}

		private void AutoOpenWetSuitUpper()
		{
			if (!_isAutoOpeningWetSuitUpper)
			{
				_isAutoOpeningWetSuitUpper = true;
				DOVirtual.Float(_wetSuitUpper.Value, 2f, 0.5f, delegate(float x)
				{
					_wetSuitUpper = _wetSuitUpper.Update(x);
				}).OnComplete(delegate
				{
					_isAutoOpeningWetSuitUpper = false;
				}).Play();
			}
		}

		private void AutoOpenCat()
		{
			if (!_isAutoOpeningCat)
			{
				_isAutoOpeningCat = true;
				DOVirtual.Float(_cat.Value, 2f, 0.5f, delegate(float x)
				{
					_cat = _cat.Update(x);
				}).OnComplete(delegate
				{
					_isAutoOpeningCat = false;
				}).Play();
			}
		}

		protected override void UpdateWhileNotClicked()
		{
			_manHand.Unlock();
			_manHand.Disappear();
		}

		public void ForceCosplayBreast(ClothName cloth, bool on)
		{
			switch (cloth)
			{
			case ClothName.Bunny:
				_bunny = _bunny.Update(on ? 1 : 0);
				break;
			case ClothName.MicroBikini:
				_wetSuitUpper = _wetSuitUpper.Update((!on) ? (-1) : 0);
				break;
			case ClothName.Cat:
				_cat = _cat.Update(on ? 1 : 0);
				break;
			case ClothName.SM:
				if (!_sm.AsBool())
				{
					WearAll(ClothName.SM);
				}
				_smNipple = _smNipple.Update(on);
				break;
			}
		}

		public void SwitchCosplayBreast(ClothName cloth)
		{
			int num = 0;
			switch (cloth)
			{
			case ClothName.Bunny:
				if (_bunny.Value == 0f)
				{
					WearAll(ClothName.Bunny);
					break;
				}
				if (_bunny.Value == 2f)
				{
					BraOn();
				}
				else
				{
					BraOff();
				}
				_bunny = _bunny.Update((_bunny.Value == 2f) ? 1 : 2);
				break;
			case ClothName.MicroBikini:
				num = -1;
				if (_wetSuitUpper.Value != 0f)
				{
					num = 0;
					_wetSuitUpper = _wetSuitUpper.MoveToUpperSection();
					BraOn();
				}
				else
				{
					_wetSuitUpper = _wetSuitUpper.MoveToLowerSection();
					BraOff();
				}
				_wetSuitUpper = _wetSuitUpper.Update(num);
				break;
			case ClothName.Cat:
				if (_cat.Value == 0f)
				{
					WearAll(cloth);
					break;
				}
				if (_cat.Value == 2f)
				{
					BraOn();
				}
				else
				{
					BraOff();
				}
				_cat = _cat.Update((_cat.Value == 2f) ? 1 : 2);
				break;
			case ClothName.SM:
			{
				if (!_sm.AsBool())
				{
					WearAll(ClothName.SM);
					break;
				}
				bool flag = true;
				if (_smNipple.AsBool())
				{
					flag = false;
				}
				if (flag)
				{
					BraOn();
				}
				else
				{
					BraOff();
				}
				_smNipple = _smNipple.Update(flag);
				break;
			}
			}
		}

		public void WearAll(ClothName cloth)
		{
			_isAbleToInsert = false;
			if (_manager.ContextManager.Context != OsawariContext.Fellatio || cloth != ClothName.Bunny)
			{
				BraOn();
				if (!_pants.IsAbleToInsert())
				{
					Goods.PantsOn();
				}
			}
			_pants.OnPants();
			switch (cloth)
			{
			case ClothName.Bunny:
				_bunny = _bunny.MoveToUpperSection();
				_bunny = _bunny.Update(1f);
				break;
			case ClothName.MicroBikini:
				_wetSuitLower = _wetSuitLower.Update(1f);
				_wetSuitUpper = _wetSuitUpper.MoveToUpperSection();
				_wetSuitUpper = _wetSuitUpper.Update(0f);
				break;
			case ClothName.Cat:
				_cat = _cat.MoveToUpperSection();
				_cat = _cat.Update(1f);
				break;
			case ClothName.SM:
				_sm = _sm.Update(val: true);
				_smBlindfold = _smBlindfold.Update(val: true);
				_smHandcuffs = _smHandcuffs.Update(val: true);
				_smNipple = _smNipple.Update(val: true);
				break;
			}
		}

		protected virtual void BraOn()
		{
			Goods.BraOn();
			_manager.GetEveryOsawariOf<OsawariNipple>().ForEach(delegate(OsawariNipple x)
			{
				x.Cancel();
			});
			_manager.GetEveryOsawariOf<OsawariBrest>().ForEach(delegate(OsawariBrest x)
			{
				x.Cancel();
			});
		}

		protected virtual void BraOff()
		{
			Goods.BraOff();
			_manager.GetEveryOsawariOf<OsawariBrest>().ForEach(delegate(OsawariBrest x)
			{
				x.Cancel();
			});
		}

		public void SwitchSM(SubIconType target)
		{
			if (!_sm.AsBool())
			{
				WearAll(ClothName.SM);
				return;
			}
			switch (target)
			{
			case SubIconType.SwitchSMBlindfold:
				_smBlindfold = _smBlindfold.Update(!_smBlindfold.AsBool());
				break;
			case SubIconType.SwitchSMHandcuffs:
				_smHandcuffs = _smHandcuffs.Update(!_smHandcuffs.AsBool());
				break;
			}
		}

		public void UnwearAll(ClothName cloth)
		{
			_isAbleToInsert = true;
			BraOff();
			Goods.PantsOff();
			_pants.OffPants();
			switch (cloth)
			{
			case ClothName.Bunny:
				_bunny = _bunny.MoveToLowerSection();
				_bunny = _bunny.Update(0f);
				break;
			case ClothName.MicroBikini:
				_wetSuitLower = _wetSuitLower.Update(0f);
				_wetSuitUpper = _wetSuitUpper.MoveToLowerSection();
				_wetSuitUpper = _wetSuitUpper.Update(-1f);
				break;
			case ClothName.Cat:
				_cat = _cat.MoveToLowerSection();
				_cat = _cat.Update(0f);
				break;
			case ClothName.SM:
				_sm = _sm.Update(val: false);
				_smBlindfold = _smBlindfold.Update(val: false);
				_smHandcuffs = _smHandcuffs.Update(val: false);
				_smNipple = _smNipple.Update(val: false);
				break;
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (IsPants() && _piston.ManPenis.Value != ManPenisStatus.Unshown)
			{
				return false;
			}
			if (_cloth == ClothName.Normal || _bunny.Value == 0f || _wetSuitUpper.Value == -1f || _cat.Value == 0f)
			{
				return false;
			}
			return !_arms.IsArmClosed;
		}

		public override void SwitchContext()
		{
			switch (_manager.TemporaryStatus.Cloth)
			{
			case ClothName.Bunny:
				_manager.OsawariFellatio.SetCosplay(_bunny.Value);
				if (_manager.ContextManager.Context == OsawariContext.Fellatio)
				{
					BraOff();
				}
				else if (_bunny.Value == 1f)
				{
					BraOn();
				}
				break;
			case ClothName.MicroBikini:
				_manager.OsawariFellatio.SetCosplay(0f, (_wetSuitUpper.Value == 0f) ? 1 : 0);
				break;
			case ClothName.Cat:
				_manager.OsawariFellatio.SetCosplay(0f, 0f, 0f, _cat.Value);
				break;
			case ClothName.SM:
				_manager.OsawariFellatio.SetCosplay(0f, 0f, _sm.Value);
				break;
			case ClothName.Normal:
				break;
			}
		}

		private void Update()
		{
			if (IsLoaded)
			{
				switch (_manager.TemporaryStatus.Cloth)
				{
				case ClothName.Bunny:
					_manager.OsawariFellatio.SetCosplay(_bunny.Value);
					break;
				case ClothName.MicroBikini:
					_manager.OsawariFellatio.SetCosplay(0f, (_wetSuitUpper.Value == 0f) ? 1 : 0);
					break;
				case ClothName.Cat:
					_manager.OsawariFellatio.SetCosplay(0f, 0f, 0f, _cat.Value);
					break;
				case ClothName.SM:
					_manager.OsawariFellatio.SetCosplay(0f, 0f, _sm.Value, 0f, _smNipple.Value, _smBlindfold.Value);
					break;
				case ClothName.Normal:
					break;
				}
			}
		}

		public override void OnMouseUp(bool fromCancel = false)
		{
			_pants.EnableUpdateWhileNotClicked();
		}

		public bool IsWearing()
		{
			return _cloth switch
			{
				ClothName.Normal => false, 
				ClothName.MicroBikini => _wetSuitUpper.Value == 0f, 
				ClothName.Bunny => _bunny.Value == 1f, 
				ClothName.Cat => _cat.Value == 1f, 
				ClothName.SM => _sm.AsBool(), 
				_ => false, 
			};
		}

		public bool IsWearingBra()
		{
			return _cloth switch
			{
				ClothName.Normal => false, 
				ClothName.MicroBikini => _wetSuitUpper.Value == 0f, 
				ClothName.Bunny => _bunny.Value == 1f, 
				ClothName.Cat => _cat.Value == 1f, 
				ClothName.SM => _smNipple.AsBool(), 
				_ => false, 
			};
		}

		public override bool CanTouchMesh(CubismDrawable targetMesh)
		{
			if (_cloth == ClothName.Cat && targetMesh != CatMeshPants)
			{
				if (_cat.Value <= 1.3f)
				{
					return base.CanTouchMesh(targetMesh);
				}
				return false;
			}
			return base.CanTouchMesh(targetMesh);
		}
	}
}
