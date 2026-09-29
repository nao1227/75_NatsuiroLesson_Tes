using System;
using UnityEngine;

namespace Live2D.Cubism.Framework.UserData
{
	[Serializable]
	public struct CubismUserDataBody
	{
		[SerializeField]
		public string Id;

		[SerializeField]
		public string Value;
	}
}
