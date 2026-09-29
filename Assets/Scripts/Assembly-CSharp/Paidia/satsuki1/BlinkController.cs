using System.Threading;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class BlinkController : MonoBehaviour
	{
		private CubismModel _model;

		public int EyeOpenLeftIndex;

		public int EyeOpenRightIndex;

		public int BlinkIntervalMinSecond = 5;

		public int BlinkIntervalMaxSecond = 10;

		private bool _isBlinking;

		private float _eyeBlinkLeft;

		private float _eyeBlinkRight;

		private bool _blinkReturning;

		private float _blinkOpenRatio;

		private float EPS = 0.0001f;

		public CubismModel Model;

		private void Start()
		{
			_model = (Model ? Model : GetComponent<OsawariManager>().Model);
			_isBlinking = false;
			_blinkReturning = false;
			AutoBlink(3, this.GetCancellationTokenOnDestroy()).Forget();
		}

		private async UniTask AutoBlink(int blinkTime, CancellationToken token)
		{
			await UniTask.Delay(blinkTime, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			_isBlinking = true;
			await Blink(_model.Parameters[EyeOpenLeftIndex].Value, _model.Parameters[EyeOpenRightIndex].Value, token);
			AutoBlink(Random.Range(BlinkIntervalMinSecond, BlinkIntervalMaxSecond) * 1000, token).Forget();
		}

		private async UniTask Blink(float originalLeft, float originalRight, CancellationToken token)
		{
			_eyeBlinkLeft = originalLeft;
			_eyeBlinkRight = originalRight;
			while (_isBlinking)
			{
				if (_blinkReturning)
				{
					_blinkOpenRatio += 0.1f;
					if (_blinkOpenRatio >= 1f)
					{
						_blinkOpenRatio = 1f;
						_blinkReturning = false;
						_isBlinking = false;
					}
				}
				else if (_eyeBlinkLeft <= EPS && _eyeBlinkRight <= EPS)
				{
					_eyeBlinkLeft = 0f;
					_eyeBlinkRight = 0f;
					_blinkOpenRatio = 0f;
					_blinkReturning = true;
				}
				else if (Mathf.Abs(_eyeBlinkLeft) > 0f + EPS && Mathf.Abs(_eyeBlinkRight) > 0f + EPS)
				{
					_eyeBlinkRight -= 0.1f;
					_eyeBlinkLeft -= 0.1f;
				}
				await UniTask.Yield(token);
			}
		}

		private void LateUpdate()
		{
			if (_isBlinking)
			{
				if (_blinkReturning)
				{
					_model.Parameters[EyeOpenLeftIndex].BlendToValue(CubismParameterBlendMode.Multiply, _blinkOpenRatio);
					_model.Parameters[EyeOpenRightIndex].BlendToValue(CubismParameterBlendMode.Multiply, _blinkOpenRatio);
				}
				else
				{
					_model.Parameters[EyeOpenLeftIndex].BlendToValue(CubismParameterBlendMode.Override, _eyeBlinkLeft);
					_model.Parameters[EyeOpenRightIndex].BlendToValue(CubismParameterBlendMode.Override, _eyeBlinkRight);
				}
			}
		}
	}
}
