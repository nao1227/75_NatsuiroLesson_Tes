using System;
using System.Security.Cryptography;

namespace Paidia.Utils
{
	public static class Randomize
	{
		public static string RandomString(int length)
		{
			using RNGCryptoServiceProvider rNGCryptoServiceProvider = new RNGCryptoServiceProvider();
			bool flag = true;
			string text = "";
			while (flag)
			{
				byte[] array = new byte[(length * 6 + 7) / 8];
				rNGCryptoServiceProvider.GetBytes(array);
				text = Convert.ToBase64String(array);
				flag = text.Contains("/");
			}
			return text;
		}
	}
}
