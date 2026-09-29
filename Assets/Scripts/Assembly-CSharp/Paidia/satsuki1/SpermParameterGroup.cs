using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class SpermParameterGroup
	{
		public List<int> SpermParameterID;

		private List<bool> _ejaculated;

		private int _targetIndex;

		private bool _locked;

		public void Initialize()
		{
			_ejaculated = new List<bool>();
			for (int i = 0; i < SpermParameterID.Count; i++)
			{
				_ejaculated.Add(item: false);
			}
		}

		public bool CanEjaculate()
		{
			if (_ejaculated.Count((bool x) => !x) > 0)
			{
				return !_locked;
			}
			return false;
		}

		public int GetRandomSpermparameter()
		{
			if (!CanEjaculate())
			{
				return SpermParameterID.Count();
			}
			int num = UnityEngine.Random.Range(0, SpermParameterID.Count());
			while (_ejaculated[num])
			{
				num = UnityEngine.Random.Range(0, SpermParameterID.Count());
			}
			_locked = true;
			_targetIndex = num;
			return _targetIndex;
		}

		public void FinishEjaculate()
		{
			_ejaculated[_targetIndex] = true;
			_locked = false;
		}

		public IEnumerator<int> GetEnumerator()
		{
			for (int i = 0; i < _ejaculated.Count; i++)
			{
				if (_ejaculated[i])
				{
					yield return SpermParameterID[i];
				}
			}
		}
	}
}
