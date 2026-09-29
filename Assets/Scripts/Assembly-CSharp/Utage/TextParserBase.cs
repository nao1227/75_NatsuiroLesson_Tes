using System;
using System.Collections.Generic;
using System.Text;

namespace Utage
{
	public abstract class TextParserBase
	{
		protected List<CharData> charList = new List<CharData>();

		protected List<IParsedTextData> parsedDataList = new List<IParsedTextData>();

		private List<IParsedTextData> poolList = new List<IParsedTextData>();

		protected string errorMsg;

		protected string originalText;

		protected string noneMetaString;

		protected CharData.CustomCharaInfo parsingInfo = new CharData.CustomCharaInfo();

		internal virtual List<CharData> CharList => charList;

		public virtual List<IParsedTextData> ParsedDataList => parsedDataList;

		private List<IParsedTextData> PoolList => poolList;

		public virtual string ErrorMsg => errorMsg;

		public virtual int Length => CharList.Count;

		public virtual string OriginalText => originalText;

		public virtual string NoneMetaString
		{
			get
			{
				InitNoneMetaText();
				return noneMetaString;
			}
		}

		public static string AddTag(string text, string tag, string arg)
		{
			return string.Format("<{1}={2}>{0}</{1}>", text, tag, arg);
		}

		protected virtual void AddErrorMsg(string msg)
		{
			if (string.IsNullOrEmpty(errorMsg))
			{
				errorMsg = "";
			}
			else
			{
				errorMsg += "\n";
			}
			errorMsg += msg;
		}

		protected virtual void InitNoneMetaText()
		{
			if (string.IsNullOrEmpty(noneMetaString))
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < CharList.Count; i++)
				{
					stringBuilder.Append(CharList[i].Char);
				}
				noneMetaString = stringBuilder.ToString();
			}
		}

		public TextParserBase(string text)
		{
			originalText = text;
		}

		protected virtual void Parse()
		{
			try
			{
				int length = OriginalText.Length;
				int num = 0;
				while (num < length)
				{
					if (ParseEscapeSequence(num))
					{
						num += 2;
					}
					else
					{
						string tagName = "";
						string tagArg = "";
						int num2 = ParserUtil.ParseTag(OriginalText, num, delegate(string name, string arg)
						{
							bool num3 = ParseTag(name, arg);
							if (num3)
							{
								tagName = name;
								tagArg = arg;
							}
							return num3;
						});
						if (num == num2)
						{
							AddChar(OriginalText[num]);
							num++;
						}
						else
						{
							string fullString = OriginalText.Substring(num, num2 - num + 1);
							PoolList.Insert(0, MakeTag(fullString, tagName, tagArg));
							num = num2 + 1;
						}
					}
					ParsedDataList.AddRange(PoolList);
					PoolList.Clear();
				}
				PoolList.Clear();
			}
			catch (Exception ex)
			{
				AddErrorMsg(ex.Message + ex.StackTrace);
			}
		}

		protected virtual TagData MakeTag(string fullString, string name, string arg)
		{
			return new TagData(fullString, name, arg);
		}

		protected virtual void AddCharData(CharData data)
		{
			CharList.Add(data);
			PoolList.Add(data);
			parsingInfo.ClearOnNextChar();
		}

		protected virtual void AddChar(char c)
		{
			CharData data = new CharData(c, parsingInfo);
			AddCharData(data);
		}

		protected virtual void AddStrng(string text)
		{
			foreach (char c in text)
			{
				AddChar(c);
			}
		}

		protected virtual bool ParseEscapeSequence(int index)
		{
			if (index + 1 >= OriginalText.Length)
			{
				return false;
			}
			char c = OriginalText[index];
			char c2 = OriginalText[index + 1];
			if (c == '\\' && c2 == 'n')
			{
				AddDoubleLineBreak();
				return true;
			}
			if (c == '\r' && c2 == '\n')
			{
				AddDoubleLineBreak();
				return true;
			}
			return false;
		}

		protected virtual void AddDoubleLineBreak()
		{
			CharData charData = new CharData('\n', parsingInfo);
			charData.CustomInfo.IsDoubleWord = true;
			AddCharData(charData);
		}

		protected abstract bool ParseTag(string name, string arg);
	}
}
