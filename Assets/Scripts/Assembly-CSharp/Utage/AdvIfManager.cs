using System.IO;
using UnityEngine;

namespace Utage
{
	internal class AdvIfManager
	{
		private const int Version = 0;

		private AdvIfData Current { get; set; }

		private bool SaveDataStart { get; set; }

		public bool OldSaveDataStart { get; set; }

		public void ResetOnJump()
		{
			if (!SaveDataStart)
			{
				Current = null;
				OldSaveDataStart = false;
			}
			SaveDataStart = false;
		}

		public void BeginIf(AdvParamManager param, ExpressionParser exp)
		{
			OldSaveDataStart = false;
			Current = new AdvIfData(Current);
			if (Current.IsParantSkipping)
			{
				Current.IsSkpping = true;
			}
			else
			{
				Current.BeginIf(param, exp);
			}
		}

		public void ElseIf(AdvParamManager param, ExpressionParser exp)
		{
			if (Current == null)
			{
				if (!OldSaveDataStart)
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.ElseIf, exp));
				}
				Current = new AdvIfData(Current);
			}
			if (!Current.IsParantSkipping)
			{
				Current.ElseIf(param, exp);
			}
		}

		public void Else()
		{
			if (Current == null)
			{
				if (!OldSaveDataStart)
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.Else));
				}
				Current = new AdvIfData(Current);
			}
			if (!Current.IsParantSkipping)
			{
				Current.Else();
			}
		}

		public void EndIf()
		{
			if (Current == null)
			{
				if (!OldSaveDataStart)
				{
					Debug.LogError(LanguageAdvErrorMsg.LocalizeTextFormat(AdvErrorMsg.EndIf));
				}
				Current = new AdvIfData(Current);
			}
			if (!Current.IsParantSkipping)
			{
				Current.EndIf();
			}
			Current = Current.Parent;
		}

		public bool CheckSkip(AdvCommand command)
		{
			if (command == null)
			{
				return false;
			}
			if (Current == null)
			{
				return false;
			}
			if (Current.IsSkpping && !command.IsIfCommand)
			{
				return true;
			}
			return false;
		}

		public void Write(BinaryWriter writer)
		{
			writer.Write(0);
			int num = 0;
			for (AdvIfData advIfData = Current; advIfData != null; advIfData = advIfData.Parent)
			{
				num++;
			}
			writer.Write(num);
			for (AdvIfData advIfData = Current; advIfData != null; advIfData = advIfData.Parent)
			{
				writer.Write(advIfData.IsSkpping);
				writer.Write(advIfData.IsIf);
			}
		}

		public void Read(BinaryReader reader)
		{
			SaveDataStart = true;
			OldSaveDataStart = false;
			int num = reader.ReadInt32();
			if (0 <= num && num <= 0)
			{
				Current = null;
				int num2 = reader.ReadInt32();
				for (int i = 0; i < num2; i++)
				{
					Current = new AdvIfData(Current);
				}
				for (AdvIfData advIfData = Current; advIfData != null; advIfData = advIfData.Parent)
				{
					advIfData.IsSkpping = reader.ReadBoolean();
					advIfData.IsIf = reader.ReadBoolean();
				}
			}
			else
			{
				Debug.LogError(LanguageErrorMsg.LocalizeTextFormat(ErrorMsg.UnknownVersion, num));
			}
		}

		public void ReadOld()
		{
			SaveDataStart = true;
			OldSaveDataStart = true;
		}
	}
}
