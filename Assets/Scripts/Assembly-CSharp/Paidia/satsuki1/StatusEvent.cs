using Cysharp.Threading.Tasks;

namespace Paidia.satsuki1
{
	public class StatusEvent : OsawariEvent
	{
		public StatusName TargetStatus;

		public int Addition;

		protected override async UniTask InvokeCore(TemporaryStatus status, OsawariConditions conditions)
		{
			switch (TargetStatus)
			{
			case StatusName.Favorability:
				StatusObject.PersistantStatus.AddFavourability(Addition);
				break;
			case StatusName.Sensitivity:
				StatusObject.PersistantStatus.AddSensitivity(Addition);
				break;
			case StatusName.EjaculateCount:
				StatusObject.PersistantStatus.AddEjaculateCount(Addition);
				break;
			case StatusName.ExciteValue:
				StatusObject.TemporaryStatus.AddExciteValue(Addition);
				break;
			case StatusName.Atm_Nervous:
				StatusObject.TemporaryStatus.AddNervous(Addition);
				break;
			case StatusName.Atm_Excited:
				StatusObject.TemporaryStatus.AddExcited(Addition);
				break;
			case StatusName.Atm_Rut:
				StatusObject.TemporaryStatus.AddRut(Addition);
				break;
			}
			await UniTask.Yield();
		}
	}
}
