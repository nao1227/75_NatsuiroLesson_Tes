using Cysharp.Threading.Tasks;
using Paidia.satsuki1;
using Stubs;

namespace Stubs
{
    public class TimeManager
    {
        public float ModifiedPassedTime;
    }
}

public class SerialEvent : OsawariEvent
{
    protected override UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
    {
        return UniTask.CompletedTask;
    }
}

public class HScene3ModeChangeEvent : OsawariEvent
{
    protected override UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
    {
        return UniTask.CompletedTask;
    }
}

public class DetailAnimationEvent : OsawariEvent
{
    protected override UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
    {
        return UniTask.CompletedTask;
    }
}