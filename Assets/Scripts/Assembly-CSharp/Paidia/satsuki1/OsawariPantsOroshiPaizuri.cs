using DG.Tweening;
using Live2D.Cubism.Core;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariPantsOroshiPaizuri : AbstractOsawariAnotherModel
	{
		private ParameterValue _oroshi;

		private OsawariPantsPaizuri _pants;

		private int _reset;

		private Live2DAnimator _animator;

		public OsawariPantsOroshi _pantsOroshi;

		private Tweener _oroshiAnim;

		public float AnimationTime = 0.5f;

		public bool IsOnHip => _oroshi.Value == 0f;

		private bool IsMB()
		{
			return _manager.TemporaryStatus.Cloth == ClothName.MicroBikini;
		}

		protected override void AutoAnimation()
		{
		}

		protected override void SetTouchableMeshs()
		{
			if (IsMB())
			{
				TouchableMeshs = new CubismDrawable[2]
				{
					TouchableMeshs[2],
					TouchableMeshs[3]
				};
			}
			else
			{
				TouchableMeshs = new CubismDrawable[2]
				{
					TouchableMeshs[0],
					TouchableMeshs[1]
				};
			}
		}

		protected override int GetHandParamIndex(HandType handType)
		{
			return GetParameterNumber(IsMB() ? ParameterName.HandOnMBPants : ParameterName.DoubleHandOnPants);
		}

		protected override void InitializeParams()
		{
			_animator = GetComponent<Live2DAnimator>();
			_oroshi = new ParameterValue(parameters[ParameterName.Pants]);
			_pants = _manager.GetOsawariOf<OsawariPantsPaizuri>();
			SetOroshiAnimation();
		}

		private void SetOroshiAnimation()
		{
			_oroshiAnim = DOVirtual.Float(0f, 1f, AnimationTime, delegate(float x)
			{
				_oroshi = _oroshi.Update(x);
				if (x > 0.6f)
				{
					_pants.OffPants();
				}
			}).OnComplete(delegate
			{
				IsAnimating = false;
			});
		}

		protected override void OnLateUpdate()
		{
			SetLive2D(ParameterName.Pants, _oroshi);
			_pantsOroshi.SynchroValue(_oroshi.Value);
		}

		protected override void UpdateParamsCore(Vector3 move)
		{
			if (!IsAnimating && _oroshi.Value != 1f)
			{
				_manHand.Appear();
				if (move.y / SensitivityY < 0f && !_oroshiAnim.IsPlaying())
				{
					IsAnimating = true;
					SetOroshiAnimation();
					_oroshiAnim.Play();
					_manHand.SetValue(HandType.Left, 1f);
					_manHand.SetValue(HandType.Right, 1f);
				}
			}
		}

		protected override void UpdateWhileNotClicked()
		{
			if (!IsAnimating)
			{
				_manHand.Disappear();
			}
		}

		protected override bool GetConstraintsCore()
		{
			if (_oroshi.Value == 0f && _pants.IsClosed)
			{
				return _pants.IsWearing();
			}
			return false;
		}

		public void ResetPantsOroshi()
		{
			_oroshi = _oroshi.Update(0f);
		}

		public void SynchroValue(float value)
		{
			_oroshi = _oroshi.Update(value);
			_pants.SetPantsFlag(value == 0f);
		}

		public override void SetAuto()
		{
		}
	}
}
