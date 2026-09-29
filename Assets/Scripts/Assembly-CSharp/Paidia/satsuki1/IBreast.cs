using UniRx;

namespace Paidia.satsuki1
{
	public interface IBreast
	{
		IReadOnlyReactiveProperty<bool> GetIsGrabbing();
	}
}
