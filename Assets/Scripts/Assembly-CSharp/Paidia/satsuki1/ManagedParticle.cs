using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	[Serializable]
	public class ManagedParticle
	{
		public ParticleName Name;

		public ParticleSystem Particle;

		public int CoolTime = 1000;

		public OsawariContext Context;

		[NonSerialized]
		public bool IsInCooltime;

		public void Play(CancellationToken token)
		{
			if (!IsInCooltime)
			{
				WaitForCooltime(token).Forget();
				Particle.Play();
			}
		}

		private async UniTask WaitForCooltime(CancellationToken token)
		{
			IsInCooltime = true;
			await UniTask.Delay(CoolTime, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			IsInCooltime = false;
		}

		public async void PlayShower(int count, int delay)
		{
			for (int i = 0; i < count; i++)
			{
				Particle.Play();
				await UniTask.Delay(delay);
			}
		}
	}
}
