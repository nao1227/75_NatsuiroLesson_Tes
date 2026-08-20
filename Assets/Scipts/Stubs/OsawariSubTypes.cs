using System;

namespace Stubs
{
    public class OsawariPiston : AbstractOsawari
    {
        public IObservable<bool> OnInsert() { return new UniRx.Subject<bool>(); }
        public IObservable<bool> OnPiston() { return new UniRx.Subject<bool>(); }
    }
    public class OsawariKiss : AbstractOsawari
    {
        public bool IsKissing;
        public UniRx.Subject<UniRx.Unit> OnKissStart = new UniRx.Subject<UniRx.Unit>();
        public UniRx.Subject<UniRx.Unit> OnKissEnd = new UniRx.Subject<UniRx.Unit>();
    }

    public class OsawariWithoutHand : AbstractOsawari { }
    public class OsawariDoublehanded : AbstractOsawari { }
    public class OsawariPants : AbstractOsawari
    {
        public bool IsAbleToInsert() { return false; }
    }

    public class OsawariGoods : UnityEngine.MonoBehaviour { }

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