namespace Utage
{
	internal interface IAdvGraphicObjectParticleController
	{
		bool EnableSave { get; }

		void Stop(AdvParticleStopType stopType);
	}
}
