using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class VFXManager : MonoBehaviour
	{
		public List<ManagedParticle> Particles;

		public ParticleSystem BreathParticle;

		public bool On;

		public void InvokeParticle(ParticleName name)
		{
			if (On)
			{
				Particles.First((ManagedParticle x) => x.Name == name && x.Context == Object.FindObjectOfType<OsawariManager>().ContextManager.Context).Play(this.GetCancellationTokenOnDestroy());
			}
		}

		public async UniTask ChangeBreathSpeed(float val)
		{
			if (!On)
			{
				return;
			}
			ParticleSystem.MainModule main = BreathParticle.main;
			if (!(Mathf.Abs(val - main.duration) < 0.0001f))
			{
				if (val == 0f)
				{
					BreathParticle.Stop();
				}
				await UniTask.WaitUntil(() => BreathParticle.isStopped, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				main.duration = val;
				BreathParticle.Play();
			}
		}

		public void PlayShower(int count, int delay)
		{
			Particles.First((ManagedParticle x) => x.Name == ParticleName.Heart && x.Context == Object.FindObjectOfType<OsawariManager>().ContextManager.Context).PlayShower(count, delay);
		}
	}
}
