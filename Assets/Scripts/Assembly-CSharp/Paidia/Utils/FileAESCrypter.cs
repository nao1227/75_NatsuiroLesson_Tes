using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Paidia.Utils
{
	public class FileAESCrypter
	{
		private static readonly string EncryptKey = "sMhUM44U6yNRD93s";

		public static void EncryptJsonToFile<T>(T data, string savePath)
		{
			EncryptToFile(JsonUtility.ToJson(data), savePath);
		}

		public static void EncryptToFile(string data, string savePath)
		{
			EncryptAes(Encoding.UTF8.GetBytes(data), out var initialVector, out var dst);
			byte[] bytes = Encoding.UTF8.GetBytes(initialVector);
			if (!File.Exists(savePath))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(savePath));
			}
			using FileStream output = new FileStream(savePath, FileMode.Create, FileAccess.Write);
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(bytes.Length);
			binaryWriter.Write(bytes);
			binaryWriter.Write(dst.Length);
			binaryWriter.Write(dst);
			binaryWriter.Close();
		}

		public static bool TryDecryptJsonFromFile<T>(out T data, string savePath)
		{
			string data2;
			bool flag = TryDecryptFromFile(out data2, savePath);
			data = (flag ? JsonUtility.FromJson<T>(data2) : default(T));
			return flag;
		}

		public static bool TryDecryptFromFile(out string data, string savePath)
		{
			byte[] bytes = null;
			byte[] src = null;
			if (!File.Exists(savePath))
			{
				Debug.LogError("cant find decrpt file path:" + savePath);
				data = string.Empty;
				return false;
			}
			using (FileStream input = new FileStream(savePath, FileMode.Open, FileAccess.Read))
			{
				using BinaryReader binaryReader = new BinaryReader(input);
				int count = binaryReader.ReadInt32();
				bytes = binaryReader.ReadBytes(count);
				count = binaryReader.ReadInt32();
				src = binaryReader.ReadBytes(count);
			}
			string initialVector = Encoding.UTF8.GetString(bytes);
			DecryptAes(src, initialVector, out var dst);
			data = Encoding.UTF8.GetString(dst);
			return true;
		}

		public static void EncryptAes(byte[] src, out string initialVector, out byte[] dst)
		{
			initialVector = Guid.NewGuid().ToString("N").Substring(0, EncryptKey.Length);
			dst = null;
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.Mode = CipherMode.CBC;
			rijndaelManaged.KeySize = 256;
			rijndaelManaged.BlockSize = 128;
			byte[] bytes = Encoding.UTF8.GetBytes(EncryptKey);
			byte[] bytes2 = Encoding.UTF8.GetBytes(initialVector);
			using ICryptoTransform transform = rijndaelManaged.CreateEncryptor(bytes, bytes2);
			using MemoryStream memoryStream = new MemoryStream();
			using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			cryptoStream.Write(src, 0, src.Length);
			cryptoStream.FlushFinalBlock();
			dst = memoryStream.ToArray();
		}

		public static void DecryptAes(byte[] src, string initialVector, out byte[] dst)
		{
			dst = new byte[src.Length];
			using RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Padding = PaddingMode.PKCS7;
			rijndaelManaged.Mode = CipherMode.CBC;
			rijndaelManaged.KeySize = 256;
			rijndaelManaged.BlockSize = 128;
			byte[] bytes = Encoding.UTF8.GetBytes(EncryptKey);
			byte[] bytes2 = Encoding.UTF8.GetBytes(initialVector);
			using ICryptoTransform transform = rijndaelManaged.CreateDecryptor(bytes, bytes2);
			using MemoryStream stream = new MemoryStream(src);
			using CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
			cryptoStream.Read(dst, 0, dst.Length);
		}
	}
}
