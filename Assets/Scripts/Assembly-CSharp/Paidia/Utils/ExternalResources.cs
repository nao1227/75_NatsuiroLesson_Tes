using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Paidia.Utils
{
	public static class ExternalResources
	{
		private static byte[] LoadBytes(string path)
		{
			BinaryReader binaryReader = new BinaryReader(new FileStream(path, FileMode.Open));
			byte[] result = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
			binaryReader.Close();
			return result;
		}

		private static async UniTask<byte[]> LoadBytesAsync(string path)
		{
			byte[] result;
			using (FileStream stream = File.Open(path, FileMode.Open))
			{
				result = new byte[stream.Length];
				await stream.ReadAsync(result, 0, (int)stream.Length);
			}
			return result;
		}

		public static Texture2D readImage(string name)
		{
			Texture2D texture2D = new Texture2D(0, 0);
			texture2D.LoadImage(LoadBytes(name));
			return texture2D;
		}

		public static async UniTask<Texture2D> ReadImageAsync(string name)
		{
			string uri = "file://" + name;
			try
			{
				using UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
				await request.SendWebRequest();
				return DownloadHandlerTexture.GetContent(request);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
