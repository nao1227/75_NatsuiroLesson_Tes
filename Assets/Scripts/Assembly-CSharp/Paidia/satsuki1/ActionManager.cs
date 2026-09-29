using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Paidia.satsuki1
{
	public class ActionManager : MonoBehaviour
	{
		public List<OsawariAction> Actions;

		public StatusObject Status;

		[NonSerialized]
		public InsertController InsertController;

		public OsawariManager osawariManager;

		public Text DebugDisplay;

		public int AchievedActions => Actions.Count((OsawariAction x) => x.Count > 0);

		public IEnumerable<OsawariAction> GetActionOf<T>()
		{
			return Actions.Where((OsawariAction x) => x is T);
		}

		public void ManagedStart()
		{
			foreach (OsawariAction action7 in Actions)
			{
				action7.Initialize(Status.TemporaryStatus);
			}
			foreach (OsawariAction item in Actions.Where((OsawariAction x) => x is IBeforeWomanExtacy))
			{
				IBeforeWomanExtacy tmp = item as IBeforeWomanExtacy;
				Status.TemporaryStatus.Feelings.ExciteRx.Subscribe(delegate(int x)
				{
					if ((double)x >= 95000.0 && (double)x <= 99000.0)
					{
						tmp.StartAction();
					}
					else
					{
						tmp.CancelAction();
					}
				}).AddTo(this);
			}
			foreach (OsawariAction item2 in Actions.Where((OsawariAction x) => x is IAfterWomanExtacy))
			{
				IAfterWomanExtacy tmp2 = item2 as IAfterWomanExtacy;
				Status.TemporaryStatus.Feelings.ExciteRx.Subscribe(delegate(int x)
				{
					if (x == 100000)
					{
						tmp2.StartExtacy();
					}
				}).AddTo(this);
			}
			foreach (OsawariAction action in Actions.Where((OsawariAction x) => x is IEjaculateTrigger))
			{
				InsertController.OnEjaculate.Subscribe(delegate
				{
					action.StartAction();
					action.FinishAction();
				}).AddTo(this);
			}
			foreach (IOnExtacyAdded action2 in Actions.Where((OsawariAction x) => x is IOnExtacyAdded))
			{
				Status.TemporaryStatus.Feelings.ExciteRx.Where((int x) => x > 0).Subscribe(delegate(int x)
				{
					action2.OnExtacyAdded(x);
				}).AddTo(this);
			}
			foreach (IOnExtraExtacyAdded action3 in Actions.Where((OsawariAction x) => x is IOnExtraExtacyAdded))
			{
				Status.TemporaryStatus.Feelings.OnExtraExcite.Subscribe(delegate(int x)
				{
					action3.OnExtraExtacyAdded(x);
				}).AddTo(this);
			}
			foreach (IPettingTrigger action4 in Actions.Where((OsawariAction x) => x is IPettingTrigger))
			{
				foreach (IAibu item3 in osawariManager.GetEveryInterfaceOf<IAibu>())
				{
					item3.GetAverageSpeed().Subscribe(delegate(float x)
					{
						action4.OnAverageSpeedChanged(x);
					}).AddTo(this);
				}
			}
			foreach (IPistonTrigger action5 in Actions.Where((OsawariAction x) => x is IPistonTrigger))
			{
				osawariManager.GetOsawariOf<OsawariPiston>()?.AverageSpeed.Subscribe(delegate(float x)
				{
					action5.OnAverageSpeedChanged(x);
				}).AddTo(this);
			}
			foreach (IWhileBreastGrab action6 in Actions.Where((OsawariAction x) => x is IWhileBreastGrab))
			{
				foreach (OsawariBrest item4 in osawariManager.GetEveryOsawariOf<OsawariBrest>())
				{
					item4.GetIsGrabbing().Subscribe(delegate(bool x)
					{
						if (x)
						{
							action6.GrabBreast(grab: true);
						}
						else
						{
							using (List<OsawariBrest>.Enumerator enumerator5 = osawariManager.GetEveryOsawariOf<OsawariBrest>().GetEnumerator())
							{
								while (enumerator5.MoveNext() && !enumerator5.Current.GetIsGrabbing().Value)
								{
								}
							}
							action6.GrabBreast(grab: false);
						}
					}).AddTo(this);
				}
			}
			OsawariHead osawariOf = osawariManager.GetOsawariOf<OsawariHead>();
			if (null != osawariOf)
			{
				osawariOf.OnStroke.Subscribe(delegate
				{
					foreach (OsawariAction item5 in Actions.Where((OsawariAction x) => x is IStrokeTrigger))
					{
						item5.StartAction();
					}
				}).AddTo(this);
			}
			OsawariHeadFellatio osawariOf2 = osawariManager.GetOsawariOf<OsawariHeadFellatio>();
			if (null != osawariOf2)
			{
				osawariOf2.OnStroke.Subscribe(delegate
				{
					foreach (OsawariAction item6 in Actions.Where((OsawariAction x) => x is IStrokeTrigger))
					{
						item6.StartAction();
					}
				}).AddTo(this);
			}
			OsawariHeadPaizuri osawariOf3 = osawariManager.GetOsawariOf<OsawariHeadPaizuri>();
			if (!(null != osawariOf3))
			{
				return;
			}
			osawariOf3.OnStroke.Subscribe(delegate
			{
				foreach (OsawariAction item7 in Actions.Where((OsawariAction x) => x is IStrokeTrigger))
				{
					item7.StartAction();
				}
			}).AddTo(this);
		}

		public void ManagedUpdate()
		{
			string text = "";
			foreach (OsawariAction action in Actions)
			{
				text += $"{action.DisplayName} : {action.Count}\n";
				if (!action.GetStatusCondition())
				{
					action.FinishAction();
				}
			}
			text += osawariManager.OsawariFaceState;
			if (null != DebugDisplay)
			{
				DebugDisplay.text = text;
			}
		}

		public List<OsawariAction> GetAchievedActions()
		{
			return Actions.Where((OsawariAction x) => x.Count > 0).ToList();
		}
	}
}
