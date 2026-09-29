using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.satsuki1
{
	public class AverageCalculator
	{
		private int _max;

		private List<float> _list;

		public float Average => _list.Sum() / (float)_list.Count;

		public float AbsAverage => _list.Sum((float x) => Math.Abs(x)) / (float)_list.Count;

		public float GetAverageOf(int frame, bool isAbs = false)
		{
			if (_list.Count < frame)
			{
				if (!isAbs)
				{
					return Average;
				}
				return AbsAverage;
			}
			float num = 0f;
			for (int num2 = _list.Count - 1; num2 > _list.Count - 1 - frame; num2--)
			{
				num = ((!isAbs) ? (num + _list[num2]) : (num + Math.Abs(_list[num2])));
			}
			return num / (float)frame;
		}

		public AverageCalculator(int max = 30)
		{
			_list = new List<float> { 0f };
			_max = max;
		}

		public void Add(float val)
		{
			if (_list.Count < _max)
			{
				_list.Add(val);
				return;
			}
			_list.RemoveAt(0);
			Add(val);
		}

		public void Clear()
		{
			_list.Clear();
			_list.Add(0f);
		}
	}
}
