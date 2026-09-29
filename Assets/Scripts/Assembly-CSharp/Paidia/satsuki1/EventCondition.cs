using System;
using System.ComponentModel;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class EventCondition
	{
		public EventConditionName Condition;

		public float Value;

		private GameObject _cache;

		private OsawariManager _manager;

		public bool IsFUllfilSubEventCondition()
		{
			return Condition switch
			{
				EventConditionName.FlagOn => SaveLoadManager.UnsavedData.GlobalFlags.IsOn((FlagEnum)Value), 
				EventConditionName.FlagOff => !SaveLoadManager.UnsavedData.GlobalFlags.IsOn((FlagEnum)Value), 
				EventConditionName.ScenarioRead => SaveLoadManager.UnsavedData.GlobalFlags.IsRead((ScenarioLabel)Value), 
				EventConditionName.ScenarioNotRead => !SaveLoadManager.UnsavedData.GlobalFlags.IsRead((ScenarioLabel)Value), 
				EventConditionName.LeastFavorability => (float)SaveLoadManager.UnsavedData.PersistantStatus.GetFavorabilityLevel() >= Value, 
				EventConditionName.LeastSensitivity => (float)SaveLoadManager.UnsavedData.PersistantStatus.GetSensitivityLevel() >= Value, 
				EventConditionName.IsDay => (float)SaveLoadManager.UnsavedData.Days == Value, 
				EventConditionName.IsDayBefore => (float)SaveLoadManager.UnsavedData.Days < Value, 
				EventConditionName.IsDayAfter => (float)SaveLoadManager.UnsavedData.Days > Value, 
				EventConditionName.LastSelected => (float)SaveLoadManager.UnsavedData.LastSelectedIndex == Value, 
				_ => throw new InvalidEnumArgumentException($"{Condition} is not sub event condition"), 
			};
		}

		public bool IsFullfillCondition(TemporaryStatus status, OsawariConditions conditions)
		{
			PersistantStatus persistantStatus = SaveLoadManager.UnsavedData.PersistantStatus;
			switch (Condition)
			{
			case EventConditionName.LeastExcitement:
				return (float)status.Feelings.Excite >= Value;
			case EventConditionName.MostExcitement:
				return (float)status.Feelings.Excite <= Value;
			case EventConditionName.LeastAtomosphere:
				return (float)status.GetCurrentAtomosphere() >= Value;
			case EventConditionName.MostAtomosphere:
				return (float)status.GetCurrentAtomosphere() <= Value;
			case EventConditionName.LeastStimulus:
				return (float)status.Feelings.Stimulus > Value;
			case EventConditionName.LeastSpeed:
				return conditions.Speed > Value;
			case EventConditionName.MostSpeed:
				return conditions.Speed < Value;
			case EventConditionName.LeastDistance:
				return conditions.MovedDistance > Value;
			case EventConditionName.IsAtomosphere:
				return status.GetCurrentAtomosphere() == (AtomosphereName)(int)Value;
			case EventConditionName.IsRelationShip:
				return persistantStatus.Relationship == (Relationship)(int)Value;
			case EventConditionName.LeastRelationShip:
				return (int)persistantStatus.Relationship >= (int)Value;
			case EventConditionName.MostRelationShip:
				return (int)persistantStatus.Relationship <= (int)Value;
			case EventConditionName.FlagOn:
				return SaveLoadManager.UnsavedData.GlobalFlags.IsOn((FlagEnum)Value);
			case EventConditionName.FlagOff:
				return !SaveLoadManager.UnsavedData.GlobalFlags.IsOn((FlagEnum)Value);
			case EventConditionName.ScenarioRead:
				return SaveLoadManager.UnsavedData.GlobalFlags.IsRead((ScenarioLabel)Value);
			case EventConditionName.ScenarioNotRead:
				return !SaveLoadManager.UnsavedData.GlobalFlags.IsRead((ScenarioLabel)Value);
			case EventConditionName.IsDay:
				return (float)SaveLoadManager.UnsavedData.Days == Value;
			case EventConditionName.IsDayBefore:
				return (float)SaveLoadManager.UnsavedData.Days < Value;
			case EventConditionName.IsDayAfter:
				return (float)SaveLoadManager.UnsavedData.Days > Value;
			case EventConditionName.LeastFavorability:
				return (float)persistantStatus.GetFavorabilityLevel() >= Value;
			case EventConditionName.LeastSensitivity:
				return (float)persistantStatus.GetSensitivityLevel() >= Value;
			case EventConditionName.IsCloth:
				try
				{
					if (UnityEngine.Object.FindObjectOfType<HScene>().Name == SceneName.HScene3)
					{
						return UnityEngine.Object.FindObjectOfType<UtageManager>().GetInt("study_cloth") == (int)Value;
					}
				}
				catch
				{
				}
				return status.Cloth == (ClothName)(int)Value;
			case EventConditionName.LastSelected:
				return (float)SaveLoadManager.UnsavedData.LastSelectedIndex == Value;
			case EventConditionName.ParamXDiffMoreThan:
				return conditions.Move.x > Value;
			case EventConditionName.ParamXDiffLessThan:
				return conditions.Move.x < Value;
			case EventConditionName.ParamYDiffMoreThan:
				return conditions.Move.y > Value;
			case EventConditionName.ParamYDiffLessThan:
				return conditions.Move.y < Value;
			case EventConditionName.NoCondom:
				if (null == _manager)
				{
					_manager = UnityEngine.Object.FindObjectOfType<OsawariManager>();
				}
				if (UnityEngine.Object.FindObjectOfType<BaseScene>().GetActiveScene().Name == SceneName.HScene1)
				{
					return _manager.Model.Parameters[(int)Value].Value < 2f;
				}
				return _manager.Model.Parameters[(int)Value].Value < _manager.Model.Parameters[(int)Value].MaximumValue;
			case EventConditionName.MostDistance:
				return conditions.MovedDistance < Value;
			case EventConditionName.TouchingTimeLessThan:
				return conditions.TimeCount <= Value;
			case EventConditionName.TouchingTimeMoreThan:
				return conditions.TimeCount >= Value;
			case EventConditionName.IsWomanMoving:
			{
				if (Value > 2f || Value < 0f)
				{
					throw new Exception("Invalid value for IsWomanMoving. 0 is not moving, 1 is moving slow, 2 is moving fast");
				}
				if (null == _cache || null == _cache.GetComponent<Hscene2OsawariPiston>())
				{
					_cache = UnityEngine.Object.FindObjectOfType<Hscene2OsawariPiston>().gameObject;
				}
				Hscene2OsawariPiston component = _cache.GetComponent<Hscene2OsawariPiston>();
				return (int)Value switch
				{
					0 => !component.IsWomanMoving, 
					1 => component.IsWomanMoving && !component.WomanMoveFast, 
					2 => component.IsWomanMoving && component.WomanMoveFast, 
					_ => throw new InvalidOperationException(), 
				};
			}
			case EventConditionName.IsInFellatio:
				if (null == _cache || _cache.GetComponent<OsawariFellatio>() == null)
				{
					OsawariFellatio osawariFellatio = UnityEngine.Object.FindObjectOfType<OsawariFellatio>();
					if (osawariFellatio == _cache)
					{
						return false;
					}
					_cache = osawariFellatio.gameObject;
				}
				return _cache.GetComponent<OsawariFellatio>().IsInFellatio == (Value == 1f);
			case EventConditionName.Context:
				if (null == _manager)
				{
					_manager = UnityEngine.Object.FindObjectOfType<OsawariManager>();
				}
				return _manager.ContextManager.Context == (OsawariContext)(int)Value;
			case EventConditionName.IsKissing:
				return conditions.IsKissing == (Value == 1f);
			default:
				return true;
			}
		}
	}
}
