using UnityEngine;

namespace Utage
{
	public static class AlignmentUtil
	{
		public static Vector2 GetAlignmentValue(Alignment alignment)
		{
			return alignment switch
			{
				Alignment.TopLeft => new Vector2(0f, 1f), 
				Alignment.LeftCenter => new Vector2(0f, 0.5f), 
				Alignment.BottomLeft => new Vector2(0f, 0f), 
				Alignment.TopCenter => new Vector2(0.5f, 1f), 
				Alignment.Center => new Vector2(0.5f, 0.5f), 
				Alignment.BottomCenter => new Vector2(0.5f, 0f), 
				Alignment.TopRight => new Vector2(1f, 1f), 
				Alignment.RightCenter => new Vector2(1f, 0.5f), 
				Alignment.BottomRight => new Vector2(1f, 0f), 
				_ => new Vector2(0.5f, 0.5f), 
			};
		}
	}
}
