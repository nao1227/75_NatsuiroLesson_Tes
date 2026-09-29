using System.Threading;
using Cysharp.Threading.Tasks;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class HScene4OsawariHelper : OsawariHelper
{
	public FaceController FaceController;

	public int WaitTime = 300;

	public override void ManagedStart(OsawariManager osawariManager, CancellationToken token)
	{
		base.ManagedStart(osawariManager, token);
		if (SaveLoadManager.UnsavedData.GlobalFlags.IsRead(ScenarioLabel.Sub_Pool_H) || SingletonManager<SceneContextManager>.Instance.CurrentSceneContext == SceneContext.FreeH)
		{
			FaceController.SetDirectionDefault(-30f);
			return;
		}
		Object.FindObjectOfType<UtageManager>().OnFinishPlaying.Where((ScenarioLabel x) => x == ScenarioLabel.Sub_Pool_H).Subscribe(delegate
		{
			FaceController.SetDirectionDefault(-30f);
		}).AddTo(this);
	}

	protected override void InitializeParams()
	{
		if (Object.FindObjectOfType<UtageManager>().GetInt("cloth") == 2)
		{
			SetMicroBikini();
		}
	}

	public override async void OnUtageAnimation()
	{
		base.OnUtageAnimation();
		await UniTask.Delay(WaitTime);
		FaceController.SetControlHeadX(control: false);
	}

	public override void OnUtageAnimationFinished()
	{
		base.OnUtageAnimationFinished();
		FaceController.SetControlHeadX(control: true);
	}

	public void SetMicroBikini()
	{
		_manager.GetOsawariOf<OsawariMizugiHimo>().SetMicroBikini();
		_manager.GetOsawariOf<HScene4OsawariPants>().SetMicroBikini();
	}
}
