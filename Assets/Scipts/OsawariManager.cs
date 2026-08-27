using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Raycasting;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;
using Stubs;  //スタブ

public class OsawariManager : MonoBehaviour, IInputTrigger
{
	public ContextOsawariTargetList ContextOsawariTargets;

	public ContextRaycasterList Raycasters;

	public OsawariCameraManager cameraManager;

	public CrossSectionManager CsManager;

	public ActionManager ActionManager;

	[SerializeField]
	private StatusObject StatusObject;

	public ContextManager ContextManager;

	public InputManager OsawariInput;

	public CubismModel Model;

	public CubismModel CSModel;

	public HandManager HandManager;

	[NonSerialized]
	public Vector3 MousePos;

	[NonSerialized]
	public Vector3 MousePosOnLastFrame;

	public HScene Scene;

	public Camera MainCamera;

	[SerializeField]
	private List<ContextObject> ContextObjects;

	[SerializeField]
	private List<ContextAnimator> ContextAnimator;

	private AbstractOsawari _targetOsawari;

	public ValuePreserver Preserver;

	public int HitAreaNumber;

	public int FellatioHitAreaNumber;

	public int PaizuriHitAreaNumber;

	public int HitPeriodMillSec;

	public int BlinkTime;

	public OsawariFellatio OsawariFellatio;

	public OsawariPaizuri OsawariPaizuri;

	public int AtomosphereMinimum;

	[NonSerialized]
	public UtageManager UtageManager;

	[NonSerialized]
	public bool IsAction;

	private ParameterValue _hitArea;

	private bool _isShowingAnimation;

	[NonSerialized]
	public bool IsLoaded;

	private Subject<FaceState> _onFaceChanged;

	public Subject<Unit> OnKissStart;

	public Subject<Unit> OnKissEnd;

	private Subject<HintButtonName> _onHint;

	public FaceState OsawariFaceState;

	public bool IsKissing;

	public bool IsPistonMoving;

	public OsawariResult Result;

	protected CubismDrawable targetMesh;

	public TemporaryStatus TemporaryStatus => StatusObject.TemporaryStatus;

	public PersistantStatus PersistantStatus => StatusObject.PersistantStatus;

	public bool AllResourceLoaded { get; private set; }

	private List<AbstractOsawari> _targets => ContextOsawariTargets.GetOsawariTargets(ContextManager.Context);

	public IObservable<FaceState> OnFaceChanged => _onFaceChanged;

	public IObservable<HintButtonName> OnHint => _onHint;

	public StatusObject GetStatusObject()
	{
		return StatusObject;
	}

	public void ManagedStart()
	{
		Preserver.ManagedStart();
		Preserver.SetModel(Model);
		Result = new OsawariResult(ActionManager);
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		_hitArea = new ParameterValue(Model.Parameters[HitAreaNumber]);
		ContextManager = UnityEngine.Object.FindObjectOfType<ContextManager>();
		_onFaceChanged = new Subject<FaceState>();
		_onHint = new Subject<HintButtonName>();
		OnKissStart = new Subject<Unit>();
		OnKissEnd = new Subject<Unit>();
		HandManager = new HandManager();
		UtageManager = UnityEngine.Object.FindObjectOfType<UtageManager>();
		if (null != UtageManager)
		{
			TemporaryStatus.Cloth = (ClothName)UtageManager.GetInt("cloth");
		}
		TemporaryStatus.Feelings.TemporaryAtomosphereMinimum = AtomosphereMinimum;
		OsawariInput.ManagedStart(GetCubismRaycaster(), new MouseInputProvider(UtageManager), this);
		AllResourceLoaded = true;
		OsawariFellatio?.ManagedStart(this);
		foreach (List<AbstractOsawari> contextOsawariTarget in ContextOsawariTargets)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < contextOsawariTarget.Count; i++)
			{
				contextOsawariTarget[i].ManagedStart(this, cancellationTokenOnDestroy);
				list.Add(contextOsawariTarget[i].Priority);
				if (contextOsawariTarget[i] is OsawariPiston)
				{
					(contextOsawariTarget[i] as OsawariPiston).OnInsert().Subscribe(delegate(bool x)
					{
						OsawariFaceState = (x ? FaceState.Insert : FaceState.Eject);
						_onFaceChanged.OnNext(OsawariFaceState);
					}).AddTo(this);
					(contextOsawariTarget[i] as OsawariPiston).OnPiston().Subscribe(delegate(bool x)
					{
						IsPistonMoving = x;
						OsawariFaceState = (x ? FaceState.Piston : FaceState.Idle);
						_onFaceChanged.OnNext(OsawariFaceState);
					}).AddTo(this);
				}
				else if (contextOsawariTarget[i] is OsawariKiss)
				{
					OsawariKiss obj = contextOsawariTarget[i] as OsawariKiss;
					obj.OnKissStart.Subscribe(delegate
					{
						OnKissStart.OnNext(Unit.Default);
						SaveLoadManager.UnsavedData.KissCount++;
					}).AddTo(this);
					obj.OnKissEnd.Subscribe(delegate
					{
						OnKissEnd.OnNext(Unit.Default);
					}).AddTo(this);
				}
			}
			for (int num = 0; num < contextOsawariTarget.Count; num++)
			{
				contextOsawariTarget[num].PostInitialize();
			}
			OsawariGoods component = GetComponent<OsawariGoods>();
			if (null != component && component is IInsertable)
			{
				(component as IInsertable).OnInsert().Subscribe(delegate(bool x)
				{
					OsawariFaceState = (x ? FaceState.Insert : FaceState.Eject);
					_onFaceChanged.OnNext(OsawariFaceState);
				}).AddTo(this);
			}
			if (null != component && component is IPiston)
			{
				(component as IPiston).OnPiston().Subscribe(delegate(bool x)
				{
					IsPistonMoving = x;
					OsawariFaceState = (x ? FaceState.Piston : FaceState.Idle);
					_onFaceChanged.OnNext(OsawariFaceState);
				}).AddTo(this);
			}
		}
		SwitchContext();
		MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		if (null != OsawariFellatio)
		{
			Preserver.SetModel(OsawariFellatio.Model);
		}
		if (null != OsawariPaizuri)
		{
			Preserver.SetModel(OsawariPaizuri.AnotherModel);
		}
		SingletonManager<SceneContextManager>.Instance.AllowOsawari = true;
		IsLoaded = true;
	}

	public void CheckLeastAtomosphere()
	{
		TemporaryStatus.Feelings.TemporaryAtomosphereMinimum = AtomosphereMinimum;
		TemporaryStatus.Feelings.SetAtomosphere(AtomosphereMinimum, forceUpdate: true);
	}

	public void SetMinimumAtomosphere(int value)
	{
		AtomosphereMinimum = value;
		TemporaryStatus.Feelings.TemporaryAtomosphereMinimum = value;
		if (TemporaryStatus.Feelings.Atomosphere.Value <= value)
		{
			TemporaryStatus.Feelings.SetAtomosphere(value, forceUpdate: true);
		}
	}

	public void SwitchContext()
	{
		List<GameObject> objects = ContextObjects.First((ContextObject x) => x.Context == ContextManager.Context).Objects;
		IEnumerable<ContextObject> enumerable = ContextObjects.Where((ContextObject x) => x.Context != ContextManager.Context);
		objects.ForEach(delegate(GameObject x)
		{
			x.SetActive(value: true);
		});
		foreach (ContextObject item in enumerable)
		{
			item.Objects.ForEach(delegate(GameObject x)
			{
				x.SetActive(value: false);
			});
		}
		foreach (List<AbstractOsawari> contextOsawariTarget in ContextOsawariTargets)
		{
			for (int num = 0; num < contextOsawariTarget.Count; num++)
			{
				contextOsawariTarget[num].Cancel();
				contextOsawariTarget[num].SwitchContext();
			}
		}
		OsawariInput.SwitchContext();
		if (ContextManager.Context == OsawariContext.Fellatio)
		{
			SaveLoadManager.UnsavedData.FellaSceneCount++;
			OsawariFellatio.SwitchContext();
		}
		else if (ContextManager.Context == OsawariContext.Paizuri)
		{
			OsawariPaizuri.SwitchContext();
		}
		else
		{
			OsawariFellatio?.LeaveContext();
		}
	}

	private void Update()
	{
		if (!IsLoaded)
		{
			return;
		}
		MousePosOnLastFrame = MousePos;
		MousePos = GetCurrentMousePosition();
		if (Input.GetKey(KeyCode.Tab))
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		bool flag = true;
		for (int i = 0; i < _targets.Count; i++)
		{
			_targets[i].ManagedUpdate();
			if (_targets[i] is IWearable && ((IWearable)_targets[i]).IsWearing())
			{
				flag = false;
			}
			if (_targets[i] is OsawariKiss)
			{
				IsKissing = (_targets[i] as OsawariKiss).IsKissing;
			}
		}
		foreach (List<AbstractOsawari> item in ContextOsawariTargets.GetOsawariTargetsOfNotInContext(ContextManager.Context))
		{
			foreach (AbstractOsawari item2 in item)
			{
				item2.ManagedUpdateWhileNotActive();
			}
		}
		if (ContextManager.Context == OsawariContext.Fellatio)
		{
			OsawariFellatio?.ManagedUpdate();
			OsawariFellatio osawariFellatio = OsawariFellatio;
			if ((object)osawariFellatio != null && osawariFellatio.IsWearing())
			{
				flag = false;
			}
		}
		if (!flag)
		{
			return;
		}
		foreach (OsawariAction item3 in ActionManager.GetActionOf<IUndressTrigger>())
		{
			item3.StartAction();
			item3.FinishAction();
		}
	}

	public Vector3 GetMove()
	{
		return MousePos - MousePosOnLastFrame;
	}

	public void SetTemporaryStatus(TemporaryStatus status)
	{
		StatusObject.TemporaryStatus.CopyValues(status);
	}

	public void UpdateWhileClicked(CubismRaycastHit[] results, int hitCount, bool isFirst)
	{
		Debug.Log("HandManager は null か: " + (HandManager == null));
		Debug.Log("AllowOsawari は: " + SingletonManager<SceneContextManager>.Instance.AllowOsawari);
		if (!SingletonManager<SceneContextManager>.Instance.AllowOsawari)
		{
			return;
		}
		int num = -1;
		
		if (isFirst)
		{
			targetMesh = null;
			if (hitCount > 0 && _targetOsawari == null)
			{
				string text = hitCount.ToString();
				for (int i = 0; i < hitCount; i++)
				{
					text = text + "\n" + results[i].Drawable.name;
					for (int j = 0; j < _targets.Count; j++)
					{
						if (_targets[j].CanTouchMesh(results[i].Drawable) && IsTouchable(_targets[j]) && num < _targets[j].Priority)
						{
							_targetOsawari = _targets[j];
							targetMesh = results[i].Drawable;
							num = _targetOsawari.Priority;
						}
					}
				}
			}
		}
		if (_targetOsawari != null)
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Confined;
			_targetOsawari.OnClick(targetMesh, isFirst);
		}
	}

	public AbstractOsawari GetOsawariFromDrawable(CubismDrawable drawable)
	{
		foreach (AbstractOsawari target in _targets)
		{
			if (target.TouchableMeshs.Contains(drawable))
			{
				return target;
			}
		}
		return null;
	}

	bool IInputTrigger.IsClickingAtMesh(CubismRaycastHit[] results, int hitCount)
	{
		if (hitCount <= 1)
		{
			return _targetOsawari != null;
		}
		return true;
	}

	public void OnAutoTrigger(CubismRaycastHit[] results, int hitCount)
	{
		if (_targetOsawari != null)
		{
			_targetOsawari.SetAuto();
		}
	}

	public void OnMouseUpTrigger()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		if (_targetOsawari != null)
		{
			_targetOsawari.OnMouseUp();
			if (_targetOsawari.IsAnimating)
			{
				_ = _targetOsawari is OsawariWithoutHand;
			}
		}
		_targetOsawari = null;
		IsAction = false;
	}

	public void OnInputSpecialTrigger(CubismRaycastHit[] results, int hitCount)
	{
		if (_targetOsawari != null)
		{
			_targetOsawari.OnSpecial();
		}
	}

	public bool IsTouchable(AbstractOsawari target)
	{
		if (target is OsawariWithoutHand || target is OsawariPiston)
		{
			return target.GetConstraints();
		}
		if (target is ISwitchable)
		{
			if (((ISwitchable)target).Touchable)
			{
				return target.GetConstraints();
			}
			return false;
		}
		if (HandManager.IsGrabbing(target) || CanGrab(target))
		{
			return target.GetConstraints();
		}
		return false;
	}

	public bool CanGrab(AbstractOsawari target)
	{
		if (target == null)
		{
			return false;
		}
		bool flag = HandManager.IsAnyHandEmpty;
		if (target is IParticularHand)
		{
			flag = !((IParticularHand)target).GetParticularHand().IsGrabbing;
		}
		if (target is OsawariDoublehanded || target is IDoubleHanded)
		{
			flag = !HandManager.IsGrabbingAny;
		}
		return !IsAction && !HandManager.IsGrabbing(target) && flag;
	}

	public Vector3 GetCurrentMousePosition()
	{
		return OsawariInput.GetCurrentMousePosition();
	}

	public T GetOsawariOf<T>(bool onlyFromCurrentContext = false) where T : AbstractOsawari
	{
		T result = null;
		IEnumerable<AbstractOsawari> enumerable = new List<AbstractOsawari>();
		if (onlyFromCurrentContext)
		{
			enumerable = _targets;
		}
		else
		{
			foreach (List<AbstractOsawari> contextOsawariTarget in ContextOsawariTargets)
			{
				enumerable = enumerable.Concat(contextOsawariTarget);
			}
		}
		try
		{
			result = (T)enumerable.First((AbstractOsawari x) => x is T);
			return result;
		}
		catch (Exception)
		{
		}
		return result;
	}

	public T GetInterfaceOf<T>(bool onlyFromCurrentContext = false)
	{
		AbstractOsawari abstractOsawari = null;
		IEnumerable<AbstractOsawari> enumerable = new List<AbstractOsawari>();
		if (onlyFromCurrentContext)
		{
			enumerable = _targets;
		}
		else
		{
			foreach (List<AbstractOsawari> contextOsawariTarget in ContextOsawariTargets)
			{
				enumerable = enumerable.Concat(contextOsawariTarget);
			}
		}
		try
		{
			abstractOsawari = enumerable.First((AbstractOsawari x) => x is T);
		}
		catch (Exception)
		{
		}
		return (T)(object)abstractOsawari;
	}

	public List<T> GetEveryInterfaceOf<T>(bool onlyFromCurrentContext = false)
	{
		List<T> tmp = new List<T>();
		IEnumerable<AbstractOsawari> enumerable = new List<AbstractOsawari>();
		if (onlyFromCurrentContext)
		{
			enumerable = _targets;
		}
		else
		{
			foreach (List<AbstractOsawari> contextOsawariTarget in ContextOsawariTargets)
			{
				enumerable = enumerable.Concat(contextOsawariTarget);
			}
		}
		try
		{
			enumerable.Where((AbstractOsawari x) => x is T).ToList().ForEach(delegate(AbstractOsawari x)
			{
				tmp.Add((T)(object)x);
			});
			return tmp;
		}
		catch (Exception)
		{
		}
		return tmp;
	}

	public List<T> GetEveryOsawariOf<T>(bool onlyFromCurrentContext = false) where T : AbstractOsawari
	{
		List<T> list = new List<T>();
		IEnumerable<AbstractOsawari> enumerable = new List<AbstractOsawari>();
		if (onlyFromCurrentContext)
		{
			enumerable = _targets;
		}
		else
		{
			foreach (List<AbstractOsawari> contextOsawariTarget in ContextOsawariTargets)
			{
				enumerable = enumerable.Concat(contextOsawariTarget);
			}
		}
		foreach (AbstractOsawari item in enumerable.Where((AbstractOsawari x) => x is T))
		{
			list.Add((T)item);
		}
		return list;
	}

	public List<T> GetEveryOsawariByInterface<T>() where T : IBreast
	{
		List<T> list = new List<T>();
		foreach (AbstractOsawari item in _targets.Where((AbstractOsawari x) => x is T))
		{
			list.Add((T)(object)item);
		}
		return list;
	}

	public bool IsAbleToInsert()
	{
		return GetOsawariOf<OsawariPants>().IsAbleToInsert();
	}

	public bool IsClickingAt(AbstractOsawari target)
	{
		return _targetOsawari == target;
	}

	private void LateUpdate()
	{
		if (!IsLoaded)
		{
			return;
		}
		if (ContextManager.Context == OsawariContext.Fellatio)
		{
			Preserver.SetValue(FellatioHitAreaNumber, _hitArea, CubismParameterBlendMode.Override, 1);
		}
		else if (ContextManager.Context == OsawariContext.Paizuri)
		{
			Preserver.SetValue(PaizuriHitAreaNumber, _hitArea, CubismParameterBlendMode.Override, 1);
		}
		else
		{
			Preserver.SetValue(HitAreaNumber, _hitArea);
		}
		List<float> list = new List<float>();
		foreach (AbstractOsawari target in _targets)
		{
			target.ManagedLateUpdate();
		}
		OsawariFellatio?.ManagedLateUpdate();
		CubismParameter[] parameters = Model.Parameters;
		foreach (CubismParameter cubismParameter in parameters)
		{
			list.Add(cubismParameter.Value);
		}
	}

	public Scene GetScene()
	{
		return Scene;
	}

	public void AddFirstStimulus(int stimulusVal)
	{
		if (TemporaryStatus.IsStimulusDecreasable)
		{
			CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
			int num = Math.Max(stimulusVal - TemporaryStatus.Feelings.Stimulus, 0);
			if (num != 0)
			{
				TemporaryStatus.Feelings.SetStimulusWithTimer(num, cancellationTokenOnDestroy).Forget();
				TemporaryStatus.IsStimulusDecreasable = false;
				SetCooldownTimer(1500, cancellationTokenOnDestroy).Forget();
			}
		}
	}

	private async UniTask SetCooldownTimer(int time, CancellationToken token)
	{
		await UniTask.Delay(time, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		TemporaryStatus.IsStimulusDecreasable = true;
	}

	public async UniTask ShowHitAreaAnimation()
	{
		if (_isShowingAnimation)
		{
			return;
		}
		_isShowingAnimation = true;
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		for (int j = 0; j < BlinkTime; j++)
		{
			for (int i = 0; i < HitPeriodMillSec / 20; i++)
			{
				_hitArea += 20f / (float)HitPeriodMillSec;
				await UniTask.Delay(10, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
			for (int i = 0; i < HitPeriodMillSec / 10; i++)
			{
				_hitArea -= 20f / (float)HitPeriodMillSec;
				await UniTask.Delay(10, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			}
		}
		Preserver.InactivateValue(HitAreaNumber);
		_isShowingAnimation = false;
	}

	public CubismRaycaster GetCubismRaycaster()
	{
		return Raycasters.GetRaycaster(ContextManager.Context);
	}

	public void ShowHint(HintButtonName hintButton)
	{
		_onHint.OnNext(hintButton);
	}

	public void FinishScene()
	{
		if (SaveLoadManager.UnsavedData.Days >= 7)
		{
			Result.CalcResult(TemporaryStatus);
		}
	}

	public Live2DAnimator GetAnimator()
	{
		return ContextAnimator.First((ContextAnimator x) => x.Context == ContextManager.Context).Animator;
	}

	private void OnDestroy()
	{
		_onFaceChanged?.Dispose();
		_onHint?.Dispose();
		OnKissStart?.Dispose();
		OnKissEnd?.Dispose();
		if (null != SingletonManager<SceneContextManager>.Instance)
		{
			SingletonManager<SceneContextManager>.Instance.AllowOsawari = false;
		}
	}
}
