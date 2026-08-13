using UnityEngine;
using Paidia.satsuki1;
using Stubs;

public class MouseInputTester : MonoBehaviour
{
    private MouseInputProvider _input;

    void Start()
    {
        // MouseInputProviderを作る(UtageManagerはnullでもOK)
        _input = new MouseInputProvider(null);
        Debug.Log("MouseInputProvider を作成しました");
    }

    void Update()
    {
        // 毎フレーム、マウスの状態をログに出す
        if (_input.InputGrab())
        {
            Debug.Log("InputGrab: true (マウスの左ボタンが押されている)");
        }

        if (_input.InputMouse())
        {
            Debug.Log("InputMouse: マウスがクリックされた瞬間");
        }

        Vector3 pos = _input.GetPosition();
        // ポジションは毎フレーム出ると多すぎるので、クリック時だけ出す
        if (_input.InputMouse())
        {
            Debug.Log("マウス座標: " + pos);
        }
    }
}