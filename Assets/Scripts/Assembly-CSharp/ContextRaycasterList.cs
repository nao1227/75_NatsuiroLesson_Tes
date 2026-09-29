using System;
using System.Collections.Generic;
using System.Linq;
using Live2D.Cubism.Framework.Raycasting;
using Paidia.satsuki1;

[Serializable]
public class ContextRaycasterList
{
	public List<ContextRaycaster> Raycasters;

	public CubismRaycaster GetRaycaster(OsawariContext context)
	{
		return Raycasters.Where((ContextRaycaster x) => x.Context == context).First().Raycaster;
	}
}
