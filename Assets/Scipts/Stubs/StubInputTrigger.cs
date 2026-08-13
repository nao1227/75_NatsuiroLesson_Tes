using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Raycasting;
using Paidia.satsuki1;

namespace Stubs
{
    public class StubInputTrigger : IInputTrigger
    {
        public void UpdateWhileClicked(CubismRaycastHit[] results, int hitCount, bool isFirst) { }
        public void OnMouseUpTrigger() { }
        public void OnAutoTrigger(CubismRaycastHit[] results, int hitCount) { }
        public void OnInputSpecialTrigger(CubismRaycastHit[] results, int hitCount) { }
        public bool IsClickingAtMesh(CubismRaycastHit[] results, int hitCount) => false;
        public CubismRaycaster GetCubismRaycaster() => null;
        public AbstractOsawari GetOsawariFromDrawable(CubismDrawable mesh) => null;
        public Scene GetScene() => null;
    }
}