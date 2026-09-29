using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Live2D.Cubism.Core;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class OsawariCrossSection : MonoBehaviour
	{
		public CubismModel Model;

		public InsertController InsertController;

		[SerializeField]
		protected ParameterDictionary CSParameterNumbers;

		protected Dictionary<ParameterName, CubismParameter> csParameters;

		protected Live2DAnimator _animator;

		private ParameterValue _piston;

		private ParameterValue _scum;

		private ParameterValue _aibu;

		private BoolParameterValue _condom;

		private ParameterValue _ejaculation;

		private bool _vibrator;

		private HandType _hand;

		public void ManagedStart()
		{
			_animator = GetComponent<Live2DAnimator>();
			csParameters = new Dictionary<ParameterName, CubismParameter>();
			foreach (KeyValuePair<ParameterName, int> item in CSParameterNumbers.GetTable())
			{
				csParameters[item.Key] = Model.Parameters[item.Value];
			}
			_piston = new ParameterValue(csParameters[ParameterName.PistonCS]);
			_condom = new BoolParameterValue();
			_aibu = new ParameterValue(csParameters[ParameterName.CSAibu]);
			_hand = HandType.Left;
			_ejaculation = new ParameterValue(csParameters[ParameterName.CSEjaculation]);
			SetRX().Forget();
		}

		private async UniTask SetRX()
		{
			await UniTask.WaitUntil(() => InsertController.OnEjaculate != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
			InsertController.OnEjaculate.Where((Unit _) => Object.FindObjectOfType<OsawariManager>().ContextManager.Context == OsawariContext.Osawari).Subscribe(delegate
			{
				Ejaculation().Forget();
			}).AddTo(this);
		}

		public void SetCondom(bool on)
		{
			_condom = _condom.Update(on);
			_animator.SetBool("Condom", on);
		}

		public void SyncroPiston(float piston)
		{
			if (piston >= 0f)
			{
				_piston = _piston.Update(piston * 0.75f + 0.5f);
			}
			else
			{
				_piston = _piston.Update(piston * 1.5f + 0.5f);
			}
		}

		public void SynchroPistonFromHScene2(float piston)
		{
			_piston = _piston.Update(piston * 0.9f + 1.1f);
		}

		public void SynchroPistonFromHScene4(float piston)
		{
			_piston = _piston.Update(piston * 1.5f + 0.5f);
		}

		public void SyncroAibu(float aibu)
		{
			_aibu = _aibu.Update(aibu);
		}

		public void SetHand(HandType hand)
		{
			_hand = hand;
		}

		public void ResetPenisStatus()
		{
			_animator.SetInteger("EjaculateCount", 0);
			_animator.SetBool("Ejected", on: false);
		}

		protected async UniTask Ejaculation()
		{
			_animator.SetTrigger("Ejaculate");
			if (!_condom.AsBool())
			{
				await UniTask.Delay(100);
				_animator.SetInteger("EjaculateCount", _animator.GetInteger("EjaculateCount") + 1);
			}
		}

		public void EjectPenis()
		{
			_animator.SetBool("Ejected", on: true);
			_animator.SetTrigger("Eject");
		}

		private void LateUpdate()
		{
			if (_hand == HandType.Left)
			{
				csParameters[ParameterName.CSRightHandFlag].Value = 1f;
				csParameters[ParameterName.CSLeftHandFlag].Value = 0f;
			}
			else
			{
				csParameters[ParameterName.CSLeftHandFlag].Value = 1f;
				csParameters[ParameterName.CSRightHandFlag].Value = 0f;
			}
			csParameters[ParameterName.CSCondomFlag].Value = _condom.Value;
			csParameters[ParameterName.PistonCS].Value = _piston.Value;
			csParameters[ParameterName.CSAibu].Value = _aibu.Value;
		}
	}
}
