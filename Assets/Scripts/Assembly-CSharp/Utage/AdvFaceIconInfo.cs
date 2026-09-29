using System;
using UnityEngine;

namespace Utage
{
	public class AdvFaceIconInfo
	{
		public enum Type
		{
			None = 0,
			IconImage = 1,
			DicingPattern = 2,
			RectImage = 3
		}

		public Type IconType { get; internal set; }

		public string FileName { get; internal set; }

		public AssetFile File { get; set; }

		public Rect IconRect { get; internal set; }

		public string IconSubFileName { get; internal set; }

		public AdvFaceIconInfo(StringGridRow row)
		{
			FileName = AdvParser.ParseCellOptional(row, AdvColumnName.Icon, "");
			if (!string.IsNullOrEmpty(FileName))
			{
				if (!AdvParser.IsEmptyCell(row, AdvColumnName.IconSubFileName))
				{
					IconType = Type.DicingPattern;
					IconSubFileName = AdvParser.ParseCell<string>(row, AdvColumnName.IconSubFileName);
				}
				else
				{
					IconType = Type.IconImage;
				}
			}
			else if (!AdvParser.IsEmptyCell(row, AdvColumnName.IconRect))
			{
				float[] array = row.ParseCellArray<float>(AdvColumnName.IconRect.QuickToString());
				if (array.Length == 4)
				{
					IconType = Type.RectImage;
					IconRect = new Rect(array[0], array[1], array[2], array[3]);
				}
				else
				{
					Debug.LogError(row.ToErrorString("IconRect. Array size is not 4"));
				}
			}
			else
			{
				IconType = Type.None;
			}
		}

		public void BootInit(Func<string, string> fileNameToPath)
		{
			if (!string.IsNullOrEmpty(FileName))
			{
				File = AssetFileManager.GetFileCreateIfMissing(fileNameToPath(FileName));
			}
		}
	}
}
