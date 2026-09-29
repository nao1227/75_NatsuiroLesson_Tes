using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class VariableSizeObject : MonoBehaviour
	{
		[NonSerialized]
		public bool IsMouseOn;

		private bool _isAble = true;

		private RectTransform rect;

		public float EdgePercentage = 0.1f;

		public IObservable<PointerEventData> OnClick { get; private set; }

		public bool IsMouseOnEdge => IsOnEdge();

		private Vector3 rectPos => Camera.main.WorldToScreenPoint(rect.position);

		private void Start()
		{
			rect = GetComponent<RectTransform>();
			RawImage component = GetComponent<RawImage>();
			(from _ in component.OnPointerEnterAsObservable()
				where _isAble
				select _).Subscribe(delegate
			{
				IsMouseOn = true;
			}).AddTo(this);
			(from _ in component.OnPointerExitAsObservable()
				where _isAble
				select _).Subscribe(delegate
			{
				IsMouseOn = false;
			}).AddTo(this);
		}

		public void SetAble(bool able)
		{
			_isAble = able;
		}

		private bool IsOnEdge()
		{
			float num = (float)Screen.width / 800f;
			float num2 = (float)Screen.height / 450f;
			Vector3 mousePosition = Input.mousePosition;
			float num3 = rect.sizeDelta.x * num;
			float num4 = rect.sizeDelta.y * num2;
			float num5 = Mathf.Abs(mousePosition.x - (rectPos.x + num3 / 2f));
			float num6 = Mathf.Abs(mousePosition.y - (rectPos.y + num4 / 2f));
			if (!(num5 > num3 * (1f - EdgePercentage) / 2f))
			{
				return num6 > num4 * (1f - EdgePercentage) / 2f;
			}
			return true;
		}

		private void Update()
		{
			Vector3 mousePosition = Input.mousePosition;
			new Vector3((mousePosition.x - (float)(Screen.width / 2)) / (float)Screen.width, (mousePosition.y - (float)(Screen.height / 2)) / (float)Screen.height, 0f);
			_ = rect.sizeDelta;
			_ = rect.lossyScale;
			_ = rect.sizeDelta;
			_ = rect.lossyScale;
		}
	}
}
