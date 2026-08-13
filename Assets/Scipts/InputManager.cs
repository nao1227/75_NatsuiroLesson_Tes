using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Framework.Raycasting;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;
using Stubs;

public class InputManager : MonoBehaviour
{
	private MouseOn _pressedMouseOn;

	private IInputProvider _input;

	private CubismRaycaster _raycaster;

	private IInputTrigger _manager;

	public OsawariCameraManager CameraManager;

	public CrossSectionManager CsManager;

	private UtageManager _utage;

	private MessageWindowUIPresenter _messageWindowUIPresenter;

	private bool _pressed;

	[NonSerialized]
	public bool IsMouseOnUI;

	[NonSerialized]
	public bool IsMouseOnVariableSize;

	[NonSerialized]
	public bool IsMouseOnEdgeOfVariableSize;

	private CompositeDisposable _disposables = new CompositeDisposable();

	private CancellationTokenSource _tokenSource;

	private BoolReactiveProperty _isInOsawari;

	public MouseOn MouseOn
	{
		get
		{
			if (_anyModalOpen)
			{
				return MouseOn.UI;
			}
			if (_pressed)
			{
				return _pressedMouseOn;
			}
			return _mouseOn;
		}
	}

	private MouseOn _mouseOn
	{
		get
		{
			if (IsMouseOnUI)
			{
				return MouseOn.UI;
			}
			if (IsMouseOnEdgeOfVariableSize)
			{
				return MouseOn.Edge;
			}
			if (IsMouseOnVariableSize)
			{
				return MouseOn.VariableSizeObject;
			}
			if (IsMouseOnOsawariParts)
			{
				return MouseOn.Osawari;
			}
			return MouseOn.None;
		}
	}

	private bool _anyModalOpen
	{
		get
		{
			if (_manager.GetScene() != null)
			{
				if (!_manager.GetScene().IsModalWindowOpen.Value && !_manager.GetScene().IsResultWindowOpen.Value)
				{
					return _messageWindowUIPresenter.ShowingMessage;
				}
				return true;
			}
			return false;
		}
	}

	public IReadOnlyReactiveProperty<bool> IsInOsawari => _isInOsawari;

	public bool IsMouseOnOsawariParts
	{
		get
		{
			try
			{
				CubismRaycastHit[] array = new CubismRaycastHit[8];
				Ray ray = Camera.main.ScreenPointToRay(_input.GetPosition());
				_raycaster.Raycast(ray, array);
				CubismRaycastHit[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					CubismRaycastHit cubismRaycastHit = array2[i];
					if (cubismRaycastHit.Drawable == null)
					{
						return false;
					}
					AbstractOsawari osawariFromDrawable = _manager.GetOsawariFromDrawable(cubismRaycastHit.Drawable);
					if (null != osawariFromDrawable && osawariFromDrawable.GetConstraints() && osawariFromDrawable.CanTouchMesh(cubismRaycastHit.Drawable))
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}
	}

	public bool IsClickingAny
	{
		get
		{
			if (_input != null)
			{
				return _input.InputMouse();
			}
			return false;
		}
	}

	public void ManagedStart(CubismRaycaster raycaster, IInputProvider input, IInputTrigger manager)
	{
		_raycaster = raycaster;
		_input = input;
		_manager = manager;
		_pressed = false;
		_tokenSource = new CancellationTokenSource();
		_isInOsawari = new BoolReactiveProperty(initialValue: false);
		IsMouseOnUI = false;
		_utage = UnityEngine.Object.FindObjectOfType<UtageManager>();
		_messageWindowUIPresenter = UnityEngine.Object.FindObjectOfType<MessageWindowUIPresenter>();
		SetUpRx();
	}

	public void SwitchContext()
	{
		_raycaster = _manager.GetCubismRaycaster();
	}

	private void SetUpRx()
	{
		
		IObservable<long> source = from _ in Observable.EveryGameObjectUpdate()
			where _input.InputMouseRelease()
			select _;
		IObservable<long> source2 = from _ in Observable.EveryGameObjectUpdate()
			where _input.InputGrab() && !_pressed
			select _;
		IObservable<long> source3 = from _ in Observable.EveryUpdate()
			where _input.GetAxis() != 0f
			select _;
		_ = from _ in Observable.EveryGameObjectUpdate()
			where _input.InputSpecialRelease()
			select _;
		(from _ in source2
			where _manager.GetScene() == null || !_manager.GetScene().IsModalWindowOpen.Value
			// where _raycaster != null && null != Camera.main
			where null != Camera.main //テスト用
			select new
			{
				Results = new CubismRaycastHit[8],
				Ray = Camera.main.ScreenPointToRay(_input.GetPosition())
			}).Subscribe(async x =>
		{
			 Debug.Log("Subscribeの中身に到達した");   // ← この1行を追加
			_pressed = true;
			_pressedMouseOn = _mouseOn;
			try
			{
				switch (MouseOn)
				{
				case MouseOn.None:
					_isInOsawari.Value = true;
					await MoveCamera(_tokenSource.Token);
					break;
				case MouseOn.Osawari:
				{
					_isInOsawari.Value = true;
					int hitCount = _raycaster.Raycast(x.Ray, x.Results);
					await UpdateWhileClick(x.Results, hitCount, _tokenSource.Token);
					_manager.OnMouseUpTrigger();
					break;
				}
				case MouseOn.Edge:
					await ResizeVariableSizeObject(_tokenSource.Token);
					break;
				case MouseOn.VariableSizeObject:
					await MoveVariableSizeObject(_tokenSource.Token);
					break;
				case MouseOn.UI:
					break;
				}
			}
			catch (OperationCanceledException)
			{
				_disposables.Dispose();
			}
			finally
			{
				if (_isInOsawari.Value)
				{
					_manager.OnMouseUpTrigger();
				}
			}
		}).AddTo(_disposables);
		source.Subscribe(delegate
		{
			_isInOsawari.Value = false;
			_pressed = false;
		}).AddTo(_disposables);
		source3.Where((Func<long, bool>)delegate
		{
			UtageManager utage = _utage;
			return (object)utage != null && !utage.IsPlaying;
		}).Subscribe(delegate
		{
			CameraManager.CameraZoom(_input.GetAxis() > 0f);
		}).AddTo(_disposables);
	}

	public Vector3 GetCurrentMousePosition()
	{
		return _input.GetPosition();
	}

	private async UniTask UpdateWhileClick(CubismRaycastHit[] results, int hitCount, CancellationToken token)
	{
		_manager.UpdateWhileClicked(results, hitCount, isFirst: true);
		while (_pressed)
		{
			if (_input.InputSpecial())
			{
				_manager.OnInputSpecialTrigger(results, hitCount);
			}
			if (_input.InputAuto())
			{
				_manager.OnAutoTrigger(results, hitCount);
			}
			else
			{
				_manager.UpdateWhileClicked(results, hitCount, isFirst: false);
			}
			await UniTask.Yield(token);
		}
	}

	private async UniTask MoveCamera(CancellationToken token)
	{
		CameraManager.SetMousePos(GetCurrentMousePosition());
		while (_pressed)
		{
			await UniTask.Yield(token);
			CameraManager.MoveCamera(GetCurrentMousePosition());
		}
	}

	private async UniTask ResizeVariableSizeObject(CancellationToken token)
	{
		CsManager.SetResizeFrom();
		while (_pressed)
		{
			Vector3 currentMousePosition = GetCurrentMousePosition();
			CsManager.Resize(currentMousePosition);
			await UniTask.Yield(token);
		}
		CsManager.ResetResizeFrom();
	}

	public Vector3 GetMouseMove()
	{
		return _input.GetMouseMove();
	}

	private async UniTask MoveVariableSizeObject(CancellationToken token)
	{
		while (_pressed)
		{
			CsManager.Move();
			await UniTask.Yield(token);
		}
		CsManager.ResetOriginalVecs();
	}

	private void OnDestroy()
	{
		_disposables?.Dispose();
		_tokenSource?.Cancel();
	}
}
