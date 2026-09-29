using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariLeftHand : AbstractOsawari
	{
		private ParameterValue _leftHand;

		private bool _showLeftHand;

		private bool _autoMoveUp = true;

		protected override void InitializeParams()
		{
			_leftHand = new ParameterValue(parameters[ParameterName.LeftHand]);
			UtageManager utageManager = Object.FindObjectOfType<UtageManager>();
			utageManager.OnStartPlaying.Where((ScenarioLabel x) => x == ScenarioLabel.Click_LeftHand_Day5_2).Subscribe(delegate
			{
				_showLeftHand = true;
			}).AddTo(this);
			utageManager.OnFinishPlaying.Where((ScenarioLabel x) => x == ScenarioLabel.Click_LeftHand_Day5_2).Subscribe(delegate
			{
				_showLeftHand = false;
				InactivateLive2D(ParameterName.HandOnLeftHand);
			}).AddTo(this);
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			float num = (move.x / SensitivityX + move.y / SensitivityY) / 2f;
			_leftHand += num;
			_manHand.Appear();
		}

		protected override void AutoAnimation()
		{
			if (_autoMoveUp)
			{
				_leftHand += 0.01f;
				if (_leftHand.Value == 1f)
				{
					_autoMoveUp = false;
				}
			}
			else
			{
				_leftHand -= 0.01f;
				if (_leftHand.Value == 0f)
				{
					_autoMoveUp = true;
				}
			}
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return ParameterNumbers.GetTable()[ParameterName.HandOnLeftHand];
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.LeftHand, _leftHand);
			if (_showLeftHand)
			{
				_manHand.Appear();
				SetLive2D(ParameterName.HandOnLeftHand, _manHand.GetValue(HandType.Left));
			}
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!_showLeftHand)
			{
				_manHand.Disappear();
			}
		}

		protected override bool GetRestrictedCore()
		{
			if (!SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_LeftHand_Day5_2))
			{
				return SaveLoadManager.UnsavedData.Days < 6;
			}
			return false;
		}

		protected override bool GetConstraintsCore()
		{
			if (SaveLoadManager.UnsavedData.Days == 4)
			{
				return !SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Click_LeftHand_Day4);
			}
			return true;
		}
	}
}
