using System;

namespace Utage
{
	public class ExpressionCast
	{
		public static int ToInt(object value)
		{
			if (value.GetType() == typeof(int))
			{
				return (int)value;
			}
			if (value.GetType() == typeof(float))
			{
				return (int)(float)value;
			}
			throw new Exception("Cant cast :" + value.GetType()?.ToString() + " ToInt");
		}

		public static float ToFloat(object value)
		{
			if (value.GetType() == typeof(float))
			{
				return (float)value;
			}
			if (value.GetType() == typeof(int))
			{
				return (int)value;
			}
			throw new Exception("Cant cast :" + value.GetType()?.ToString() + " ToFloat");
		}
	}
}
