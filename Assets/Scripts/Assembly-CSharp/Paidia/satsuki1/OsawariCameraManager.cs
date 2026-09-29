using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Paidia.Extensions;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariCameraManager : MonoBehaviour
	{
		public CinemachineVirtualCamera VCamera;

		public float Sensitivity;

		public float ZoomSpeed;

		public float ZoomOutLimitRate = 1f;

		public float ZoomUpLimitRate = 100f;

		private float _defaultSize;

		private Vector3 _lastMousePos;

		public GameObject Focus;

		public BoxCollider2D Frame;

		public float DutchRate;

		public PenisFollower PenisFollower;

		[Range(0f, 1f)]
		public float PenisMosaicChangeRate;

		private void Start()
		{
			_defaultSize = VCamera.m_Lens.OrthographicSize;
		}

		public async void CameraZoom(bool zoomUp)
		{
			if (!(null == Camera.main))
			{
				Vector3 localPosition = Camera.main.transform.localPosition;
				float num = ((!zoomUp) ? (0f - ZoomSpeed) : ZoomSpeed);
				float num2 = num;
				float orthographicSize = VCamera.m_Lens.OrthographicSize;
				VCamera.m_Lens.OrthographicSize -= num2;
				if (VCamera.m_Lens.OrthographicSize > _defaultSize * ZoomOutLimitRate)
				{
					VCamera.m_Lens.OrthographicSize = _defaultSize * ZoomOutLimitRate;
				}
				if (VCamera.m_Lens.OrthographicSize < 1f / ZoomUpLimitRate)
				{
					VCamera.m_Lens.OrthographicSize = 1f / ZoomUpLimitRate;
				}
				Vector3 localPosition2 = Camera.main.transform.localPosition;
				CorrectCamera(localPosition2 - localPosition, VCamera.m_Lens.OrthographicSize / orthographicSize);
				PenisFollower?.GetComponent<MeshRenderer>().material.SetFloat("_Size", 100f - 100f * (1f - VCamera.m_Lens.OrthographicSize / _defaultSize) * PenisMosaicChangeRate);
				await UniTask.Yield();
				CorrectCamera(Vector3.zero);
				VCamera.m_Lens.Dutch = (0f - DutchRate) * VCamera.transform.localPosition.x;
			}
		}

		public void SetMousePos(Vector3 MousePos)
		{
			_lastMousePos = MousePos;
		}

		public void MoveCamera(Vector3 MousePos)
		{
			if (!(null == Focus))
			{
				float x = MousePos.x - _lastMousePos.x;
				float y = MousePos.y - _lastMousePos.y;
				VCamera.m_Lens.Dutch = (0f - DutchRate) * VCamera.transform.localPosition.x;
				Vector3 offset = -new Vector3(x, y, 0f) * Sensitivity;
				CorrectCamera(offset);
				SetMousePos(MousePos);
			}
		}

		public void Shake(float impact, float decreaseRate)
		{
			float num = VCamera.m_Lens.OrthographicSize / _defaultSize;
			GetComponent<CinemachineImpulseSource>().GenerateImpulseAt(Vector3.zero, new Vector3(0f, impact * (1f - (1f - num) * decreaseRate), 0f));
		}

		private void CorrectCamera(Vector3 offset, float zoomChangeRate = 1f)
		{
			if (!(null == Camera.main))
			{
				float num2;
				float num3;
				float num4;
				float num5;
				if (Screen.width / 16 > Screen.height / 9)
				{
					int num = Screen.height * 16 / 9;
					num2 = (Screen.width - num) / 2;
					num3 = 0f;
					num4 = num;
					num5 = Screen.height;
				}
				else
				{
					int num6 = Screen.width * 9 / 16;
					int num7 = Screen.height - num6;
					num2 = 0f;
					num3 = num7 / 2;
					num4 = Screen.width;
					num5 = num6;
				}
				_ = new Vector3(num4, num5, 0f) * (zoomChangeRate - 1f);
				_ = new Vector3(num4, 0f - num5, 0f) * (zoomChangeRate - 1f);
				_ = new Vector3(0f - num4, 0f - num5, 0f) * (zoomChangeRate - 1f);
				_ = new Vector3(0f - num4, num5, 0f) * (zoomChangeRate - 1f);
				Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(num2 + num4, num3 + num5, 0f)) + offset;
				Vector3 vector2 = Camera.main.ScreenToWorldPoint(new Vector3(num2 + num4, num3, 0f)) + offset;
				Vector3 vector3 = Camera.main.ScreenToWorldPoint(new Vector3(num2, num3, 0f)) + offset;
				Vector3 vector4 = Camera.main.ScreenToWorldPoint(new Vector3(num2, num3 + num5, 0f)) + offset;
				float num8 = Mathf.Min(vector4.x, vector3.x);
				float num9 = Mathf.Min(vector2.y, vector3.y);
				float num10 = Mathf.Max(vector.x, vector2.x);
				float num11 = Mathf.Max(vector.y, vector4.y);
				Dictionary<Axis.VerticeName2D, Vector3> boxColliderVertices = Frame.GetBoxColliderVertices();
				if (num10 > boxColliderVertices[Axis.VerticeName2D.RightUp].x)
				{
					offset.x += boxColliderVertices[Axis.VerticeName2D.RightUp].x - num10;
				}
				else if (num8 < boxColliderVertices[Axis.VerticeName2D.LeftBottom].x)
				{
					offset.x += boxColliderVertices[Axis.VerticeName2D.LeftBottom].x - num8;
				}
				if (num11 > boxColliderVertices[Axis.VerticeName2D.RightUp].y)
				{
					offset.y += boxColliderVertices[Axis.VerticeName2D.RightUp].y - num11;
				}
				else if (num9 < boxColliderVertices[Axis.VerticeName2D.LeftBottom].y)
				{
					offset.y += boxColliderVertices[Axis.VerticeName2D.LeftBottom].y - num9;
				}
				Focus.transform.position += offset;
			}
		}

		private Vector2 GetMousePosAsAnchored()
		{
			float num3;
			float num4;
			float num5;
			float num6;
			if (Screen.width / 16 > Screen.height / 9)
			{
				int num = Screen.height * 16 / 9;
				int num2 = Screen.width - num;
				num3 = Screen.height;
				num4 = num;
				num5 = num2 / 2;
				num6 = 0f;
			}
			else
			{
				int num7 = Screen.width * 9 / 16;
				int num8 = Screen.height - num7;
				num3 = num7;
				num4 = Screen.width;
				num5 = 0f;
				num6 = num8 / 2;
			}
			float num9 = num4 / 800f;
			float num10 = num3 / 450f;
			return new Vector2((Input.mousePosition.x - num5) / num9, (Input.mousePosition.y - num6) / num10);
		}
	}
}
