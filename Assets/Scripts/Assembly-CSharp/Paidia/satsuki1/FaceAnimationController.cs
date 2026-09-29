using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public abstract class FaceAnimationController : MonoBehaviour, IFaceController
	{
		public VoiceManager VoiceManager;

		public Live2DAnimator Animator;

		public int LayerIndex;

		protected StateMachineObservables[] _observables;

		protected StringReactiveProperty _state;

		public FaceStatesList FaceStates;

		protected WeightedStateList _faceCandidates;

		protected FaceListName _candidateName;

		protected List<Tuple<FaceListName, bool>> _reservedFaceList;

		protected OsawariManager _manager;

		protected CancellationTokenSource _cts;

		protected CancellationTokenSource _moveStateTokenSource;

		public int WomanExtacyThreshold;

		protected int _womanExtacy;

		protected Dictionary<string, AudioClip> _cache;

		protected bool _isKissFace;

		protected bool _isKissing;

		private InsertController _insertController;

		public bool IsStateChangeAllowed;

		public List<FaceListName> AllowExtacyFaceList;

		[SerializeField]
		protected bool _animationStarted;

		protected bool _isUtageAnimating;

		protected bool _isWomanExtacy => _womanExtacy > WomanExtacyThreshold;

		public virtual void SetUtageAnimating(bool value)
		{
			_isUtageAnimating = value;
		}

		public virtual Live2DAnimator GetAnimator()
		{
			return Animator;
		}

		public virtual async UniTask ManagedStart(OsawariManager manager)
		{
			_cts = new CancellationTokenSource();
			_candidateName = GetIdleStateName();
			_reservedFaceList = new List<Tuple<FaceListName, bool>>();
			_faceCandidates = FaceStates.GetFaceList(_candidateName, manager);
			_state = new StringReactiveProperty();
			_manager = manager;
			_insertController = UnityEngine.Object.FindObjectOfType<InsertController>();
			IsStateChangeAllowed = true;
			await SetCache();
			if (null != Animator)
			{
				_observables = Animator.GetRx();
				if (_observables.Count() > 0)
				{
					SetUpAnimation();
				}
			}
			SetRx();
		}

		protected void ReserveFace(FaceListName name, bool restrictStateChange = false)
		{
			_reservedFaceList.Add(new Tuple<FaceListName, bool>(name, restrictStateChange));
		}

		public void SetAnimationEnable()
		{
			_animationStarted = true;
		}

		protected async UniTask SetCache()
		{
			_cache = new Dictionary<string, AudioClip>();
			foreach (NamedWeightedStateList item in FaceStates.List)
			{
				foreach (WeightedState state in item.List)
				{
					if (state.Voice.RuntimeKey.ToString() != "")
					{
						AudioClip value = await state.Voice.LoadAssetAsync<AudioClip>();
						if (!_cache.ContainsKey(state.StateName.ToString()))
						{
							_cache.Add(state.StateName.ToString(), value);
						}
					}
				}
			}
		}

		protected virtual void SetRx()
		{
			_manager.OnFaceChanged.DistinctUntilChanged().Subscribe(delegate(FaceState x)
			{
				FaceChange(x);
			}).AddTo(this);
			_manager.TemporaryStatus.Feelings.OnAtomosphereChanged.DistinctUntilChanged().Subscribe(async delegate
			{
				await UniTask.Yield();
				ReloadFace();
			}).AddTo(this);
		}

		public void SetFaceList(FaceListName name, bool reload = true, bool restrictStateChange = false)
		{
			if (reload)
			{
				_candidateName = name;
				_reservedFaceList.Clear();
				_faceCandidates = FaceStates.GetFaceList(name, _manager);
				IsStateChangeAllowed = !restrictStateChange;
				if (reload)
				{
					MoveState(_cts.Token).Forget();
				}
			}
			else
			{
				ReserveFace(name, restrictStateChange);
			}
		}

		protected void UpdateAllowExtacy()
		{
			if (AllowExtacyFaceList.Count == 0)
			{
				_manager.TemporaryStatus.Feelings.IsExtacyAllowed = true;
			}
			else
			{
				_manager.TemporaryStatus.Feelings.IsExtacyAllowed = AllowExtacyFaceList.Contains(_candidateName);
			}
		}

		protected virtual async UniTask MoveState(CancellationToken token, FaceStateName forceNext = FaceStateName.None, int crossFadeTime = 300, int crossFadeOutTime = 0)
		{
			_moveStateTokenSource?.Cancel();
			_moveStateTokenSource = new CancellationTokenSource();
			FaceStateName next = GetNextState();
			try
			{
				await UniTask.WaitUntil(() => _animationStarted, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			}
			catch (OperationCanceledException)
			{
				return;
			}
			AudioClip clip = null;
			if (_cache.ContainsKey(next.ToString()))
			{
				clip = _cache[next.ToString()];
			}
			try
			{
				await Animator.PlayFaceAnimation(next, crossFadeTime, clip, 100);
				MoveState(_moveStateTokenSource.Token).Forget();
			}
			catch (PlayingTemporaryAnimationException)
			{
			}
			catch (PlayingSameAnimationException)
			{
			}
			if (_isKissFace)
			{
				_isKissFace = false;
			}
		}

		protected virtual void FaceChange(FaceState state)
		{
		}

		public virtual void ManagedUpdate()
		{
			if (!(null == Animator))
			{
				UpdateAllowExtacy();
			}
		}

		protected abstract void SetUpAnimation();

		protected virtual FaceStateName GetNextState()
		{
			if (_reservedFaceList.Count > 0)
			{
				FaceListName faceListName = _reservedFaceList[0].Item1;
				IsStateChangeAllowed = !_reservedFaceList[0].Item2;
				_reservedFaceList.RemoveAt(0);
				if (_isWomanExtacy)
				{
					switch (faceListName)
					{
					case FaceListName.Piston:
						faceListName = FaceListName.PistonExtacy;
						break;
					case FaceListName.PistonKiss:
						faceListName = FaceListName.PistonKissExtacy;
						break;
					case FaceListName.Idle:
						faceListName = FaceListName.IdleExtacy;
						break;
					}
				}
				else
				{
					switch (faceListName)
					{
					case FaceListName.PistonExtacy:
						faceListName = FaceListName.Piston;
						break;
					case FaceListName.PistonKissExtacy:
						faceListName = FaceListName.PistonKiss;
						break;
					case FaceListName.IdleExtacy:
						faceListName = FaceListName.Idle;
						break;
					}
				}
				_candidateName = faceListName;
				_faceCandidates = FaceStates.GetFaceList(faceListName, _manager);
			}
			return GetRandomState(_faceCandidates);
		}

		protected FaceStateName GetRandomState(WeightedStateList list, bool idle = true, bool insert = false)
		{
			return list.GetRandomState(Animator.GetCurrentAnimatorStateInfo(LayerIndex).GetStateName<FaceStateName>());
		}

		public virtual void ResetFace(bool reload = true)
		{
			Animator.SetDetailAnimationEnable(enable: true);
			if (_isWomanExtacy && FaceStates.GetFaceList(FaceListName.IdleExtacy, _manager) != null)
			{
				SetFaceList(FaceListName.IdleExtacy, reload);
			}
			else
			{
				SetFaceList(FaceListName.Idle, reload);
			}
		}

		public virtual void ReloadFace()
		{
			_faceCandidates = FaceStates.GetFaceList(_candidateName, _manager);
			MoveState(_cts.Token).Forget();
		}

		public void RestartPlayingAutoFaceAnimation()
		{
			MoveState(_cts.Token).Forget();
		}

		private void OnDestroy()
		{
			foreach (NamedWeightedStateList item in FaceStates.List)
			{
				foreach (WeightedState item2 in item.List)
				{
					if (item2.Voice.IsValid())
					{
						item2.Voice.ReleaseAsset();
					}
				}
			}
			_cts?.Cancel();
			_moveStateTokenSource?.Cancel();
		}

		protected void RefreshToken()
		{
			_cts.Cancel();
			_cts = new CancellationTokenSource();
		}

		protected void SetFace(FaceListName fln)
		{
			SetFaceList(fln);
		}

		protected virtual FaceListName GetIdleStateName()
		{
			return FaceListName.Idle;
		}

		public void CancelToken()
		{
			_cts?.Cancel();
			_moveStateTokenSource?.Cancel();
			Animator.CancelFaceAnimation();
		}
	}
}
