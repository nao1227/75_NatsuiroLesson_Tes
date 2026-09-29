using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class IconObject : MonoBehaviour
	{
		private Image _img;

		[SerializeField]
		private Sprite DisabledSprite;

		[SerializeField]
		private List<Sprite> AlternateIcons;

		private Sprite _defaultSprite;

		[SerializeField]
		private List<string> Descriptions;

		public AssetReference ClickSE;

		private AudioClip _onMouseSE;

		private AudioClip _clickSE;

		private const float ALPHA_DEFAULT = 0.6f;

		private const float ALPHA_ON_MOUSE = 1f;

		private const float ALPHA_ON_SELECT = 1f;

		private const float ALPHA_UNABLE = 0.15f;

		private Subject<string> _onMouse = new Subject<string>();

		private Subject<Unit> _onClick;

		[NonSerialized]
		public bool IsMouseOn;

		[SerializeField]
		private bool IsShown;

		public bool IsAble;

		private bool _isInOsawari;

		private Color Color;

		[NonSerialized]
		public bool IsLoaded;

		private bool _inHilight;

		private Func<bool> _clickAllowedFunc;

		public IObservable<Unit> OnClick => _onClick.Where((Unit _) => IsAble);

		public IObservable<string> OnMouse => _onMouse;

		public IObservable<string> OnMouseExit => from _ in _img.OnPointerExitAsObservable()
			select string.Empty;

		public int Index { get; private set; }

		public bool IsInHilight => _inHilight;

		private bool _clickALlowed => _clickAllowedFunc();

		public virtual async UniTask ManagedStart(Func<bool> clickAllowedFunc)
		{
			Color = SaveLoadManager.GlobalData.GameOption.UIColor;
			_clickAllowedFunc = clickAllowedFunc;
			_isInOsawari = false;
			_img = GetComponent<Image>();
			_defaultSprite = _img.sprite;
			_onClick = new Subject<Unit>();
			AsyncOperationHandle<AudioClip> onMouseHandle = Addressables.LoadAssetAsync<AudioClip>("SE/OnMouse");
			AsyncOperationHandle<AudioClip> clickHandle = Addressables.LoadAssetAsync<AudioClip>("SE/Click");
			await onMouseHandle;
			await clickHandle;
			if (onMouseHandle.Status == AsyncOperationStatus.Succeeded)
			{
				_onMouseSE = onMouseHandle.Result;
			}
			if (clickHandle.Status == AsyncOperationStatus.Succeeded)
			{
				_clickSE = clickHandle.Result;
			}
			(from _ in _img.OnPointerEnterAsObservable()
				where !_isInOsawari && _clickALlowed
				where IsAble && !IsInHilight && !Input.GetMouseButton(0)
				select _).Subscribe(delegate
			{
				_onMouse.OnNext(Descriptions[Index]);
				if (null != _onMouseSE)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_onMouseSE);
				}
				IsMouseOn = true;
				_img.color = new Color(Color.r, Color.g, Color.b, 1f);
			}).AddTo(this);
			(from _ in _img.OnPointerDownAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where IsAble && !IsInHilight && _clickALlowed
				select _).Subscribe(delegate
			{
				_img.color = new Color(Color.r * 0.7f, Color.g * 0.7f, Color.b * 0.7f, 1f);
			}).AddTo(this);
			(from _ in _img.OnPointerUpAsObservable()
				where IsAble && !IsInHilight
				select _).Subscribe(delegate
			{
				if (IsMouseOn)
				{
					_img.color = new Color(Color.r, Color.g, Color.b, 1f);
				}
				else
				{
					_img.color = new Color(Color.r, Color.g, Color.b, 0.6f);
				}
			}).AddTo(this);
			_img.OnPointerExitAsObservable().Subscribe(delegate
			{
				IsMouseOn = false;
				if (IsAble)
				{
					_img.color = new Color(Color.r, Color.g, Color.b, 0.6f);
				}
			}).AddTo(this);
			(from _ in _img.OnPointerClickAsObservable()
				where _.button == PointerEventData.InputButton.Left
				where IsAble && !_isInOsawari && !IsInHilight
				select _).Subscribe(delegate
			{
				if (null != _clickSE)
				{
					SingletonManager<SoundManager>.Instance.PlaySE(_clickSE);
				}
				_onClick.OnNext(Unit.Default);
			}).AddTo(this);
			IsAble = true;
			ShowIcon(IsShown);
			_img.color = new Color(Color.r, Color.g, Color.b, 0.6f);
			IsLoaded = true;
		}

		private void Disable()
		{
			_img.color = new Color(Color.r, Color.g, Color.b, 0.15f);
			IsAble = false;
			if (null != DisabledSprite)
			{
				_img.sprite = DisabledSprite;
			}
		}

		private void Enable()
		{
			_img.color = new Color(Color.r, Color.g, Color.b, 0.6f);
			IsAble = true;
			ChangeIcon(Index);
		}

		public void ShowIcon(bool show)
		{
			base.gameObject.SetActive(show);
		}

		public void SetEnable(bool isAble)
		{
			if (IsLoaded)
			{
				if (isAble)
				{
					Enable();
				}
				else
				{
					Disable();
				}
			}
		}

		public void SetIsInOsawari(bool isInOsawari)
		{
			_isInOsawari = isInOsawari;
		}

		public void ChangeIcon(int index)
		{
			Index = index;
			if (Index > 0 && Index <= AlternateIcons.Count)
			{
				_img.sprite = AlternateIcons[Index - 1];
			}
			else if (IsAble || null == DisabledSprite)
			{
				_img.sprite = _defaultSprite;
				Index = 0;
			}
			else
			{
				_img.sprite = DisabledSprite;
			}
		}

		public void Click()
		{
			_onClick.OnNext(Unit.Default);
		}

		protected void Update()
		{
			if (IsLoaded && !_inHilight && Color != SaveLoadManager.GlobalData.GameOption.UIColor)
			{
				Color = SaveLoadManager.GlobalData.GameOption.UIColor;
				if (IsAble)
				{
					_img.color = new Color(Color.r, Color.g, Color.b, 0.6f);
				}
				else
				{
					_img.color = new Color(Color.r, Color.g, Color.b, 0.15f);
				}
			}
		}

		public void Highlight()
		{
			_inHilight = true;
			DOVirtual.Float(0.4f, 1f, 0.5f, delegate(float x)
			{
				_img.color = new Color(Color.r, Color.g, Color.b, x);
			}).SetLoops(5, LoopType.Yoyo).OnComplete(delegate
			{
				_inHilight = false;
			})
				.Play();
		}

		public string GetDescription()
		{
			return Descriptions[Index];
		}
	}
}
