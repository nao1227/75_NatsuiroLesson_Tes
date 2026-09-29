using System;

namespace Paidia.satsuki1
{
	public interface IPiston
	{
		IObservable<bool> OnPiston();
	}
}
