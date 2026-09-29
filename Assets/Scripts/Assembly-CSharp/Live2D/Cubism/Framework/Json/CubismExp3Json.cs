using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	[Serializable]
	public sealed class CubismExp3Json
	{
		[Serializable]
		public struct SerializableExpressionParameter
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public float Value;

			[SerializeField]
			public string Blend;
		}

		[SerializeField]
		public string Type;

		[SerializeField]
		public float FadeInTime = 1f;

		[SerializeField]
		public float FadeOutTime = 1f;

		[SerializeField]
		public SerializableExpressionParameter[] Parameters;

		public static CubismExp3Json LoadFrom(string exp3Json)
		{
			if (!string.IsNullOrEmpty(exp3Json))
			{
				return JsonUtility.FromJson<CubismExp3Json>(exp3Json);
			}
			return null;
		}

		public static CubismExp3Json LoadFrom(TextAsset exp3JsonAsset)
		{
			if (!(exp3JsonAsset == null))
			{
				return LoadFrom(exp3JsonAsset.text);
			}
			return null;
		}
	}
}
