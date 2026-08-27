using Live2D.Cubism.Core;

namespace Stubs
{
    public class AbstractOsawari
    {
        public int Priority;
        public bool IsAnimating;

        public bool GetConstraints()
        {
            return true;
        }

        public bool CanTouchMesh(CubismDrawable mesh)
        {
            return true;
        }

        public void ManagedStart(object owner, System.Threading.CancellationToken token) { }
        public void PostInitialize() { }
        public void Cancel() { }
        public void SwitchContext() { }
        public void ManagedUpdate() { }
        public void ManagedUpdateWhileNotActive() { }
        public void ManagedLateUpdate() { }
        public void OnClick(CubismDrawable mesh, bool isFirst)
        {
            UnityEngine.Debug.Log("OnClick が呼ばれた! isFirst=" + isFirst);
        }
        public void OnMouseUp() { }
        public void OnSpecial() { }
        public void SetAuto() { }

        public System.Collections.Generic.List<CubismDrawable> TouchableMeshs = new System.Collections.Generic.List<CubismDrawable>();
    }
}