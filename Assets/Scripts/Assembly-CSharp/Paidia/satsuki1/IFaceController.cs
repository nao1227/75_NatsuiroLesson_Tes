using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public interface IFaceController
	{
		UniTask ManagedStart(OsawariManager manager);

		void ManagedUpdate();
	}
}
