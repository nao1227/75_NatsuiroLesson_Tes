using System;

namespace Paidia.satsuki1
{
	public interface IInsertable
	{
		IObservable<bool> OnInsert();
	}
}
