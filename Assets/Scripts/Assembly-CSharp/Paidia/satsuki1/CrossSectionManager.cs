using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class CrossSectionManager : MonoBehaviour
	{
		public Camera CrossSectionVCam;

		public GameObject CrossSection;

		public RawImage TouchArea;

		public OsawariCrossSection OsawariCrossSection;

		public int BaseHeight = 180;

		public int BaseWidth = 120;

		public float MaxRatio = 2f;

		private float _nowRatio;

		public float OriginalRatio = 1f;

		private RectTransform rect;

		private int _resizeFrom;

		public CanvasGroup CSCG;

		public Vector2 OriginalRightLowerVec { get; private set; }

		public Vector2 OriginalSize { get; private set; }

		private Vector3 rectPos => Camera.main.WorldToScreenPoint(rect.position);

		private void Start()
		{
			rect = TouchArea.GetComponent<RectTransform>();
			OriginalRatio = rect.localScale.x;
			OriginalSize = rect.sizeDelta;
			ResetOriginalVecs();
			OsawariCrossSection.ManagedStart();
			_nowRatio = 1f;
		}

		public void ResetOriginalVecs()
		{
			OriginalRightLowerVec = GetOriginalRightLowerVec();
		}

		public void SetResizeFrom()
		{
			_resizeFrom = 0;
			Vector2 mousePosAsAnchored = GetMousePosAsAnchored();
			if (mousePosAsAnchored.x > rect.anchoredPosition.x + rect.sizeDelta.x * rect.localScale.x / 2f)
			{
				_resizeFrom++;
			}
			if (mousePosAsAnchored.y < rect.anchoredPosition.y + rect.sizeDelta.y * rect.localScale.y / 2f)
			{
				_resizeFrom += 2;
			}
		}

		public void ResetResizeFrom()
		{
			_resizeFrom = 0;
			if (rect.pivot == new Vector2(0.5f, 0.5f))
			{
				rect.pivot = Vector2.zero;
				rect.anchoredPosition -= new Vector2(rect.sizeDelta.x / 2f, rect.sizeDelta.y / 2f) * rect.localScale.x;
			}
		}

		public void Resize(Vector3 mousePos)
		{
			int num = ((_resizeFrom % 2 != 1) ? 1 : (-1));
			int num2 = ((_resizeFrom / 2 != 0) ? 1 : (-1));
			if (rect.pivot == Vector2.zero)
			{
				rect.pivot = new Vector2(0.5f, 0.5f);
				rect.anchoredPosition += new Vector2(rect.sizeDelta.x / 2f, rect.sizeDelta.y / 2f) * rect.localScale.x;
			}
			Vector2 mousePosAsAnchored = GetMousePosAsAnchored();
			float a = (float)num * (rect.anchoredPosition.x - mousePosAsAnchored.x) / (OriginalSize.x * OriginalRatio / 2f);
			float b = (float)num2 * (rect.anchoredPosition.y - mousePosAsAnchored.y) / (OriginalSize.y * OriginalRatio / 2f);
			float num3 = (_nowRatio = Mathf.Max(Mathf.Min(Mathf.Max(a, b), MaxRatio), 0.5f));
			TouchArea.transform.localScale = new Vector3(num3, num3, num3);
			ForceRectInView();
		}

		public void Move()
		{
			float x = rect.sizeDelta.x;
			float y = rect.sizeDelta.y;
			rect.anchoredPosition = GetMousePosAsAnchored() - new Vector2(x / 2f, y / 2f) * rect.localScale.x;
			ForceRectInView();
		}

		private void ForceRectInView()
		{
			rect.anchoredPosition = new Vector2(Mathf.Clamp(rect.anchoredPosition.x, rect.pivot.x * rect.sizeDelta.x, 800f - rect.sizeDelta.x * (1f - rect.pivot.x) * rect.localScale.x), Mathf.Clamp(rect.anchoredPosition.y, rect.pivot.y * rect.sizeDelta.y, 450f - rect.sizeDelta.y * (1f - rect.pivot.y) * rect.localScale.y));
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

		private Vector2 GetOriginalRightLowerVec()
		{
			float x = rect.lossyScale.x;
			float y = rect.lossyScale.y;
			Vector3 vector = rectPos;
			Vector2 sizeDelta = rect.sizeDelta;
			return new Vector2(vector.x + sizeDelta.x * x / rect.localScale.x, vector.y + sizeDelta.y * y / rect.localScale.y);
		}

		private Vector2 GetSizeDelta()
		{
			Vector2 sizeDelta = rect.sizeDelta;
			return new Vector2(sizeDelta.x * rect.lossyScale.x, sizeDelta.y * rect.lossyScale.y) / rect.localScale.x;
		}

		public void SetVisible(bool visible)
		{
			if (!(null == CrossSection))
			{
				CSCG.blocksRaycasts = visible;
				CrossSectionVCam.gameObject.SetActive(visible);
				TouchArea.gameObject.SetActive(visible);
			}
		}
	}
}
