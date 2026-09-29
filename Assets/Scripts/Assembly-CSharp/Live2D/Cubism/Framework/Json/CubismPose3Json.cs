using System;
using System.Collections.Generic;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	[Serializable]
	public sealed class CubismPose3Json
	{
		[Serializable]
		public struct SerializablePoseGroup
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public string[] Link;
		}

		[SerializeField]
		public string Type;

		[SerializeField]
		public float FadeInTime;

		[SerializeField]
		public SerializablePoseGroup[][] Groups;

		public static CubismPose3Json LoadFrom(string pose3Json)
		{
			if (string.IsNullOrEmpty(pose3Json))
			{
				return null;
			}
			CubismPose3Json cubismPose3Json = new CubismPose3Json();
			Value value = CubismJsonParser.ParseFromString(pose3Json);
			cubismPose3Json.Type = ((value.Get("Type") == null) ? null : value.Get("Type").toString());
			cubismPose3Json.FadeInTime = ((value.Get("FadeInTime") == null) ? 0.5f : value.Get("FadeInTime").ToFloat());
			List<Value> list = ((value.Get("Groups") == null) ? null : value.Get("Groups").GetVector(null));
			if (list != null)
			{
				cubismPose3Json.Groups = new SerializablePoseGroup[list.Count][];
				for (int i = 0; i < list.Count; i++)
				{
					int count = list[i].GetVector(null).Count;
					cubismPose3Json.Groups[i] = new SerializablePoseGroup[count];
					for (int j = 0; j < count; j++)
					{
						cubismPose3Json.Groups[i][j].Id = list[i].GetVector(null)[j].Get("Id").toString();
						List<Value> vector = list[i].GetVector(null)[j].Get("Link").GetVector(null);
						if (vector.Count != 0)
						{
							cubismPose3Json.Groups[i][j].Link = new string[vector.Count];
							for (int k = 0; k < vector.Count; k++)
							{
								cubismPose3Json.Groups[i][j].Link[k] = vector[k].toString();
							}
						}
					}
				}
			}
			return cubismPose3Json;
		}

		public static CubismPose3Json LoadFrom(TextAsset pose3JsonAsset)
		{
			if (!(pose3JsonAsset == null))
			{
				return LoadFrom(pose3JsonAsset.text);
			}
			return null;
		}
	}
}
