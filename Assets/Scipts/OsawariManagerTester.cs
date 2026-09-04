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
        TargetOsawariManager.Scene = new Stubs.HScene();

        // ← 追加: HandManagerが未セットならここで作る(保険)
        TargetOsawariManager.HandManager ??= new Stubs.HandManager();

        var scene = TargetOsawariManager.GetScene();
        var raycaster = TargetOsawariManager.GetCubismRaycaster();

        // モデルからDrawable(パーツ)を全部取得する
        var drawables = TargetOsawariManager.Model.Drawables;

        // 本物のOsawariHeadを取得(あらかじめGameObjectにアタッチしておく)
        var osawariHead = TargetOsawariManager.GetComponent<OsawariHead>();


        // ContextOsawariTargets に、OsawariHeadを登録する
        var targetList = new System.Collections.Generic.List<AbstractOsawari> { osawariHead };
        TargetOsawariManager.ContextOsawariTargets = new Stubs.ContextOsawariTargetList();
        TargetOsawariManager.ContextOsawariTargets.RegisterTargets(targetList);

        var dummyEventObj = new GameObject("DummyTouchEvent");
        
        var dummyEvent = dummyEventObj.AddComponent<DummyTouchEvent>();
        dummyEvent.StatusObject = FindObjectOfType<Stubs.StatusObject>();   // ← この1行を追加

        osawariHead.OnTouchEvents.Add(dummyEvent);
        // ← 追加: ここでManagedStart()を呼ぶ(これが今回追加したかった1行)
        // osawariHead.ManagedStart(TargetOsawariManager, System.Threading.CancellationToken.None);
        try
        {
            osawariHead.ManagedStart(TargetOsawariManager, System.Threading.CancellationToken.None);
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }



        // ← 移動: ManagedStart()の中でTouchableMeshsが上書きされるので、その後にセットし直す
        osawariHead.TouchableMeshs = drawables;
        Debug.Log(osawariHead.TouchableMeshs.Length + "個の全メッシュを登録しました");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TargetOsawariManager.OnMouseUpTrigger();
        }
    }
}