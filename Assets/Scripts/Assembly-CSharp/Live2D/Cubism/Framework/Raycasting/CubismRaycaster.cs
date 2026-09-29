using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using UnityEngine;

namespace Live2D.Cubism.Framework.Raycasting
{
	public sealed class CubismRaycaster : MonoBehaviour
	{
		private CubismRenderer[] Raycastables { get; set; }

		private CubismRaycastablePrecision[] RaycastablePrecisions { get; set; }

		private void Refresh()
		{
			CubismDrawable[] drawables = this.FindCubismModel().Drawables;
			List<CubismRenderer> list = new List<CubismRenderer>();
			List<CubismRaycastablePrecision> list2 = new List<CubismRaycastablePrecision>();
			for (int i = 0; i < drawables.Length; i++)
			{
				if (!(drawables[i].GetComponent<CubismRaycastable>() == null))
				{
					list.Add(drawables[i].GetComponent<CubismRenderer>());
					list2.Add(drawables[i].GetComponent<CubismRaycastable>().Precision);
				}
			}
			Raycastables = list.ToArray();
			RaycastablePrecisions = list2.ToArray();
		}

		private void Start()
		{
			Refresh();
		}

		public int Raycast(Vector3 origin, Vector3 direction, CubismRaycastHit[] result, float maximumDistance = float.PositiveInfinity)
		{
			return Raycast(new Ray(origin, direction), result, maximumDistance);
		}

		public int Raycast(Ray ray, CubismRaycastHit[] result, float maximumDistance = float.PositiveInfinity)
		{
			Vector3 vector = ray.origin + ray.direction * (ray.direction.z / ray.origin.z);
			Vector3 vector2 = base.transform.InverseTransformPoint(vector);
			vector2.z = 0f;
			float magnitude = vector.magnitude;
			if (magnitude > maximumDistance)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < Raycastables.Length; i++)
			{
				CubismRenderer cubismRenderer = Raycastables[i];
				CubismRaycastablePrecision cubismRaycastablePrecision = RaycastablePrecisions[i];
				if (cubismRenderer.MeshRenderer.enabled && cubismRenderer.Mesh.bounds.Contains(vector2) && (cubismRaycastablePrecision != CubismRaycastablePrecision.Triangles || ContainsInTriangles(cubismRenderer.Mesh, vector2)))
				{
					result[num].Drawable = cubismRenderer.GetComponent<CubismDrawable>();
					result[num].Distance = magnitude;
					result[num].LocalPosition = vector2;
					result[num].WorldPosition = vector;
					num++;
					if (num == result.Length)
					{
						break;
					}
				}
			}
			return num;
		}

		private bool ContainsInTriangles(Mesh mesh, Vector3 inputPosition)
		{
			for (int i = 0; i < mesh.triangles.Length; i += 3)
			{
				Vector3 vector = mesh.vertices[mesh.triangles[i]];
				Vector3 vector2 = mesh.vertices[mesh.triangles[i + 1]];
				Vector3 vector3 = mesh.vertices[mesh.triangles[i + 2]];
				float num = (vector2.x - vector.x) * (inputPosition.y - vector2.y) - (vector2.y - vector.y) * (inputPosition.x - vector2.x);
				float num2 = (vector3.x - vector2.x) * (inputPosition.y - vector3.y) - (vector3.y - vector2.y) * (inputPosition.x - vector3.x);
				float num3 = (vector.x - vector3.x) * (inputPosition.y - vector.y) - (vector.y - vector3.y) * (inputPosition.x - vector.x);
				if ((num > 0f && num2 > 0f && num3 > 0f) || (num < 0f && num2 < 0f && num3 < 0f))
				{
					return true;
				}
			}
			return false;
		}
	}
}
