using UnityEngine;

namespace Utage
{
	[AddComponentMenu("Utage/ADV/GraphicObject/AdvGraphicObjectParticleController")]
	public class AdvGraphicObjectParticleController : MonoBehaviour, IAdvGraphicObjectParticleController
	{
		[SerializeField]
		protected bool enableSave = true;

		[SerializeField]
		protected bool stopWithChildren = true;

		[SerializeField]
		protected ParticleSystemStopBehavior stopBehavior = ParticleSystemStopBehavior.StopEmitting;

		public bool EnableSave => enableSave;

		public virtual void Stop(AdvParticleStopType stopType)
		{
			ParticleSystem componentInChildren = GetComponentInChildren<ParticleSystem>();
			if (!(componentInChildren == null))
			{
				ParticleSystem.MainModule main = componentInChildren.main;
				main.loop = false;
				switch (stopType)
				{
				case AdvParticleStopType.StopEmitting:
					componentInChildren.Stop(stopWithChildren, ParticleSystemStopBehavior.StopEmitting);
					break;
				case AdvParticleStopType.Clear:
					componentInChildren.Stop(stopWithChildren, ParticleSystemStopBehavior.StopEmittingAndClear);
					break;
				default:
					componentInChildren.Stop(stopWithChildren, stopBehavior);
					break;
				}
			}
		}
	}
}
