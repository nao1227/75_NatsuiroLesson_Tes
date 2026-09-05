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

    TargetOsawariManager.HandManager ??= new Stubs.HandManager();

    var scene = TargetOsawariManager.GetScene();
    var raycaster = TargetOsawariManager.GetCubismRaycaster();

    var drawables = TargetOsawariManager.Model.Drawables;

    var osawariHead = TargetOsawariManager.GetComponent<OsawariHead>();

    var targetList = new System.Collections.Generic.List<AbstractOsawari> { osawariHead };
    TargetOsawariManager.ContextOsawariTargets = new Stubs.ContextOsawariTargetList();
    TargetOsawariManager.ContextOsawariTargets.RegisterTargets(targetList);

    osawariHead.ManagedStart(TargetOsawariManager, System.Threading.CancellationToken.None);
}
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TargetOsawariManager.OnMouseUpTrigger();
        }
    }
}