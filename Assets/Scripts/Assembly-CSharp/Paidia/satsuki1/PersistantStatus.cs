using System;
using System.Text;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class PersistantStatus
	{
		[SerializeField]
		private int _favorability;

		[SerializeField]
		private int _sensitivity;

		[SerializeField]
		private Relationship _relationship;

		[SerializeField]
		private int _ejaculateCount;

		public int Favorability => _favorability;

		public int Sensitivity => _sensitivity;

		public int EjaculateCount
		{
			get
			{
				if (SingletonManager<SceneContextManager>.Instance.CurrentSceneContext != SceneContext.FreeH)
				{
					return (int)(Relationship + ((!SingletonManager<SceneContextManager>.Instance.EjaculationPlus) ? 1 : 2));
				}
				return 99;
			}
		}

		public Relationship Relationship => CalcRelationship();

		public void AddEjaculateCount(int val)
		{
			_ejaculateCount += val;
		}

		public Relationship CalcRelationship()
		{
			return _relationship;
		}

		public int GetAtomosphereBonusByLevel()
		{
			return GetFavorabilityLevel() * 20;
		}

		public int GetExciteBonusByLevel()
		{
			return GetSensitivityLevel() * 20;
		}

		public int GetFavorabilityLevel()
		{
			return Mathf.Min(_favorability / 100, 7);
		}

		public int GetSensitivityLevel()
		{
			return Mathf.Min(_sensitivity / 100, 6);
		}

		public void SetRelationship(Relationship relationship)
		{
			_relationship = relationship;
		}

		public bool AddFavourability(int val, bool debug = false)
		{
			int favorability = _favorability;
			_favorability += (debug ? val : Mathf.Clamp(val, 0, 100));
			return favorability / 100 != _favorability / 100;
		}

		public bool AddSensitivity(int val, bool debug = false)
		{
			int sensitivity = _sensitivity;
			_sensitivity += (debug ? val : Mathf.Clamp(val, 0, 100));
			return sensitivity / 100 != _sensitivity / 100;
		}

		public void SetMaxLevel()
		{
			_favorability += 700;
			_sensitivity += 600;
			_relationship = Relationship.LoveyDovey;
		}

		public PersistantStatus(int favourability = 0, int sensitivity = 0, int ejaculateCount = 1, Relationship relationship = Relationship.None)
		{
			_favorability = favourability;
			_sensitivity = sensitivity;
			_ejaculateCount = ejaculateCount;
			_relationship = relationship;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Favorability:").Append(_favorability).Append(" Sensitivity:")
				.Append(_sensitivity)
				.Append(" Relationship:")
				.Append(_relationship);
			return stringBuilder.ToString();
		}
	}
}
