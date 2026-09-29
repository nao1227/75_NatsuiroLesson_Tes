using System;
using System.Collections.Generic;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariResult
	{
		private ActionManager _actionManager;

		private bool _calculated;

		public int FavPoint { get; protected set; }

		public int SensitivityPoint { get; protected set; }

		public int ExciteCount { get; protected set; }

		public int EjaculateCount { get; protected set; }

		public string AtomosphereText { get; protected set; }

		public OsawariResult(ActionManager actionManager)
		{
			FavPoint = 0;
			SensitivityPoint = 0;
			_actionManager = actionManager;
		}

		public void CalcResult(TemporaryStatus status)
		{
			if (!_calculated)
			{
				FavPoint = Mathf.Min(Mathf.Min(status.Feelings.AtomosphereAdded / 200, 100) + _actionManager.AchievedActions * 5, 100);
				SensitivityPoint = Mathf.Min(status.ExciteCount * 10 + status.EjaculateCount * 10 + _actionManager.AchievedActions * 5, 100);
				ExciteCount = status.ExciteCount;
				EjaculateCount = status.EjaculateCount;
				AtomosphereText = status.GetCurrentAtomosphere() switch
				{
					AtomosphereName.Nervous => "緊張", 
					AtomosphereName.Relief => "安心", 
					AtomosphereName.Excited => "興奮", 
					AtomosphereName.Rut => "発情", 
					_ => throw new Exception("Invalid Atomosphere"), 
				};
				SaveLoadManager.UnsavedData.PersistantStatus.AddFavourability(FavPoint);
				SaveLoadManager.UnsavedData.PersistantStatus.AddSensitivity(SensitivityPoint);
				_calculated = true;
			}
		}

		public List<OsawariAction> GetAchievedActions()
		{
			return _actionManager.GetAchievedActions();
		}
	}
}
