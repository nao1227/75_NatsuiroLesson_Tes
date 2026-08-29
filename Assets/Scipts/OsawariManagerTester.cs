using UnityEngine;
using Paidia.satsuki1;
using Live2D.Cubism.Core;

public class OsawariManagerTester : MonoBehaviour
{
    public OsawariManager TargetOsawariManager;

    void Start()
    {
        Debug.Log("=== OsawariManager 検証開始 ===");

    TargetOsawariManager.Raycasters = new Stubs.ContextRaycasterList();
    TargetOsawariManager.Scene = new Stubs.HScene();   // ← この1行を追加

    var scene = TargetOsawariManager.GetScene();
    // Debug.Log("GetScene() 呼び出し成功: " + (scene != null));

    var raycaster = TargetOsawariManager.GetCubismRaycaster();
    // Debug.Log("GetCubismRaycaster() 呼び出し成功");

    // モデルからDrawable(パーツ)を全部取得する
// モデルからDrawable(パーツ)を全部取得する
var drawables = TargetOsawariManager.Model.Drawables;

// 本物のOsawariHeadを取得(あらかじめGameObjectにアタッチしておく)
var osawariHead = TargetOsawariManager.GetComponent<OsawariHead>();
osawariHead.TouchableMeshs = drawables;
Debug.Log(osawariHead.TouchableMeshs.Length + "個の全メッシュを登録しました");

// ContextOsawariTargets に、OsawariHeadを登録する
var targetList = new System.Collections.Generic.List<AbstractOsawari> { osawariHead };
TargetOsawariManager.ContextOsawariTargets = new Stubs.ContextOsawariTargetList();
TargetOsawariManager.ContextOsawariTargets.RegisterTargets(targetList);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("クリック検知、OnMouseUpTrigger を呼び出します");
            TargetOsawariManager.OnMouseUpTrigger();
            // Debug.Log("OnMouseUpTrigger 呼び出し完了(エラーなし)");
        }
    }
}