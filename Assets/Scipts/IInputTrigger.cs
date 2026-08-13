using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Raycasting;
using Stubs;

namespace Paidia.satsuki1
{
	public interface IInputTrigger
	{
		void UpdateWhileClicked(CubismRaycastHit[] results, int hitCount, bool isFirst);

		void OnMouseUpTrigger();

		void OnAutoTrigger(CubismRaycastHit[] results, int hitCount);

		void OnInputSpecialTrigger(CubismRaycastHit[] results, int hitCount);

		bool IsClickingAtMesh(CubismRaycastHit[] results, int hitCount);

		CubismRaycaster GetCubismRaycaster();

		AbstractOsawari GetOsawariFromDrawable(CubismDrawable mesh);

		Scene GetScene();
	}
}
