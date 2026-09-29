using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Paidia.satsuki1;
using UniRx;
using UnityEngine;

public class HScene3OsawariHelper : OsawariHelper
{
	public ClothStatus ClothStatus;

	public CubismDrawable BreastNakedOrSwimsuitR;

	public CubismDrawable BreastNakedOrSwimsuitL;

	public CubismDrawable BreastWearedR;

	public CubismDrawable BreastWearedL;

	public CubismDrawable BreastHalfWearedR;

	public CubismDrawable BreastHalfWearedL;

	public CubismDrawable BreastUnderwearR;

	public CubismDrawable BreastUnderwearL;

	public FaceController FaceController;

	public int IdleDuration = 5000;

	public HScene4Mode Mode;

	private Subject<HScene4Mode> _onModeChange;

	private CancellationTokenSource _cts;

	private Live2DAnimator _animator;

	public IObservable<HScene4Mode> OnModeChange => _onModeChange;

	protected override void InitializeParams()
	{
		List<HScene3OsawariBreast> everyOsawariOf = _manager.GetEveryOsawariOf<HScene3OsawariBreast>();
		_animator = GetComponent<Live2DAnimator>();
		HScene3OsawariBreast hScene3OsawariBreast = everyOsawariOf[1];
		HScene3OsawariBreast hScene3OsawariBreast2 = everyOsawariOf[0];
		if (everyOsawariOf[0].GetParticularHand().HandType == HandType.Left)
		{
			hScene3OsawariBreast = everyOsawariOf[0];
			hScene3OsawariBreast2 = everyOsawariOf[1];
		}
		ClothStatus = (ClothStatus)UnityEngine.Object.FindObjectOfType<UtageManager>().GetInt("study_cloth");
		hScene3OsawariBreast.SetHandParam(this);
		hScene3OsawariBreast2.SetHandParam(this);
		Mode = HScene4Mode.StudyH;
		switch (ClothStatus)
		{
		case ClothStatus.WearAll:
			hScene3OsawariBreast.TouchableMeshs = new CubismDrawable[1] { BreastWearedL };
			hScene3OsawariBreast2.TouchableMeshs = new CubismDrawable[1] { BreastWearedR };
			Mode = HScene4Mode.StudyNormal;
			break;
		case ClothStatus.WearHalf:
			hScene3OsawariBreast.TouchableMeshs = new CubismDrawable[1] { BreastHalfWearedL };
			hScene3OsawariBreast2.TouchableMeshs = new CubismDrawable[1] { BreastHalfWearedR };
			Mode = HScene4Mode.StudyH;
			break;
		case ClothStatus.Underwear:
			hScene3OsawariBreast.TouchableMeshs = new CubismDrawable[1] { BreastUnderwearL };
			hScene3OsawariBreast2.TouchableMeshs = new CubismDrawable[1] { BreastUnderwearR };
			break;
		case ClothStatus.Naked:
			hScene3OsawariBreast.TouchableMeshs = new CubismDrawable[1] { BreastNakedOrSwimsuitL };
			hScene3OsawariBreast2.TouchableMeshs = new CubismDrawable[1] { BreastNakedOrSwimsuitR };
			break;
		case ClothStatus.SwimSuit:
			hScene3OsawariBreast.TouchableMeshs = new CubismDrawable[1] { BreastNakedOrSwimsuitL };
			hScene3OsawariBreast2.TouchableMeshs = new CubismDrawable[1] { BreastNakedOrSwimsuitR };
			_manager.GetOsawariOf<HScene3OsawariPants>().SetPantsName(GetParameterNumber(ParameterName.WetSuitUpperFlag));
			break;
		}
		_onModeChange = new Subject<HScene4Mode>();
		_onModeChange.OnNext(Mode);
		_cts = new CancellationTokenSource();
	}

	private void RefreshToken()
	{
		_cts.Cancel();
		_cts = new CancellationTokenSource();
	}

	protected override void OnLateUpdate()
	{
		switch (ClothStatus)
		{
		case ClothStatus.WearAll:
			SetLive2D(ParameterName.ShirtFlag, 2f);
			SetLive2D(ParameterName.SkirtFlag, 1f);
			SetLive2D(ParameterName.Bra, 1f);
			SetLive2D(ParameterName.PantsFlag, 1f);
			break;
		case ClothStatus.WearHalf:
			SetLive2D(ParameterName.ShirtFlag, 1f);
			SetLive2D(ParameterName.SkirtFlag, 1f);
			SetLive2D(ParameterName.Bra, 1f);
			SetLive2D(ParameterName.PantsFlag, 1f);
			break;
		case ClothStatus.Underwear:
			SetLive2D(ParameterName.ShirtFlag, 0f);
			SetLive2D(ParameterName.SkirtFlag, 0f);
			SetLive2D(ParameterName.Bra, 1f);
			SetLive2D(ParameterName.PantsFlag, 1f);
			break;
		case ClothStatus.Naked:
			SetLive2D(ParameterName.ShirtFlag, 0f);
			SetLive2D(ParameterName.SkirtFlag, 0f);
			SetLive2D(ParameterName.Bra, 0f);
			SetLive2D(ParameterName.PantsFlag, 0f);
			break;
		case ClothStatus.SwimSuit:
			SetLive2D(ParameterName.ShirtFlag, 0f);
			SetLive2D(ParameterName.SkirtFlag, 0f);
			SetLive2D(ParameterName.Bra, 0f);
			SetLive2D(ParameterName.PantsFlag, 0f);
			SetLive2D(ParameterName.WetSuitUpperFlag, 1f);
			break;
		}
	}

	public async UniTask MoveToStudyHMode(bool wait = true)
	{
		try
		{
			if (wait)
			{
				await UniTask.Delay(IdleDuration, ignoreTimeScale: false, PlayerLoopTiming.Update, _cts.Token);
			}
			ChangeMode(HScene4Mode.StudyH);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	public void ChangeMode(HScene4Mode mode)
	{
		RefreshToken();
		if (Mode != mode)
		{
			Mode = mode;
			_onModeChange.OnNext(mode);
			_animator.SetBool("StudyMode", mode == HScene4Mode.StudyH);
			if (mode == HScene4Mode.H)
			{
				_animator.SetTrigger("TriggerHMode");
			}
		}
	}

	public override void OnUtageAnimation()
	{
		base.OnUtageAnimation();
		UnityEngine.Object.FindObjectOfType<OsawariUIPresenter>().SetFadeInEnable();
		FaceController.SetControlHeadX(control: false);
		FaceController.AllowBlink = false;
		GetComponent<Live2DAnimator>().StopAnimation();
	}

	public override void OnUtageAnimationFinished()
	{
		base.OnUtageAnimationFinished();
		FaceController.SetControlHeadX(control: true);
		FaceController.ResetBlink();
		GetComponent<Live2DAnimator>().RestartAnimation();
	}

	private void OnDestroy()
	{
		_cts?.Cancel();
	}
}
