using UnityEngine;

namespace Utage
{
	public static class PivotUtil
	{
		public static Vector2 PivotEnumToVector2(Pivot pivot)
		{
			return pivot switch
			{
				Pivot.TopLeft => new Vector2(0f, 1f), 
				Pivot.Left => new Vector2(0f, 0.5f), 
				Pivot.BottomLeft => new Vector2(0f, 0f), 
				Pivot.Top => new Vector2(0.5f, 1f), 
				Pivot.Center => new Vector2(0.5f, 0.5f), 
				Pivot.Bottom => new Vector2(0.5f, 0f), 
				Pivot.TopRight => new Vector2(1f, 1f), 
				Pivot.Right => new Vector2(1f, 0.5f), 
				Pivot.BottomRight => new Vector2(1f, 0f), 
				_ => new Vector2(0.5f, 0.5f), 
			};
		}
	}
}
