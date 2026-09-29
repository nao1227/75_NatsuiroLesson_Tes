using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class InsertController : MonoBehaviour
	{
		public FaceController FaceController;

		public StatusObject Status;

		public OsawariEvent WomanExtacyEvent;

		public OsawariEvent WomanExtacyFinishEvent;

		public OsawariEvent ManExtacyEvent;

		public OsawariEvent ManExtacyFinishEvent;

		public string ZettyouFaceAnimationName = "Zettyou";

		public string ZettyouFaceAnimationStateName = "Face_Zettyou";

		public string ShaseiSoundSetName = "shasei";

		public string ShaseiFaceAnimationName = "Shasei";

		public string ShaseiFaceAnimationStateName = "Syasei_1";

		public float ExtacyTime = 5.5f;

		private IntReactiveProperty _ejaculateCount;

		private Live2DAnimator _animator;

		public Subject<Unit> OnEjaculate;

		private Subject<Unit> _onWomanExcite = new Subject<Unit>();

		public int HeartCountOnExtacy = 20;

		public int HeartDelayOnExtacy = 1000;

		private OsawariConditions _emptyConditions = OsawariConditions.Empty;

		public IReadOnlyReactiveProperty<int> EjaculateCount => _ejaculateCount;

		public bool IsEjaculatable => _ejaculateCount.Value > 0;

		public bool IsWomanExtacy { get; private set; }

		public bool IsManExtacy { get; private set; }

		public IObservable<Unit> OnWomanExcite => _onWomanExcite;

		public IReadOnlyReactiveProperty<int> WomanExtacy => Status.TemporaryStatus.Feelings.ExciteRx;

		public IReadOnlyReactiveProperty<float> ManExtacy => Status.TemporaryStatus.ManExtacy;

		public void ManagedStart()
		{
			OnEjaculate = new Subject<Unit>();
			_ejaculateCount = new IntReactiveProperty(Status.PersistantStatus.EjaculateCount);
			WomanExtacyEvent?.Initialize();
			WomanExtacyFinishEvent?.Initialize();
			ManExtacyEvent?.Initialize();
			ManExtacyFinishEvent?.Initialize();
			_animator = GetComponent<Live2DAnimator>();
			IsWomanExtacy = false;
			IsManExtacy = false;
			(from x in Status.TemporaryStatus.Feelings.ExciteRx
				where !IsWomanExtacy
				where x == 100000
				select x).Subscribe(async delegate
			{
				Status.TemporaryStatus.ExciteCount++;
				_onWomanExcite.OnNext(Unit.Default);
				UnityEngine.Object.FindObjectOfType<VFXManager>().PlayShower(HeartCountOnExtacy, HeartDelayOnExtacy);
				IsWomanExtacy = true;
				Status.TemporaryStatus.Feelings.ExcitePhase = ExcitePhase.Locked;
				FaceController.SetSight(Bool: false);
				FaceController.SetDirectionFixed(Bool: true);
				_animator.SetBool(ZettyouFaceAnimationName, on: true);
				SaveLoadManager.UnsavedData.WomanExtacyCount++;
				_animator.SetDetailAnimationEnable(enable: false);
				WomanExtacyEvent?.InvokeEvent(Status.TemporaryStatus, _emptyConditions);
				try
				{
					await UniTask.Delay(TimeSpan.FromSeconds(ExtacyTime), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				}
				catch
				{
				}
				FinishExtacy();
			}).AddTo(this);
			(from x in Status.TemporaryStatus.ManExtacy
				where !IsManExtacy
				where x >= (float)Status.TemporaryStatus.ManExtacyMax
				select x).Subscribe(async delegate
			{
				OsawariManager component = GetComponent<OsawariManager>();
				Status.TemporaryStatus.EjaculateCount++;
				IsManExtacy = true;
				if (component.ContextManager.Context == OsawariContext.Osawari)
				{
					OsawariGoods osawariGoods = UnityEngine.Object.FindObjectOfType<OsawariGoods>();
					bool flag = false;
					if (null != osawariGoods)
					{
						flag = osawariGoods.IsCondomSet;
					}
					if (SaveLoadManager.GlobalData.GameOption.CountEjaculationWithCondom || !flag)
					{
						SaveLoadManager.UnsavedData.EjaculationInVaginaCount++;
					}
				}
				else
				{
					SaveLoadManager.UnsavedData.FellatioEjaculationCount++;
				}
				OnEjaculate.OnNext(Unit.Default);
				if (component.ContextManager.Context == OsawariContext.Osawari)
				{
					_animator.SetBool(ShaseiFaceAnimationName, on: true);
				}
				ManExtacyEvent?.InvokeEvent(Status.TemporaryStatus, _emptyConditions);
				await Status.TemporaryStatus.ResetExtacyManGradually();
				_animator.SetBool(ShaseiFaceAnimationName, on: false);
				IsManExtacy = false;
				_ejaculateCount.Value--;
				ManExtacyFinishEvent?.InvokeEvent(Status.TemporaryStatus, _emptyConditions);
			}).AddTo(this);
			_animator.GetRx();
		}

		public void SetIsKissing(bool isKissing)
		{
			_emptyConditions.IsKissing = isKissing;
		}

		public void FinishExtacy()
		{
			Status.TemporaryStatus.Feelings.ExcitePhase = ExcitePhase.OnlyDown;
			_animator.SetBool(ZettyouFaceAnimationName, on: false);
			FaceController.SetSight(Bool: true);
			FaceController.SetDirectionFixed(Bool: false);
			IsWomanExtacy = false;
			WomanExtacyFinishEvent?.InvokeEvent(Status.TemporaryStatus, _emptyConditions);
			_animator.SetDetailAnimationEnable(enable: true);
		}

		public void ManagedUpdate()
		{
		}

		public void AddWomanExtacy(float value)
		{
			Status.TemporaryStatus.AddExciteValue((int)value);
		}

		public void AddManExtacy(float value)
		{
			if (!IsEjaculatable)
			{
				Status.TemporaryStatus.ResetExtacyMan();
			}
			else if (!IsManExtacy)
			{
				Status.TemporaryStatus.AddManExtacy(value);
			}
		}
	}
}
