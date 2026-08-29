using System.Collections.Generic;
using UnityEngine;

namespace Stubs
{
    public class TemporaryStatus
    {
        public ClothName Cloth;
        public FeelingsClass Feelings = new FeelingsClass();
        public bool IsStimulusDecreasable;
        public void CopyValues(TemporaryStatus status) { }
        public void AddExciteValue(int value) { }
    }

    public class FeelingsClass
    {
        public int TemporaryAtomosphereMinimum;
        public int Stimulus;
        public UniRx.IntReactiveProperty Atomosphere = new UniRx.IntReactiveProperty(0);
        public void SetAtomosphere(int value, bool forceUpdate) { }
        public Cysharp.Threading.Tasks.UniTask SetStimulusWithTimer(int value, System.Threading.CancellationToken token)
        {
            return Cysharp.Threading.Tasks.UniTask.CompletedTask;
        }
        public void AddAtomosphere(int value) { }
    }

    public enum ClothName
    {
        Default
    }

    public class PersistantStatus
    {
    }



    public enum FaceState
    {
        Idle,
        Insert,
        Eject,
        Piston
    }

    public enum HintButtonName
    {
        None
    }

    public class Live2DAnimator
    {
    }

    public class ActionManager
    {
        public List<OsawariAction> GetActionOf<T>()
        {
            return new List<OsawariAction>();
        }
    }

    public class OsawariAction
    {
        public void StartAction() { }
        public void FinishAction() { }
    }

    public class ContextObject
    {
        public OsawariContext Context;
        public List<GameObject> Objects = new List<GameObject>();
    }

    public class ContextAnimator
    {
        public OsawariContext Context;
        public Live2DAnimator Animator;
    }

    public class ValuePreserver
    {
        public void ManagedStart() { }
        public void SetModel(object model) { }
        public void SetValue(int number, object value, object mode = null, int priority = 0) { }
        public void InactivateValue(int number) { }
    }

    public class OsawariFellatio
    {
        public object Model;
        public void ManagedStart(object owner) { }
        public void SwitchContext() { }
        public void LeaveContext() { }
        public void ManagedUpdate() { }
        public void ManagedLateUpdate() { }
        public bool IsWearing() { return false; }
    }

    public class OsawariPaizuri
    {
        public object AnotherModel;
        public void SwitchContext() { }
    }

    public class ParameterValue
    {
        public float Value;
        public ParameterValue(object parameter) { }
        public static ParameterValue operator +(ParameterValue p, float v) { p.Value += v; return p; }
        public static ParameterValue operator -(ParameterValue p, float v) { p.Value -= v; return p; }

        public ParameterValue Update(float value)
        {
            Value = value;
            return this;
        }

        public bool IsAlmostZero()
        {
            return UnityEngine.Mathf.Abs(Value) < 0.01f;
        }
    }

    public class OsawariResult
    {
        public OsawariResult(ActionManager manager) { }
        public void CalcResult(TemporaryStatus status) { }
    }

    public interface IBreast
    {
    }
}