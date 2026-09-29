using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class GaugeObject : MonoBehaviour
	{
		private Image _image;

		private bool _isGaugeMoving;

		private float _target;

		private CancellationToken token;

		public CanvasGroup CanvasGroup;

		private void SetUp()
		{
			_image = GetComponent<Image>();
			token = this.GetCancellationTokenOnDestroy();
		}

		public void SetRx(IReadOnlyReactiveProperty<float> _flt, int max)
		{
			SetUp();
			_flt.Subscribe(delegate(float x)
			{
				_target = x / (float)max;
				_image.fillAmount = _target;
			}).AddTo(this);
		}

		public void SetRx(IReadOnlyReactiveProperty<int> _int, int max)
		{
			SetUp();
			_int.Subscribe(delegate(int x)
			{
				float fillAmount = _image.fillAmount;
				_target = (float)x / (float)max;
				_image.fillAmount = _target;
				if (_target == 1f && fillAmount != 1f)
				{
					CanvasGroup.DOFade(0f, 0.75f).SetLoops(4, LoopType.Yoyo).Play();
				}
			}).AddTo(this);
		}

		private async UniTask MoveGauge(CancellationToken token)
		{
			await UniTask.WaitUntil(() => !_isGaugeMoving, PlayerLoopTiming.Update, token);
			_isGaugeMoving = true;
			float diff = _target - _image.fillAmount;
			if (diff > 0f)
			{
				while (_image.fillAmount < _target)
				{
					_image.fillAmount += diff / 20f;
					await UniTask.Yield(token);
				}
			}
			else
			{
				while (_image.fillAmount > _target)
				{
					_image.fillAmount += diff / 20f;
					await UniTask.Yield(token);
				}
			}
			_image.fillAmount = _target;
			_isGaugeMoving = false;
		}
	}
}
