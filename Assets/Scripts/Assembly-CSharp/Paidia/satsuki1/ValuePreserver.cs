using System;
using System.Collections.Generic;
using System.Linq;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class ValuePreserver : MonoBehaviour
	{
		private List<List<float>> _valueLists;

		private List<List<float>> _correctionValueLists;

		private List<CubismModel> _models;

		private List<Dictionary<int, ModeValuePair>> _valuePairList;

		private List<Dictionary<int, ModeValuePair>> _correctionValuePairList;

		private List<Dictionary<int, ModeValuePair>> _lastValuePairList;

		private List<Dictionary<int, ModeValuePair>> _lastCorrectionValuePairList;

		private List<Dictionary<int, float>> _lastActualValuePairList;

		public void ManagedStart()
		{
			_models = new List<CubismModel>();
			_valueLists = new List<List<float>>();
			_correctionValueLists = new List<List<float>>();
			_valuePairList = new List<Dictionary<int, ModeValuePair>>();
			_lastValuePairList = new List<Dictionary<int, ModeValuePair>>();
			_correctionValuePairList = new List<Dictionary<int, ModeValuePair>>();
			_lastCorrectionValuePairList = new List<Dictionary<int, ModeValuePair>>();
			_lastActualValuePairList = new List<Dictionary<int, float>>();
		}

		public void SetModel(CubismModel model)
		{
			_models.Add(model);
			_valueLists.Add(new List<float>());
			_correctionValueLists.Add(new List<float>());
			_valuePairList.Add(new Dictionary<int, ModeValuePair>());
			_lastValuePairList.Add(new Dictionary<int, ModeValuePair>());
			_correctionValuePairList.Add(new Dictionary<int, ModeValuePair>());
			_lastCorrectionValuePairList.Add(new Dictionary<int, ModeValuePair>());
			_lastActualValuePairList.Add(new Dictionary<int, float>());
		}

		public void SetValue(int idx, float val, CubismParameterBlendMode mode = CubismParameterBlendMode.Override, int modelIndex = 0)
		{
			if (idx >= 0)
			{
				if (_valuePairList[modelIndex].ContainsKey(idx))
				{
					_valuePairList[modelIndex][idx] = new ModeValuePair(val, mode);
				}
				else
				{
					_valuePairList[modelIndex].Add(idx, new ModeValuePair(val, mode));
				}
			}
		}

		public void SetValue(int idx, ParameterValue val, CubismParameterBlendMode mode = CubismParameterBlendMode.Override, int modelIndex = 0)
		{
			SetValue(idx, val.Value, mode, modelIndex);
		}

		public void SetCorrectionValue(int idx, float val, int modelIndex = 0)
		{
			if (!(Math.Abs(val) < 0.01f))
			{
				if (_correctionValuePairList[modelIndex].ContainsKey(idx))
				{
					_correctionValuePairList[modelIndex][idx] = new ModeValuePair(val, CubismParameterBlendMode.Additive);
				}
				else
				{
					_correctionValuePairList[modelIndex].Add(idx, new ModeValuePair(val, CubismParameterBlendMode.Additive));
				}
			}
		}

		public void InactivateValue(int idx, int modelIndex = 0)
		{
			if (_valuePairList[modelIndex].ContainsKey(idx))
			{
				_valuePairList[modelIndex].Remove(idx);
			}
			if (_lastActualValuePairList[modelIndex].ContainsKey(idx))
			{
				_lastActualValuePairList[modelIndex].Remove(idx);
			}
		}

		public float GetLastValue(int idx, int modelIndex = 0)
		{
			if (_lastValuePairList[modelIndex].ContainsKey(idx))
			{
				return _lastValuePairList[modelIndex][idx].Value;
			}
			return -99f;
		}

		private void LateUpdate()
		{
			if (_models == null)
			{
				return;
			}
			int num = 0;
			foreach (CubismModel model in _models)
			{
				foreach (KeyValuePair<int, ModeValuePair> item in _valuePairList[num])
				{
					model.Parameters[item.Key].BlendToValue(item.Value.Mode, item.Value.Value);
					if (_lastValuePairList[num].ContainsKey(item.Key))
					{
						_lastValuePairList[num][item.Key] = item.Value;
					}
					else
					{
						_lastValuePairList[num].Add(item.Key, item.Value);
					}
					if (_lastActualValuePairList[num].ContainsKey(item.Key))
					{
						_lastActualValuePairList[num][item.Key] = model.Parameters[item.Key].Value;
					}
					else
					{
						_lastActualValuePairList[num].Add(item.Key, model.Parameters[item.Key].Value);
					}
				}
				for (int i = 0; i < _correctionValuePairList[num].Count; i++)
				{
					KeyValuePair<int, ModeValuePair> keyValuePair = _correctionValuePairList[num].ElementAt(i);
					model.Parameters[keyValuePair.Key].BlendToValue(keyValuePair.Value.Mode, keyValuePair.Value.Value);
					if (_lastCorrectionValuePairList[num].ContainsKey(keyValuePair.Key))
					{
						_lastCorrectionValuePairList[num][keyValuePair.Key] = keyValuePair.Value;
					}
					else
					{
						_lastCorrectionValuePairList[num].Add(keyValuePair.Key, keyValuePair.Value);
					}
					_correctionValuePairList[num][keyValuePair.Key] = new ModeValuePair(0f, CubismParameterBlendMode.Additive);
				}
				foreach (int item2 in _correctionValuePairList[num].Keys.ToList())
				{
					if (_correctionValuePairList[num][item2].Value < 0.01f)
					{
						_correctionValuePairList[num].Remove(item2);
					}
				}
				num++;
			}
		}

		public float GetActualValue(ParameterName name, int modelIndex = 0)
		{
			if (_lastActualValuePairList[modelIndex].ContainsKey((int)name))
			{
				return _lastActualValuePairList[modelIndex][(int)name];
			}
			return -99f;
		}
	}
}
