using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Paidia.satsuki1;
using UnityEngine;

public class FaceController : MonoBehaviour
{
	public enum FaceDirection
	{
		Default = 0,
		Front = 1
	}

	[Serializable]
	private class FaceAnimeGroup
	{
		[SerializeField]
		private string FaceAnimeGroupName;

		[SerializeField]
		private string AnimationVariableName;

		[SerializeField]
		private int Random;

		public FaceAnimeGroup()
		{
			FaceAnimeGroupName = "";
			AnimationVariableName = "";
			Random = 1;
		}

		public string GetFaceAnimeGroupName()
		{
			return FaceAnimeGroupName;
		}

		public string GetFaceAnimationVariableName()
		{
			return AnimationVariableName;
		}

		public int GetRandom()
		{
			return Random;
		}
	}

	private float EPS = 0.001f;

	public FaceParamCorrection FaceParamCorrection;

	public bool AutoChangeFaceDirection = true;

	public int HeadXIndex;

	public int HeadYIndex = 1;

	public int EyeOpenLeftIndex = 3;

	public int EyeOpenRightIndex = 4;

	public int EyeBallXIndex = 5;

	public int EyeBallYIndex = 6;

	public int EyeSizeIndex = 7;

	public int BrowTypeLeftIndex = 10;

	public int BrowTypeRightIndex = 11;

	public int BrowUpDownLeftIndex = 8;

	public int BrowUpdownRightIndex = 9;

	public int MouseTypeIndex = 12;

	public int MouseOpenIndex = 13;

	public List<int> SweatIndex;

	public int SweatIntervalMillSec;

	public int SweatIntervalDisperse;

	public StatusObject StatusObject;

	private CubismModel _model;

	private Live2DAnimator animator;

	private CubismParameter headXParam;

	private CubismParameter headYParam;

	private CubismParameter eyeBallXParam;

	private CubismParameter eyeBallYParam;

	private CubismParameter eyeOpenLeftParam;

	private CubismParameter eyeOpenRightParam;

	private CubismParameter eyeSizeParam;

	private CubismParameter browTypeLeftParam;

	private CubismParameter browTypeRightParam;

	private CubismParameter browUpDownLeftParam;

	private CubismParameter browUpdownRightParam;

	private CubismParameter mouseTypeParam;

	private CubismParameter mouseOpenParam;

	public OsawariContext Context;

	private FeelingParams _feelings;

	private bool _animeDone;

	private bool _animeMode;

	private bool _directionFixed;

	private InsertController _insertController;

	[SerializeField]
	private float DirectionChangeTime = 5f;

	[SerializeField]
	private float DirectionChangeProbability = 0.5f;

	[SerializeField]
	private float DirectionFront = -30f;

	[SerializeField]
	private float DirectionDefault;

	private bool _Sight = true;

	private bool _EyeBlinkMode = true;

	private bool _isBlinkingRight;

	private bool _isBlinkingLeft;

	private float _eyeOpenRightDefault;

	private float _eyeOpenLeftDefault;

	private ParameterValue _headX;

	private ParameterValue _headY;

	private ParameterValue _eyeBallX;

	private ParameterValue _eyeBallY;

	private ParameterValue _eyeOpenRight;

	private ParameterValue _eyeOpenLeft;

	private ParameterValue _sweatVal;

	private OsawariManager _manager;

	private float savedEyeBallY;

	private float savedEyeOpenLeft;

	private float savedEyeOpenRight;

	private float _eyeBlinkLeft;

	private float _eyeBlinkRight;

	private bool _loaded;

	private int _targetIndex;

	private bool _blinkReturningRight;

	private bool _blinkReturningLeft;

	private float _blinkOpenRatioRight;

	private float _blinkOpenRatioLeft;

	private FaceDirection _faceDirection;

	[SerializeField]
	private FaceAnimeGroup[] faceAnimeGroup;

	public float SweatDropSpeedFirstSection = 0.02f;

	public float SweatDropSpeedThirdSection = 0.01f;

	public float SweatDropSpeedSecondSectionLower = 0.001f;

	public float SweatDropSpeedSecondSectionUpper = 0.005f;

	public bool AllowBlink = true;

	[SerializeField]
	private bool _controllHeadX = true;

	private CancellationTokenSource _blinkToken;

	private bool _hasSweat => SweatIndex.Count > 0;

	public void ManagedStart()
	{
		_insertController = GetComponent<InsertController>();
		_manager = UnityEngine.Object.FindObjectOfType<OsawariManager>();
		_model = Context switch
		{
			OsawariContext.Osawari => _manager.Model, 
			OsawariContext.Fellatio => _manager.OsawariFellatio.Model, 
			OsawariContext.Paizuri => _manager.OsawariPaizuri.AnotherModel, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
		headXParam = _model.Parameters[HeadXIndex];
		headYParam = _model.Parameters[HeadYIndex];
		eyeOpenLeftParam = _model.Parameters[EyeOpenLeftIndex];
		eyeOpenRightParam = _model.Parameters[EyeOpenRightIndex];
		eyeBallXParam = _model.Parameters[EyeBallXIndex];
		eyeBallYParam = _model.Parameters[EyeBallYIndex];
		eyeSizeParam = _model.Parameters[EyeSizeIndex];
		browTypeLeftParam = _model.Parameters[BrowTypeLeftIndex];
		browTypeRightParam = _model.Parameters[BrowTypeRightIndex];
		browUpDownLeftParam = _model.Parameters[BrowUpDownLeftIndex];
		browUpdownRightParam = _model.Parameters[BrowUpdownRightIndex];
		mouseTypeParam = _model.Parameters[MouseTypeIndex];
		mouseOpenParam = _model.Parameters[MouseOpenIndex];
		_headX = new ParameterValue(headXParam);
		_headY = new ParameterValue(headYParam);
		_eyeBallX = new ParameterValue(eyeBallXParam);
		_eyeBallY = new ParameterValue(eyeBallYParam);
		_eyeOpenRight = new ParameterValue(eyeOpenRightParam);
		_eyeOpenLeft = new ParameterValue(eyeOpenLeftParam);
		if (_hasSweat)
		{
			_targetIndex = SweatIndex[0];
			_sweatVal = new ParameterValue(_model.Parameters[_targetIndex]);
		}
		SaveOriginalValues();
		animator = GetComponent<Live2DAnimator>();
		_eyeOpenLeftDefault = eyeOpenLeftParam.Value;
		_eyeOpenRightDefault = eyeOpenRightParam.Value;
		_feelings = StatusObject.TemporaryStatus.Feelings;
		_blinkToken = new CancellationTokenSource();
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		if (AutoChangeFaceDirection)
		{
			FaceDirectionAutoChange(cancellationTokenOnDestroy).Forget();
		}
		AutoBlink(UnityEngine.Random.Range(0, 8) * 1000, _blinkToken.Token).Forget();
		AutoSweat(cancellationTokenOnDestroy).Forget();
		_loaded = true;
	}

	public void ManagedUpdate()
	{
		if (_animeDone)
		{
			_animeDone = false;
			_animeMode = false;
			int num = faceAnimeGroup.Length;
			for (int i = 0; i < num; i++)
			{
				animator.SetBool(faceAnimeGroup[i].GetFaceAnimationVariableName(), on: false);
			}
		}
		float num2 = 0f;
		if (_faceDirection == FaceDirection.Default)
		{
			num2 = DirectionDefault;
		}
		else if (_faceDirection == FaceDirection.Front)
		{
			num2 = DirectionFront;
		}
		if (Mathf.Abs(_headX.Value - num2) > EPS)
		{
			_headX = _headX.Update(0.95f * (_headX.Value - num2) + num2);
		}
		else
		{
			_headX = _headX.Update(num2);
		}
		if (_Sight)
		{
			float value = headXParam.Value;
			float value2 = headYParam.Value;
			float num3 = 0f;
			float num4 = 0f;
			if (value <= 0f)
			{
				num3 += -0.0033333334f * value;
				num4 += 1f / 75f * value;
			}
			else
			{
				num3 += -0.020000001f * value;
				num4 += 0f;
			}
			if (value2 <= 0f)
			{
				num3 += 0f;
				num4 += -1f / 75f * value2;
			}
			else
			{
				num3 += -1f / 150f * value2;
				num4 += -1f / 30f * value2;
			}
			if (Mathf.Abs(num3 - _eyeBallX.Value) <= EPS)
			{
				_eyeBallX = _eyeBallX.Update(num3);
				_eyeBallY = _eyeBallY.Update(num4);
			}
			else
			{
				_eyeBallX = _eyeBallX.Update(0.9f * (_eyeBallX.Value - num3) + num3);
				_eyeBallY = _eyeBallY.Update(0.9f * (_eyeBallY.Value - num4) + num4);
			}
		}
		else
		{
			_eyeBallX = _eyeBallX.Update(eyeBallXParam.Value);
			_eyeBallY = _eyeBallY.Update(savedEyeBallY);
		}
		_eyeOpenLeft = _eyeOpenLeft.Update(savedEyeOpenLeft);
		_eyeOpenRight = _eyeOpenRight.Update(savedEyeOpenRight);
	}

	private void SetLive2D(int idx, float val, CubismParameterBlendMode mode = CubismParameterBlendMode.Override)
	{
		_manager.Preserver.SetValue(idx, val, mode, (Context != OsawariContext.Osawari) ? 1 : 0);
	}

	private void SetLive2D(int idx, ParameterValue val, CubismParameterBlendMode mode = CubismParameterBlendMode.Override)
	{
		SetLive2D(idx, val.Value, mode);
	}

	public void SetControlHeadX(bool control)
	{
		_controllHeadX = control;
	}

	public void ResetBlink()
	{
		AllowBlink = true;
		_blinkToken?.Cancel();
		_blinkToken = new CancellationTokenSource();
		AutoBlink(UnityEngine.Random.Range(5, 10) * 1000, _blinkToken.Token).Forget();
	}

	private void LateUpdate()
	{
		if (!_loaded)
		{
			return;
		}
		SaveOriginalValues();
		if (_controllHeadX)
		{
			SetLive2D(HeadXIndex, _headX);
		}
		else
		{
			_manager.Preserver.InactivateValue(HeadXIndex, (Context != OsawariContext.Osawari) ? 1 : 0);
		}
		FaceParamCorrection.GetEyeDirection(_eyeBallX.Value, _eyeBallY.Value, StatusObject.TemporaryStatus, _Sight);
		if (_hasSweat)
		{
			SetLive2D(_targetIndex, _sweatVal);
		}
		if (_animeMode)
		{
			return;
		}
		if (IsBlinking() && AllowBlink)
		{
			if (_blinkReturningLeft)
			{
				SetLive2D(EyeOpenLeftIndex, _blinkOpenRatioLeft, CubismParameterBlendMode.Multiply);
			}
			else
			{
				SetLive2D(EyeOpenLeftIndex, _eyeBlinkLeft);
			}
			if (_blinkReturningRight)
			{
				SetLive2D(EyeOpenRightIndex, _blinkOpenRatioRight, CubismParameterBlendMode.Multiply);
			}
			else
			{
				SetLive2D(EyeOpenRightIndex, _eyeBlinkRight);
			}
		}
		else
		{
			_manager.Preserver.InactivateValue(EyeOpenLeftIndex);
			_manager.Preserver.InactivateValue(EyeOpenRightIndex);
		}
	}

	private void SaveOriginalValues()
	{
		savedEyeBallY = _eyeBallY.Value;
		savedEyeOpenLeft = _eyeOpenLeft.Value;
		savedEyeOpenRight = _eyeOpenRight.Value;
	}

	protected bool IsBlinking()
	{
		if (!_isBlinkingRight)
		{
			return _isBlinkingLeft;
		}
		return true;
	}

	public void FacialAnimation(string name)
	{
		if (!(name != "") || _insertController.IsWomanExtacy)
		{
			return;
		}
		_animeMode = true;
		int num = faceAnimeGroup.Length;
		int num2 = 0;
		for (int i = 0; i <= num; i++)
		{
			if (faceAnimeGroup[i].GetFaceAnimeGroupName() == name)
			{
				num2 = i;
				break;
			}
		}
		int random = faceAnimeGroup[num2].GetRandom();
		string faceAnimationVariableName = faceAnimeGroup[num2].GetFaceAnimationVariableName();
		int val = UnityEngine.Random.Range(0, random);
		animator.SetBool(faceAnimationVariableName, on: true);
		animator.SetInteger("Random", val);
	}

	public void SetSight(bool Bool)
	{
		_Sight = Bool;
	}

	public void SetDirectionFixed(bool Bool)
	{
		_directionFixed = Bool;
	}

	public void SetFaceDirection(FaceDirection faceDirection)
	{
		_faceDirection = faceDirection;
	}

	public FaceDirection GetFaceDiraction()
	{
		return _faceDirection;
	}

	public bool IsAnimeMode()
	{
		return _animeMode;
	}

	private IEnumerator _StateListener()
	{
		_animeDone = true;
		yield return null;
	}

	public void StateListener()
	{
		StartCoroutine(_StateListener());
	}

	private async UniTask FaceDirectionAutoChange(CancellationToken token)
	{
		await UniTask.Delay((int)(DirectionChangeTime * 1000f), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		float value = UnityEngine.Random.value;
		if (!_animeMode && !_directionFixed && value <= DirectionChangeProbability)
		{
			if (_faceDirection == FaceDirection.Default)
			{
				_faceDirection = FaceDirection.Front;
			}
			else if (_faceDirection == FaceDirection.Front)
			{
				_faceDirection = FaceDirection.Default;
			}
		}
		FaceDirectionAutoChange(token).Forget();
	}

	private async UniTask AutoBlink(int blinkTime, CancellationToken token)
	{
		_ = 1;
		try
		{
			await UniTask.Delay(blinkTime, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			if (!_animeMode)
			{
				await Blink(_manager.Model.Parameters[EyeOpenLeftIndex].Value, _manager.Model.Parameters[EyeOpenRightIndex].Value, token);
			}
			AutoBlink(UnityEngine.Random.Range(5, 10) * 1000, token).Forget();
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			Debug.LogError(ex2.StackTrace);
		}
	}

	private async UniTask Blink(float originalLeft, float originalRight, CancellationToken token)
	{
		_isBlinkingRight = true;
		_isBlinkingLeft = true;
		_eyeBlinkLeft = originalLeft;
		_eyeBlinkRight = originalRight;
		while (IsBlinking())
		{
			if (_blinkReturningRight && _blinkReturningLeft)
			{
				_blinkOpenRatioRight += 0.1f;
				if (_blinkOpenRatioRight >= 1f)
				{
					_blinkOpenRatioRight = 1f;
					_blinkReturningRight = false;
					_isBlinkingRight = false;
				}
				_blinkOpenRatioLeft += 0.1f;
				if (_blinkOpenRatioLeft >= 1f)
				{
					_blinkOpenRatioLeft = 1f;
					_blinkReturningLeft = false;
					_isBlinkingLeft = false;
				}
			}
			if (!_blinkReturningRight)
			{
				if (_eyeBlinkRight <= EPS)
				{
					_eyeBlinkRight = 0f;
					_blinkOpenRatioRight = 0f;
					_blinkReturningRight = true;
				}
				else if (Mathf.Abs(_eyeBlinkRight) > 0f + EPS)
				{
					_eyeBlinkRight -= 0.1f;
				}
			}
			if (!_blinkReturningLeft)
			{
				if (_eyeBlinkLeft <= EPS)
				{
					_eyeBlinkLeft = 0f;
					_blinkOpenRatioLeft = 0f;
					_blinkReturningLeft = true;
				}
				else if (Mathf.Abs(_eyeBlinkLeft) > 0f + EPS)
				{
					_eyeBlinkLeft -= 0.1f;
				}
			}
			await UniTask.Yield(token);
		}
	}

	private async UniTask AutoSweat(CancellationToken token)
	{
		if (_hasSweat)
		{
			await UniTask.Delay(SweatIntervalMillSec + UnityEngine.Random.Range(-SweatIntervalDisperse, SweatIntervalDisperse), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			_targetIndex = SweatIndex[UnityEngine.Random.Range(0, SweatIndex.Count)];
			while (_sweatVal.Value < 1f)
			{
				_sweatVal += SweatDropSpeedFirstSection;
				await UniTask.Yield(token);
			}
			_sweatVal = _sweatVal.Update(1f);
			while (_sweatVal.Value < 2f)
			{
				_sweatVal += UnityEngine.Random.Range(SweatDropSpeedSecondSectionLower, SweatDropSpeedSecondSectionUpper);
				await UniTask.Yield(token);
			}
			_sweatVal = _sweatVal.Update(2f);
			while (_sweatVal.Value <= 2.999f)
			{
				_sweatVal += SweatDropSpeedThirdSection;
				await UniTask.Yield(token);
			}
			_sweatVal = _sweatVal.Update(0f);
			AutoSweat(token).Forget();
		}
	}

	public void SetDirectionDefault(float direction)
	{
		DirectionDefault = direction;
	}
}
