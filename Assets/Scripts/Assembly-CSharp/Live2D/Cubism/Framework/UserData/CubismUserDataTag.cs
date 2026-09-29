using UnityEngine;

namespace Live2D.Cubism.Framework.UserData
{
	[CubismDontMoveOnReimport]
	public class CubismUserDataTag : MonoBehaviour
	{
		[SerializeField]
		[HideInInspector]
		private string _value;

		[SerializeField]
		[HideInInspector]
		private CubismUserDataBody _body;

		public string Value
		{
			get
			{
				if (string.IsNullOrEmpty(_value) && !string.IsNullOrEmpty(Body.Value))
				{
					_value = Body.Value;
				}
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		private CubismUserDataBody Body
		{
			get
			{
				return _body;
			}
			set
			{
				_body = value;
			}
		}

		public void Initialize(CubismUserDataBody body)
		{
			Body = body;
		}
	}
}
