using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariHelperEndOfDay : OsawariHelper
	{
		private ParameterValue _headZ;

		private CancellationTokenSource _cts;

		private bool _isUtageAnimating;

		protected override void InitializeParams()
		{
			_headZ = new ParameterValue(parameters[ParameterName.HeadZ]);
			_cts = new CancellationTokenSource();
		}

		public override void OnUtageAnimation()
		{
			base.OnUtageAnimation();
			_cts.Cancel();
			_isUtageAnimating = true;
		}

		public override void OnUtageAnimationFinished()
		{
			_headZ = _headZ.Update(parameters[ParameterName.HeadZ].Value);
			ResetHeadZ().Forget();
		}

		protected override void OnLateUpdate()
		{
			base.OnLateUpdate();
			if (_isUtageAnimating)
			{
				InactivateLive2D(ParameterName.HeadZ);
				_headZ = _headZ.Update(parameters[ParameterName.HeadZ].Value);
			}
			else
			{
				SetLive2D(ParameterName.HeadZ, _headZ);
			}
		}

		private async UniTask ResetHeadZ()
		{
			_cts = new CancellationTokenSource();
			try
			{
				_isUtageAnimating = false;
				while (Mathf.Abs(_headZ.Value) > 0.01f)
				{
					_headZ *= 0.95f;
					await UniTask.Yield(_cts.Token);
				}
			}
			catch (OperationCanceledException)
			{
			}
			_headZ = _headZ.Update(0f);
		}
	}
}
