public abstract class OsawariWithoutAuto : AbstractOsawari
{
	public override bool IsAuto => false;

	public override void SetAuto()
	{
	}

	protected override void AutoAnimation()
	{
	}
}
