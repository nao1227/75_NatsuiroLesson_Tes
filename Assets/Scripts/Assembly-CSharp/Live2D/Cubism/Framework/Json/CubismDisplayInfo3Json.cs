using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.Json
{
	[Serializable]
	public sealed class CubismDisplayInfo3Json
	{
		[Serializable]
		public struct SerializableParameters
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public string GroupId;

			[SerializeField]
			public string Name;
		}

		[Serializable]
		public struct SerializableParameterGroups
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public string GroupId;

			[SerializeField]
			public string Name;
		}

		[Serializable]
		public struct SerializableParts
		{
			[SerializeField]
			public string Id;

			[SerializeField]
			public string Name;
		}

		[SerializeField]
		public int Version;

		[SerializeField]
		public SerializableParameters[] Parameters;

		[SerializeField]
		public SerializableParameterGroups[] ParameterGroups;

		[SerializeField]
		public SerializableParts[] Parts;

		public static CubismDisplayInfo3Json LoadFrom(string cdi3Json)
		{
			if (string.IsNullOrEmpty(cdi3Json))
			{
				return null;
			}
			return JsonUtility.FromJson<CubismDisplayInfo3Json>(cdi3Json);
		}
	}
}
