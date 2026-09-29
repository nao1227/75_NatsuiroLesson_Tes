using System;
using System.Collections.Generic;
using System.Linq;
using Paidia.satsuki1;

[Serializable]
public class ContextOsawariTargetList
{
	public List<ContextOsawariTarget> List;

	public int Count => List.Count;

	public List<AbstractOsawari> GetOsawariTargets(OsawariContext context)
	{
		return List.Where((ContextOsawariTarget x) => x.Context == context).First().Targets;
	}

	public List<List<AbstractOsawari>> GetOsawariTargetsOfNotInContext(OsawariContext context)
	{
		return (from x in List
			where x.Context != context
			select x.Targets).ToList();
	}

	public IEnumerator<List<AbstractOsawari>> GetEnumerator()
	{
		foreach (ContextOsawariTarget item in List)
		{
			yield return item.Targets;
		}
	}
}
