using UnityEngine;
using Paidia.satsuki1;
using Stubs;
using Live2D.Cubism.Framework.Raycasting;
public class InputManagerTester : MonoBehaviour
{
    public InputManager TargetInputManager;
    public CubismRaycaster ModelRaycaster;
    void Start()
    {
        var mouseInput = new MouseInputProvider(null);
        var stubTrigger = new StubInputTrigger();
        TargetInputManager.CameraManager = new OsawariCameraManager();
        TargetInputManager.ManagedStart(ModelRaycaster, mouseInput, stubTrigger);

        Debug.Log("InputManager を初期化しました");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("マウスクリック検知(Unity標準)");
        }
        Debug.Log("MouseOn: " + TargetInputManager.MouseOn);
    }
}