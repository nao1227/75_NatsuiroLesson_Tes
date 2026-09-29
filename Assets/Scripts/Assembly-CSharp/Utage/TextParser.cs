using System;
using System.Collections.Generic;

namespace Utage
{
	public class TextParser : TextParserBase
	{
		public const string TagSound = "sound";

		public const string TagSpeed = "speed";

		public const string TagUnderLine = "u";

		public static Func<string, object> CallbackCalcExpression;

		protected bool isParseParamOnly;

		[Obsolete("Use TextData.MakeLogText")]
		public static string MakeLogText(string text)
		{
			return new TextParser(text, isParseParamOnly: true).NoneMetaString;
		}

		public TextParser(string text, bool isParseParamOnly = false)
			: base(text)
		{
			this.isParseParamOnly = isParseParamOnly;
			Parse();
		}

		protected override bool ParseTag(string name, string arg)
		{
			if (isParseParamOnly)
			{
				return ParseTagParamOnly(name, arg);
			}
			return ParseNovelTag(name, arg);
		}

		protected virtual bool ParseNovelTag(string name, string arg)
		{
			switch (name)
			{
			case "b":
				return parsingInfo.TryParseBold(arg);
			case "/b":
				parsingInfo.ResetBold();
				return true;
			case "i":
				return parsingInfo.TryParseItalic(arg);
			case "/i":
				parsingInfo.ResetItalic();
				return true;
			case "color":
				return parsingInfo.TryParseColor(arg);
			case "/color":
				parsingInfo.ResetColor();
				return true;
			case "size":
				return parsingInfo.TryParseSize(arg);
			case "/size":
				parsingInfo.ResetSize();
				return true;
			case "ruby":
				return parsingInfo.TryParseRuby(arg);
			case "/ruby":
				parsingInfo.ResetRuby();
				return true;
			case "em":
				return parsingInfo.TryParseEmphasisMark(arg);
			case "/em":
				parsingInfo.ResetEmphasisMark();
				return true;
			case "sup":
				return parsingInfo.TryParseSuperScript(arg);
			case "/sup":
				parsingInfo.ResetSuperScript();
				return true;
			case "sub":
				return parsingInfo.TryParseSubScript(arg);
			case "/sub":
				parsingInfo.ResetSubScript();
				return true;
			case "u":
				return parsingInfo.TryParseUnderLine(arg);
			case "/u":
				parsingInfo.ResetUnderLine();
				return true;
			case "strike":
				return parsingInfo.TryParseStrike(arg);
			case "/strike":
				parsingInfo.ResetStrike();
				return true;
			case "group":
				return parsingInfo.TryParseGroup(arg);
			case "/group":
				parsingInfo.ResetGroup();
				return true;
			case "emoji":
				return TryAddEmoji(arg);
			case "dash":
				AddDash(arg);
				return true;
			case "space":
				return TryAddSpace(arg);
			case "link":
				return parsingInfo.TryParseLink(arg);
			case "/link":
				parsingInfo.ResetLink();
				return true;
			case "tips":
				return parsingInfo.TryParseTips(arg);
			case "/tips":
				parsingInfo.ResetTips();
				return true;
			case "sound":
				return parsingInfo.TryParseSound(arg);
			case "/sound":
				parsingInfo.ResetSound();
				return true;
			case "speed":
				return parsingInfo.TryParseSpeed(arg);
			case "/speed":
				parsingInfo.ResetSpeed();
				return true;
			case "interval":
				return TryAddInterval(arg);
			case "param":
				return ParseParam(arg);
			case "format":
				return ParseParamFormat(arg);
			default:
				return false;
			}
		}

		protected virtual bool ParseTagParamOnly(string name, string arg)
		{
			return name switch
			{
				"param" => ParseParam(arg), 
				"format" => ParseParamFormat(arg), 
				_ => false, 
			};
		}

		protected virtual bool ParseParam(string arg)
		{
			string text = ExpressionToString(arg);
			AddStrng(text);
			return true;
		}

		protected virtual bool ParseParamFormat(string arg)
		{
			char[] separator = new char[1] { ':' };
			string[] array = arg.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			int num = array.Length - 1;
			string[] array2 = new string[num];
			Array.Copy(array, 1, array2, 0, num);
			string text = FormatExpressionToString(array[0], array2);
			AddStrng(text);
			return true;
		}

		protected virtual void AddDash(string arg)
		{
			if (!int.TryParse(arg, out var result))
			{
				result = 1;
			}
			CharData charData = new CharData('—', parsingInfo);
			charData.CustomInfo.IsDash = true;
			charData.CustomInfo.DashSize = result;
			AddCharData(charData);
		}

		protected virtual bool TryAddEmoji(string arg)
		{
			if (string.IsNullOrEmpty(arg))
			{
				return false;
			}
			CharData charData = new CharData('□', parsingInfo);
			charData.CustomInfo.IsEmoji = true;
			charData.CustomInfo.EmojiKey = arg;
			AddCharData(charData);
			return true;
		}

		protected virtual bool TryAddSpace(string arg)
		{
			CharData charData = new CharData(' ', parsingInfo);
			charData.CustomInfo.IsSpace = true;
			AddCharData(charData);
			if (int.TryParse(arg, out var result))
			{
				charData.CustomInfo.SpaceSize = result;
				return true;
			}
			return false;
		}

		protected virtual bool TryAddInterval(string arg)
		{
			if (CharList.Count <= 0)
			{
				return false;
			}
			return CharList[charList.Count - 1].TryParseInterval(arg);
		}

		protected virtual string ExpressionToString(string exp)
		{
			if (CallbackCalcExpression == null)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextCallbackCalcExpression));
				return "";
			}
			object obj = CallbackCalcExpression(exp);
			if (obj == null)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextFailedCalcExpression));
				return "";
			}
			return obj.ToString();
		}

		protected virtual string FormatExpressionToString(string format, string[] exps)
		{
			if (CallbackCalcExpression == null)
			{
				AddErrorMsg(LanguageErrorMsg.LocalizeTextFormat(Utage.ErrorMsg.TextCallbackCalcExpression));
				return "";
			}
			List<object> list = new List<object>();
			foreach (string arg in exps)
			{
				list.Add(CallbackCalcExpression(arg));
			}
			return string.Format(format, list.ToArray());
		}
	}
}
