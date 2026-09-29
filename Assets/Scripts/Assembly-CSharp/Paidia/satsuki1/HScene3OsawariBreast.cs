using UnityEngine;

namespace Paidia.satsuki1
{
	public class HScene3OsawariBreast : OsawariBrest
	{
		public int ManHandIndexWearing;

		public int ManHandIndexNakedOrSwimsuit;

		public int ManHandIndexUnderwear;

		private HScene3OsawariHelper _helper;

		protected override bool _isBraOn
		{
			get
			{
				if (_helper.ClothStatus != ClothStatus.WearHalf)
				{
					return _helper.ClothStatus == ClothStatus.Underwear;
				}
				return true;
			}
		}

		protected override void InitializeParams()
		{
			base.InitializeParams();
		}

		public void SetHandParam(HScene3OsawariHelper helper)
		{
			_helper = helper;
			switch (_helper.ClothStatus)
			{
			case ClothStatus.WearAll:
				ParameterNumbers.GetTable()[ParameterName.HandOnBreast] = ManHandIndexWearing;
				ParameterNumbers.GetTable()[ParameterName.HandOnBreastWithBra] = ManHandIndexWearing;
				break;
			case ClothStatus.SwimSuit:
			case ClothStatus.Naked:
				ParameterNumbers.GetTable()[ParameterName.HandOnBreast] = ManHandIndexNakedOrSwimsuit;
				break;
			case ClothStatus.WearHalf:
			case ClothStatus.Underwear:
				ParameterNumbers.GetTable()[ParameterName.HandOnBreast] = ManHandIndexUnderwear;
				ParameterNumbers.GetTable()[ParameterName.HandOnBreastWithBra] = ManHandIndexUnderwear;
				break;
			}
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			base.UpdateParamsCore(move);
		}

		protected override void AutoAnimation()
		{
			base.AutoAnimation();
			if (_isBraOn)
			{
				breastParamX = breastParamX.Update(breastParamX.Value * 0.1f);
				breastParamY = breastParamY.Update(breastParamY.Value * 0.1f);
			}
		}

		protected override bool GetRestrictedCore()
		{
			return SaveLoadManager.UnsavedData.Days < 5;
		}

		protected override bool GetConstraintsCore()
		{
			bool flag = base.GetConstraintsCore();
			if (flag)
			{
				flag = SaveLoadManager.UnsavedData.Days switch
				{
					1 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Breast_Day1), 
					2 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Breast_Day2), 
					3 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Breast_Day3), 
					4 => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_Breast_Day4), 
					_ => SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Study_H) || SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH, 
				};
			}
			return flag;
		}
	}
}
