using Cysharp.Threading.Tasks;
using Stubs;

public class OsawariEvent : UnityEngine.MonoBehaviour
{
    public bool IsInvoked;

    public virtual bool IsFullfillCondition(TemporaryStatus status, OsawariConditions conditions)
    {
        return true;
    }

    public virtual UniTask InvokeEvent(TemporaryStatus status, OsawariConditions conditions)
    {
        UnityEngine.Debug.Log("OsawariEvent の InvokeEvent が呼ばれた!");
        IsInvoked = true;
        return UniTask.CompletedTask;
    }

    public virtual void Cancel()
    {
        IsInvoked = false;
    }

    public Cysharp.Threading.Tasks.UniTask Initialize(AbstractOsawari parent)
    {
        return Cysharp.Threading.Tasks.UniTask.CompletedTask;
    }
}