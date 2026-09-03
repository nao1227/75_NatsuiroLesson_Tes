using System;

namespace Stubs
{
    public enum EventWaitPhase
    {
        None,
        WaitingForInvoke,
        WaitingForCancel
    }

    public enum FlagEnum
    {
        None
    }

    [Serializable]
    public class RandomSE
    {
        public bool HasSE;

        public Tuple<UnityEngine.AudioClip, int> GetRandomSE()
        {
            return null;
        }

        public Cysharp.Threading.Tasks.UniTask LoadSE()
        {
            return Cysharp.Threading.Tasks.UniTask.CompletedTask;
        }
    }

    [Serializable]
    public class StatusChange
    {
        public int Excite;
        public int Atomosphere;
        public int Stimulus;
    }

    [Serializable]
    public class EventCondition
    {
        public bool IsFullfillCondition(TemporaryStatus status, OsawariConditions conditions)
        {
            return true;
        }
    }

    [Serializable]
    public class EventFlagCondition
    {
        public bool IsFullfillCondition()
        {
            return true;
        }
    }

    [Serializable]
    public class EventScenarioReadCondition
    {
        public bool IsFullfillCondition()
        {
            return true;
        }
    }
    public class SoundManager
    {
        public void PlaySE(UnityEngine.AudioClip clip, object a, int b, int priority) { }
    }

    public enum SceneContext
    {
        Normal,
        FreeH
    }

    public class EmptyEvent : Paidia.satsuki1.OsawariEvent
    {
        protected override Cysharp.Threading.Tasks.UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
        {
            return Cysharp.Threading.Tasks.UniTask.CompletedTask;
        }
    }
}