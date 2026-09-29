using System;
using System.Collections.Generic;
using Paidia.satsuki1;

[Serializable]
public class ContextOsawariTarget
{
	public OsawariContext Context;

	public List<AbstractOsawari> Targets;

	public int Count => Targets.Count;
}
