using UnityEngine;
using Paidia.satsuki1;
using Stubs;
using Live2D.Cubism.Framework.Raycasting;
public class InputManagerTester : MonoBehaviour
{
    public InputManager TargetInputManager;
    public CubismRaycaster ModelRaycaster;
    public OsawariManager TargetOsawariManager;   // ← 追加
    void Start()
    {
        var mouseInput = new MouseInputProvider(null);
        TargetOsawariManager.HandManager = new Stubs.HandManager();
        TargetOsawariManager.Raycasters = new Stubs.ContextRaycasterList();
        
        
        TargetInputManager.CameraManager = new OsawariCameraManager();
        Stubs.SingletonManager<Stubs.SceneContextManager>.Instance.AllowOsawari = true;
        TargetInputManager.ManagedStart(ModelRaycaster, mouseInput, TargetOsawariManager);

        Debug.Log("InputManager を初期化しました");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("マウスクリック検知(Unity標準)");
        }
        Debug.Log("MouseOn: " + TargetInputManager.MouseOn);
    }
}