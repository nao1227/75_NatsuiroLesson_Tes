using System;
using System.Collections.Generic;
using System.Text;

namespace Live2D.Cubism.Framework.Json
{
	public class CubismJsonParser
	{
		private char[] buffer;

		private int length;

		private int line_count;

		private Value root;

		public CubismJsonParser(char[] jsonBytes)
		{
			buffer = jsonBytes;
			length = jsonBytes.Length;
		}

		public Value Parse()
		{
			try
			{
				int[] endPos = new int[1];
				root = ParseValue(buffer, length, 0, endPos);
				return root;
			}
			catch (Exception ex)
			{
				throw new Exception("json error @line:" + line_count + " / " + ex.Message, ex);
			}
		}

		public static Value ParseFromBytes(char[] jsonBytes)
		{
			return new CubismJsonParser(jsonBytes).Parse();
		}

		public static Value ParseFromString(string jsonString)
		{
			return new CubismJsonParser(jsonString.ToCharArray()).Parse();
		}

		private static string ParseString(char[] str, int length, int pos, int[] endPos)
		{
			StringBuilder stringBuilder = null;
			int num = pos;
			for (int i = pos; i < length; i++)
			{
				switch ((char)(ushort)(str[i] & 0xFF))
				{
				case '"':
					endPos[0] = i + 1;
					if (stringBuilder != null)
					{
						if (i - 1 > num)
						{
							stringBuilder.Append(new string(str, num, i - 1 - num));
						}
						return stringBuilder.ToString();
					}
					return new string(str, pos, i - pos);
				case '\\':
					if (stringBuilder == null)
					{
						stringBuilder = new StringBuilder();
					}
					if (i > num)
					{
						stringBuilder.Append(new string(str, num, i - num));
					}
					i++;
					if (i < length)
					{
						switch ((char)(ushort)(str[i] & 0xFF))
						{
						case '\\':
							stringBuilder.Append('\\');
							break;
						case '"':
							stringBuilder.Append('"');
							break;
						case '/':
							stringBuilder.Append('/');
							break;
						case 'b':
							stringBuilder.Append('\b');
							break;
						case 'f':
							stringBuilder.Append('\f');
							break;
						case 'n':
							stringBuilder.Append('\n');
							break;
						case 'r':
							stringBuilder.Append('\r');
							break;
						case 't':
							stringBuilder.Append('\t');
							break;
						case 'u':
							throw new Exception("parse string/unicode escape not supported");
						}
						num = i + 1;
						break;
					}
					throw new Exception("parse string/escape error");
				}
			}
			throw new Exception("parse string/illegal end");
		}

		private Value ParseObject(char[] buffer, int length, int pos, int[] endPos)
		{
			Dictionary<string, Value> dictionary = new Dictionary<string, Value>();
			string key = null;
			int i = pos;
			int[] array = new int[1];
			bool flag = false;
			while (i < length)
			{
				for (; i < length; i++)
				{
					switch ((char)(ushort)(buffer[i] & 0xFF))
					{
					case '"':
						break;
					case '}':
						endPos[0] = i + 1;
						return new Value(dictionary);
					case ':':
						throw new Exception("illegal ':' position");
					default:
						continue;
					}
					key = ParseString(buffer, length, i + 1, array);
					i = array[0];
					flag = true;
					break;
				}
				if (!flag)
				{
					throw new Exception("key not found");
				}
				flag = false;
				for (; i < length; i++)
				{
					switch ((char)(ushort)(buffer[i] & 0xFF))
					{
					case ':':
						break;
					case '}':
						throw new Exception("illegal '}' position");
					case '\n':
						line_count++;
						continue;
					default:
						continue;
					}
					flag = true;
					i++;
					break;
				}
				if (!flag)
				{
					throw new Exception("':' not found");
				}
				Value value = ParseValue(buffer, length, i, array);
				i = array[0];
				dictionary.Add(key, value);
				for (; i < length; i++)
				{
					switch ((char)(ushort)(buffer[i] & 0xFF))
					{
					case '}':
						endPos[0] = i + 1;
						return new Value(dictionary);
					case '\n':
						line_count++;
						continue;
					default:
						continue;
					case ',':
						break;
					}
					break;
				}
				i++;
			}
			throw new Exception("illegal end of ParseObject");
		}

		private Value ParseArray(char[] buffer, int length, int pos, int[] endPos)
		{
			List<Value> list = new List<Value>();
			int num = pos;
			int[] array = new int[1];
			while (num < length)
			{
				Value value = ParseValue(buffer, length, num, array);
				num = array[0];
				if (value != null)
				{
					list.Add(value);
				}
				for (; num < length; num++)
				{
					switch ((char)(ushort)(buffer[num] & 0xFF))
					{
					case ']':
						endPos[0] = num + 1;
						return new Value(list);
					case '\n':
						line_count++;
						continue;
					default:
						continue;
					case ',':
						break;
					}
					break;
				}
				num++;
			}
			throw new Exception("illegal end of ParseObject");
		}

		public static double strToDouble(char[] str, int length, int pos, int[] endPos)
		{
			int i = pos;
			bool flag = false;
			bool flag2 = false;
			double num = 0.0;
			char c = (char)(str[i] & 0xFF);
			if (c == '-')
			{
				flag = true;
				i++;
			}
			for (; i < length; i++)
			{
				switch ((char)(ushort)(str[i] & 0xFF))
				{
				case '0':
					num *= 10.0;
					continue;
				case '1':
					num = num * 10.0 + 1.0;
					continue;
				case '2':
					num = num * 10.0 + 2.0;
					continue;
				case '3':
					num = num * 10.0 + 3.0;
					continue;
				case '4':
					num = num * 10.0 + 4.0;
					continue;
				case '5':
					num = num * 10.0 + 5.0;
					continue;
				case '6':
					num = num * 10.0 + 6.0;
					continue;
				case '7':
					num = num * 10.0 + 7.0;
					continue;
				case '8':
					num = num * 10.0 + 8.0;
					continue;
				case '9':
					num = num * 10.0 + 9.0;
					continue;
				case '.':
					flag2 = true;
					i++;
					break;
				}
				break;
			}
			if (flag2)
			{
				for (double num2 = 0.1; i < length; num2 *= 0.1, i++)
				{
					switch ((char)(ushort)(str[i] & 0xFF))
					{
					case '1':
						num += num2 * 1.0;
						continue;
					case '2':
						num += num2 * 2.0;
						continue;
					case '3':
						num += num2 * 3.0;
						continue;
					case '4':
						num += num2 * 4.0;
						continue;
					case '5':
						num += num2 * 5.0;
						continue;
					case '6':
						num += num2 * 6.0;
						continue;
					case '7':
						num += num2 * 7.0;
						continue;
					case '8':
						num += num2 * 8.0;
						continue;
					case '9':
						num += num2 * 9.0;
						continue;
					case '0':
						continue;
					}
					break;
				}
			}
			if (flag)
			{
				num = 0.0 - num;
			}
			endPos[0] = i;
			return num;
		}

		private Value ParseValue(char[] buffer, int length, int pos, int[] endPos)
		{
			for (int i = pos; i < length; i++)
			{
				switch ((char)(ushort)(buffer[i] & 0xFF))
				{
				case '-':
				case '.':
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					return new Value(strToDouble(buffer, length, i, endPos));
				case '"':
					return new Value(ParseString(buffer, length, i + 1, endPos));
				case '[':
					return ParseArray(buffer, length, i + 1, endPos);
				case ']':
					endPos[0] = i;
					return null;
				case '{':
					return ParseObject(buffer, length, i + 1, endPos);
				case 'n':
					if (i + 3 < length)
					{
						return null;
					}
					throw new Exception("parse null");
				case 't':
					if (i + 3 < length)
					{
						return new Value(true);
					}
					throw new Exception("parse true");
				case 'f':
					if (i + 4 < length)
					{
						return new Value(false);
					}
					throw new Exception("parse false");
				case ',':
					throw new Exception("illegal ',' position");
				case '\n':
					line_count++;
					break;
				}
			}
			return null;
		}
	}
}
