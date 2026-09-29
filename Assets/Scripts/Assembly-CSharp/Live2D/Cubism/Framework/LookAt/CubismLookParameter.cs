using Live2D.Cubism.Core;
using UnityEngine;

namespace Live2D.Cubism.Framework.LookAt
{
	public sealed class CubismLookParameter : MonoBehaviour
	{
		[SerializeField]
		public CubismLookAxis Axis;

		[SerializeField]
		public float Factor;

		private void Reset()
		{
			CubismParameter component = GetComponent<CubismParameter>();
			if (!(component == null))
			{
				if (component.name.EndsWith("Y"))
				{
					Axis = CubismLookAxis.Y;
				}
				else if (component.name.EndsWith("Z"))
				{
					Axis = CubismLookAxis.Z;
				}
				else
				{
					Axis = CubismLookAxis.X;
				}
				Factor = component.MaximumValue;
			}
		}

		internal float TickAndEvaluate(Vector3 targetOffset)
		{
			float num = ((Axis == CubismLookAxis.X) ? targetOffset.x : targetOffset.y);
			if (Axis == CubismLookAxis.Z)
			{
				num = targetOffset.z;
			}
			return num * Factor;
		}
	}
}
