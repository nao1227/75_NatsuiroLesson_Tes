using System.Collections.Generic;

namespace Live2D.Cubism.Framework.Json
{
	public class Value
	{
		private object _object;

		public Value(object obj)
		{
			_object = obj;
		}

		public string toString()
		{
			return toString("");
		}

		public string toString(string indent)
		{
			if (_object is string)
			{
				return (string)_object;
			}
			if (_object is List<Value>)
			{
				string text = indent + "[\n";
				foreach (Value item in (List<Value>)_object)
				{
					text = text + indent + "    " + item.toString(indent + "    ") + "\n";
				}
				return text + indent + "]\n";
			}
			if (_object is Dictionary<string, Value>)
			{
				string text2 = indent + "{\n";
				foreach (KeyValuePair<string, Value> item2 in (Dictionary<string, Value>)_object)
				{
					Value value = item2.Value;
					text2 = text2 + indent + "    " + item2.Key + " : " + value.toString(indent + "    ") + "\n";
				}
				return text2 + indent + "}\n";
			}
			return _object?.ToString() ?? "";
		}

		public int toInt()
		{
			return toInt(0);
		}

		public int toInt(int defaultValue)
		{
			if (!(_object is double))
			{
				return defaultValue;
			}
			return (int)(double)_object;
		}

		public float ToFloat()
		{
			return ToFloat(0f);
		}

		public float ToFloat(float defaultValue)
		{
			if (!(_object is double))
			{
				return defaultValue;
			}
			return (float)(double)_object;
		}

		public double ToDouble()
		{
			return ToDouble(0.0);
		}

		public double ToDouble(double defaultValue)
		{
			if (!(_object is double))
			{
				return defaultValue;
			}
			return (double)_object;
		}

		public List<Value> GetVector(List<Value> defalutV)
		{
			if (!(_object is List<Value>))
			{
				return defalutV;
			}
			return (List<Value>)_object;
		}

		public Value Get(int index)
		{
			if (!(_object is List<Value>))
			{
				return null;
			}
			return ((List<Value>)_object)[index];
		}

		public Dictionary<string, Value> GetMap(Dictionary<string, Value> defalutV)
		{
			if (!(_object is Dictionary<string, Value>))
			{
				return defalutV;
			}
			return (Dictionary<string, Value>)_object;
		}

		public Value Get(string key)
		{
			if (_object is Dictionary<string, Value> && ((Dictionary<string, Value>)_object).ContainsKey(key))
			{
				return ((Dictionary<string, Value>)_object)[key];
			}
			return null;
		}

		public List<string> KeySet()
		{
			if (!(_object is Dictionary<string, Value>))
			{
				return null;
			}
			return new List<string>(((Dictionary<string, Value>)_object).Keys);
		}

		public Dictionary<string, Value> ToMap()
		{
			if (!(_object is Dictionary<string, Value>))
			{
				return null;
			}
			return (Dictionary<string, Value>)_object;
		}

		public bool isNull()
		{
			return _object == null;
		}

		public bool isBoolean()
		{
			return _object is bool;
		}

		public bool isDouble()
		{
			return _object is double;
		}

		public bool isString()
		{
			return _object is string;
		}

		public bool isArray()
		{
			return _object is List<Value>;
		}

		public bool isMap()
		{
			return _object is Dictionary<string, Value>;
		}
	}
}
