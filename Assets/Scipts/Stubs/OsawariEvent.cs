using Cysharp.Threading.Tasks;

namespace Stubs
{
    public class OsawariEvent : UnityEngine.MonoBehaviour
    {
        public virtual bool IsFullfillCondition(TemporaryStatus status, OsawariConditions conditions)
        {
            return true;
        }

        public virtual UniTask InvokeEvent(TemporaryStatus status, OsawariConditions conditions)
        {
            UnityEngine.Debug.Log("OsawariEvent の InvokeEvent が呼ばれた!");
            return UniTask.CompletedTask;
        }
    }
}