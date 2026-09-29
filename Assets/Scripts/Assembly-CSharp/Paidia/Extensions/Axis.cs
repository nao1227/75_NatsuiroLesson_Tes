using System.Collections.Generic;
using UnityEngine;

namespace Paidia.Extensions
{
	public static class Axis
	{
		public enum VerticeName2D
		{
			RightUp = 0,
			RightBottom = 1,
			LeftUp = 2,
			LeftBottom = 3
		}

		public static Dictionary<VerticeName2D, Vector3> GetBoxColliderVertices(this BoxCollider2D col)
		{
			Transform transform = col.transform;
			Vector3 lossyScale = transform.lossyScale;
			lossyScale.x *= col.size.x;
			lossyScale.y *= col.size.y;
			lossyScale *= 0.5f;
			Vector3 vector = transform.TransformPoint(col.offset);
			Vector3 vector2 = transform.right * lossyScale.x;
			Vector3 vector3 = transform.up * lossyScale.y;
			Vector3 vector4 = -vector2 + vector3;
			Vector3 vector5 = vector2 + vector3;
			Vector3 vector6 = vector2 + -vector3;
			Vector3 vector7 = -vector2 + -vector3;
			return new Dictionary<VerticeName2D, Vector3>
			{
				{
					VerticeName2D.LeftUp,
					vector + vector4
				},
				{
					VerticeName2D.RightUp,
					vector + vector5
				},
				{
					VerticeName2D.RightBottom,
					vector + vector6
				},
				{
					VerticeName2D.LeftBottom,
					vector + vector7
				}
			};
		}
	}
}
