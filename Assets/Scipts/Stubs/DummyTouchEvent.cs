using Cysharp.Threading.Tasks;
using UnityEngine;
using Paidia.satsuki1;
using Stubs;   

public class DummyTouchEvent : OsawariEvent
{
    protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
    {
        Debug.Log("★ダミーイベント発火した!★");
        await UniTask.Yield();
    }
}