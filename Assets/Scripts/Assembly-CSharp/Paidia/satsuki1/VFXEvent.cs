using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class VFXEvent : OsawariEvent
	{
		public ParticleName ParticleName;

		private VFXManager _vfxManager;

		protected override void PostInitialize()
		{
			_vfxManager = Object.FindObjectOfType<VFXManager>();
		}

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			_vfxManager.InvokeParticle(ParticleName);
			await UniTask.Yield();
		}
	}
}
