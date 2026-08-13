using UnityEngine;
using Paidia.satsuki1;
using Stubs;

public class InputManagerTester : MonoBehaviour
{
    public InputManager TargetInputManager;

    void Start()
    {
        var mouseInput = new MouseInputProvider(null);
        var stubTrigger = new StubInputTrigger();

        TargetInputManager.ManagedStart(null, mouseInput, stubTrigger);

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