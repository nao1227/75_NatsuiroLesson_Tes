namespace Utage
{
	internal class AdvCommandParticleOff : AdvCommand
	{
		private string name;

		private AdvParticleStopType stopType;

		public AdvCommandParticleOff(StringGridRow row)
			: base(row)
		{
			name = ParseCellOptional(AdvColumnName.Arg1, "");
			stopType = ParseCellOptional(AdvColumnName.Arg2, AdvParticleStopType.Default);
		}

		public override void DoCommand(AdvEngine engine)
		{
			if (string.IsNullOrEmpty(name))
			{
				engine.GraphicManager.FadeOutAllParticle(stopType);
				return;
			}
			if (engine.GraphicManager.FindParticle(name) != null)
			{
				engine.GraphicManager.FadeOutParticle(name, stopType);
				return;
			}
			AdvGraphicLayer advGraphicLayer = engine.GraphicManager.FindLayer(name);
			if (advGraphicLayer != null)
			{
				advGraphicLayer.FadeOutAllParticle(stopType);
			}
		}
	}
}
