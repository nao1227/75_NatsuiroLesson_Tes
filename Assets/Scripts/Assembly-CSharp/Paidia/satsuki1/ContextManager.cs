using System;
using UniRx;
using UnityEngine;

namespace Paidia.satsuki1
{
	public class ContextManager : MonoBehaviour
	{
		private Subject<OsawariContext> _onContextChanged = new Subject<OsawariContext>();

		public OsawariContext Context { get; private set; }

		public IObservable<OsawariContext> OnContextChanged => _onContextChanged;

		public bool IsLoaded => _onContextChanged != null;

		public void SetContext(OsawariContext context)
		{
			Context = context;
			_onContextChanged.OnNext(context);
		}
	}
}
