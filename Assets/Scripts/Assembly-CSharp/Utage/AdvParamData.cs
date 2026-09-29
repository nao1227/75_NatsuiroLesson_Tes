using UnityEngine;

namespace Utage
{
	public class AdvParamData
	{
		public enum ParamType
		{
			Bool = 0,
			Float = 1,
			Int = 2,
			String = 3
		}

		public enum FileType
		{
			Default = 0,
			System = 1,
			Const = 2
		}

		private string key;

		private ParamType type;

		private bool boolValue;

		private float floatValue;

		private int intValue;

		private string stringValue;

		private FileType fileType;

		public string Key => key;

		public ParamType Type => type;

		public object Parameter
		{
			get
			{
				return GetValueWithBoxing();
			}
			set
			{
				SetValueWithBoxing(value);
			}
		}

		public bool BoolValue
		{
			get
			{
				if (Type != ParamType.Bool)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not Bool type. This type is {1} ", Key, Type);
					return false;
				}
				return boolValue;
			}
			set
			{
				if (Type != ParamType.Bool)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not Bool type. This type is {1} ", Key, Type);
				}
				else
				{
					boolValue = value;
				}
			}
		}

		public float FloatValue
		{
			get
			{
				if (Type != ParamType.Float)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not Float type. This type is {1} ", Key, Type);
					return 0f;
				}
				return floatValue;
			}
			set
			{
				if (Type != ParamType.Float)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not Float type. This type is {1} ", Key, Type);
				}
				else
				{
					floatValue = value;
				}
			}
		}

		public int IntValue
		{
			get
			{
				if (Type != ParamType.Int)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not Int type. This type is {1} ", Key, Type);
					return 0;
				}
				return intValue;
			}
			set
			{
				if (Type != ParamType.Int)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not Int type. This type is {1} ", Key, Type);
				}
				else
				{
					intValue = value;
				}
			}
		}

		public string StringValue
		{
			get
			{
				if (Type != ParamType.String)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not String type. This type is {1} ", Key, Type);
					return "";
				}
				return stringValue;
			}
			set
			{
				if (Type != ParamType.String)
				{
					Debug.LogErrorFormat("Parameter [{0}] is not String type. This type is {1} ", Key, Type);
				}
				else
				{
					stringValue = value;
				}
			}
		}

		public FileType SaveFileType => fileType;

		public string ParameterString
		{
			get
			{
				switch (Type)
				{
				case ParamType.Bool:
					return boolValue.ToString();
				case ParamType.Float:
					return WrapperUnityVersion.ToStringFloatGlobal(floatValue);
				case ParamType.Int:
					return intValue.ToString();
				case ParamType.String:
					return stringValue.ToString();
				default:
					Debug.LogErrorFormat("Unknown Type {0}", Type);
					return stringValue.ToString();
				}
			}
		}

		public bool TryParse(string name, string type, string fileType)
		{
			key = name;
			if (!ParserUtil.TryParaseEnum<ParamType>(type, out this.type))
			{
				Debug.LogError(type + " is not ParamType");
				return false;
			}
			if (string.IsNullOrEmpty(fileType))
			{
				this.fileType = FileType.Default;
			}
			else if (!ParserUtil.TryParaseEnum<FileType>(fileType, out this.fileType))
			{
				Debug.LogError(fileType + " is not FileType");
				return false;
			}
			return true;
		}

		public bool TryParse(AdvParamData src, string value)
		{
			key = src.Key;
			type = src.Type;
			fileType = src.SaveFileType;
			try
			{
				ParseParameterString(value);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TryParse(StringGridRow row)
		{
			string value = AdvParser.ParseCell<string>(row, AdvColumnName.Label);
			if (string.IsNullOrEmpty(value))
			{
				return false;
			}
			key = value;
			type = AdvParser.ParseCell<ParamType>(row, AdvColumnName.Type);
			fileType = AdvParser.ParseCellOptional(row, AdvColumnName.FileType, FileType.Default);
			try
			{
				string parameterString = AdvParser.ParseCellOptional(row, AdvColumnName.Value, "");
				ParseParameterString(parameterString);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Copy(AdvParamData src)
		{
			key = src.Key;
			type = src.type;
			fileType = src.fileType;
			switch (Type)
			{
			case ParamType.Bool:
				boolValue = src.boolValue;
				break;
			case ParamType.Float:
				floatValue = src.floatValue;
				break;
			case ParamType.Int:
				intValue = src.intValue;
				break;
			case ParamType.String:
				stringValue = src.stringValue;
				break;
			}
		}

		private object GetValueWithBoxing()
		{
			switch (Type)
			{
			case ParamType.Bool:
				return boolValue;
			case ParamType.Float:
				return floatValue;
			case ParamType.Int:
				return intValue;
			case ParamType.String:
				return stringValue;
			default:
				Debug.LogErrorFormat("Unknown Type {0}", Type);
				return stringValue;
			}
		}

		private void SetValueWithBoxing(object value)
		{
			switch (Type)
			{
			case ParamType.Bool:
				boolValue = (bool)value;
				break;
			case ParamType.Float:
				floatValue = ExpressionCast.ToFloat(value);
				break;
			case ParamType.Int:
				intValue = ExpressionCast.ToInt(value);
				break;
			case ParamType.String:
				stringValue = (string)value;
				break;
			default:
				Debug.LogErrorFormat("Unknown Type {0}", Type);
				break;
			}
		}

		private void ParseParameterString(string parameterString)
		{
			switch (Type)
			{
			case ParamType.Bool:
				boolValue = bool.Parse(parameterString);
				break;
			case ParamType.Float:
				floatValue = WrapperUnityVersion.ParseFloatGlobal(parameterString);
				break;
			case ParamType.Int:
				intValue = int.Parse(parameterString);
				break;
			case ParamType.String:
				stringValue = parameterString;
				break;
			default:
				Debug.LogErrorFormat("Unknown Type {0}", Type);
				break;
			}
		}

		public void CopySaveData(AdvParamData src)
		{
			if (key != src.Key)
			{
				Debug.LogError(src.key + "is different name of Saved param");
			}
			if (type != src.type)
			{
				Debug.LogError(src.type.ToString() + "is different type of Saved param");
			}
			if (fileType != src.fileType)
			{
				Debug.LogError(src.fileType.ToString() + "is different fileType of Saved param");
			}
			Copy(src);
		}

		public void Read(string paramString)
		{
			ParseParameterString(paramString);
		}
	}
}
