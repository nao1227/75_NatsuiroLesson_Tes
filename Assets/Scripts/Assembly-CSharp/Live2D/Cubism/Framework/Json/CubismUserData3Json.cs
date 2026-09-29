using System;
using System.Collections.Generic;
using Live2D.Cubism.Framework.UserData;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	[Serializable]
	public sealed class CubismUserData3Json
	{
		[Serializable]
		public struct SerializableMeta
		{
			[SerializeField]
			public int UserDataCount;

			[SerializeField]
			public int TotalUserDataCount;
		}

		[Serializable]
		public struct SerializableUserData
		{
			[SerializeField]
			public string Target;

			[SerializeField]
			public string Id;

			[SerializeField]
			public string Value;
		}

		[SerializeField]
		public int Version;

		[SerializeField]
		public SerializableMeta Meta;

		[SerializeField]
		public SerializableUserData[] UserData;

		public static CubismUserData3Json LoadFrom(string userData3Json)
		{
			if (!string.IsNullOrEmpty(userData3Json))
			{
				return JsonUtility.FromJson<CubismUserData3Json>(userData3Json);
			}
			return null;
		}

		public static CubismUserData3Json LoadFrom(TextAsset userData3JsonAsset)
		{
			if (!(userData3JsonAsset == null))
			{
				return LoadFrom(userData3JsonAsset.text);
			}
			return null;
		}

		public CubismUserDataBody[] ToBodyArray(CubismUserDataTargetType targetType)
		{
			List<CubismUserDataBody> list = new List<CubismUserDataBody>();
			for (int i = 0; i < UserData.Length; i++)
			{
				CubismUserDataBody item = new CubismUserDataBody
				{
					Id = UserData[i].Id,
					Value = UserData[i].Value
				};
				if (targetType == CubismUserDataTargetType.ArtMesh && UserData[i].Target == "ArtMesh")
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}
	}
}
