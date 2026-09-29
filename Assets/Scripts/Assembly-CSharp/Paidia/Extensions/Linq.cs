using System;
using System.Collections.Generic;
using System.Linq;

namespace Paidia.Extensions
{
	public static class Linq
	{
		private static Random _Rand = new Random();

		public static T Random<T>(this IEnumerable<T> source)
		{
			return source.ElementAt(_Rand.Next(source.Count()));
		}
	}
}
