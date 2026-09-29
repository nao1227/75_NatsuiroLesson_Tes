using UniRx;

namespace Paidia.satsuki1
{
	public interface IAibu
	{
		IReadOnlyReactiveProperty<float> GetAverageSpeed();
	}
}
