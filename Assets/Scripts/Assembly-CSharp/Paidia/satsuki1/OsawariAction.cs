using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Paidia.satsuki1
{
	public abstract class OsawariAction : MonoBehaviour
	{
		public string DisplayName;

		public bool Unique;

		protected bool _isInAction;

		protected TemporaryStatus _status;

		public int ExciteIncrement;

		public int AtomosphereIncrement;

		public int StimulusIncrement;

		public int Count { get; protected set; }

		public virtual void Initialize(TemporaryStatus status)
		{
			_status = status;
		}

		public virtual bool GetStatusCondition()
		{
			return true;
		}

		public virtual bool GetConditionOnFinish()
		{
			return true;
		}

		public virtual void StartAction()
		{
			if (!_isInAction && GetStatusCondition())
			{
				if (Unique)
				{
					CountAction();
				}
				else
				{
					_isInAction = true;
				}
			}
		}

		public virtual void FinishAction()
		{
			if (_isInAction)
			{
				if (GetConditionOnFinish())
				{
					CountAction();
				}
				_isInAction = false;
			}
		}

		protected void CountAction()
		{
			if (_status != null && (!Unique || Count <= 0))
			{
				CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
				Count++;
				_status.Feelings.SetExciteWithTimer(ExciteIncrement, cancellationTokenOnDestroy).Forget();
				_status.Feelings.SetAtomosphereWithTimer(AtomosphereIncrement, cancellationTokenOnDestroy).Forget();
				_status.Feelings.SetStimulusWithTimer(StimulusIncrement, cancellationTokenOnDestroy).Forget();
			}
		}
	}
}
