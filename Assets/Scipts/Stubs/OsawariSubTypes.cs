using System;
using Stubs;
public class OsawariPiston : AbstractOsawari
{
    public IObservable<bool> OnInsert() { return new UniRx.Subject<bool>(); }
    public IObservable<bool> OnPiston() { return new UniRx.Subject<bool>(); }

    protected override void InitializeParams() { }
    protected override void AutoAnimation() { }
    protected override void UpdateWhileNotClicked() { }
    protected override void UpdateParamsCore(UnityEngine.Vector3 move) { }
    protected override int GetHandParamIndex(HandType handType) { return 0; }
    protected override void OnLateUpdate() { }
}

public class OsawariKiss : AbstractOsawari
{
    public bool IsKissing;
    public UniRx.Subject<UniRx.Unit> OnKissStart = new UniRx.Subject<UniRx.Unit>();
    public UniRx.Subject<UniRx.Unit> OnKissEnd = new UniRx.Subject<UniRx.Unit>();

    protected override void InitializeParams() { }
    protected override void AutoAnimation() { }
    protected override void UpdateWhileNotClicked() { }
    protected override void UpdateParamsCore(UnityEngine.Vector3 move) { }
    protected override int GetHandParamIndex(HandType handType) { return 0; }
    protected override void OnLateUpdate() { }
}

public class OsawariWithoutHand : AbstractOsawari
{
    protected override void InitializeParams() { }
    protected override void AutoAnimation() { }
    protected override void UpdateWhileNotClicked() { }
    protected override void UpdateParamsCore(UnityEngine.Vector3 move) { }
    protected override int GetHandParamIndex(HandType handType) { return 0; }
    protected override void OnLateUpdate() { }
}

public class OsawariDoublehanded : AbstractOsawari
{
    protected override void InitializeParams() { }
    protected override void AutoAnimation() { }
    protected override void UpdateWhileNotClicked() { }
    protected override void UpdateParamsCore(UnityEngine.Vector3 move) { }
    protected override int GetHandParamIndex(HandType handType) { return 0; }
    protected override void OnLateUpdate() { }
}

public class OsawariPants : AbstractOsawari
{
    public bool IsAbleToInsert() { return false; }

    protected override void InitializeParams() { }
    protected override void AutoAnimation() { }
    protected override void UpdateWhileNotClicked() { }
    protected override void UpdateParamsCore(UnityEngine.Vector3 move) { }
    protected override int GetHandParamIndex(HandType handType) { return 0; }
    protected override void OnLateUpdate() { }
}

public class OsawariGoods : UnityEngine.MonoBehaviour { }

namespace Stubs
{
    public interface IWearable
    {
        bool IsWearing();
    }
    public interface IInsertable
    {
        IObservable<bool> OnInsert();
    }
    public interface IPiston
    {
        IObservable<bool> OnPiston();
    }
    public interface ISwitchable
    {
        bool Touchable { get; }
    }
    public interface IParticularHand
    {
        ParticularHandStub GetParticularHand();
    }
    public class ParticularHandStub
    {
        public bool IsGrabbing;
    }
    public interface IDoubleHanded { }
    public interface IUndressTrigger { }
}