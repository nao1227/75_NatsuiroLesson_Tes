using System;
using System.Collections;
using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/Lib/Sound/Timer")]
	public class Timer : MonoBehaviour
	{
		[SerializeField]
		private float duration;

		[SerializeField]
		private float delay;

		[SerializeField]
		private bool unscaled;

		[SerializeField]
		[NotEditable]
		private float time;

		[SerializeField]
		[NotEditable]
		private float time01;

		public TimerEvent onStart = new TimerEvent();

		public TimerEvent onUpdate = new TimerEvent();

		public TimerEvent onComplete = new TimerEvent();

		[SerializeField]
		private bool autoDestroy;

		[SerializeField]
		private bool autoStart;

		private Action<Timer> callbackUpdate;

		private Action<Timer> callbackComplete;

		public float Duration
		{
			get
			{
				return duration;
			}
			protected set
			{
				duration = value;
			}
		}

		public float Delay
		{
			get
			{
				return delay;
			}
			protected set
			{
				delay = value;
			}
		}

		public bool Unscaled
		{
			get
			{
				return unscaled;
			}
			set
			{
				unscaled = value;
			}
		}

		public float Time
		{
			get
			{
				return time;
			}
			protected set
			{
				time = value;
			}
		}

		public float Time01
		{
			get
			{
				return time01;
			}
			protected set
			{
				time01 = value;
			}
		}

		public float Time01Inverse => 1f - Time01;

		public bool AutoDestroy
		{
			get
			{
				return autoDestroy;
			}
			set
			{
				autoDestroy = value;
			}
		}

		public bool IsPlaying { get; protected set; }

		public float GetCurve01(EaseType easeType = EaseType.Linear)
		{
			return Easing.GetCurve01(Time01, easeType);
		}

		public float GetCurve01Inverse(EaseType easeType = EaseType.Linear)
		{
			return Easing.GetCurve01(Time01Inverse, easeType);
		}

		public float GetCurve(float start, float end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public float GetCurve(float start, float end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public Vector2 GetCurve(Vector2 start, Vector2 end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public Vector2 GetCurve(Vector2 start, Vector2 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public Vector3 GetCurve(Vector3 start, Vector3 end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public Vector3 GetCurve(Vector3 start, Vector3 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public Vector4 GetCurve(Vector4 start, Vector4 end)
		{
			return GetCurve(start, end, EaseType.Linear);
		}

		public Vector4 GetCurve(Vector4 start, Vector4 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01, easeType);
		}

		public float GetCurveInverse(float start, float end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public float GetCurveInverse(float start, float end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		public Vector2 GetCurveInverse(Vector2 start, Vector2 end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public Vector2 GetCurveInverse(Vector2 start, Vector2 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		public Vector3 GetCurveInverse(Vector3 start, Vector3 end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public Vector3 GetCurveInverse(Vector3 start, Vector3 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		public Vector4 GetCurveInverse(Vector4 start, Vector4 end)
		{
			return GetCurveInverse(start, end, EaseType.Linear);
		}

		public Vector4 GetCurveInverse(Vector4 start, Vector4 end, EaseType easeType)
		{
			return Easing.GetCurve(start, end, Time01Inverse, easeType);
		}

		private void Start()
		{
			if (autoStart)
			{
				StartCoroutine(CoTimer(Duration, Delay, Unscaled));
			}
		}

		public void Cancel()
		{
			OnCompleteCallback();
			StopAllCoroutines();
		}

		public void StartTimer(float duration, Action<Timer> onUpdate = null, Action<Timer> onComplete = null, float delay = 0f)
		{
			StartTimer(duration, Unscaled, onUpdate, onComplete, delay);
		}

		public void StartTimer(float duration, bool unscaled, Action<Timer> onUpdate = null, Action<Timer> onComplete = null, float delay = 0f)
		{
			callbackUpdate = onUpdate;
			callbackComplete = onComplete;
			StartTimer(duration, unscaled, delay);
		}

		public void StartTimer(float duration, float delay = 0f)
		{
			StartTimer(duration, Unscaled, delay);
		}

		public void StartTimer(float duration, bool unscaled, float delay = 0f)
		{
			autoStart = false;
			StopAllCoroutines();
			StartCoroutine(CoTimer(duration, delay, unscaled));
		}

		private IEnumerator CoTimer(float duration, float delay, bool unscaled)
		{
			Duration = duration;
			Delay = delay;
			Unscaled = unscaled;
			IsPlaying = true;
			yield return new WaitTimer(Duration, Delay, Unscaled, OnStart, OnUpdate, OnComplete);
		}

		private void OnStart(WaitTimer timer)
		{
			onStart.Invoke(this);
		}

		private void OnUpdate(WaitTimer timer)
		{
			Time = timer.Time;
			Time01 = timer.Time01;
			OnUpdate();
		}

		private void OnUpdate()
		{
			onUpdate.Invoke(this);
			if (callbackUpdate != null)
			{
				callbackUpdate(this);
			}
		}

		private void OnComplete(WaitTimer timer)
		{
			OnComplete();
		}

		private void OnComplete()
		{
			OnCompleteCallback();
			if (AutoDestroy)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnCompleteCallback()
		{
			IsPlaying = false;
			onComplete.Invoke(this);
			if (callbackComplete != null)
			{
				callbackComplete(this);
			}
			callbackComplete = null;
		}

		public void SkipToEnd()
		{
			Time = Duration;
			Time01 = 1f;
			OnUpdate();
			OnComplete();
			StopAllCoroutines();
		}
	}
}
