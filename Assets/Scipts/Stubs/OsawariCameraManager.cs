namespace Stubs
{
    public class OsawariCameraManager
    {
        public void CameraZoom(bool zoomIn) { }
        public void SetMousePos(UnityEngine.Vector3 pos) { }
        public void MoveCamera(UnityEngine.Vector3 pos)
        {
            UnityEngine.Debug.Log("MoveCamera が呼ばれた: " + pos);
        }
    }
}